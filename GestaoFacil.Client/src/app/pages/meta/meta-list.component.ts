import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MetaService } from './meta.service';
import { Meta } from '../../models/meta.model';
import { MetaFormComponent } from './meta-form.component';
import { CategoriaService } from '../../shared/categoria.service';
import { Categoria } from '../../models/categoria.model';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-meta-list',
  standalone: true,
  imports: [CommonModule, MetaFormComponent],
  templateUrl: './meta-list.component.html',
  styleUrls: ['./meta-list.component.scss']
})
export class MetaListComponent implements OnInit, OnDestroy {
  metas: Meta[] = [];
  loading = false;
  errorMsg = '';
  infoMsg = '';
  confirmacaoExclusaoAberta = false;
  metaParaExcluir: Meta | null = null;

  categoriasDespesa: Categoria[] = [];
  categoriasReceita: Categoria[] = [];

  constructor(
    private svc: MetaService,
    private router: Router,
    private categoriaService: CategoriaService
  ) {}

  ngOnInit(): void {
    document.body.classList.add('fullscreen-layout');
    this.carregarCategorias();
    this.carregar();
  }

  ngOnDestroy(): void {
    document.body.classList.remove('fullscreen-layout');
  }

  carregar(): void {
    this.loading = true;
    this.errorMsg = '';

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

  modalAberto = false;
  modoEdicao = false;
  metaEditando: Meta | null = null;

  novaMeta(): void {
    this.metaEditando = null;
    this.modoEdicao = false;
    this.modalAberto = true;
  }

  editar(item: Meta): void {
    this.metaEditando = item;
    this.modoEdicao = true;
    this.modalAberto = true;
  }

  fecharModal(): void {
    this.modalAberto = false;
    this.metaEditando = null;
    this.modoEdicao = false;
  }

  aoSalvarModal(): void {
    const mensagem = this.modoEdicao ? 'Meta editada com sucesso.' : '';
    this.fecharModal();
    if (mensagem) {
      this.infoMsg = mensagem;
    }
    this.carregarCategorias();
    this.carregar();
  }

  excluir(item: Meta): void {
    this.metaParaExcluir = item;
    this.confirmacaoExclusaoAberta = true;
  }

  cancelarExclusao(): void {
    this.confirmacaoExclusaoAberta = false;
    this.metaParaExcluir = null;
  }

  confirmarExclusao(): void {
    const item = this.metaParaExcluir;
    if (!item) return;
    this.cancelarExclusao();

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

  private carregarCategorias(): void {
    forkJoin({
      despesas: this.categoriaService.getDespesas(),
      receitas: this.categoriaService.getReceitas()
    }).subscribe({
      next: ({ despesas, receitas }) => {
        this.categoriasDespesa = despesas.filter((categoria) => categoria.ativo !== false);
        this.categoriasReceita = receitas.filter((categoria) => categoria.ativo !== false);
      },
      error: () => {
        this.categoriasDespesa = [];
        this.categoriasReceita = [];
      }
    });
  }
}
