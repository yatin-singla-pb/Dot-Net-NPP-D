import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { FeatureFlagService } from '../../../services/feature-flag.service';
import { ManufacturerService } from '../../../services/manufacturer.service';
import { forkJoin } from 'rxjs';


@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})

export class HeaderComponent implements OnInit {
  user: any = null;
  isMenuCollapsed = true;
  manufacturerNameLabel: string = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    public featureFlags: FeatureFlagService,
    private manufacturerService: ManufacturerService
  ) {}

  ngOnInit(): void {
    // Prefer the BehaviorSubject so it stays reactive as auth state changes
    this.authService.currentUser$.subscribe(u => {
      this.user = u;
      this.loadManufacturerNames();
    });

    // Also attempt to refresh from API to ensure latest user info
    this.authService.getCurrentUser().subscribe({
      next: (u) => { this.user = u; this.loadManufacturerNames(); },
      error: () => {/* ignore if endpoint not available */}
    });
  }

  toggleMenu(): void {
    this.isMenuCollapsed = !this.isMenuCollapsed;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  goToProfile(event?: Event): void {
    if (event) { event.preventDefault(); }
    this.router.navigate(['/profile']);
  }

  getUserDisplayName(): string {
    if (this.user) {
      return this.user.firstName && this.user.lastName
        ? `${this.user.firstName} ${this.user.lastName}`
        : this.user.email;
    }
    // Fallback to template name when no user data is available
    return 'Patrick Hall';
  }

  getUserRole(): string {
    return (this.user?.roles && this.user.roles.length)
      ? this.user.roles.map((r: any) => r.name || r).join(', ')
      : 'User';
  }

  isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  isManufacturerUser(): boolean {
    return this.authService.hasRole('Manufacturer');
  }

  isContractViewerUser(): boolean {
    return this.authService.isContractViewer();
  }

  isAdminUser(): boolean {
    return this.authService.isAdminOrManager();
  }

  isHeadlessUser(): boolean {
    return this.authService.isHeadlessOnly();
  }

  hasManufacturerAssignments(): boolean {
    return (this.authService.manufacturerIds || []).length > 0;
  }

  private loadManufacturerNames(): void {
    if (!this.isManufacturerUser()) { this.manufacturerNameLabel = ''; return; }
    const ids = (this.authService.manufacturerIds || []).map((x: any) => Number(x)).filter((n: number) => !isNaN(n));
    if (!ids.length) { this.manufacturerNameLabel = ''; return; }
    const requests = ids.map(id => this.manufacturerService.getById(id));
    forkJoin(requests).subscribe({
      next: (list: any[]) => {
        const names = (list || []).map((m: any) => m?.name).filter((n: any) => !!n);
        this.manufacturerNameLabel = names.join(', ');
      },
      error: () => {
        this.manufacturerNameLabel = ids.length === 1 ? `Manufacturer #${ids[0]}` : `Manufacturers (${ids.length})`;
      }
    });
  }
}
