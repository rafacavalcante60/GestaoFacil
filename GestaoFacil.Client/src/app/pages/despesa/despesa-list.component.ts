import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Despesa } from '../../models/despesa.model';
import { DespesaService } from './despesa.service';

@Component({
  selector: 'app-despesa-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './despesa-list.component.html',
  styleUrls: ['./despesa-list.component.scss']
})
export class DespesaListComponent implements OnInit {
  despesas: Despesa[] = [];
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

  categoriasDespesa = [
    { id: 1, nome: 'Alimentacao' },
    { id: 2, nome: 'Transporte' },
    { id: 3, nome: 'Moradia' },
    { id: 4, nome: 'Lazer' },
    { id: 5, nome: 'Educacao' },
    { id: 6, nome: 'Saude' },
    { id: 7, nome: 'Outra' }
  ];

  constructor(private svc: DespesaService, private router: Router) {}

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
        this.despesas = res.items ?? [];

        if (res.pagination) {
          this.pageNumber = res.pagination.currentPage;
          this.totalPages = Math.max(1, res.pagination.totalPages);
          this.totalCount = res.pagination.totalCount;
        } else {
          this.totalPages = this.despesas.length < this.pageSize ? this.pageNumber : this.pageNumber + 1;
          this.totalCount = this.despesas.length;
        }
        this.loading = false;
      },
      error: (err) => {
        this.errorMsg = err.error?.mensagem || err.error?.message || 'Erro ao carregar despesas.';
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

  novaDespesa(): void {
    this.router.navigate(['/despesa/nova']);
  }

  irAtividades(): void {
    this.router.navigate(['/atividades']);
  }

  editar(item: Despesa): void {
    if (!item.id) return;
    this.router.navigate(['/despesa', item.id, 'editar']);
  }

  excluir(item: Despesa): void {
    if (!item.id) return;
    const ok = window.confirm(`Deseja excluir a despesa "${item.nome ?? ''}"?`);
    if (!ok) return;

    this.svc.delete(item.id).subscribe({
      next: () => {
        this.infoMsg = 'Despesa excluida com sucesso.';
        if (this.despesas.length === 1 && this.pageNumber > 1) {
          this.pageNumber--;
        }
        this.carregar();
      },
      error: (err) => {
        this.errorMsg = err.error?.mensagem || err.error?.message || 'Erro ao excluir despesa.';
      }
    });
  }

  nomeCategoria(id?: number): string {
    return this.categoriasDespesa.find((c) => c.id === id)?.nome ?? '-';
  }

  nomeFormaPagamento(id?: number): string {
    return this.formasPagamento.find((f) => f.id === id)?.nome ?? '-';
  }
}
