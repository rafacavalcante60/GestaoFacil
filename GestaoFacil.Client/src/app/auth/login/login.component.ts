import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  email = '';
  senha = '';
  errorMsg = '';

  constructor(private auth: AuthService, private router: Router) {}

  submit() {
    this.errorMsg = '';
    this.auth.login({ email: this.email, senha: this.senha }).subscribe({
      next: () => this.router.navigate(['/atividades']),
      error: err => this.errorMsg = 'Login falhou: ' + (err.error?.message || err.message)
    });
  }

  goToCadastro() {
    this.router.navigate(['/auth/cadastro']);
  }

  goToForgotPassword() {
    this.router.navigate(['/auth/forgot-password']);
  }
}
