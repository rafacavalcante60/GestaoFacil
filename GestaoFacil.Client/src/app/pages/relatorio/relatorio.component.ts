import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';
import '../../shared/chart-config';
import { CHART_COLORS, categoryPalette } from '../../shared/chart-colors';
import { ReportService } from './report.service';

const brlAxisFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
  maximumFractionDigits: 0
});

const brlCompactFormatter = new Intl.NumberFormat('pt-BR', {
  notation: 'compact',
  compactDisplay: 'short',
  maximumFractionDigits: 1
});

function formatCurrencyAxisTick(value: string | number): string {
  const amount = typeof value === 'string' ? Number(value) : value;
  if (!Number.isFinite(amount)) return String(value);
  return brlAxisFormatter.format(amount);
}

function formatCurrencyCompactTick(value: string | number): string {
  const amount = typeof value === 'string' ? Number(value) : value;
  if (!Number.isFinite(amount)) return String(value);

  if (Math.abs(amount) < 1000) return brlAxisFormatter.format(amount);
  return `R$ ${brlCompactFormatter.format(amount)}`;
}

@Component({
  selector: 'app-relatorio',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective],
  templateUrl: './relatorio.component.html',
  styleUrls: ['./relatorio.component.scss']
})
export class RelatorioComponent implements OnInit, OnDestroy {
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

  // Chart data
  resumoChartData: ChartData<'doughnut'> = { labels: [], datasets: [] };
  resumoChartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'bottom', labels: { padding: 16 } },
      title: { display: true, text: 'Receitas vs Despesas', font: { size: 14 } }
    }
  };

  categoriaChartData: ChartData<'pie'> = { labels: [], datasets: [] };
  categoriaChartOptions: ChartOptions<'pie'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'bottom', labels: { padding: 12, font: { size: 11 } } },
      title: { display: true, text: 'Distribuição por Categoria', font: { size: 14 } }
    }
  };

  fluxoChartData: ChartData<'line'> = { labels: [], datasets: [] };
  fluxoChartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      title: { display: true, text: 'Fluxo de Caixa (Saldo Acumulado)', font: { size: 14 } }
    },
    scales: {
      x: { grid: { display: false } },
      y: {
        beginAtZero: false,
        ticks: {
          callback: (value) => formatCurrencyAxisTick(value)
        }
      }
    }
  };

  mensalChartData: ChartData<'bar'> = { labels: [], datasets: [] };
  mensalChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    layout: {
      padding: {
        top: 20
      }
    },
    plugins: {
      legend: { position: 'bottom', labels: { padding: 16 } },
      title: { display: true, text: 'Resumo Mensal', font: { size: 14 } },
      tooltip: {
        callbacks: {
          label: (context) => {
            const label = context.dataset.label || '';
            const value = context.parsed.y;
            if (value === null || value === undefined) return label;
            return `${label}: ${formatCurrencyAxisTick(value)}`;
          }
        }
      }
    },
    scales: {
      x: { grid: { display: false } },
      y: {
        beginAtZero: true,
        ticks: {
          maxTicksLimit: 5,
          autoSkip: true,
          callback: (value) => formatCurrencyCompactTick(value)
        }
      }
    }
  };

  constructor(private svc: ReportService, private router: Router) {
    this.syncDateInputsFromFiltros();
  }

  ngOnInit() {
    document.body.classList.add('fullscreen-layout', 'reports-page-open');
  }

  ngOnDestroy() {
    document.body.classList.remove('fullscreen-layout', 'reports-page-open');
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
        this.updateResumoChart();
      },
      error: () => {
        this.resumo = null;
        this.resumoLoaded = false;
      }
    });

    // categoria
    this.porCategoriaLoaded = false;
    const despesasFlag = this.filtros.tipo === 'despesa' ? true : (this.filtros.tipo === 'receita' ? false : true);
    this.svc.categoria({ ...q, despesas: despesasFlag }).subscribe({
      next: r => {
        this.porCategoria = r;
        this.porCategoriaLoaded = true;
        this.updateCategoriaChart();
      },
      error: () => { this.porCategoria = []; this.porCategoriaLoaded = false; }
    });

    // fluxo
    this.fluxoLoaded = false;
    this.svc.fluxo(q).subscribe({
      next: r => {
        this.fluxo = r;
        this.fluxoLoaded = true;
        this.updateFluxoChart();
      },
      error: () => { this.fluxo = []; this.fluxoLoaded = false; }
    });

    // mensal
    this.mensalLoaded = false;
    this.svc.mensal({ ano: this.anoSelecionado }).subscribe({
      next: r => {
        this.mensal = r;
        this.mensalLoaded = true;
        this.updateMensalChart();
      },
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

  // --- Chart update methods ---

  private updateResumoChart() {
    if (!this.resumo) return;
    this.resumoChartData = {
      labels: ['Receitas', 'Despesas'],
      datasets: [{
        data: [this.resumo.totalReceitas, this.resumo.totalDespesas],
        backgroundColor: [CHART_COLORS.green, CHART_COLORS.red],
        hoverBackgroundColor: ['#27ae60', '#c0392b'],
        borderWidth: 2,
        borderColor: '#fff'
      }]
    };
  }

  private updateCategoriaChart() {
    if (!this.porCategoria.length) return;
    const labels = this.porCategoria.map(c => c.categoria);
    const data = this.porCategoria.map(c => c.total);
    const colors = categoryPalette(data.length);
    this.categoriaChartData = {
      labels,
      datasets: [{
        data,
        backgroundColor: colors,
        borderWidth: 2,
        borderColor: '#fff'
      }]
    };
  }

  private updateFluxoChart() {
    if (!this.fluxo.length) return;
    const labels = this.fluxo.map(f => this.formatDateBr(f.data));
    const data = this.fluxo.map(f => f.saldoAcumulado);
    this.fluxoChartData = {
      labels,
      datasets: [{
        data,
        label: 'Saldo Acumulado',
        borderColor: CHART_COLORS.blue,
        backgroundColor: 'rgba(52, 152, 219, 0.15)',
        fill: true,
        tension: 0.3,
        pointRadius: 3,
        pointBackgroundColor: CHART_COLORS.blue
      }]
    };
  }

  private updateMensalChart() {
    if (!this.mensal.length) return;
    const meses = ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'];
    const labels = this.mensal.map(m => meses[(m.mes - 1) % 12] || String(m.mes));
    const receitas = this.mensal.map(m => m.totalReceitas);
    const despesas = this.mensal.map(m => m.totalDespesas);
    const positivos = [...receitas, ...despesas].filter(v => v > 0);
    const maxValor = Math.max(...receitas, ...despesas, 0);
    const minPositivo = positivos.length ? Math.min(...positivos) : 0;
    const razaoAmplitude = minPositivo > 0 ? maxValor / minPositivo : 1;
    const minBarLength = razaoAmplitude >= 100 ? 4 : undefined;
    const stepSize = this.getNiceStep(maxValor);
    const eixoMax = maxValor > 0 ? Math.ceil(maxValor / stepSize) * stepSize : 10;

    this.mensalChartData = {
      labels,
      datasets: [
        {
          label: 'Receitas',
          data: receitas,
          backgroundColor: CHART_COLORS.green,
          borderRadius: 4,
          minBarLength
        },
        {
          label: 'Despesas',
          data: despesas,
          backgroundColor: CHART_COLORS.red,
          borderRadius: 4,
          minBarLength
        }
      ]
    };

    this.mensalChartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      layout: {
        padding: {
          top: 20
        }
      },
      plugins: {
        legend: { position: 'bottom', labels: { padding: 16 } },
        title: { display: true, text: 'Resumo Mensal', font: { size: 14 } },
        tooltip: {
          callbacks: {
            label: (context) => {
              const label = context.dataset.label || '';
              const value = context.parsed.y;
              if (value === null || value === undefined) return label;
              return `${label}: ${formatCurrencyAxisTick(value)}`;
            }
          }
        }
      },
      scales: {
        x: { grid: { display: false } },
        y: {
          type: 'linear',
          min: 0,
          max: eixoMax,
          beginAtZero: true,
          ticks: {
            stepSize,
            maxTicksLimit: 5,
            autoSkip: true,
            callback: (value) => formatCurrencyCompactTick(value)
          }
        }
      }
    };
  }

  private getNiceStep(maxValue: number): number {
    if (maxValue <= 0) return 1;

    const targetTicks = 6;
    const rawStep = maxValue / targetTicks;
    const magnitude = Math.pow(10, Math.floor(Math.log10(rawStep)));
    const normalized = rawStep / magnitude;

    if (normalized <= 1) return 1 * magnitude;
    if (normalized <= 2) return 2 * magnitude;
    if (normalized <= 5) return 5 * magnitude;
    return 10 * magnitude;
  }
}
