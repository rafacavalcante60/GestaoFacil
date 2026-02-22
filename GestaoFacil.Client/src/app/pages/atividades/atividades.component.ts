import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-atividades',
  imports: [CommonModule],
  templateUrl: './atividades.component.html',
  styleUrl: './atividades.component.scss'
})
export class AtividadesComponent {
  isAdmin = false;

  constructor(private router: Router, private auth: AuthService) {
    this.isAdmin = this.auth.isAdmin();
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

  logout() {
    this.auth.logout();
  }

  goToAdminUsuarios() {
    this.router.navigate(['/admin/usuarios']);
  }
}
