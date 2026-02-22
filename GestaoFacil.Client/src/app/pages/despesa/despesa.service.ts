import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Despesa } from '../../models/despesa.model';

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

export interface DespesaPageResult {
  items: Despesa[];
  pagination: PaginationMeta | null;
  mensagem: string;
  status: boolean;
}

type DespesaQuery = {
  PageNumber?: number;
  PageSize?: number;
  BuscaTexto?: string;
  ValorMin?: number;
  ValorMax?: number;
  DataInicial?: string;
  DataFinal?: string;
  CategoriaDespesaId?: number;
  FormaPagamentoId?: number;
};

@Injectable({ providedIn: 'root' })
export class DespesaService {
  private base = 'https://localhost:7285/api/Despesa';

  constructor(private http: HttpClient) {}

  get(id: number): Observable<Despesa> {
    return this.http.get<ApiResponse<Despesa>>(`${this.base}/${id}`).pipe(
      map((res) => this.extractData(res))
    );
  }

  create(d: Despesa): Observable<ApiResponse<Despesa>> {
    return this.http.post<ApiResponse<Despesa>>(this.base, d);
  }

  update(id: number, d: Despesa): Observable<ApiResponse<boolean>> {
    const payload = { ...d, id };
    return this.http.put<ApiResponse<boolean>>(`${this.base}/${id}`, payload);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.base}/${id}`);
  }

  pagination(query: DespesaQuery = {}): Observable<DespesaPageResult> {
    const params = this.buildParams(query);
    return this.http
      .get<ApiResponse<Despesa[]>>(`${this.base}/pagination`, { params, observe: 'response' })
      .pipe(map((resp) => this.mapPageResult(resp.body, resp.headers)));
  }

  filterPagination(query: DespesaQuery = {}): Observable<DespesaPageResult> {
    const params = this.buildParams(query);
    return this.http
      .post<ApiResponse<Despesa[]>>(`${this.base}/filter/pagination`, {}, { params, observe: 'response' })
      .pipe(map((resp) => this.mapPageResult(resp.body, resp.headers)));
  }

  exportExcel(query: DespesaQuery = {}): Observable<Blob> {
    const params = this.buildParams(query);
    return this.http.post(`${this.base}/filter/export-excel-completo`, {}, { params, responseType: 'blob' });
  }

  private mapPageResult(body: ApiResponse<Despesa[]> | null, headers: HttpHeaders): DespesaPageResult {
    return {
      items: body ? this.extractList(body) : [],
      pagination: this.readPagination(headers),
      mensagem: body ? this.extractMessage(body) : '',
      status: body ? this.extractStatus(body) : true
    };
  }

  private extractData(res: ApiResponse<Despesa>): Despesa {
    const data = res?.dados ?? res?.Dados;
    if (!data) {
      throw new Error('Resposta inválida ao obter despesa.');
    }
    return data;
  }

  private extractList(res: ApiResponse<Despesa[]>): Despesa[] {
    const data = res?.dados ?? res?.Dados;
    return Array.isArray(data) ? data : [];
  }

  private extractMessage(res: ApiResponse<unknown>): string {
    return res?.mensagem ?? res?.Mensagem ?? '';
  }

  private extractStatus(res: ApiResponse<unknown>): boolean {
    return res?.status ?? res?.Status ?? true;
  }

  private buildParams(query: DespesaQuery): HttpParams {
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
