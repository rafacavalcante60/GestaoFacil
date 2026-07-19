import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  // Enquanto um refresh esta em voo, as demais requisicoes que tomaram 401 ficam
  // esperando aqui em vez de dispararem refreshes concorrentes. Isso importa porque
  // o backend rotaciona o refresh token: dois refreshes em paralelo fariam o segundo
  // usar um token ja revogado pelo primeiro, derrubando a sessao sem necessidade.
  private refreshEmAndamento = false;
  private tokenNovo$ = new BehaviorSubject<string | null>(null);

  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(this.comToken(req)).pipe(
      catchError(err => {
        const expirou = err instanceof HttpErrorResponse
          && err.status === 401
          && !this.ehChamadaDeAuth(req);

        if (expirou) {
          return this.tratarTokenExpirado(req, next);
        }

        return throwError(() => err);
      })
    );
  }

  private comToken(req: HttpRequest<any>): HttpRequest<any> {
    const token = this.authService.getToken();
    if (!token) return req;

    return req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  // login/refresh respondem 401 por credencial errada, nao por token expirado —
  // tentar renovar nesses casos criaria um laco.
  private ehChamadaDeAuth(req: HttpRequest<any>): boolean {
    return /\/Auth\/(login|refresh|register|forgot-password|reset-password)$/i.test(req.url);
  }

  private tratarTokenExpirado(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (this.refreshEmAndamento) {
      // espera o refresh que ja esta rodando e repete com o token que ele trouxe
      return this.tokenNovo$.pipe(
        filter((token): token is string => token !== null),
        take(1),
        switchMap(() => next.handle(this.comToken(req)))
      );
    }

    this.refreshEmAndamento = true;
    this.tokenNovo$.next(null);

    return this.authService.refreshSession().pipe(
      switchMap(token => {
        this.refreshEmAndamento = false;
        this.tokenNovo$.next(token);
        return next.handle(this.comToken(req));
      }),
      catchError(err => {
        // refresh token invalido, expirado ou revogado (troca de senha, mudanca de
        // role): nao ha o que renovar, a sessao acabou de verdade.
        this.refreshEmAndamento = false;
        this.authService.logout();
        return throwError(() => err);
      })
    );
  }
}
