// src/app/pages/receita/receita.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Receita } from '../../models/receita.model';

@Injectable({ providedIn: 'root' })
export class ReceitaService {
  private base = 'https://localhost:7285/api/Receita';

  constructor(private http: HttpClient) {}

  // GET /api/Receita/{id}
  get(id: number): Observable<Receita> {
    return this.http.get<Receita>(`${this.base}/${id}`);
  }

  // POST /api/Receita
  create(r: Receita): Observable<any> {
    return this.http.post(this.base, r);
  }

  // PUT /api/Receita/{id}
  update(id: number, r: Receita): Observable<any> {
    return this.http.put(`${this.base}/${id}`, r);
  }

  // DELETE /api/Receita/{id}
  delete(id: number): Observable<any> {
    return this.http.delete(`${this.base}/${id}`);
  }

  // GET /api/Receita/pagination (exemplo com query params)
  pagination(query: {
    PageNumber?: number;
    PageSize?: number;
    BuscaTexto?: string;
    ValorMin?: number;
    ValorMax?: number;
    DataInicial?: string;
    DataFinal?: string;
    CategoriaReceitaId?: number;
    FormaPagamentoId?: number;
  } = {}): Observable<any> {
    let params = new HttpParams();
    Object.keys(query).forEach(key => {
      const v = (query as any)[key];
      if (v !== undefined && v !== null && v !== '') params = params.set(key, String(v));
    });
    return this.http.get(`${this.base}/pagination`, { params });
  }

  // POST /api/Receita/filter/pagination (body empty, uses query params per swagger style)
  filterPagination(query: {
    PageNumber?: number;
    PageSize?: number;
    BuscaTexto?: string;
    ValorMin?: number;
    ValorMax?: number;
    DataInicial?: string;
    DataFinal?: string;
    CategoriaReceitaId?: number;
    FormaPagamentoId?: number;
  } = {}): Observable<any> {
    let params = new HttpParams();
    Object.keys(query).forEach(key => {
      const v = (query as any)[key];
      if (v !== undefined && v !== null && v !== '') params = params.set(key, String(v));
    });
    return this.http.post(`${this.base}/filter/pagination`, {}, { params });
  }

  // POST /api/Receita/filter/export-excel-completo -> returns blob
  exportExcel(query: {
    PageNumber?: number;
    PageSize?: number;
    BuscaTexto?: string;
    ValorMin?: number;
    ValorMax?: number;
    DataInicial?: string;
    DataFinal?: string;
    CategoriaReceitaId?: number;
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
