import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { DistributorProductCodeService } from '../../services/distributor-product-code.service';
import { DistributorService } from '../../services/distributor.service';
import { ProductService } from '../../services/product.service';
import { Distributor } from '../../models/distributor.model';
import { Product, ProductHelper, ProductStatus } from '../../models/product.model';
import { ProductSearchModalComponent } from '../../shared/components/product-search-modal/product-search-modal.component';

@Component({
  selector: 'app-distributor-product-code-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, ProductSearchModalComponent],
  templateUrl: './distributor-product-code-form.component.html',
  styleUrls: ['./distributor-product-code-form.component.css']
})
export class DistributorProductCodeFormComponent implements OnInit {
  form!: FormGroup;
  isEdit = false;
  isViewMode = false;
  id: number | null = null;

  distributors: Distributor[] = [];

  // Distributor searchable panel
  filteredDistributors: Distributor[] = [];
  distributorFilter = '';
  showDistributorPanel = false;

  // Product — displayed via single-row table + search modal
  selectedProduct: Product | null = null;
  showProductSearchModal = false;

  saving = false;
  error: string | null = null;
  successMessage: string | null = null;
  item: any = null;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private svc: DistributorProductCodeService,
    private distributorService: DistributorService,
    private productService: ProductService
  ) {}

  private preselectedProductId: number | null = null;

  ngOnInit(): void {
    this.form = this.fb.group({
      distributorId: [null, Validators.required],
      productId: [null, Validators.required],
      distributorCode: ['', [Validators.required, Validators.maxLength(255)]],
      catchWeight: [false],
      eBrand: [false]
    });

    // Check for pre-populated productId from query params (e.g. from product view page)
    const qpProductId = this.route.snapshot.queryParamMap.get('productId');
    if (qpProductId && !isNaN(Number(qpProductId))) {
      this.preselectedProductId = Number(qpProductId);
      // Load the preselected product details
      this.productService.getById(this.preselectedProductId).subscribe({
        next: (product) => {
          this.selectedProduct = product;
          this.form.patchValue({ productId: product.id }, { emitEvent: false });
        },
        error: () => {}
      });
    }

    this.loadDistributors();

    // Check if we're in view mode or edit mode
    const url = this.router.url;
    const idParam = this.route.snapshot.paramMap.get('id');

    if (idParam) {
      if (url.includes('/view/')) {
        this.isViewMode = true;
        this.isEdit = true;
      } else if (url.includes('/edit/')) {
        this.isViewMode = false;
        this.isEdit = true;
      }
      this.id = +idParam;
      this.svc.getById(this.id).subscribe(item => {
        this.item = item;
        this.form.patchValue({
          distributorId: item.distributorId,
          productId: item.productId,
          distributorCode: item.distributorCode,
          catchWeight: !!item.catchWeight,
          eBrand: !!item.eBrand
        });

        // Load the assigned product details for the table display
        if (item.productId) {
          this.productService.getById(item.productId).subscribe({
            next: (product) => this.selectedProduct = product,
            error: () => {}
          });
        }

        if (this.isViewMode) {
          this.form.disable({ emitEvent: false });
        }
      });
    }
  }

  private loadDistributors(): void {
    this.distributorService.getPaginated(1, 1000, 'name', 'asc').subscribe({
      next: (resp: any) => this.distributors = resp.items ?? [],
      error: () => this.distributors = []
    });
  }

  // --- Distributor searchable panel ---
  toggleDistributorPanel(): void {
    this.showDistributorPanel = !this.showDistributorPanel;
    if (this.showDistributorPanel) this.applyDistributorFilter('');
  }
  closeDistributorPanel(): void { this.showDistributorPanel = false; }
  applyDistributorFilter(term: string): void {
    this.distributorFilter = term || '';
    const t = this.distributorFilter.toLowerCase();
    this.filteredDistributors = (this.distributors || []).filter(d => (d.name || '').toLowerCase().includes(t));
  }
  selectDistributor(id: number): void {
    this.form.get('distributorId')?.setValue(id);
    this.showDistributorPanel = false;
  }

  getDistributorName(id: number | null): string {
    const d = (this.distributors || []).find(x => x.id === Number(id));
    return d?.name || '';
  }

  // --- Product search modal ---
  openProductSearch(): void {
    this.showProductSearchModal = true;
  }

  onProductSelected(product: Product): void {
    this.selectedProduct = product;
    this.form.patchValue({ productId: product.id });
    this.form.get('productId')?.markAsTouched();
    this.showProductSearchModal = false;
  }

  clearProduct(): void {
    this.selectedProduct = null;
    this.form.patchValue({ productId: null });
  }

  getStatusColor(status: string): string {
    return ProductHelper.getStatusColor(status as ProductStatus);
  }

  // --- Form submission ---
  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving = true;
    this.error = null;

    const payload = { ...this.form.value };
    payload.distributorCode = (payload.distributorCode || '').trim();

    if (this.isEdit && this.id) {
      this.svc.update(this.id, payload).subscribe({
        next: (updatedItem) => {
          this.saving = false;
          this.successMessage = 'Distributor Product Code updated successfully!';
          this.item = updatedItem;

          // Refresh product display if changed
          if (updatedItem.productId && updatedItem.productId !== this.selectedProduct?.id) {
            this.productService.getById(updatedItem.productId).subscribe({
              next: (product) => this.selectedProduct = product,
              error: () => {}
            });
          }

          setTimeout(() => {
            this.isViewMode = true;
            this.form.disable({ emitEvent: false });
            this.form.patchValue({
              distributorId: updatedItem.distributorId,
              productId: updatedItem.productId,
              distributorCode: updatedItem.distributorCode,
              catchWeight: !!updatedItem.catchWeight,
              eBrand: !!updatedItem.eBrand
            });
            this.router.navigate(['/admin/distributor-product-codes/view', this.id], { replaceUrl: true });
            this.successMessage = null;
          }, 1000);
        },
        error: (err) => {
          this.error = err?.error?.message || 'Failed to update';
          this.saving = false;
        }
      });
    } else {
      this.svc.create(payload).subscribe({
        next: () => this.router.navigate(['/admin/distributor-product-codes']),
        error: (err) => this.error = err?.error?.message || 'Failed to create'
      }).add(() => this.saving = false);
    }
  }

  toggleEditMode(): void {
    this.isViewMode = false;
    this.form.enable({ emitEvent: false });
    if (this.id) {
      this.router.navigate(['/admin/distributor-product-codes/edit', this.id], { replaceUrl: true });
    }
  }

  private cancelToViewMode(): void {
    this.isViewMode = true;
    this.form.disable({ emitEvent: false });
    if (this.item) {
      this.form.patchValue({
        distributorId: this.item.distributorId,
        productId: this.item.productId,
        distributorCode: this.item.distributorCode,
        catchWeight: !!this.item.catchWeight,
        eBrand: !!this.item.eBrand
      });
      // Restore original product display
      if (this.item.productId) {
        this.productService.getById(this.item.productId).subscribe({
          next: (product) => this.selectedProduct = product,
          error: () => {}
        });
      }
      this.router.navigate(['/admin/distributor-product-codes/view', this.id], { replaceUrl: true });
    }
  }

  cancel(): void {
    if (this.isViewMode) {
      this.router.navigate(['/admin/distributor-product-codes']);
    } else if (this.isEdit) {
      if (this.form.dirty) {
        if (confirm('You have unsaved changes. Are you sure you want to cancel?')) {
          this.cancelToViewMode();
        }
      } else {
        this.cancelToViewMode();
      }
    } else {
      if (this.form.dirty) {
        if (confirm('You have unsaved changes. Are you sure you want to leave?')) {
          this.router.navigate(['/admin/distributor-product-codes']);
        }
      } else {
        this.router.navigate(['/admin/distributor-product-codes']);
      }
    }
  }
}
