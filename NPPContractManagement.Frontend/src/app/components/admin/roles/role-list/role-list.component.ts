import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-role-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `<div class="container"><h2>Role List</h2><p>Role management functionality coming soon...</p></div>`
})
export class RoleListComponent {}
