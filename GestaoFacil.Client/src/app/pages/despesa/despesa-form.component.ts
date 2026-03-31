import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DespesaService } from './despesa.service';
import { Despesa } from '../../models/despesa.model';
import { AuthService } from '../../auth/auth.service';
import { LookupService } from '../../shared/lookup.service';

@Component({
  selector: 'app-despesa-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './despesa-form.component.html',
  styleUrls: ['./despesa-form.component.scss']
})
export class DespesaFormComponent implements OnInit {
  @Input() despesa: Despesa | null = null;
  @Input() isEdit = false;
  @Output() salvo = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  nome: string = '';
  data = new Date().toISOString().substring(0, 10);
  valor: number | null = null;
  descricao = '';
  formaPagamentoId: number | null = null;
  categoriaDespesaId: number | null = null;

  errorMsg = '';
  formasPagamento;
  categoriasDespesa;

  constructor(
    private svc: DespesaService,
    private lookup: LookupService
  ) {
    this.formasPagamento = this.lookup.formasPagamento;
    this.categoriasDespesa = this.lookup.categoriasDespesa;
  }

  ngOnInit() {
    if (this.despesa && this.isEdit) {
      this.nome = this.despesa.nome ?? '';
      this.data = this.despesa.data ? this.despesa.data.substring(0, 10) : new Date().toISOString().substring(0, 10);
      this.valor = this.despesa.valor;
      this.descricao = this.despesa.descricao ?? '';
      this.categoriaDespesaId = this.despesa.categoriaDespesaId ?? null;
      this.formaPagamentoId = this.despesa.formaPagamentoId ?? null;
    }
  }

  salvarDespesa() {
    this.errorMsg = '';

    if (!this.data || this.valor === null || this.valor === undefined || !this.categoriaDespesaId || !this.formaPagamentoId) {
      this.errorMsg = 'Preencha Data, Valor, Categoria e Forma de Pagamento.';
      return;
    }

    const payload: Despesa = {
      nome: this.nome,
      data: new Date(this.data).toISOString(),
      descricao: this.descricao,
      valor: Number(this.valor),
      categoriaDespesaId: this.categoriaDespesaId,
      formaPagamentoId: this.formaPagamentoId
    };

    if (this.isEdit && this.despesa?.id) {
      this.svc.update(this.despesa.id, payload).subscribe({
        next: () => this.salvo.emit(),
        error: (err) => this.errorMsg = AuthService.parseError(err, 'Erro ao atualizar despesa.')
      });
    } else {
      this.svc.create(payload).subscribe({
        next: () => this.salvo.emit(),
        error: (err) => this.errorMsg = AuthService.parseError(err, 'Erro ao criar despesa.')
      });
    }
  }

  limpar() {
    this.nome = '';
    this.data = new Date().toISOString().substring(0, 10);
    this.valor = null;
    this.descricao = '';
    this.formaPagamentoId = null;
    this.categoriaDespesaId = null;
    this.errorMsg = '';
  }

  cancelar() {
    this.cancelado.emit();
  }
}
