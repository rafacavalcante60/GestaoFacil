import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReceitaService } from './receita.service';
import { Receita } from '../../models/receita.model';
import { AuthService } from '../../auth/auth.service';
import { LookupService } from '../../shared/lookup.service';
import { CategoriaService } from '../../shared/categoria.service';
import { Categoria } from '../../models/categoria.model';

@Component({
  selector: 'app-receita-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './receita-form.component.html',
  styleUrls: ['./receita-form.component.scss']
})
export class ReceitaFormComponent implements OnInit, OnChanges {
  @Input() receita: Receita | null = null;
  @Input() isEdit = false;
  @Input() categoriaRefreshToken = 0;
  @Output() salvo = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();
  @Output() abrirCategorias = new EventEmitter<void>();

  nome: string = '';
  data = new Date().toISOString().substring(0, 10);
  valor: number | null = null;
  descricao = '';
  formaPagamentoId: number | null = null;
  categoriaReceitaId: number | null = null;

  errorMsg = '';
  formasPagamento;
  categoriasReceita: Categoria[];

  constructor(
    private svc: ReceitaService,
    private lookup: LookupService,
    private categoriaService: CategoriaService
  ) {
    this.formasPagamento = this.lookup.formasPagamento;
    this.categoriasReceita = this.lookup.categoriasReceita;
  }

  ngOnInit() {
    this.carregarCategorias();

    if (this.receita && this.isEdit) {
      this.nome = this.receita.nome ?? '';
      this.data = this.receita.data ? this.receita.data.substring(0, 10) : new Date().toISOString().substring(0, 10);
      this.valor = this.receita.valor;
      this.descricao = this.receita.descricao ?? '';
      this.categoriaReceitaId = this.receita.categoriaReceitaId ?? null;
      this.formaPagamentoId = this.receita.formaPagamentoId ?? null;
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['categoriaRefreshToken'] && !changes['categoriaRefreshToken'].firstChange) {
      this.carregarCategorias();
    }
  }

  salvarReceita() {
    this.errorMsg = '';

    if (!this.data || this.valor === null || this.valor === undefined || !this.categoriaReceitaId || !this.formaPagamentoId) {
      this.errorMsg = 'Preencha Data, Valor, Categoria e Forma de Pagamento.';
      return;
    }

    if (this.valor <= 0 || this.valor > 999999.99) {
      this.errorMsg = 'O valor deve estar entre 0,01 e 999.999,99.';
      return;
    }

    const payload: Receita = {
      nome: this.nome,
      data: new Date(this.data).toISOString(),
      descricao: this.descricao,
      valor: Number(this.valor),
      categoriaReceitaId: this.categoriaReceitaId,
      formaPagamentoId: this.formaPagamentoId
    };

    if (this.isEdit && this.receita?.id) {
      this.svc.update(this.receita.id, payload).subscribe({
        next: () => this.salvo.emit(),
        error: (err) => this.errorMsg = AuthService.parseError(err, 'Erro ao atualizar receita.')
      });
    } else {
      this.svc.create(payload).subscribe({
        next: () => this.salvo.emit(),
        error: (err) => this.errorMsg = AuthService.parseError(err, 'Erro ao criar receita.')
      });
    }
  }

  limpar() {
    this.nome = '';
    this.data = new Date().toISOString().substring(0, 10);
    this.valor = null;
    this.descricao = '';
    this.formaPagamentoId = null;
    this.categoriaReceitaId = null;
    this.errorMsg = '';
  }

  cancelar() {
    this.cancelado.emit();
  }

  abrirModalCategorias() {
    this.abrirCategorias.emit();
  }

  private carregarCategorias() {
    this.categoriaService.getReceitas().subscribe({
      next: (categorias) => {
        this.categoriasReceita = categorias.filter((categoria) => categoria.ativo !== false);
      },
      error: (err) => {
        console.error('Erro ao recarregar categorias:', err);
      }
    });
  }
}
