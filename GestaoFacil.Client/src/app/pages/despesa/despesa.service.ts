import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Despesa } from '../../models/despesa.model';

@Injectable({ providedIn: 'root' })
export class DespesaService {
  private base = 'https://localhost:7285/api/Despesa';

  constructor(private http: HttpClient) {}

  // GET /api/Despesa/{id}
  get(id: number): Observable<Despesa> {
    return this.http.get<Despesa>(`${this.base}/${id}`);
  }

  // POST /api/Despesa
  create(d: Despesa): Observable<any> {
    return this.http.post(this.base, d);
  }

  // PUT /api/Despesa/{id}
  update(id: number, d: Despesa): Observable<any> {
    return this.http.put(`${this.base}/${id}`, d);
  }

  // DELETE /api/Despesa/{id}
  delete(id: number): Observable<any> {
    return this.http.delete(`${this.base}/${id}`);
  }

  // GET /api/Despesa/pagination  (exemplo com query params)
  pagination(query: {
    PageNumber?: number;
    PageSize?: number;
    BuscaTexto?: string;
    ValorMin?: number;
    ValorMax?: number;
    DataInicial?: string;
    DataFinal?: string;
    CategoriaDespesaId?: number;
    FormaPagamentoId?: number;
  } = {}): Observable<any> {
    let params = new HttpParams();
    Object.keys(query).forEach(key => {
      const v = (query as any)[key];
      if (v !== undefined && v !== null && v !== '') params = params.set(key, String(v));
    });
    return this.http.get(`${this.base}/pagination`, { params });
  }

  // POST /api/Despesa/filter/pagination (body empty, uses query string in swagger)
  filterPagination(query: {
    PageNumber?: number;
    PageSize?: number;
    BuscaTexto?: string;
    ValorMin?: number;
    ValorMax?: number;
    DataInicial?: string;
    DataFinal?: string;
    CategoriaDespesaId?: number;
    FormaPagamentoId?: number;
  } = {}): Observable<any> {
    let params = new HttpParams();
    Object.keys(query).forEach(key => {
      const v = (query as any)[key];
      if (v !== undefined && v !== null && v !== '') params = params.set(key, String(v));
    });
    return this.http.post(`${this.base}/filter/pagination`, {}, { params });
  }

  // POST /api/Despesa/filter/export-excel-completo -> returns blob
  exportExcel(query: {
    PageNumber?: number;
    PageSize?: number;
    BuscaTexto?: string;
    ValorMin?: number;
    ValorMax?: number;
    DataInicial?: string;
    DataFinal?: string;
    CategoriaDespesaId?: number;
    FormaPagamentoId?: number;
  } = {}): Observable<Blob> {
    let params = new HttpParams();
    Object.keys(query).forEach(key => {
      const v = (query as any)[key];
      if (v !== undefined && v !== null && v !== '') params = params.set(key, String(v));
    });
    return this.http.post(`${this.base}/filter/export-excel-completo`, {}, { params, responseType: 'blob' });
  }
}
