import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Receita } from '../../models/receita.model';

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

export interface ReceitaPageResult {
  items: Receita[];
  pagination: PaginationMeta | null;
  mensagem: string;
  status: boolean;
}

type ReceitaQuery = {
  PageNumber?: number;
  PageSize?: number;
  BuscaTexto?: string;
  ValorMin?: number;
  ValorMax?: number;
  DataInicial?: string;
  DataFinal?: string;
  CategoriaReceitaId?: number;
  FormaPagamentoId?: number;
};

@Injectable({ providedIn: 'root' })
export class ReceitaService {
  private base = 'https://localhost:7285/api/Receita';

  constructor(private http: HttpClient) {}

  get(id: number): Observable<Receita> {
    return this.http.get<ApiResponse<Receita>>(`${this.base}/${id}`).pipe(
      map((res) => this.extractData(res))
    );
  }

  create(r: Receita): Observable<ApiResponse<Receita>> {
    return this.http.post<ApiResponse<Receita>>(this.base, r);
  }

  update(id: number, r: Receita): Observable<ApiResponse<boolean>> {
    const payload = { ...r, id };
    return this.http.put<ApiResponse<boolean>>(`${this.base}/${id}`, payload);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.base}/${id}`);
  }

  pagination(query: ReceitaQuery = {}): Observable<ReceitaPageResult> {
    const params = this.buildParams(query);
    return this.http
      .get<ApiResponse<Receita[]>>(`${this.base}/pagination`, { params, observe: 'response' })
      .pipe(map((resp) => this.mapPageResult(resp.body, resp.headers)));
  }

  filterPagination(query: ReceitaQuery = {}): Observable<ReceitaPageResult> {
    const params = this.buildParams(query);
    return this.http
      .post<ApiResponse<Receita[]>>(`${this.base}/filter/pagination`, {}, { params, observe: 'response' })
      .pipe(map((resp) => this.mapPageResult(resp.body, resp.headers)));
  }

  exportExcel(query: ReceitaQuery = {}): Observable<Blob> {
    const params = this.buildParams(query);
    return this.http.post(`${this.base}/filter/export-excel-completo`, {}, { params, responseType: 'blob' });
  }

  private mapPageResult(body: ApiResponse<Receita[]> | null, headers: HttpHeaders): ReceitaPageResult {
    return {
      items: body ? this.extractList(body) : [],
      pagination: this.readPagination(headers),
      mensagem: body ? this.extractMessage(body) : '',
      status: body ? this.extractStatus(body) : true
    };
  }

  private extractData(res: ApiResponse<Receita>): Receita {
    const data = res?.dados ?? res?.Dados;
    if (!data) {
      throw new Error('Resposta inválida ao obter receita.');
    }
    return data;
  }

  private extractList(res: ApiResponse<Receita[]>): Receita[] {
    const data = res?.dados ?? res?.Dados;
    return Array.isArray(data) ? data : [];
  }

  private extractMessage(res: ApiResponse<unknown>): string {
    return res?.mensagem ?? res?.Mensagem ?? '';
  }

  private extractStatus(res: ApiResponse<unknown>): boolean {
    return res?.status ?? res?.Status ?? true;
  }

  private buildParams(query: ReceitaQuery): HttpParams {
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
