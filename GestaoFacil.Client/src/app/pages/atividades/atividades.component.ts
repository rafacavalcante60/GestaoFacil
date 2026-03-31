import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';
import { MetaService } from '../meta/meta.service';
import { Meta } from '../../models/meta.model';

@Component({
  selector: 'app-atividades',
  imports: [CommonModule],
  templateUrl: './atividades.component.html',
  styleUrl: './atividades.component.scss'
})
export class AtividadesComponent implements OnInit, OnDestroy {
  isAdmin = false;
  metasAtivas: Meta[] = [];

  constructor(private router: Router, private auth: AuthService, private metaSvc: MetaService) {
    this.isAdmin = this.auth.isAdmin();
  }

  ngOnInit(): void {
    document.body.classList.add('fullscreen-layout');
    this.metaSvc.getAtivas().subscribe({
      next: (metas) => {
        this.metasAtivas = metas.slice(0, 3);
      },
      error: () => {
        this.metasAtivas = [];
      }
    });
  }

  ngOnDestroy(): void {
    document.body.classList.remove('fullscreen-layout');
  }

  goToDespesa() {
    this.router.navigate(['/despesa']);
  }

  goToReceita() {
    this.router.navigate(['/receita']);
  }

  goToRelatorio() {
    this.router.navigate(['/relatorio']);
  }

  goToMetas() {
    this.router.navigate(['/meta']);
  }

  logout() {
    this.auth.logout();
  }

  goToAdminUsuarios() {
    this.router.navigate(['/admin/usuarios']);
  }

  corBarra(meta: Meta): string {
    if (meta.tipo === 1) {
      if (meta.percentual >= 100) return '#b93817';
      if (meta.percentual >= 75) return '#f59e0b';
      return '#16a34a';
    } else {
      if (meta.percentual >= 100) return '#16a34a';
      if (meta.percentual >= 50) return '#f59e0b';
      return '#b93817';
    }
  }

  larguraBarra(meta: Meta): string {
    return Math.min(meta.percentual, 100) + '%';
  }
}
