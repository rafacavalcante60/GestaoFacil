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
    return this.http.get<ApiResponse<Categoria[]>>(this.baseDespesa).pipe(
      map((res) => this.extractList(res))
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
    return this.http.get<ApiResponse<Categoria[]>>(this.baseReceita).pipe(
      map((res) => this.extractList(res))
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
    const data = this.normalizeCategoria(res?.dados ?? res?.Dados);
    if (!data) {
      throw new Error('Resposta inválida.');
    }
    return data;
  }

  private extractList(res: ApiResponse<Categoria[]>): Categoria[] {
    const data = res?.dados ?? res?.Dados;
    if (!Array.isArray(data)) {
      return [];
    }

    return data
      .map((item) => this.normalizeCategoria(item))
      .filter((item): item is Categoria => item !== null);
  }

  private normalizeCategoria(item: unknown): Categoria | null {
    if (!item || typeof item !== 'object') {
      return null;
    }

    const data = item as Record<string, unknown>;
    const rawId = data['id'] ?? data['Id'];
    const id = typeof rawId === 'number' ? rawId : Number(rawId);
    const rawNome = data['nome'] ?? data['Nome'];
    const nome = typeof rawNome === 'string' ? rawNome.trim() : '';
    const rawAtivo = data['ativo'] ?? data['Ativo'];

    if (!Number.isFinite(id) || id <= 0 || !nome) {
      return null;
    }

    const categoria: Categoria = { id, nome };
    if (typeof rawAtivo === 'boolean') {
      categoria.ativo = rawAtivo;
    }

    return categoria;
  }
}
