import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-debug',
  standalone: true,
  imports: [CommonModule],
  template: `<div class="container"><h2>Debug</h2><p>Debug functionality...</p></div>`
})
export class DebugComponent {}
