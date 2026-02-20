import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container text-center py-5">
      <h1 class="display-1 text-danger">403</h1>
      <h2>Unauthorized Access</h2>
      <p class="lead">You don't have permission to access this resource.</p>
      <a routerLink="/dashboard" class="btn btn-primary">Go to Dashboard</a>
    </div>
  `
})
export class UnauthorizedComponent {}
