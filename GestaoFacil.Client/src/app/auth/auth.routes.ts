import { Routes } from '@angular/router';

export const AUTH_ROUTES: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./login/login.component')
      .then(m => m.LoginComponent)
  },
  {
    path: 'cadastro',
    loadComponent: () => import('./cadastro/cadastro.component')
      .then(m => m.CadastroComponent)
  },

  //redireciona para login
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  }
];