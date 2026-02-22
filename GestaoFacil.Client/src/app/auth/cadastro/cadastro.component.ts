import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-cadastro',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cadastro.component.html',
  styleUrls: ['./cadastro.component.scss']
})
export class CadastroComponent {
  //variaveis
  nome = ''; 
  email = '';
  senha = '';
  senhaConfirm = '';
  errorMsg = '';
  isSubmitting = false;

  //injecao de dependencia
  constructor(private router: Router, 
              private auth: AuthService
  ) {}

  submit() {
    if (this.isSubmitting) return;
    if (this.senha !== this.senhaConfirm) {
      this.errorMsg = "As senhas não coincidem!";
      return;
    }
  
    const body = {
      nome: this.nome,
      email: this.email,
      senha: this.senha,
      confirmarSenha: this.senhaConfirm
    };
  
    this.isSubmitting = true;
    this.auth.register(body)
      .pipe(finalize(() => this.isSubmitting = false))
      .subscribe({
        next: () => {
          alert("Cadastro realizado!");
          this.router.navigate(['/auth/login']);
        },
        error: (err) => {
          this.errorMsg = err.error?.message || "Erro ao cadastrar";
        }
      });
  }

  goToLogin() {
    this.router.navigate(['/auth/login']);
  }
}
