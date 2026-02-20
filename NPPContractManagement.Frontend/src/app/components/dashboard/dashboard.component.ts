import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { AuthService } from '../../services/auth.service';
import { ContractService } from '../../services/contract.service';
import { ProposalService } from '../../services/proposal.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { VelocityExceptionService, VelocityExceptionDto } from '../../services/velocity-exception.service';
import { DashboardPreferencesService, DashboardWidget } from '../../services/dashboard-preferences.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, DragDropModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  Math = Math; // Expose Math to template
  user: any = null;
  currentUser: any = null;

  recentContracts: Array<{ id: number; name: string; createdDate?: Date; modifiedDate?: Date }>=[];
  recentProposals: Array<{ id: number; title: string; proposalStatusName?: string; createdDate?: Date; modifiedDate?: Date }>=[];
  expiringContractsWithoutProposals: Array<{ id: number; name: string; endDate: Date; daysUntilExpiry: number }>=[];
  velocityExceptions: VelocityExceptionDto[] = [];
  velocityExceptionsTotalCount = 0;
  velocityExceptionsPage = 1;
  velocityExceptionsPageSize = 5;

  // Manufacturer-specific proposals
  requestedProposals: Array<{ id: number; title: string; createdDate?: Date }>=[];
  pendingProposals: Array<{ id: number; title: string; createdDate?: Date }>=[];
  completedProposals: Array<{ id: number; title: string; createdDate?: Date }>=[];

  // NPP User specific proposals
  submittedProposals: Array<{ id: number; title: string; manufacturerName?: string; createdDate?: Date; modifiedDate?: Date }>=[];

  loadingRecent = false;
  loadingExpiringContracts = false;
  loadingVelocityExceptions = false;
  loadingRequestedProposals = false;
  loadingPendingProposals = false;
  loadingCompletedProposals = false;
  loadingSubmittedProposals = false;
  manufacturerNameLabel: string = '';
  completedProposalsDays = 30; // Default to last 30 days

  // Widget management
  widgets: DashboardWidget[] = [];
  editMode = false;

  stats = {
    totalContracts: 0,
    activeContracts: 0,
    totalIndustries: 0,
    totalDistributors: 0,
    totalOpCos: 0,
    totalManufacturers: 0
  };

  constructor(
    private authService: AuthService,
    private router: Router,
    private contractService: ContractService,
    private proposalService: ProposalService,
    private manufacturerService: ManufacturerService,
    private velocityExceptionService: VelocityExceptionService,
    private dashboardPreferencesService: DashboardPreferencesService
  ) {}

  ngOnInit(): void {
    this.loadUserInfo();
    this.loadManufacturerNames();
    this.loadDashboardStats();

    // Initialize widgets based on role
    this.dashboardPreferencesService.initializeForRole(this.isManufacturerUser());
    this.loadWidgetPreferences();

    // Load data based on role
    if (this.isManufacturerUser()) {
      this.loadManufacturerProposals();
    } else {
      this.loadRecent();
      this.loadExpiringContractsWithoutProposals();
      this.loadVelocityExceptions();

      // Load submitted proposals for NPP users with award/complete authority
      if (this.isNppUser()) {
        this.loadSubmittedProposals();
      }
    }
  }

  private loadUserInfo(): void {
    this.user = this.authService.getCurrentUser();
    this.currentUser = this.user;
  }

  private loadDashboardStats(): void {
    this.contractService.getDashboardStats().subscribe({
      next: (data: any) => {
        this.stats = {
          totalContracts: data.totalContracts ?? 0,
          activeContracts: data.activeContracts ?? 0,
          totalIndustries: data.totalIndustries ?? 0,
          totalDistributors: data.totalDistributors ?? 0,
          totalOpCos: data.totalOpCos ?? 0,
          totalManufacturers: data.totalManufacturers ?? 0
        };
      },
      error: () => {
        this.stats = { totalContracts: 0, activeContracts: 0, totalIndustries: 0, totalDistributors: 0, totalOpCos: 0, totalManufacturers: 0 };
      }
    });
  }

  private loadRecent(): void {
    this.loadingRecent = true;
    // Contracts
    this.contractService.getPaginated(1, 5).subscribe({
      next: (resp) => {
        const allowed = new Set((this.authService.manufacturerIds || []).map((x: any) => Number(x)));
        let items = resp?.items || [];
        if (this.isManufacturerUser()) {
          items = items.filter((c: any) => c && c.manufacturerId != null && allowed.has(Number(c.manufacturerId)));
        }
        this.recentContracts = items.map((c: any) => ({ id: c.id, name: c.name ?? 'Contract', createdDate: c.createdDate ? new Date(c.createdDate) : undefined, modifiedDate: c.modifiedDate ? new Date(c.modifiedDate) : undefined }));
        this.loadingRecent = false;
      },
      error: () => { this.loadingRecent = false; }
    });
    // Proposals
    this.proposalService.getPaginated(1, 5).subscribe({
      next: (resp: any) => {
        const allowed = new Set((this.authService.manufacturerIds || []).map((x: any) => Number(x)));
        let items = resp?.data ?? resp?.items ?? [];
        if (this.isManufacturerUser()) {
          items = items.filter((p: any) => p && p.manufacturerId != null && allowed.has(Number(p.manufacturerId)));
        } else {
          // Admin/NPP: hide 'Saved' proposals (status id 3 per seed)
          items = items.filter((p: any) => Number(p?.proposalStatusId) !== 3);
        }
        this.recentProposals = items.map((p: any) => ({
          id: p.id ?? p.Id,
          title: p.title ?? p.Title ?? 'Proposal',
          proposalStatusName: p.proposalStatusName ?? p.ProposalStatusName,
          createdDate: (p.createdDate || p.CreatedDate) ? new Date(p.createdDate ?? p.CreatedDate) : undefined,
          modifiedDate: (p.modifiedDate || p.ModifiedDate) ? new Date(p.modifiedDate ?? p.ModifiedDate) : undefined
        }));
      },
      error: () => {}
    });
  }

  getUserDisplayName(): string {
    return 'Patrick Hall';
  }

  getUserRole(): string {
    return this.user?.role || 'User';
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  isManufacturerUser(): boolean {
    return this.authService.hasRole('Manufacturer');
  }

  isAdminUser(): boolean {
    return this.authService.hasAnyRole(['System Administrator', 'Contract Manager']);
  }

  isNppUser(): boolean {
    return this.authService.hasAnyRole(['System Administrator', 'Contract Manager']);
  }

  private loadExpiringContractsWithoutProposals(): void {
    // Only load for admin users
    if (!this.isAdminUser()) {
      return;
    }

    this.loadingExpiringContracts = true;
    this.contractService.getExpiringWithoutProposals(90).subscribe({
      next: (contracts) => {
        this.expiringContractsWithoutProposals = contracts.map((c: any) => {
          const endDate = new Date(c.endDate);
          const today = new Date();
          const diffTime = endDate.getTime() - today.getTime();
          const daysUntilExpiry = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

          return {
            id: c.id,
            name: c.name ?? `Contract #${c.id}`,
            endDate: endDate,
            daysUntilExpiry: daysUntilExpiry
          };
        });
        this.loadingExpiringContracts = false;
      },
      error: () => {
        this.loadingExpiringContracts = false;
      }
    });
  }

  getDaysUntilExpiry(contract: any): number {
    const endDate = new Date(contract.endDate);
    const today = new Date();
    const diffTime = endDate.getTime() - today.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }

  private loadVelocityExceptions(): void {
    // Only load for admin users
    if (!this.isAdminUser()) {
      return;
    }

    this.loadingVelocityExceptions = true;
    this.velocityExceptionService.getRecentExceptions(this.velocityExceptionsPage, this.velocityExceptionsPageSize).subscribe({
      next: (response) => {
        this.velocityExceptions = response.rows;
        this.velocityExceptionsTotalCount = response.totalCount;
        this.loadingVelocityExceptions = false;
      },
      error: () => {
        this.loadingVelocityExceptions = false;
      }
    });
  }

  onVelocityExceptionsPageChange(page: number): void {
    this.velocityExceptionsPage = page;
    this.loadVelocityExceptions();
  }

  get velocityExceptionsTotalPages(): number {
    return Math.ceil(this.velocityExceptionsTotalCount / this.velocityExceptionsPageSize);
  }

  private loadSubmittedProposals(): void {
    // Only load for NPP users
    if (!this.isNppUser()) {
      return;
    }

    this.loadingSubmittedProposals = true;
    this.proposalService.getByStatus('Submitted', 10).subscribe({
      next: (proposals: any[]) => {
        this.submittedProposals = proposals.map((p: any) => ({
          id: p.id,
          title: p.title || 'Proposal',
          manufacturerName: p.manufacturerName || 'N/A',
          createdDate: p.createdDate ? new Date(p.createdDate) : undefined,
          modifiedDate: p.modifiedDate ? new Date(p.modifiedDate) : undefined
        }));
        this.loadingSubmittedProposals = false;
      },
      error: () => {
        this.loadingSubmittedProposals = false;
      }
    });
  }

  private loadWidgetPreferences(): void {
    this.widgets = this.dashboardPreferencesService.getVisibleWidgets(
      this.isAdminUser(),
      this.isManufacturerUser()
    );
  }

  private loadManufacturerProposals(): void {
    const manufacturerIds = this.authService.manufacturerIds || [];
    if (manufacturerIds.length === 0) {
      return;
    }

    // Use the first manufacturer ID (or you could load for all and combine)
    const manufacturerId = Number(manufacturerIds[0]);

    // Load Requested Proposals
    this.loadingRequestedProposals = true;
    this.proposalService.getByStatusAndManufacturer('Requested', manufacturerId, 5).subscribe({
      next: (proposals) => {
        this.requestedProposals = proposals.map((p: any) => ({
          id: p.id,
          title: p.title || 'Proposal',
          createdDate: p.createdDate ? new Date(p.createdDate) : undefined
        }));
        this.loadingRequestedProposals = false;
      },
      error: () => {
        this.loadingRequestedProposals = false;
      }
    });

    // Load Pending Proposals (Submitted status)
    this.loadingPendingProposals = true;
    this.proposalService.getByStatusAndManufacturer('Submitted', manufacturerId, 5).subscribe({
      next: (proposals) => {
        this.pendingProposals = proposals.map((p: any) => ({
          id: p.id,
          title: p.title || 'Proposal',
          createdDate: p.createdDate ? new Date(p.createdDate) : undefined
        }));
        this.loadingPendingProposals = false;
      },
      error: () => {
        this.loadingPendingProposals = false;
      }
    });

    // Load Completed Proposals (last X days)
    this.loadingCompletedProposals = true;
    this.proposalService.getByStatusAndManufacturer('Completed', manufacturerId, 5, this.completedProposalsDays).subscribe({
      next: (proposals) => {
        this.completedProposals = proposals.map((p: any) => ({
          id: p.id,
          title: p.title || 'Proposal',
          createdDate: p.createdDate ? new Date(p.createdDate) : undefined
        }));
        this.loadingCompletedProposals = false;
      },
      error: () => {
        this.loadingCompletedProposals = false;
      }
    });
  }

  onWidgetDrop(event: CdkDragDrop<DashboardWidget[]>): void {
    if (event.previousIndex !== event.currentIndex) {
      const updatedWidgets = [...this.widgets];
      moveItemInArray(updatedWidgets, event.previousIndex, event.currentIndex);
      this.widgets = updatedWidgets;
      this.dashboardPreferencesService.updateWidgetOrder(updatedWidgets);
    }
  }

  toggleEditMode(): void {
    this.editMode = !this.editMode;
  }

  toggleWidgetVisibility(widgetId: string): void {
    this.dashboardPreferencesService.toggleWidgetVisibility(widgetId);
    this.loadWidgetPreferences();
  }

  resetWidgets(): void {
    if (confirm('Are you sure you want to reset the dashboard to default layout?')) {
      this.dashboardPreferencesService.resetToDefaults(this.isManufacturerUser());
      this.loadWidgetPreferences();
      this.editMode = false;
    }
  }

  isWidgetVisible(widgetId: string): boolean {
    return this.widgets.some(w => w.id === widgetId);
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
