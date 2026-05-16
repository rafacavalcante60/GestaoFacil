import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Categoria } from '../models/categoria.model';
import { environment } from '../../environments/environment';

interface ApiResponse<T> {
  dados?: T;
  Dados?: T;
  mensagem?: string;
  Mensagem?: string;
  status?: boolean;
  Status?: boolean;
}

@Injectable({ providedIn: 'root' })
export class CategoriaService {
  private baseDespesa = `${environment.apiUrl}/CategoriaDespesa`;
  private baseReceita = `${environment.apiUrl}/CategoriaReceita`;

  constructor(private http: HttpClient) {}

  // DESPESA
  getDespesas(): Observable<Categoria[]> {
    console.log('Carregando despesas de:', this.baseDespesa);
    return this.http.get<ApiResponse<Categoria[]>>(this.baseDespesa).pipe(
      map((res) => {
        console.log('Resposta getDespesas:', res);
        return this.extractList(res);
      })
    );
  }

  getDespesa(id: number): Observable<Categoria> {
    return this.http.get<ApiResponse<Categoria>>(`${this.baseDespesa}/${id}`).pipe(
      map((res) => this.extractData(res))
    );
  }

  createDespesa(dto: Categoria): Observable<ApiResponse<Categoria>> {
    return this.http.post<ApiResponse<Categoria>>(this.baseDespesa, dto);
  }

  updateDespesa(id: number, dto: Categoria): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.baseDespesa}/${id}`, dto);
  }

  deleteDespesa(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseDespesa}/${id}`);
  }

  // RECEITA
  getReceitas(): Observable<Categoria[]> {
    console.log('Carregando receitas de:', this.baseReceita);
    return this.http.get<ApiResponse<Categoria[]>>(this.baseReceita).pipe(
      map((res) => {
        console.log('Resposta getReceitas:', res);
        return this.extractList(res);
      })
    );
  }

  getReceita(id: number): Observable<Categoria> {
    return this.http.get<ApiResponse<Categoria>>(`${this.baseReceita}/${id}`).pipe(
      map((res) => this.extractData(res))
    );
  }

  createReceita(dto: Categoria): Observable<ApiResponse<Categoria>> {
    return this.http.post<ApiResponse<Categoria>>(this.baseReceita, dto);
  }

  updateReceita(id: number, dto: Categoria): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.baseReceita}/${id}`, dto);
  }

  deleteReceita(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseReceita}/${id}`);
  }

  private extractData(res: ApiResponse<Categoria>): Categoria {
    const data = res?.dados ?? res?.Dados;
    if (!data) {
      throw new Error('Resposta inválida.');
    }
    return data;
  }

  private extractList(res: ApiResponse<Categoria[]>): Categoria[] {
    const data = res?.dados ?? res?.Dados;
    if (!Array.isArray(data)) return [];
    return data.filter((item): item is Categoria => {
      if (!item || typeof item !== 'object') return false;
      const id = (item as any).id ?? (item as any).Id;
      return typeof id === 'number' && id > 0;
    });
  }
}
