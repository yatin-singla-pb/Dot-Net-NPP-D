import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-route-test',
  standalone: true,
  imports: [CommonModule],
  template: `<div class="container"><h2>Route Test</h2><p>Route testing functionality...</p></div>`
})
export class RouteTestComponent {}
