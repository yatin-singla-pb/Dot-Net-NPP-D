import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  email = '';
  loading = false;
  message = '';
  error = '';

  constructor(private authService: AuthService) {}

  onSubmit(): void {
    if (this.email) {
      this.loading = true;
      this.error = '';
      this.message = '';
      this.authService.forgotPassword({ userId: this.email }).subscribe({
        next: () => {
          this.loading = false;
          this.message = 'If the email exists, a password reset link has been sent.';
        },
        error: () => {
          this.loading = false;
          this.error = 'An error occurred. Please try again later.';
        }
      });
    }
  }
}
