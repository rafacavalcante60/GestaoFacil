import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent {
  email = '';
  errorMsg = '';
  successMsg = '';
  isSubmitting = false;

  constructor(private auth: AuthService, private router: Router) {}

  submit() {
    if (this.isSubmitting) return;
    this.errorMsg = '';
    this.successMsg = '';

    if (!this.email || !this.email.includes('@')) {
      this.errorMsg = 'Informe um e-mail valido.';
      return;
    }

    this.isSubmitting = true;
    this.auth.forgotPassword(this.email).subscribe({
      next: (res) => {
        this.successMsg = res?.mensagem || res?.Mensagem || 'Se o e-mail existir, enviaremos o link de redefinicao.';
      },
      error: (err) => {
        this.errorMsg = err.error?.mensagem || err.error?.Mensagem || err.error?.message || 'Falha ao solicitar redefinicao.';
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  goLogin() {
    this.router.navigate(['/auth/login']);
  }
}
