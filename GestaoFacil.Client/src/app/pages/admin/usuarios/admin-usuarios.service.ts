import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Usuario } from '../../../models/usuario.model';

interface ApiResponse<T> {
  dados?: T;
  Dados?: T;
  mensagem?: string;
  Mensagem?: string;
  status?: boolean;
  Status?: boolean;
}

export interface PaginationMeta {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
  hasNext: boolean;
  hasPrevious: boolean;
}

export interface UsuarioPageResult {
  items: Usuario[];
  pagination: PaginationMeta | null;
  mensagem: string;
  status: boolean;
}

type UsuarioQuery = {
  PageNumber?: number;
  PageSize?: number;
};

@Injectable({ providedIn: 'root' })
export class AdminUsuariosService {
  private base = 'https://localhost:7285/api/Usuario';

  constructor(private http: HttpClient) {}

  pagination(query: UsuarioQuery = {}): Observable<UsuarioPageResult> {
    const params = this.buildParams(query);
    return this.http
      .get<ApiResponse<Usuario[]>>(`${this.base}/pagination`, { params, observe: 'response' })
      .pipe(map((resp) => this.mapPageResult(resp.body, resp.headers)));
  }

  update(id: number, payload: Usuario): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.base}/${id}`, payload);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.base}/${id}`);
  }

  private mapPageResult(body: ApiResponse<Usuario[]> | null, headers: HttpHeaders): UsuarioPageResult {
    return {
      items: body ? this.extractList(body) : [],
      pagination: this.readPagination(headers),
      mensagem: body ? this.extractMessage(body) : '',
      status: body ? this.extractStatus(body) : true
    };
  }

  private extractList(res: ApiResponse<Usuario[]>): Usuario[] {
    const data = res?.dados ?? res?.Dados;
    return Array.isArray(data) ? data : [];
  }

  private extractMessage(res: ApiResponse<unknown>): string {
    return res?.mensagem ?? res?.Mensagem ?? '';
  }

  private extractStatus(res: ApiResponse<unknown>): boolean {
    return res?.status ?? res?.Status ?? true;
  }

  private buildParams(query: UsuarioQuery): HttpParams {
    let params = new HttpParams();
    Object.keys(query).forEach((key) => {
      const value = (query as any)[key];
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    });
    return params;
  }

  private readPagination(headers: HttpHeaders): PaginationMeta | null {
    const raw = headers.get('X-Pagination');
    if (!raw) return null;

    try {
      const data = JSON.parse(raw);
      return {
        currentPage: Number(data.CurrentPage ?? data.currentPage ?? 1),
        totalPages: Number(data.TotalPages ?? data.totalPages ?? 1),
        pageSize: Number(data.PageSize ?? data.pageSize ?? 10),
        totalCount: Number(data.TotalCount ?? data.totalCount ?? 0),
        hasNext: Boolean(data.HasNext ?? data.hasNext ?? false),
        hasPrevious: Boolean(data.HasPrevious ?? data.hasPrevious ?? false)
      };
    } catch {
      return null;
    }
  }
}
