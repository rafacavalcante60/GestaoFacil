import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MetaService } from './meta.service';
import { Meta } from '../../models/meta.model';

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

  constructor(
    private svc: MetaService,
    private router: Router,
    private route: ActivatedRoute
  ) {
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
        error: (err) => this.errorMsg = err.error?.mensagem || 'Erro ao atualizar meta.'
      });
    } else {
      this.svc.create(payload).subscribe({
        next: () => this.router.navigate(['/meta']),
        error: (err) => this.errorMsg = err.error?.mensagem || 'Erro ao criar meta.'
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
