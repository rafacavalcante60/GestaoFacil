import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';

interface ApiResponse<T> {
  dados: T;
  mensagem: string;
  status: boolean;
}

@Injectable({ providedIn: 'root' })
export class ReportService {
  private base = 'https://localhost:7285/api/Relatorios';
  private despesaBase = 'https://localhost:7285/api/Despesa';
  private receitaBase = 'https://localhost:7285/api/Receita';

  constructor(private http: HttpClient) {}

  private buildParams(obj: any): HttpParams {
    let params = new HttpParams();
    Object.keys(obj || {}).forEach(k => {
      const v = (obj as any)[k];
      if (v !== undefined && v !== null && v !== '') {
        params = params.set(k, String(v));
      }
    });
    return params;
  }

  // retorna apenas o campo .dados do response do backend
  resumo(query: { inicio?: string; fim?: string } = {}): Observable<{ totalReceitas: number; totalDespesas: number; saldo: number }> {
    const params = this.buildParams(query);
    return this.http.get<ApiResponse<any>>(`${this.base}/resumo`, { params }).pipe(map(r => r.dados));
  }

  categoria(query: { inicio?: string; fim?: string; despesas?: boolean } = {}): Observable<Array<{ categoria: string; total: number }>> {
    const params = this.buildParams(query);
    return this.http.get<ApiResponse<any>>(`${this.base}/categoria`, { params }).pipe(map(r => r.dados || []));
  }

  fluxo(query: { inicio?: string; fim?: string } = {}): Observable<Array<{ data: string; saldoAcumulado: number }>> {
    const params = this.buildParams(query);
    return this.http.get<ApiResponse<any>>(`${this.base}/fluxo`, { params }).pipe(map(r => r.dados || []));
  }

  mensal(query: { ano?: number } = {}): Observable<Array<{ mes: number; totalReceitas: number; totalDespesas: number; saldo: number }>> {
    const params = this.buildParams(query);
    return this.http.get<ApiResponse<any>>(`${this.base}/mensal`, { params }).pipe(map(r => r.dados || []));
  }

  exportDespesa(query: any = {}): Observable<Blob> {
    const params = this.buildParams(query);
    return this.http.post(`${this.despesaBase}/filter/export-excel-completo`, {}, { params, responseType: 'blob' });
  }

  exportReceita(query: any = {}): Observable<Blob> {
    const params = this.buildParams(query);
    return this.http.post(`${this.receitaBase}/filter/export-excel-completo`, {}, { params, responseType: 'blob' });
  }
}
