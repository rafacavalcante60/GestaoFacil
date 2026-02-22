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
      import('./despesa/despesa-list.component').then((m) => m.DespesaListComponent),
  },
  {
    path: 'despesa/nova',
    loadComponent: () =>
      import('./despesa/despesa.component').then((m) => m.DespesaComponent),
  },
  {
    path: 'despesa/:id/editar',
    loadComponent: () =>
      import('./despesa/despesa.component').then((m) => m.DespesaComponent),
  },
  {
    path: 'receita',
    loadComponent: () =>
      import('./receita/receita-list.component').then((m) => m.ReceitaListComponent),
  },
  {
    path: 'receita/nova',
    loadComponent: () =>
      import('./receita/receita.component').then((m) => m.ReceitaComponent),
  },
  {
    path: 'receita/:id/editar',
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
  { path: 'despesas', redirectTo: 'despesa', pathMatch: 'full' },
  { path: 'receitas', redirectTo: 'receita', pathMatch: 'full' },
  { path: '', redirectTo: 'atividades', pathMatch: 'full' },
];
