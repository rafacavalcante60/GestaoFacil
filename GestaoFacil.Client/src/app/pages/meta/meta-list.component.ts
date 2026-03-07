import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MetaService } from './meta.service';
import { Meta } from '../../models/meta.model';

@Component({
  selector: 'app-meta-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './meta-list.component.html',
  styleUrls: ['./meta-list.component.scss']
})
export class MetaListComponent implements OnInit {
  metas: Meta[] = [];
  loading = false;
  errorMsg = '';
  infoMsg = '';

  categoriasDespesa = [
    { id: 1, nome: 'Alimentação' },
    { id: 2, nome: 'Transporte' },
    { id: 3, nome: 'Moradia' },
    { id: 4, nome: 'Lazer' },
    { id: 5, nome: 'Educação' },
    { id: 6, nome: 'Saúde' },
    { id: 7, nome: 'Outra' }
  ];

  categoriasReceita = [
    { id: 1, nome: 'Salário' },
    { id: 2, nome: 'Presente' },
    { id: 3, nome: 'Venda' },
    { id: 4, nome: 'Investimento' },
    { id: 5, nome: 'Outros' }
  ];

  constructor(private svc: MetaService, private router: Router) {}

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.loading = true;
    this.errorMsg = '';
    this.infoMsg = '';

    this.svc.getAll().subscribe({
      next: (metas) => {
        this.metas = metas;
        this.loading = false;
      },
      error: (err) => {
        this.errorMsg = err.error?.mensagem || 'Erro ao carregar metas.';
        this.loading = false;
      }
    });
  }

  novaMeta(): void {
    this.router.navigate(['/meta/nova']);
  }

  editar(item: Meta): void {
    this.router.navigate(['/meta', item.id, 'editar']);
  }

  excluir(item: Meta): void {
    const ok = window.confirm(`Deseja excluir a meta "${item.nome}"?`);
    if (!ok) return;

    this.svc.delete(item.id).subscribe({
      next: () => {
        this.infoMsg = 'Meta excluída com sucesso.';
        this.carregar();
      },
      error: (err) => {
        this.errorMsg = err.error?.mensagem || 'Erro ao excluir meta.';
      }
    });
  }

  irAtividades(): void {
    this.router.navigate(['/atividades']);
  }

  corBarra(meta: Meta): string {
    if (meta.tipo === 1) {
      if (meta.percentual >= 100) return '#b93817';
      if (meta.percentual >= 75) return '#f59e0b';
      return '#16a34a';
    } else {
      if (meta.percentual >= 100) return '#16a34a';
      if (meta.percentual >= 50) return '#f59e0b';
      return '#b93817';
    }
  }

  larguraBarra(meta: Meta): string {
    return Math.min(meta.percentual, 100) + '%';
  }

  labelTipo(tipo: 1 | 2): string {
    return tipo === 1 ? 'Despesa' : 'Receita';
  }

  nomeCategoria(meta: Meta): string {
    if (meta.tipo === 1) {
      if (!meta.categoriaDespesaId) return 'Todas';
      return this.categoriasDespesa.find(c => c.id === meta.categoriaDespesaId)?.nome ?? '-';
    } else {
      if (!meta.categoriaReceitaId) return 'Todas';
      return this.categoriasReceita.find(c => c.id === meta.categoriaReceitaId)?.nome ?? '-';
    }
  }
}
