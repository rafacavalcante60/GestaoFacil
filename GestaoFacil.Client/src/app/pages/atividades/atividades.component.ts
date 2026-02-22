import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-atividades',
  imports: [],
  templateUrl: './atividades.component.html',
  styleUrl: './atividades.component.scss'
})
export class AtividadesComponent {
  constructor(private router: Router, private auth: AuthService) {}

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
}
