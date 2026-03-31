import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MetaService } from './meta.service';
import { Meta } from '../../models/meta.model';
import { AuthService } from '../../auth/auth.service';
import { LookupService } from '../../shared/lookup.service';

@Component({
  selector: 'app-meta-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './meta-form.component.html',
  styleUrls: ['./meta-form.component.scss']
})
export class MetaFormComponent implements OnInit {
  @Input() meta: Meta | null = null;
  @Input() isEdit = false;
  @Output() salvo = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  nome = '';
  valorMeta: number | null = null;
  tipo: 1 | 2 = 1;
  categoriaDespesaId: number | null = null;
  categoriaReceitaId: number | null = null;
  dataInicio = new Date().toISOString().substring(0, 10);
  dataFim = new Date().toISOString().substring(0, 10);
  ativo = true;

  errorMsg = '';
  categoriasDespesa;
  categoriasReceita;

  constructor(
    private svc: MetaService,
    private lookup: LookupService
  ) {
    this.categoriasDespesa = this.lookup.categoriasDespesa;
    this.categoriasReceita = this.lookup.categoriasReceita;
  }

  ngOnInit() {
    if (this.meta && this.isEdit) {
      this.nome = this.meta.nome;
      this.valorMeta = this.meta.valorMeta;
      this.tipo = this.meta.tipo;
      this.categoriaDespesaId = this.meta.categoriaDespesaId ?? null;
      this.categoriaReceitaId = this.meta.categoriaReceitaId ?? null;
      this.dataInicio = this.meta.dataInicio.substring(0, 10);
      this.dataFim = this.meta.dataFim.substring(0, 10);
      this.ativo = this.meta.ativo;
    }
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

    if (this.isEdit && this.meta?.id) {
      this.svc.update(this.meta.id, payload).subscribe({
        next: () => this.salvo.emit(),
        error: (err) => this.errorMsg = AuthService.parseError(err, 'Erro ao atualizar meta.')
      });
    } else {
      this.svc.create(payload).subscribe({
        next: () => this.salvo.emit(),
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

  cancelar() {
    this.cancelado.emit();
  }
}
