import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-register-success',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <header class="container fixed-top">
      <div class="d-flex">
        <img src="assets/images/npp_logo_denim.jpg" alt="NPP Logo" />
      </div>
    </header>

    <main class="container">
      <div class="row justify-content-center">
        <div class="col-5 text-center">

          <div class="mb-4" style="font-size: 64px; color: #28a745;">
            <i class="fa-solid fa-circle-check"></i>
          </div>

          <h3 class="mb-3">Registration Complete!</h3>
          <p class="text-muted mb-4">
            Your account has been successfully set up. You can now log in with your new User ID and password.
          </p>

          <a routerLink="/login" class="btn btn-denim btn-lg">
            Go to Login
          </a>

        </div>
      </div>
    </main>
  `
})
export class RegisterSuccessComponent {}
