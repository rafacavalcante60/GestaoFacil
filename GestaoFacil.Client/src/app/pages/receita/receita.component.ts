import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ReceitaService } from '../receita/receita.service';
import { Receita } from '../../models/receita.model';
import { AuthService } from '../../auth/auth.service';
import { LookupService } from '../../shared/lookup.service';

@Component({
  selector: 'app-receita',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './receita.component.html',
  styleUrls: ['./receita.component.scss'] // opcional
})
export class ReceitaComponent {
  // campos do formulário
  nome = '';
  data = new Date().toISOString().substring(0, 10); // yyyy-mm-dd
  valor: number | null = null;
  formaPagamentoId: number | null = null;
  categoriaReceitaId: number | null = null;
  descricao = '';

  // estado
  errorMsg = '';
  isEdit = false;
  id?: number;

  formasPagamento;
  categoriasReceita;

  constructor(
    private svc: ReceitaService,
    private router: Router,
    private route: ActivatedRoute,
    private lookup: LookupService
  ) {
    this.formasPagamento = this.lookup.formasPagamento;
    this.categoriasReceita = this.lookup.categoriasReceita;
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEdit = true;
      this.id = Number(idParam);
      this.loadReceita(this.id);
    }
  }

  private loadReceita(id: number) {
    this.svc.get(id).subscribe({
      next: (r: Receita) => {
        // ajusta data para input date
        this.data = r.data ? r.data.substring(0, 10) : new Date().toISOString().substring(0, 10);
        this.nome = r.nome ?? '';
        this.valor = r.valor;
        this.descricao = r.descricao ?? '';
        this.categoriaReceitaId = r.categoriaReceitaId ?? null;
        this.formaPagamentoId = r.formaPagamentoId ?? null;
      },
      error: () => this.errorMsg = 'Erro ao carregar receita para edição.'
    });
  }

  salvarReceita() {
    this.errorMsg = '';

    if (!this.data || this.valor === null || this.valor === undefined || !this.categoriaReceitaId || !this.formaPagamentoId || !this.nome) {
      this.errorMsg = 'Preencha Nome, Data, Valor, Categoria e Forma de Pagamento.';
      return;
    }

    const payload: Receita = {
      nome: this.nome,
      data: new Date(this.data).toISOString(),
      descricao: this.descricao,
      valor: Number(this.valor),
      categoriaReceitaId: Number(this.categoriaReceitaId),
      formaPagamentoId: Number(this.formaPagamentoId),
      // nomeUsuario é algo que o backend pode preencher; não é necessário aqui
    };

    if (this.isEdit && this.id) {
      this.svc.update(this.id, payload).subscribe({
        next: () => this.router.navigate(['/receita']),
        error: (err: any) => this.errorMsg = AuthService.parseError(err, 'Erro ao atualizar receita.')
      });
    } else {
      this.svc.create(payload).subscribe({
        next: () => {
          this.router.navigate(['/receita']);
        },
        error: (err: any) => this.errorMsg = AuthService.parseError(err, 'Erro ao criar receita.')
      });
    }
  }

  limpar() {
    this.nome = '';
    this.data = new Date().toISOString().substring(0, 10);
    this.valor = null;
    this.formaPagamentoId = null;
    this.categoriaReceitaId = null;
    this.descricao = '';
    this.errorMsg = '';
    // se quiser sair do modo edição:
    // this.isEdit = false; this.id = undefined;
  }

  voltar() {
    this.router.navigate(['/receita']);
  }

  irAtividades() {
    this.router.navigate(['/atividades']);
  }
}
