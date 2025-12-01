import { Routes } from '@angular/router';

export const PAGES_ROUTES: Routes = [
  {
    path: 'atividades',
    loadComponent: () =>
      import('./atividades/atividades.component').then(
        (m) => m.AtividadesComponent
      ),
  },
  {
    path: 'despesa',
    loadComponent: () =>
      import('./despesa/despesa.component').then((m) => m.DespesaComponent),
  },
  {
    path: 'receita',
    loadComponent: () =>
      import('./receita/receita.component').then((m) => m.ReceitaComponent),
  },
  {
    path: 'relatorio',
    loadComponent: () =>
      import('./relatorio/relatorio.component').then(
        (m) => m.RelatorioComponent
      ),
  },
  { path: '', redirectTo: 'atividades', pathMatch: 'full' },
];
