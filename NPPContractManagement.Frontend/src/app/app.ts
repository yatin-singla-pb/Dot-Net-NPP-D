import { Component, signal } from '@angular/core';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { HeaderComponent } from './components/shared/header/header.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('NPPContractManagement.Frontend');
  protected showHeader = signal(false);

  constructor(private router: Router) {
    // Listen to route changes to determine if header should be shown
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      // Hide header on login, forgot-password, set-password, reset-password pages
      const hideHeaderRoutes = ['/login', '/forgot-password', '/set-password', '/reset-password'];
      const shouldHideHeader = hideHeaderRoutes.some(route => event.url.startsWith(route));
      this.showHeader.set(!shouldHideHeader);
    });
  }
}
