import { Component, OnInit, OnDestroy } from '@angular/core';
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
export class CadastroComponent implements OnInit, OnDestroy {
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

  ngOnInit(): void {
    document.body.classList.add('centered-layout');
  }

  ngOnDestroy(): void {
    document.body.classList.remove('centered-layout');
  }

  submit() {
    if (this.isSubmitting) return;
    this.errorMsg = '';

    if (!this.senha || this.senha.length < 6) {
      this.errorMsg = 'A senha deve ter pelo menos 6 caracteres.';
      return;
    }

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
          this.errorMsg = AuthService.parseError(err, 'Erro ao cadastrar. Tente novamente.');
        }
      });
  }

  goToLogin() {
    this.router.navigate(['/auth/login']);
  }
}
