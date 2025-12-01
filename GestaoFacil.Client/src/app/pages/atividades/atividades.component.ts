import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-atividades',
  imports: [],
  templateUrl: './atividades.component.html',
  styleUrl: './atividades.component.scss'
})
export class AtividadesComponent {
  constructor(private router: Router) {}

  goToDespesa() {
    this.router.navigate(['/despesa']); 
  }

  goToReceita() {
    this.router.navigate(['/receita']);
  }

  goToRelatorio() {
    this.router.navigate(['/relatorio']);
  }
}
