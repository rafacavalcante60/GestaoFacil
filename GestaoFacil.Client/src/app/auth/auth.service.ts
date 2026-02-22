import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError } from 'rxjs';

interface LoginRequest {
  email: string;
  senha: string;
}

interface ForgotPasswordRequest {
  email: string;
}

interface ResetPasswordRequest {
  token: string;
  newPassword: string;
}

interface LoginResponseRaw {
  dados?: {
    token?: string;
    refreshToken?: string;
    expiraEm?: string; // ISO
  };
  mensagem?: string;
  status?: boolean;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = 'https://localhost:7285/api/Auth';

  constructor(private http: HttpClient, private router: Router) {}

  // login adaptado ao formato que você mostrou
  login(data: LoginRequest): Observable<LoginResponseRaw> {
    return this.http.post<LoginResponseRaw>(`${this.baseUrl}/login`, data).pipe(
      tap(res => {
        console.log('Login response (raw):', res);
        const token = res?.dados?.token;
        const refreshToken = res?.dados?.refreshToken;
        const expiraEm = res?.dados?.expiraEm;

        if (token) {
          localStorage.setItem('token', token);
          if (refreshToken) localStorage.setItem('refreshToken', refreshToken);
          if (expiraEm) localStorage.setItem('tokenExpiraEm', expiraEm);
          console.log('Token salvo no localStorage (length):', token.length);
        } else {
          console.error('Login response não contém token em dados.token', res);
        }
      }),
      catchError(err => {
        console.error('Erro no login HTTP:', err);
        return throwError(() => err);
      })
    );
  }

  register(data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, data);
  }

  forgotPassword(email: string): Observable<any> {
    const body: ForgotPasswordRequest = { email };
    return this.http.post(`${this.baseUrl}/forgot-password`, body);
  }

  resetPassword(token: string, newPassword: string): Observable<any> {
    const body: ResetPasswordRequest = { token, newPassword };
    return this.http.post(`${this.baseUrl}/reset-password`, body);
  }

  // retorna token (null se não existir ou inválido)
  getToken(): string | null {
    const t = localStorage.getItem('token');
    if (!t || t === 'undefined' || t === 'null' || t.trim() === '') return null;
    return t;
  }

  // opcional: retorna true se o token existir e não expirou (usa tokenExpiraEm salvo)
  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;

    const exp = localStorage.getItem('tokenExpiraEm');
    if (!exp) return true; // se não tiver expiraEm, assume válido
    const expDate = new Date(exp);
    return expDate.getTime() > Date.now();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('tokenExpiraEm');
    this.router.navigate(['/auth/login']);
  }

  // se quiser, método para pegar refreshToken
  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  getUserRoles(): string[] {
    const payload = this.getTokenPayload();
    if (!payload) return [];

    const roleClaim =
      payload['role'] ??
      payload['roles'] ??
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    if (!roleClaim) return [];
    if (Array.isArray(roleClaim)) {
      return roleClaim.map((r) => String(r));
    }
    return [String(roleClaim)];
  }

  isAdmin(): boolean {
    return this.getUserRoles().includes('Admin');
  }

  private getTokenPayload(): Record<string, any> | null {
    const token = this.getToken();
    if (!token) return null;

    const parts = token.split('.');
    if (parts.length !== 3) return null;

    try {
      const payload = this.decodeBase64Url(parts[1]);
      return JSON.parse(payload);
    } catch {
      return null;
    }
  }

  private decodeBase64Url(input: string): string {
    const base64 = input.replace(/-/g, '+').replace(/_/g, '/');
    const padLength = (4 - (base64.length % 4)) % 4;
    const padded = base64 + '='.repeat(padLength);
    return atob(padded);
  }
}
