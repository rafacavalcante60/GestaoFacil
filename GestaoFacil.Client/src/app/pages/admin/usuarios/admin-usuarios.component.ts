import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Usuario } from '../../../models/usuario.model';
import { AdminUsuariosService } from './admin-usuarios.service';

@Component({
  selector: 'app-admin-usuarios',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-usuarios.component.html',
  styleUrl: './admin-usuarios.component.scss'
})
export class AdminUsuariosComponent implements OnInit {
  usuarios: Usuario[] = [];
  loading = false;
  saving = false;
  errorMsg = '';
  infoMsg = '';

  pageNumber = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;
  pageSizeOptions = [10, 20, 30, 50];

  filtroTexto = '';

  selected: Usuario | null = null;
  form = {
    nome: '',
    email: '',
    tipoUsuarioId: 1
  };

  tiposUsuario = [
    { id: 1, nome: 'Comum' },
    { id: 2, nome: 'Admin' }
  ];

  constructor(private svc: AdminUsuariosService, private router: Router) {}

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.loading = true;
    this.errorMsg = '';

    this.svc
      .pagination({
        PageNumber: this.pageNumber,
        PageSize: this.pageSize
      })
      .subscribe({
        next: (res) => {
          this.usuarios = res.items ?? [];
          if (res.pagination) {
            this.pageNumber = res.pagination.currentPage;
            this.totalPages = Math.max(1, res.pagination.totalPages);
            this.totalCount = res.pagination.totalCount;
          } else {
            this.totalPages = this.usuarios.length < this.pageSize ? this.pageNumber : this.pageNumber + 1;
            this.totalCount = this.usuarios.length;
          }
          this.loading = false;
        },
        error: (err) => {
          this.errorMsg = err.error?.mensagem || err.error?.message || 'Erro ao carregar usuarios.';
          this.loading = false;
        }
      });
  }

  get usuariosFiltrados(): Usuario[] {
    const q = this.filtroTexto.trim().toLowerCase();
    if (!q) return this.usuarios;
    return this.usuarios.filter((u) =>
      (u.nome || '').toLowerCase().includes(q) || (u.email || '').toLowerCase().includes(q)
    );
  }

  paginaAnterior(): void {
    if (this.pageNumber <= 1) return;
    this.pageNumber--;
    this.carregar();
  }

  proximaPagina(): void {
    if (this.pageNumber >= this.totalPages) return;
    this.pageNumber++;
    this.carregar();
  }

  mudarPageSize(): void {
    this.pageNumber = 1;
    this.carregar();
  }

  editar(item: Usuario): void {
    this.selected = item;
    this.form = {
      nome: item.nome,
      email: item.email,
      tipoUsuarioId: item.tipoUsuarioId
    };
    this.infoMsg = '';
    this.errorMsg = '';
  }

  cancelarEdicao(): void {
    this.selected = null;
    this.infoMsg = '';
    this.errorMsg = '';
  }

  salvar(): void {
    if (!this.selected) return;
    if (!this.form.nome.trim() || !this.form.email.trim()) {
      this.errorMsg = 'Nome e email são obrigatórios.';
      return;
    }

    this.saving = true;
    const payload: Usuario = {
      id: this.selected.id,
      nome: this.form.nome.trim(),
      email: this.form.email.trim(),
      tipoUsuarioId: Number(this.form.tipoUsuarioId)
    };

    this.svc.update(payload.id, payload).subscribe({
      next: (res) => {
        this.infoMsg = res?.mensagem || 'Usuario atualizado com sucesso.';
        this.saving = false;
        this.selected = null;
        this.carregar();
      },
      error: (err) => {
        this.errorMsg = err.error?.mensagem || err.error?.message || 'Erro ao atualizar usuario.';
        this.saving = false;
      }
    });
  }

  excluir(item: Usuario): void {
    const ok = window.confirm(`Deseja excluir o usuario "${item.nome}"?`);
    if (!ok) return;

    this.svc.delete(item.id).subscribe({
      next: (res) => {
        this.infoMsg = res?.mensagem || 'Usuario excluido com sucesso.';
        if (this.usuarios.length === 1 && this.pageNumber > 1) {
          this.pageNumber--;
        }
        this.carregar();
      },
      error: (err) => {
        this.errorMsg = err.error?.mensagem || err.error?.message || 'Erro ao excluir usuario.';
      }
    });
  }

  nomeTipo(id: number): string {
    return this.tiposUsuario.find((t) => t.id === id)?.nome ?? '-';
  }

  irAtividades(): void {
    this.router.navigate(['/atividades']);
  }
}
