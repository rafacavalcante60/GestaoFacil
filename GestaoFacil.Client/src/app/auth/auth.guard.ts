import { Injectable } from '@angular/core';
import { CanActivate, CanMatch, Route, UrlSegment, ActivatedRouteSnapshot, RouterStateSnapshot, Router, UrlTree } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanMatch, CanActivate {

  constructor(private auth: AuthService, private router: Router) {}

  //usado por lazy-loading / canMatch
  canMatch(route: Route, segments: UrlSegment[]): boolean | UrlTree {
    return this.checkAuth();
  }

  //usado por rotas diretas / canActivate
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    return this.checkAuth(state.url);
  }

  private checkAuth(redirectUrl?: string): boolean | UrlTree {
    if (this.auth.isLoggedIn()) {
      return true;
    }
    //e nao logado redireciona para /auth/login
    return this.router.parseUrl('/auth/login');
  }
}
