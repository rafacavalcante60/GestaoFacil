import { Injectable } from '@angular/core';
import {
  CanActivate,
  CanMatch,
  Route,
  UrlSegment,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
  UrlTree
} from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class AdminGuard implements CanMatch, CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

  canMatch(route: Route, segments: UrlSegment[]): boolean | UrlTree {
    return this.checkAdmin();
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    return this.checkAdmin();
  }

  private checkAdmin(): boolean | UrlTree {
    if (!this.auth.isLoggedIn()) {
      return this.router.parseUrl('/auth/login');
    }
    if (this.auth.isAdmin()) {
      return true;
    }
    return this.router.parseUrl('/atividades');
  }
}
