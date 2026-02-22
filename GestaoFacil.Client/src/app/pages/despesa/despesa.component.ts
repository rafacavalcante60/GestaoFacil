import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DespesaService } from './despesa.service';
import { Despesa } from '../../models/despesa.model';

@Component({
  selector: 'app-despesa',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './despesa.component.html',
  styleUrls: ['./despesa.component.scss'] // opcional: remova se não usar
})
export class DespesaComponent {
  // campos do formulário (bind com ngModel no HTML)
  nome: string = '';
  data = new Date().toISOString().substring(0, 10); // yyyy-mm-dd para input date
  valor: number | null = null;
  descricao = '';
  formaPagamentoId: number | null = null;
  categoriaDespesaId: number | null = null;

  // estado
  errorMsg = '';
  isEdit = false;
  id?: number;

  formasPagamento = [
    { id: 1, nome: 'Dinheiro' },
    { id: 2, nome: 'Cartão de Crédito' },
    { id: 3, nome: 'Cartão de Débito' },
    { id: 4, nome: 'Pix' },
    { id: 5, nome: 'Cheque' },
    { id: 6, nome: 'Boleto' },
    { id: 7, nome: 'Outro' }
  ];

  categoriasDespesa = [
    { id: 1, nome: 'Alimentação' },
    { id: 2, nome: 'Transporte' },
    { id: 3, nome: 'Moradia' },
    { id: 4, nome: 'Lazer' },
    { id: 5, nome: 'Educação' },
    { id: 6, nome: 'Saúde' },
    { id: 7, nome: 'Outra' }
  ];

  constructor(
    private svc: DespesaService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    // se existir id na rota, entra em modo edição
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEdit = true;
      this.id = Number(idParam);
      this.loadDespesa(this.id);
    }
  }

  private loadDespesa(id: number) {
    this.svc.get(id).subscribe({
      next: (d: Despesa) => {
        // adapta campos para o formulário
        // o backend geralmente retorna data em ISO, cortamos para yyyy-mm-dd
        this.data = d.data ? d.data.substring(0, 10) : new Date().toISOString().substring(0, 10);
        this.valor = d.valor;
        this.descricao = d.descricao ?? '';
        this.categoriaDespesaId = d.categoriaDespesaId ?? null;
        this.formaPagamentoId = d.formaPagamentoId ?? null;
      },
      error: () => {
        this.errorMsg = 'Erro ao carregar despesa para edição.';
      }
    });
  }

  salvarDespesa() {
    this.errorMsg = '';

    // validações básicas
    if (!this.data || this.valor === null || this.valor === undefined || !this.categoriaDespesaId || !this.formaPagamentoId) {
      this.errorMsg = 'Preencha Data, Valor, Categoria e Forma de Pagamento.';
      return;
    }

    // monta payload conforme swagger
    const payload: Despesa = {
      nome: this.nome,
      data: new Date(this.data).toISOString(),
      descricao: this.descricao,
      valor: Number(this.valor),
      categoriaDespesaId: this.categoriaDespesaId,
      formaPagamentoId: this.formaPagamentoId
    };

    if (this.isEdit && this.id) {
      this.svc.update(this.id, payload).subscribe({
        next: () => this.router.navigate(['/despesa']),
        error: (err) => this.errorMsg = err.error?.mensagem || err.error?.message || 'Erro ao atualizar despesa.'
      });
    } else {
      this.svc.create(payload).subscribe({
        next: () => {
          this.router.navigate(['/despesa']);
        },
        error: (err) => this.errorMsg = err.error?.mensagem || err.error?.message || 'Erro ao criar despesa.'
      });
    }
  }

  limpar() {
    this.data = new Date().toISOString().substring(0, 10);
    this.valor = null;
    this.descricao = '';
    this.formaPagamentoId = null;
    this.categoriaDespesaId = null;
    this.errorMsg = '';
    // se estava em edição e quiser sair do modo edição ao limpar:
    // this.isEdit = false; this.id = undefined;
  }

  voltar() {
    this.router.navigate(['/despesa']);
  }
}
