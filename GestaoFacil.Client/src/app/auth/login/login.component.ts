import { Component, OnInit, OnDestroy } from '@angular/core';
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
export class LoginComponent implements OnInit, OnDestroy {
  email = '';
  senha = '';
  errorMsg = '';

  constructor(private auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    document.body.classList.add('centered-layout');
  }

  ngOnDestroy(): void {
    document.body.classList.remove('centered-layout');
  }

  submit() {
    this.errorMsg = '';
    this.auth.login({ email: this.email, senha: this.senha }).subscribe({
      next: () => this.router.navigate(['/atividades']),
      error: err => this.errorMsg = AuthService.parseError(err, 'Email ou senha incorretos.')
    });
  }

  goToCadastro() {
    this.router.navigate(['/auth/cadastro']);
  }

  goToForgotPassword() {
    this.router.navigate(['/auth/forgot-password']);
  }
}
