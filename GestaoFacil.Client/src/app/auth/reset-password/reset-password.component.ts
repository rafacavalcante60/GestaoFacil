import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {
  token = '';
  newPassword = '';
  confirmPassword = '';
  errorMsg = '';
  successMsg = '';
  isSubmitting = false;

  constructor(
    private auth: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const tokenFromUrl = this.route.snapshot.queryParamMap.get('token');
    if (tokenFromUrl) {
      this.token = tokenFromUrl;
    }
  }

  submit() {
    if (this.isSubmitting) return;
    this.errorMsg = '';
    this.successMsg = '';

    if (!this.token.trim()) {
      this.errorMsg = 'Token invalido.';
      return;
    }

    if (!this.newPassword || this.newPassword.length < 6) {
      this.errorMsg = 'A nova senha deve ter pelo menos 6 caracteres.';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.errorMsg = 'As senhas nao coincidem.';
      return;
    }

    this.isSubmitting = true;
    this.auth.resetPassword(this.token, this.newPassword).subscribe({
      next: (res) => {
        this.successMsg = res?.mensagem || res?.Mensagem || 'Senha redefinida com sucesso.';
      },
      error: (err) => {
        this.errorMsg = err.error?.mensagem || err.error?.Mensagem || err.error?.message || 'Falha ao redefinir senha.';
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
