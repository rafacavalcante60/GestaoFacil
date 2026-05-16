import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoriaService } from '../categoria.service';
import { Categoria } from '../../models/categoria.model';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-categoria-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './categoria-modal.component.html',
  styleUrls: ['./categoria-modal.component.scss']
})
export class CategoriaModalComponent implements OnInit {
  @Input() tipo: 'despesa' | 'receita' = 'despesa';
  @Output() fechar = new EventEmitter<void>();
  @Output() atualizar = new EventEmitter<void>();

  categorias: Categoria[] = [];
  novaCategoria = '';
  editandoId: number | null = null;
  editandoNome = '';
  errorMsg = '';
  successMsg = '';

  constructor(private svc: CategoriaService) {}

  ngOnInit() {
    this.carregarCategorias();
  }

  private carregarCategorias() {
    const obs$ = this.tipo === 'despesa' ? this.svc.getDespesas() : this.svc.getReceitas();
    obs$.subscribe({
      next: (data) => {
        console.log('Categorias carregadas:', data);
        this.categorias = data.filter(c => c.ativo !== false);
      },
      error: (err) => {
        console.error('Erro ao carregar categorias:', err);
        this.errorMsg = AuthService.parseError(err, 'Erro ao carregar categorias.');
      }
    });
  }

  adicionarCategoria() {
    this.errorMsg = '';
    this.successMsg = '';

    if (!this.novaCategoria.trim()) {
      this.errorMsg = 'Digite um nome para a categoria.';
      return;
    }

    const dto: Categoria = { nome: this.novaCategoria.trim() };
    console.log('Adicionando categoria:', { tipo: this.tipo, dto });
    
    const obs$ = this.tipo === 'despesa' ? this.svc.createDespesa(dto) : this.svc.createReceita(dto);

    obs$.subscribe({
      next: (response) => {
        console.log('Categoria criada com sucesso:', response);
        this.successMsg = 'Categoria criada com sucesso!';
        this.novaCategoria = '';
        setTimeout(() => {
          this.carregarCategorias();
          this.atualizar.emit();
        }, 500);
      },
      error: (err) => {
        console.error('Erro ao criar categoria:', err);
        this.errorMsg = AuthService.parseError(err, 'Erro ao criar categoria.');
      }
    });
  }

  iniciarEdicao(cat: Categoria) {
    this.editandoId = cat.id ?? null;
    this.editandoNome = cat.nome;
    this.errorMsg = '';
    this.successMsg = '';
  }

  salvarEdicao() {
    this.errorMsg = '';
    this.successMsg = '';

    if (!this.editandoNome.trim()) {
      this.errorMsg = 'Digite um nome para a categoria.';
      return;
    }

    if (!this.editandoId) return;

    const dto: any = { 
      id: this.editandoId,
      nome: this.editandoNome.trim(),
      ativo: true
    };
    const obs$ = this.tipo === 'despesa' 
      ? this.svc.updateDespesa(this.editandoId, dto) 
      : this.svc.updateReceita(this.editandoId, dto);

    obs$.subscribe({
      next: () => {
        this.successMsg = 'Categoria atualizada com sucesso!';
        this.editandoId = null;
        this.editandoNome = '';
        setTimeout(() => {
          this.carregarCategorias();
          this.atualizar.emit();
        }, 500);
      },
      error: (err) => {
        this.errorMsg = AuthService.parseError(err, 'Erro ao atualizar categoria.');
      }
    });
  }

  cancelarEdicao() {
    this.editandoId = null;
    this.editandoNome = '';
    this.errorMsg = '';
  }

  deletarCategoria(id: number) {
    if (!confirm('Deseja excluir esta categoria?')) return;

    const obs$ = this.tipo === 'despesa' 
      ? this.svc.deleteDespesa(id) 
      : this.svc.deleteReceita(id);

    obs$.subscribe({
      next: () => {
        this.successMsg = 'Categoria removida com sucesso!';
        setTimeout(() => {
          this.carregarCategorias();
          this.atualizar.emit();
        }, 500);
      },
      error: (err) => {
        this.errorMsg = AuthService.parseError(err, 'Erro ao deletar categoria.');
      }
    });
  }

  fecharModal() {
    this.fechar.emit();
  }
}
