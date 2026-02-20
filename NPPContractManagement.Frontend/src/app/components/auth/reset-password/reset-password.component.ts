import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  token = '';
  email = '';
  password = '';
  confirmPassword = '';
  loading = false;
  success = false;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.token = this.route.snapshot.queryParamMap.get('token') || '';
    this.email = this.route.snapshot.queryParamMap.get('email') || '';

    if (!this.token || !this.email) {
      this.error = 'Invalid password reset link. Please request a new one.';
    }
  }

  get passwordsMatch(): boolean {
    return this.password === this.confirmPassword;
  }

  get formValid(): boolean {
    return !!this.token && !!this.email && this.password.length >= 6 && this.passwordsMatch;
  }

  onSubmit(): void {
    if (!this.formValid) return;

    this.loading = true;
    this.error = '';

    this.authService.resetPassword({
      token: this.token,
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => {
        this.loading = false;
        this.success = true;
      },
      error: () => {
        this.loading = false;
        this.error = 'Invalid or expired reset token. Please request a new password reset.';
      }
    });
  }
}
