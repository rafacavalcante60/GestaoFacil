import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ReportService } from './report.service';

@Component({
  selector: 'app-relatorio',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './relatorio.component.html',
  styleUrls: ['./relatorio.component.scss']
})
export class RelatorioComponent {
  filtros = {
    tipo: 'ambos' as 'ambos' | 'despesa' | 'receita',
    dataInicial: this.firstDayOfMonth(),
    dataFinal: this.today(),
    categoriaId: null as number | null,
    formaPagamentoId: null as number | null
  };

  categorias = [
    { id: 1, nome: 'Alimentação' },
    { id: 2, nome: 'Transporte' },
    { id: 3, nome: 'Moradia' }
  ];

  formasPagamento = [
    { id: 1, nome: 'Dinheiro' },
    { id: 2, nome: 'Cartão' },
    { id: 4, nome: 'Pix' }
  ];

  // dados da API
  resumo: any = null;
  porCategoria: Array<{ categoria: string; total: number }> = [];
  fluxo: Array<{ data: string; saldoAcumulado: number }> = [];
  mensal: Array<{ mes: number; totalReceitas: number; totalDespesas: number; saldo: number }> = [];

  // flags de carregamento
  resumoLoaded = false;
  porCategoriaLoaded = false;
  fluxoLoaded = false;
  mensalLoaded = false;

  anoSelecionado = new Date().getFullYear();

  constructor(private svc: ReportService,   private router: Router,) {
  }

  today() { return new Date().toISOString().slice(0, 10); }
  firstDayOfMonth() { const d = new Date(); d.setDate(1); return d.toISOString().slice(0, 10); }

  private buildQueryForRange() {
    // backend espera inicio/fim como string(date-time)
    const inicio = `${this.filtros.dataInicial}T00:00:00`;
    const fim = `${this.filtros.dataFinal}T23:59:59`;
    return { inicio, fim };
  }

  gerarRelatorio() {
    const q = this.buildQueryForRange();

    // resumo
    this.resumoLoaded = false;
    this.svc.resumo(q).subscribe({
      next: r => {
        this.resumo = r;
        this.resumoLoaded = true;
      },
      error: () => {
        this.resumo = null;
        this.resumoLoaded = false;
      }
    });

    // categoria (envia despesas flag dependendo do tipo)
    this.porCategoriaLoaded = false;
    const despesasFlag = this.filtros.tipo === 'despesa' ? true : (this.filtros.tipo === 'receita' ? false : true);
    this.svc.categoria({ ...q, despesas: despesasFlag }).subscribe({
      next: r => { this.porCategoria = r; this.porCategoriaLoaded = true; },
      error: () => { this.porCategoria = []; this.porCategoriaLoaded = false; }
    });

    // fluxo
    this.fluxoLoaded = false;
    this.svc.fluxo(q).subscribe({
      next: r => { this.fluxo = r; this.fluxoLoaded = true; },
      error: () => { this.fluxo = []; this.fluxoLoaded = false; }
    });

    // mensal
    this.mensalLoaded = false;
    this.svc.mensal({ ano: this.anoSelecionado }).subscribe({
      next: r => { this.mensal = r; this.mensalLoaded = true; },
      error: () => { this.mensal = []; this.mensalLoaded = false; }
    });
  }

  atualizar() { this.gerarRelatorio(); }
  atualizarResumo() { this.svc.resumo(this.buildQueryForRange()).subscribe({ next: r => this.resumo = r }); }
  atualizarCategoria() { this.svc.categoria({ ...this.buildQueryForRange(), despesas: true }).subscribe({ next: r => this.porCategoria = r }); }
  atualizarFluxo() { this.svc.fluxo(this.buildQueryForRange()).subscribe({ next: r => this.fluxo = r }); }
  atualizarMensal() { this.svc.mensal({ ano: this.anoSelecionado }).subscribe({ next: r => this.mensal = r }); }

  exportar() {
    const qRange = this.buildQueryForRange();
    if (this.filtros.tipo === 'despesa') {
      this.svc.exportDespesa(qRange).subscribe({ next: b => this.downloadBlob(b, `despesas_${this.filtros.dataInicial}_${this.filtros.dataFinal}.xlsx`) });
      return;
    }
    if (this.filtros.tipo === 'receita') {
      this.svc.exportReceita(qRange).subscribe({ next: b => this.downloadBlob(b, `receitas_${this.filtros.dataInicial}_${this.filtros.dataFinal}.xlsx`) });
      return;
    }
    // ambos -> baixa os dois
    this.svc.exportDespesa(qRange).subscribe({ next: b => this.downloadBlob(b, `despesas_${this.filtros.dataInicial}_${this.filtros.dataFinal}.xlsx`) });
    this.svc.exportReceita(qRange).subscribe({ next: b => this.downloadBlob(b, `receitas_${this.filtros.dataInicial}_${this.filtros.dataFinal}.xlsx`) });
  }

  private downloadBlob(blob: Blob, filename: string) {
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    a.remove();
    window.URL.revokeObjectURL(url);
  }

  formatMes(mesIndex: number) {
    if (mesIndex == null) return '';
    return String(mesIndex).padStart(2, '0');
  }

  voltar() {
    this.router.navigate(['/atividades']);
  }
}
