import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Receita } from '../../models/receita.model';
import { ReceitaService } from './receita.service';

@Component({
  selector: 'app-receita-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './receita-list.component.html',
  styleUrls: ['./receita-list.component.scss']
})
export class ReceitaListComponent implements OnInit {
  receitas: Receita[] = [];
  loading = false;
  errorMsg = '';
  infoMsg = '';

  pageNumber = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  filtros = {
    buscaTexto: '',
    dataInicial: '',
    dataFinal: ''
  };

  formasPagamento = [
    { id: 1, nome: 'Dinheiro' },
    { id: 2, nome: 'Cartao de Credito' },
    { id: 3, nome: 'Cartao de Debito' },
    { id: 4, nome: 'Pix' },
    { id: 5, nome: 'Cheque' },
    { id: 6, nome: 'Boleto' },
    { id: 7, nome: 'Outro' }
  ];

  categoriasReceita = [
    { id: 1, nome: 'Salario' },
    { id: 2, nome: 'Presente' },
    { id: 3, nome: 'Venda' },
    { id: 4, nome: 'Investimento' },
    { id: 5, nome: 'Outros' }
  ];

  constructor(private svc: ReceitaService, private router: Router) {}

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.loading = true;
    this.errorMsg = '';

    this.svc.filterPagination({
      PageNumber: this.pageNumber,
      PageSize: this.pageSize,
      BuscaTexto: this.filtros.buscaTexto || undefined,
      DataInicial: this.filtros.dataInicial || undefined,
      DataFinal: this.filtros.dataFinal || undefined
    }).subscribe({
      next: (res) => {
        const rawItems = Array.isArray(res.items) ? res.items : [];
        this.receitas = rawItems.filter((item): item is Receita => this.isReceitaValida(item));

        if (res.pagination) {
          this.pageNumber = res.pagination.currentPage;
          this.totalCount = res.pagination.totalCount;
          this.totalPages = Math.max(1, Math.ceil(this.totalCount / Math.max(1, this.pageSize)));
        } else {
          this.totalPages = this.receitas.length < this.pageSize ? this.pageNumber : this.pageNumber + 1;
          this.totalCount = this.receitas.length;
        }
        this.loading = false;
      },
      error: (err) => {
        this.errorMsg = err.error?.mensagem || err.error?.message || 'Erro ao carregar receitas.';
        this.loading = false;
      }
    });
  }

  aplicarFiltros(): void {
    this.pageNumber = 1;
    this.carregar();
  }

  limparFiltros(): void {
    this.filtros = {
      buscaTexto: '',
      dataInicial: '',
      dataFinal: ''
    };
    this.pageNumber = 1;
    this.carregar();
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

  novaReceita(): void {
    this.router.navigate(['/receita/nova']);
  }

  irAtividades(): void {
    this.router.navigate(['/atividades']);
  }

  editar(item: Receita): void {
    if (!item.id) return;
    this.router.navigate(['/receita', item.id, 'editar']);
  }

  excluir(item: Receita): void {
    if (!item.id) return;
    const ok = window.confirm(`Deseja excluir a receita "${item.nome ?? ''}"?`);
    if (!ok) return;

    this.svc.delete(item.id).subscribe({
      next: () => {
        this.infoMsg = 'Receita excluida com sucesso.';
        if (this.receitas.length === 1 && this.pageNumber > 1) {
          this.pageNumber--;
        }
        this.carregar();
      },
      error: (err) => {
        this.errorMsg = err.error?.mensagem || err.error?.message || 'Erro ao excluir receita.';
      }
    });
  }

  nomeCategoria(id?: number): string {
    return this.categoriasReceita.find((c) => c.id === id)?.nome ?? '-';
  }

  nomeFormaPagamento(id?: number): string {
    return this.formasPagamento.find((f) => f.id === id)?.nome ?? '-';
  }

  private isReceitaValida(item: unknown): item is Receita {
    if (!item || typeof item !== 'object') return false;
    const receita = item as Partial<Receita>;
    return typeof receita.id === 'number' && receita.id > 0;
  }
}
