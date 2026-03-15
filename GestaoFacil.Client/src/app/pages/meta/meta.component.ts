import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MetaService } from './meta.service';
import { Meta } from '../../models/meta.model';
import { AuthService } from '../../auth/auth.service';
import { LookupService } from '../../shared/lookup.service';

@Component({
  selector: 'app-meta',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './meta.component.html',
  styleUrls: ['./meta.component.scss']
})
export class MetaComponent {
  nome = '';
  valorMeta: number | null = null;
  tipo: 1 | 2 = 1;
  categoriaDespesaId: number | null = null;
  categoriaReceitaId: number | null = null;
  dataInicio = new Date().toISOString().substring(0, 10);
  dataFim = new Date().toISOString().substring(0, 10);
  ativo = true;

  errorMsg = '';
  isEdit = false;
  id?: number;

  categoriasDespesa;
  categoriasReceita;

  constructor(
    private svc: MetaService,
    private router: Router,
    private route: ActivatedRoute,
    private lookup: LookupService
  ) {
    this.categoriasDespesa = this.lookup.categoriasDespesa;
    this.categoriasReceita = this.lookup.categoriasReceita;
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEdit = true;
      this.id = Number(idParam);
      this.loadMeta(this.id);
    }
  }

  private loadMeta(id: number) {
    this.svc.getById(id).subscribe({
      next: (m: Meta) => {
        this.nome = m.nome;
        this.valorMeta = m.valorMeta;
        this.tipo = m.tipo;
        this.categoriaDespesaId = m.categoriaDespesaId ?? null;
        this.categoriaReceitaId = m.categoriaReceitaId ?? null;
        this.dataInicio = m.dataInicio.substring(0, 10);
        this.dataFim = m.dataFim.substring(0, 10);
        this.ativo = m.ativo;
      },
      error: () => {
        this.errorMsg = 'Erro ao carregar meta para edição.';
      }
    });
  }

  salvar() {
    this.errorMsg = '';

    if (!this.nome || this.valorMeta === null || !this.dataInicio || !this.dataFim) {
      this.errorMsg = 'Preencha Nome, Valor, Data de Início e Data de Fim.';
      return;
    }

    if (this.valorMeta <= 0) {
      this.errorMsg = 'O valor da meta deve ser maior que zero.';
      return;
    }

    if (new Date(this.dataInicio) > new Date(this.dataFim)) {
      this.errorMsg = 'A data de início não pode ser maior que a data de fim.';
      return;
    }

    const payload: Partial<Meta> = {
      nome: this.nome,
      valorMeta: Number(this.valorMeta),
      tipo: this.tipo,
      categoriaDespesaId: this.tipo === 1 ? (this.categoriaDespesaId ?? undefined) : undefined,
      categoriaReceitaId: this.tipo === 2 ? (this.categoriaReceitaId ?? undefined) : undefined,
      dataInicio: new Date(this.dataInicio).toISOString(),
      dataFim: new Date(this.dataFim).toISOString(),
      ativo: this.ativo
    };

    if (this.isEdit && this.id) {
      this.svc.update(this.id, payload).subscribe({
        next: () => this.router.navigate(['/meta']),
        error: (err) => this.errorMsg = AuthService.parseError(err, 'Erro ao atualizar meta.')
      });
    } else {
      this.svc.create(payload).subscribe({
        next: () => this.router.navigate(['/meta']),
        error: (err) => this.errorMsg = AuthService.parseError(err, 'Erro ao criar meta.')
      });
    }
  }

  limpar() {
    this.nome = '';
    this.valorMeta = null;
    this.tipo = 1;
    this.categoriaDespesaId = null;
    this.categoriaReceitaId = null;
    this.dataInicio = new Date().toISOString().substring(0, 10);
    this.dataFim = new Date().toISOString().substring(0, 10);
    this.ativo = true;
    this.errorMsg = '';
  }

  voltar() {
    this.router.navigate(['/meta']);
  }
}
