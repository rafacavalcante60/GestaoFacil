import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Meta } from '../../models/meta.model';
import { environment } from '../../../environments/environment';

interface ApiResponse<T> {
  dados?: T;
  Dados?: T;
  mensagem?: string;
  status?: boolean;
}

@Injectable({ providedIn: 'root' })
export class MetaService {
  private base = `${environment.apiUrl}/Meta`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Meta[]> {
    return this.http.get<ApiResponse<Meta[]>>(this.base).pipe(
      map((res) => res?.dados ?? res?.Dados ?? [])
    );
  }

  getAtivas(): Observable<Meta[]> {
    return this.http.get<ApiResponse<Meta[]>>(`${this.base}/ativas`).pipe(
      map((res) => res?.dados ?? res?.Dados ?? [])
    );
  }

  getById(id: number): Observable<Meta> {
    return this.http.get<ApiResponse<Meta>>(`${this.base}/${id}`).pipe(
      map((res) => {
        const data = res?.dados ?? res?.Dados;
        if (!data) throw new Error('Meta não encontrada.');
        return data;
      })
    );
  }

  create(dto: Partial<Meta>): Observable<ApiResponse<Meta>> {
    return this.http.post<ApiResponse<Meta>>(this.base, dto);
  }

  update(id: number, dto: Partial<Meta>): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.base}/${id}`, { ...dto, id });
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.base}/${id}`);
  }
}
