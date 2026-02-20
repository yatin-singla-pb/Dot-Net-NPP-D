import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container text-center py-5">
      <h1 class="display-1 text-muted">404</h1>
      <h2>Page Not Found</h2>
      <p class="lead">The page you're looking for doesn't exist.</p>
      <a routerLink="/dashboard" class="btn btn-primary">Go to Dashboard</a>
    </div>
  `
})
export class NotFoundComponent {}
