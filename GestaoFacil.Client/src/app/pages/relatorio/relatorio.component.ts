import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
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
  dateInputs = {
    dataInicial: '',
    dataFinal: ''
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

  constructor(private svc: ReportService, private router: Router) {
    this.syncDateInputsFromFiltros();
  }

  today() { return this.toLocalIsoDate(new Date()); }
  firstDayOfMonth() {
    const d = new Date();
    d.setDate(1);
    return this.toLocalIsoDate(d);
  }

  private toLocalIsoDate(date: Date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private syncDateInputsFromFiltros() {
    this.dateInputs.dataInicial = this.formatIsoToBr(this.filtros.dataInicial);
    this.dateInputs.dataFinal = this.formatIsoToBr(this.filtros.dataFinal);
  }

  onDateInputChange(field: 'dataInicial' | 'dataFinal', rawValue: string) {
    const maskedValue = this.maskDateInput(rawValue);
    this.dateInputs[field] = maskedValue;

    const isoValue = this.parseBrToIso(maskedValue);
    if (isoValue) {
      this.filtros[field] = isoValue;
    }
  }

  onDateInputBlur(field: 'dataInicial' | 'dataFinal') {
    const isoValue = this.parseBrToIso(this.dateInputs[field]);
    this.dateInputs[field] = isoValue
      ? this.formatIsoToBr(isoValue)
      : this.formatIsoToBr(this.filtros[field]);
  }

  private maskDateInput(rawValue: string) {
    const digits = rawValue.replace(/\D/g, '').slice(0, 8);
    if (digits.length <= 2) return digits;
    if (digits.length <= 4) return `${digits.slice(0, 2)}/${digits.slice(2)}`;
    return `${digits.slice(0, 2)}/${digits.slice(2, 4)}/${digits.slice(4)}`;
  }

  private formatIsoToBr(isoValue: string | null | undefined) {
    if (!isoValue) return '';

    const match = isoValue.match(/^(\d{4})-(\d{2})-(\d{2})$/);
    if (!match) return isoValue;

    const [, year, month, day] = match;
    return `${day}/${month}/${year}`;
  }

  private parseBrToIso(brValue: string | null | undefined) {
    if (!brValue) return null;

    const normalized = brValue.trim();
    const match = normalized.match(/^(\d{2})\/(\d{2})\/(\d{4})$/);
    if (!match) return null;

    const [, dayStr, monthStr, yearStr] = match;
    const day = Number(dayStr);
    const month = Number(monthStr);
    const year = Number(yearStr);

    const parsed = new Date(year, month - 1, day);
    const isValid =
      parsed.getFullYear() === year &&
      parsed.getMonth() === month - 1 &&
      parsed.getDate() === day;

    if (!isValid) return null;

    return `${yearStr}-${monthStr}-${dayStr}`;
  }

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

  formatDateBr(value: string | Date | null | undefined) {
    if (!value) return '';

    if (typeof value === 'string') {
      const isoMatch = value.match(/^(\d{4})-(\d{2})-(\d{2})/);
      if (isoMatch) {
        const [, year, month, day] = isoMatch;
        return `${day}/${month}/${year}`;
      }
    }

    const date = value instanceof Date ? value : new Date(value);
    if (Number.isNaN(date.getTime())) return String(value);

    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
  }

  voltar() {
    this.router.navigate(['/atividades']);
  }
}
