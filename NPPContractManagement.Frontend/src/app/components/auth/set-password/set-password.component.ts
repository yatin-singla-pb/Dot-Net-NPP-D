import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-set-password',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  template: `<div class="container"><h2>Set Password</h2><p>Set password functionality coming soon...</p><a routerLink="/login">Back to Login</a></div>`
})
export class SetPasswordComponent {}
