import { Routes } from '@angular/router';
import { AuthGuard, NoAuthGuard, RoleGuard } from './guards/auth.guard';

export const routes: Routes = [
  // Public routes (no authentication required)
  {
    path: 'login',
    loadComponent: () => import('./components/auth/login/login.component').then(m => m.LoginComponent),
    canActivate: [NoAuthGuard]
  },
  {
    path: 'forgot-password',
    loadComponent: () => import('./components/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent),
    canActivate: [NoAuthGuard]
  },
  {
    path: 'reset-password',
    loadComponent: () => import('./components/auth/reset-password/reset-password.component').then(m => m.ResetPasswordComponent),
    canActivate: [NoAuthGuard]
  },
  {
    path: 'set-password',
    loadComponent: () => import('./components/auth/set-password/set-password.component').then(m => m.SetPasswordComponent),
    canActivate: [NoAuthGuard]
  },
  {
    path: 'register',
    loadComponent: () => import('./components/auth/register/register-email.component').then(m => m.RegisterEmailComponent),
    canActivate: [NoAuthGuard]
  },
  {
    path: 'register/verify',
    loadComponent: () => import('./components/auth/register/register-verify-code.component').then(m => m.RegisterVerifyCodeComponent),
    canActivate: [NoAuthGuard]
  },
  {
    path: 'register/credentials',
    loadComponent: () => import('./components/auth/register/register-create-credentials.component').then(m => m.RegisterCreateCredentialsComponent),
    canActivate: [NoAuthGuard]
  },
  {
    path: 'register/success',
    loadComponent: () => import('./components/auth/register/register-success.component').then(m => m.RegisterSuccessComponent),
    canActivate: [NoAuthGuard]
  },

  // Protected routes (authentication required)
  {
    path: 'dashboard',
    loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'profile',
    loadComponent: () => import('./components/user/profile/profile.component').then(m => m.ProfileComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'route-test',
    loadComponent: () => import('./components/shared/route-test/route-test.component').then(m => m.RouteTestComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'debug',
    loadComponent: () => import('./components/debug/debug.component').then(m => m.DebugComponent)
  },
  // Contracts list (accessible to Admin, Manager, Manufacturer, Contract Viewer)
  {
    path: 'admin/contracts',
    canActivate: [AuthGuard],
    loadComponent: () => import('./admin/contracts/contracts-list.component').then(m => m.ContractsListComponent),
    data: { roles: ['System Administrator', 'Contract Manager', 'Manufacturer', 'Contract Viewer'] }
  },
  // Contract view (read-only, accessible to Contract Viewer)
  {
    path: 'admin/contracts/view/:id',
    canActivate: [AuthGuard],
    loadComponent: () => import('./admin/contracts/contract-view.component').then(m => m.ContractViewComponent),
    data: { roles: ['System Administrator', 'Contract Manager', 'Manufacturer', 'Contract Viewer'] }
  },

  // Proposals list (accessible to all non-headless roles)
  {
    path: 'admin/proposals',
    canActivate: [AuthGuard],
    loadComponent: () => import('./admin/proposals/proposals-list.component').then(m => m.ProposalsListComponent),
    data: { roles: ['System Administrator', 'Contract Manager', 'Manufacturer', 'Contract Viewer'] }
  },
  // Proposal Create (Admin, Manager, Manufacturer only - not Contract Viewer)
  {
    path: 'admin/proposals/create',
    canActivate: [AuthGuard],
    loadComponent: () => import('./admin/proposals/proposal-create.component').then(m => m.ProposalCreateComponent),
    data: { roles: ['System Administrator', 'Contract Manager', 'Manufacturer'] }
  },
  // Proposal Detail (read-only view, accessible to Contract Viewer)
  {
    path: 'admin/proposals/:id',
    canActivate: [AuthGuard],
    loadComponent: () => import('./admin/proposals/proposal-detail.component').then(m => m.ProposalDetailComponent),
    data: { roles: ['System Administrator', 'Contract Manager', 'Manufacturer', 'Contract Viewer'] }
  },
  // Proposal Edit (not Contract Viewer)
  {
    path: 'admin/proposals/:id/edit',
    canActivate: [AuthGuard],
    loadComponent: () => import('./admin/proposals/proposal-edit.component').then(m => m.ProposalEditComponent),
    data: { roles: ['System Administrator', 'Contract Manager', 'Manufacturer'] }
  },

  // Reports accessible to Contract Viewer (top-level routes before admin parent)
  {
    path: 'admin/reports/contract-over-term',
    canActivate: [AuthGuard],
    loadComponent: () => import('./components/reports/contract-over-term-report.component').then(m => m.ContractOverTermReportComponent),
    data: { roles: ['System Administrator', 'Contract Manager', 'Contract Viewer'] }
  },
  {
    path: 'admin/reports/contract-pricing',
    canActivate: [AuthGuard],
    loadComponent: () => import('./components/reports/contract-pricing-report.component').then(m => m.ContractPricingReportComponent),
    data: { roles: ['System Administrator', 'Contract Manager', 'Contract Viewer'] }
  },


  // Administration routes (admin/manager roles required)
  {
    path: 'admin',
    canActivate: [AuthGuard],
    data: { roles: ['System Administrator', 'Contract Manager'] },
    children: [
      {
        path: 'users',
        loadComponent: () => import('./components/admin/users/user-list/user-list.component').then(m => m.UserListComponent)
      },
      {
        path: 'users/create',
        loadComponent: () => import('./components/admin/users/user-form/user-form.component').then(m => m.UserFormComponent)
      },
      {
        path: 'users/view/:id',
        loadComponent: () => import('./components/admin/users/user-form/user-form.component').then(m => m.UserFormComponent)
      },
      {
        path: 'users/edit/:id',
        loadComponent: () => import('./components/admin/users/user-form/user-form.component').then(m => m.UserFormComponent)
      },
      {
        path: 'roles',
        loadComponent: () => import('./components/admin/roles/role-list/role-list.component').then(m => m.RoleListComponent)
      },
      {
        path: 'manufacturers',
        loadComponent: () => import('./admin/manufacturers/manufacturers-list.component').then(m => m.ManufacturersListComponent)
      },
      {
        path: 'manufacturers/create',
        loadComponent: () => import('./admin/manufacturers/manufacturer-form.component').then(m => m.ManufacturerFormComponent)
      },
      {
        path: 'manufacturers/view/:id',
        loadComponent: () => import('./admin/manufacturers/manufacturer-form.component').then(m => m.ManufacturerFormComponent)
      },
      {
        path: 'manufacturers/edit/:id',
        loadComponent: () => import('./admin/manufacturers/manufacturer-form.component').then(m => m.ManufacturerFormComponent)
      },
      {
        path: 'distributors',
        loadComponent: () => import('./admin/distributors/distributors-list.component').then(m => m.DistributorsListComponent)
      },
      {
        path: 'distributors/create',
        loadComponent: () => import('./admin/distributors/distributor-form.component').then(m => m.DistributorFormComponent)
      },
      {
        path: 'distributors/view/:id',
        loadComponent: () => import('./admin/distributors/distributor-form.component').then(m => m.DistributorFormComponent)
      },
      {
        path: 'distributors/edit/:id',
        loadComponent: () => import('./admin/distributors/distributor-form.component').then(m => m.DistributorFormComponent)
      },

      {
        path: 'contracts/create',
        loadComponent: () => import('./admin/contracts/contract-form.component').then(m => m.ContractFormComponent),
        data: { roles: ['System Administrator', 'Contract Manager'] }
      },
      {
        path: 'contracts/edit/:id',
        loadComponent: () => import('./admin/contracts/contract-form.component').then(m => m.ContractFormComponent),
        data: { roles: ['System Administrator', 'Contract Manager'] }
      },
      {
        path: 'contracts/view/:id',
        loadComponent: () => import('./admin/contracts/contract-view.component').then(m => m.ContractViewComponent)
      },
      {
        path: 'contracts/:id/versions/new',
        loadComponent: () => import('./admin/contracts/contract-edit-version.component').then(m => m.ContractEditVersionComponent),
        data: { roles: ['System Administrator', 'Contract Manager'] }
      },
      {
        path: 'industries',
        loadComponent: () => import('./admin/industries/industries-list.component').then(m => m.IndustriesListComponent)
      },
      {
        path: 'industries/create',
        loadComponent: () => import('./admin/industries/industry-form.component').then(m => m.IndustryFormComponent)
      },
      {
        path: 'industries/view/:id',
        loadComponent: () => import('./admin/industries/industry-form.component').then(m => m.IndustryFormComponent)
      },
      {
        path: 'industries/edit/:id',
        loadComponent: () => import('./admin/industries/industry-form.component').then(m => m.IndustryFormComponent)
      },
      {
        path: 'products',
        loadComponent: () => import('./admin/products/products-list.component').then(m => m.ProductsListComponent)
      },
      {
        path: 'products/create',
        loadComponent: () => import('./admin/products/product-form.component').then(m => m.ProductFormComponent)
      },
      {
        path: 'products/view/:id',
        loadComponent: () => import('./admin/products/product-form.component').then(m => m.ProductFormComponent)
      },
      {
        path: 'products/edit/:id',
        loadComponent: () => import('./admin/products/product-form.component').then(m => m.ProductFormComponent)
      },
      {
        path: 'distributor-product-codes',
        loadComponent: () => import('./admin/distributor-product-codes/distributor-product-codes-list.component').then(m => m.DistributorProductCodesListComponent)
      },
      {
        path: 'distributor-product-codes/create',
        loadComponent: () => import('./admin/distributor-product-codes/distributor-product-code-form.component').then(m => m.DistributorProductCodeFormComponent)
      },
      {
        path: 'distributor-product-codes/view/:id',
        loadComponent: () => import('./admin/distributor-product-codes/distributor-product-code-form.component').then(m => m.DistributorProductCodeFormComponent)
      },
      {
        path: 'distributor-product-codes/edit/:id',
        loadComponent: () => import('./admin/distributor-product-codes/distributor-product-code-form.component').then(m => m.DistributorProductCodeFormComponent)
      },
      {
        path: 'op-cos',
        loadComponent: () => import('./admin/op-cos/op-cos-list.component').then(m => m.OpCosListComponent)
      },
      {
        path: 'proposals',
        loadComponent: () => import('./admin/proposals/proposals-list.component').then(m => m.ProposalsListComponent)
      },
      {
        path: 'proposals/create',
        loadComponent: () => import('./admin/proposals/proposal-create.component').then(m => m.ProposalCreateComponent)
      },
      {
        path: 'proposals/:id',
        loadComponent: () => import('./admin/proposals/proposal-detail.component').then(m => m.ProposalDetailComponent)
      },
      {
        path: 'proposals/:id/edit',
        loadComponent: () => import('./admin/proposals/proposal-edit.component').then(m => m.ProposalEditComponent)
      },
      {
        path: 'proposals/:id/awards',
        loadComponent: () => import('./admin/proposals/proposal-awards.component').then(m => m.ProposalAwardsComponent)
      },

      {
        path: 'proposals/:id/clone',
        loadComponent: () => import('./admin/proposals/proposal-clone.component').then(m => m.ProposalCloneComponent)
      },
      {
        path: 'proposals/batch',
        loadComponent: () => import('./admin/proposals/proposal-batch.component').then(m => m.ProposalBatchComponent)
      },
      {
        path: 'proposals/:id/manufacturer-review',
        loadComponent: () => import('./admin/proposals/proposal-manufacturer-review.component').then(m => m.ProposalManufacturerReviewComponent)
      },

      {
        path: 'op-cos/create',
        loadComponent: () => import('./admin/op-cos/op-co-form.component').then(m => m.OpCoFormComponent)
      },
      {
        path: 'op-cos/view/:id',
        loadComponent: () => import('./admin/op-cos/op-co-form.component').then(m => m.OpCoFormComponent)
      },
      {
        path: 'op-cos/edit/:id',
        loadComponent: () => import('./admin/op-cos/op-co-form.component').then(m => m.OpCoFormComponent)
      },
      {
        path: 'member-accounts',
        loadComponent: () => import('./admin/member-accounts/member-accounts-list.component').then(m => m.MemberAccountsListComponent)
      },
      {
        path: 'member-accounts/create',
        loadComponent: () => import('./admin/member-accounts/member-account-form.component').then(m => m.MemberAccountFormComponent)
      },
      {
        path: 'member-accounts/view/:id',
        loadComponent: () => import('./admin/member-accounts/member-account-form.component').then(m => m.MemberAccountFormComponent)
      },
      {
        path: 'member-accounts/edit/:id',
        loadComponent: () => import('./admin/member-accounts/member-account-form.component').then(m => m.MemberAccountFormComponent)
      },
      {
        path: 'customer-accounts',
        loadComponent: () => import('./admin/customer-accounts/customer-accounts-list.component').then(m => m.CustomerAccountsListComponent)
      },
      {
        path: 'customer-accounts/create',
        loadComponent: () => import('./admin/customer-accounts/customer-account-form.component').then(m => m.CustomerAccountFormComponent)
      },
      {
        path: 'customer-accounts/view/:id',
        loadComponent: () => import('./admin/customer-accounts/customer-account-form.component').then(m => m.CustomerAccountFormComponent)
      },
      {
        path: 'customer-accounts/edit/:id',
        loadComponent: () => import('./admin/customer-accounts/customer-account-form.component').then(m => m.CustomerAccountFormComponent)
      },
      {
        path: 'velocity-data',
        loadComponent: () => import('./components/admin/velocity-data/velocity-data-list/velocity-data-list.component').then(m => m.VelocityDataListComponent)
      },
      {
        path: 'velocity',
        loadComponent: () => import('./components/velocity-reporting/velocity-reporting.component').then(m => m.VelocityReportingComponent)
      },
      {
        path: 'velocity/jobs/:jobId',
        loadComponent: () => import('./components/velocity-reporting/velocity-job-details.component').then(m => m.VelocityJobDetailsComponent)
      },
      {
        path: 'reports/contract-over-term',
        loadComponent: () => import('./components/reports/contract-over-term-report.component').then(m => m.ContractOverTermReportComponent)
      },
      {
        path: 'reports/velocity-exceptions',
        loadComponent: () => import('./components/reports/velocity-exceptions-report.component').then(m => m.VelocityExceptionsReportComponent)
      },
      {
        path: 'reports/velocity-usage',
        loadComponent: () => import('./components/reports/velocity-usage-report.component').then(m => m.VelocityUsageReportComponent)
      },
      {
        path: 'reports/contract-pricing',
        loadComponent: () => import('./components/reports/contract-pricing-report.component').then(m => m.ContractPricingReportComponent)
      }
    ]
  },

  // Error pages
  {
    path: 'unauthorized',
    loadComponent: () => import('./components/shared/unauthorized/unauthorized.component').then(m => m.UnauthorizedComponent)
  },
  {
    path: 'not-found',
    loadComponent: () => import('./components/shared/not-found/not-found.component').then(m => m.NotFoundComponent)
  },

  // Default redirects
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: '/not-found' }
];
