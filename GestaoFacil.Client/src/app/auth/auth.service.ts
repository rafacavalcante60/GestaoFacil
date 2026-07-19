import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError, map } from 'rxjs';
import { environment } from '../../environments/environment';

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
  Dados?: {
    token?: string;
    refreshToken?: string;
    expiraEm?: string; // ISO
  };
  mensagem?: string;
  Mensagem?: string;
  status?: boolean;
  Status?: boolean;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = `${environment.apiUrl}/Auth`;

  constructor(private http: HttpClient, private router: Router) {}

  // login adaptado ao formato que você mostrou
  login(data: LoginRequest): Observable<LoginResponseRaw> {
    return this.http.post<LoginResponseRaw>(`${this.baseUrl}/login`, data).pipe(
      tap(res => {
        const token = this.salvarSessao(res);
        if (!token) {
          console.error('Login response não contém token em dados.token', res);
        }
      }),
      catchError(err => {
        console.error('Erro no login HTTP:', err);
        return throwError(() => err);
      })
    );
  }

  // Troca o refresh token por um par novo. O backend rotaciona: a resposta traz
  // um refreshToken novo e revoga o anterior, entao e obrigatorio regravar os dois.
  refreshSession(): Observable<string> {
    const refreshToken = this.getRefreshToken();

    if (!refreshToken) {
      return throwError(() => new Error('Sem refresh token disponível.'));
    }

    return this.http.post<LoginResponseRaw>(`${this.baseUrl}/refresh`, { refreshToken }).pipe(
      map(res => {
        const token = this.salvarSessao(res);
        if (!token) {
          throw new Error('Refresh não retornou token.');
        }
        return token;
      })
    );
  }

  // Grava token/refreshToken/expiraEm e devolve o access token (null se a resposta
  // nao trouxe token, caso em que a sessao e limpa).
  private salvarSessao(res: LoginResponseRaw): string | null {
    const dados = res?.dados ?? res?.Dados;
    const token = dados?.token;
    const refreshToken = dados?.refreshToken;
    const expiraEm = dados?.expiraEm;

    if (!token) {
      this.limparSessao();
      return null;
    }

    localStorage.setItem('token', token);
    if (refreshToken) localStorage.setItem('refreshToken', refreshToken);
    if (expiraEm) localStorage.setItem('tokenExpiraEm', expiraEm);
    return token;
  }

  private limparSessao(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('tokenExpiraEm');
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
    this.limparSessao();
    this.router.navigate(['/auth/login']);
  }

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

  static parseError(err: any, fallback = 'Algo deu errado. Tente novamente.'): string {
    if (err.status === 0) return 'Não foi possível conectar ao servidor.';
    if (err.status === 429) return 'Você está fazendo requisições muito rapidamente. Por favor, aguarde um momento e tente novamente.';
    const errorPayload = err?.error;

    if (typeof errorPayload === 'string' && errorPayload.trim()) {
      return errorPayload;
    }

    const modelErrors = errorPayload?.errors;
    if (modelErrors && typeof modelErrors === 'object') {
      const firstError = Object.values(modelErrors)
        .flatMap((messages: unknown) => Array.isArray(messages) ? messages : [])
        .find((message): message is string => typeof message === 'string' && !!message.trim());

      if (firstError) {
        return firstError;
      }
    }

    return errorPayload?.mensagem
      || errorPayload?.Mensagem
      || errorPayload?.message
      || errorPayload?.detail
      || errorPayload?.title
      || fallback;
  }

  private decodeBase64Url(input: string): string {
    const base64 = input.replace(/-/g, '+').replace(/_/g, '/');
    const padLength = (4 - (base64.length % 4)) % 4;
    const padded = base64 + '='.repeat(padLength);
    return atob(padded);
  }
}
