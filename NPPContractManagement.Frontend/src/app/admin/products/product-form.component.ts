import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ProductService } from '../../services/product.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { DistributorProductCodeService } from '../../services/distributor-product-code.service';
import { DistributorProductCode } from '../../models/distributor-product-code.model';
import { Product, CreateProductRequest, UpdateProductRequest, ProductStatus } from '../../models/product.model';
import { Manufacturer } from '../../models/manufacturer.model';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css']
})
export class ProductFormComponent implements OnInit {
  form: FormGroup;
  product: Product | null = null;
  manufacturers: Manufacturer[] = [];
  filteredManufacturers: Manufacturer[] = [];
  manufacturerFilter = '';
  showManufacturerPanel = false;
  isEditMode = false;
  isViewMode = false;  // ADDED for view/edit mode pattern
  loading = false;
  submitting = false;
  error: string | null = null;
  successMessage: string | null = null;

  ProductStatus = ProductStatus;

  distributorProductCodes: DistributorProductCode[] = [];
  loadingDpcs = false;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private manufacturerService: ManufacturerService,
    private distributorProductCodeService: DistributorProductCodeService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.createForm();
  }

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.error = 'You must be logged in to access this page.';
      setTimeout(() => this.router.navigate(['/login']), 1500);
      return;
    }

    this.loadManufacturers();

    // Check if we're in view mode or edit mode
    const url = this.router.url;
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      if (url.includes('/view/')) {
        this.isViewMode = true;
        this.isEditMode = true;
      } else if (url.includes('/edit/')) {
        this.isViewMode = false;
        this.isEditMode = true;
      }
      this.loadProduct(+id);
    }
  }

  private createForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      manufacturerProductCode: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      gtin: ['', [Validators.maxLength(100)]],
      upc: ['', [Validators.maxLength(100)]],
      sku: ['', [Validators.maxLength(100)]],
      packSize: ['', [Validators.required, Validators.maxLength(100)]],
      manufacturerId: [null, [Validators.required]],
      category: ['', [Validators.maxLength(100)]],
      subCategory: ['', [Validators.maxLength(100)]],
      brand: ['', [Validators.required, Validators.maxLength(100)]],
      tertiaryCategory: ['', [Validators.maxLength(100)]],
      alwaysList: [null],
      notes: ['', [Validators.maxLength(1000)]],
      status: ['Active', [Validators.required]]
    });
  }

  private loadProduct(id: number): void {
    this.loading = true;
    this.error = null;

    this.productService.getById(id).subscribe({
      next: (p) => {
        this.product = p;
        this.populateForm(p);
        this.loadDistributorProductCodes(id);

        // Disable form if in view mode
        if (this.isViewMode) {
          this.form.disable();
        }

        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load product. Please try again.';
        this.loading = false;
        console.error('Error loading product:', err);
      }
    });
  }

  private populateForm(p: Product): void {
    this.form.patchValue({
      name: (p as any).name || p.description || '',
      manufacturerProductCode: p.manufacturerProductCode || '',
      description: p.description || '',

      gtin: p.gtin || '',
      upc: p.upc || '',
      sku: (p as any).sku || '',
      packSize: p.packSize || '',
      manufacturerId: p.manufacturerId,
      category: p.category || '',
      subCategory: p.subCategory || '',
      brand: (p as any).brand || '',
      tertiaryCategory: (p as any).tertiaryCategory || '',
      alwaysList: (p as any).alwaysList ?? null,
      notes: (p as any).notes || '',
      status: p.status
    });
  }
  private loadDistributorProductCodes(productId: number): void {
    this.loadingDpcs = true;
    this.distributorProductCodeService.getPaginated(1, 100, undefined, 'asc', undefined, undefined, [productId]).subscribe({
      next: (result) => {
        this.distributorProductCodes = result.items;
        this.loadingDpcs = false;
      },
      error: () => {
        this.loadingDpcs = false;
      }
    });
  }

  private loadManufacturers(): void {
    this.manufacturerService.getAllActive().subscribe({
      next: (items) => {
        this.manufacturers = items || [];
        this.applyManufacturerFilter('');
      },
      error: () => { /* ignore */ }
    });
  }

  // --- Manufacturer searchable single-select helpers ---
  toggleManufacturerPanel(): void {
    this.showManufacturerPanel = !this.showManufacturerPanel;
    if (this.showManufacturerPanel) {
      this.applyManufacturerFilter(this.manufacturerFilter);
    }
  }

  closeManufacturerPanel(): void {
    this.showManufacturerPanel = false;
  }

  applyManufacturerFilter(term: string): void {
    this.manufacturerFilter = term || '';
    const t = this.manufacturerFilter.toLowerCase();
    this.filteredManufacturers = (this.manufacturers || []).filter(m => (m.name || '').toLowerCase().includes(t));
  }

  selectManufacturer(id: number): void {
    this.form.get('manufacturerId')?.setValue(id);
    this.showManufacturerPanel = false;
  }

  getManufacturerName(id: number | null): string {
    const m = (this.manufacturers || []).find(x => x.id === Number(id));
    return m?.name || '';
  }


  onSubmit(): void {
    if (this.form.invalid) {
      this.markFormGroupTouched();
      return;
    }


    this.submitting = true;
    this.error = null;

    const v = this.form.value;

    if (this.isEditMode && this.product) {
      const req: UpdateProductRequest = {
        name: v.name?.trim(),
        manufacturerProductCode: v.manufacturerProductCode?.trim(),
        description: v.description?.trim(),
        gtin: v.gtin?.trim(),
        upc: v.upc?.trim(),
        packSize: v.packSize?.trim(),
        manufacturerId: v.manufacturerId,
        category: v.category?.trim(),
        subCategory: v.subCategory?.trim(),
        brand: v.brand?.trim(),
        tertiaryCategory: v.tertiaryCategory?.trim(),
        alwaysList: v.alwaysList ?? null,
        notes: v.notes?.trim(),
        status: v.status
      } as any;
      // Additional optional mappings
      (req as any).sku = v.sku?.trim();

      this.productService.update(this.product.id, req).subscribe({
        next: (updatedProduct) => {
          this.submitting = false;
          this.successMessage = 'Product updated successfully!';
          this.product = updatedProduct;

          setTimeout(() => {
            this.isViewMode = true;
            this.form.disable();
            this.populateForm(updatedProduct);
            this.router.navigate(['/admin/products/view', this.product!.id], { replaceUrl: true });
            this.successMessage = null;
          }, 1000);
        },
        error: (error) => {
          this.handleHttpError(error, 'update');
          this.submitting = false;
        }
      });
    } else {
      const req: CreateProductRequest = {
        name: v.name?.trim(),
        manufacturerProductCode: v.manufacturerProductCode?.trim(),
        description: v.description?.trim(),
        gtin: v.gtin?.trim(),
        upc: v.upc?.trim(),
        packSize: v.packSize?.trim(),
        manufacturerId: v.manufacturerId,
        category: v.category?.trim(),
        subCategory: v.subCategory?.trim(),
        brand: v.brand?.trim(),
        tertiaryCategory: v.tertiaryCategory?.trim(),
        alwaysList: v.alwaysList ?? null,
        notes: v.notes?.trim(),
        status: v.status
      } as any;
      (req as any).sku = v.sku?.trim();

      this.productService.create(req).subscribe({
        next: () => {
          this.successMessage = 'Product created successfully!';
          this.submitting = false;
          setTimeout(() => this.router.navigate(['/admin/products']), 1000);
        },
        error: (error) => {
          this.handleHttpError(error, 'create');
          this.submitting = false;
        }
      });
    }
  }

  toggleEditMode(): void {
    this.isViewMode = false;
    this.form.enable();
    if (this.product) {
      this.router.navigate(['/admin/products/edit', this.product.id], { replaceUrl: true });
    }
  }

  private cancelToViewMode(): void {
    this.isViewMode = true;
    this.form.disable();
    if (this.product) {
      this.populateForm(this.product);
      this.router.navigate(['/admin/products/view', this.product.id], { replaceUrl: true });
    }
  }

  onCancel(): void {
    if (this.isViewMode) {
      // In view mode, just go back to list
      this.router.navigate(['/admin/products']);
    } else if (this.isEditMode) {
      // In edit mode, cancel back to view mode
      if (this.form.dirty) {
        if (confirm('You have unsaved changes. Are you sure you want to cancel?')) {
          this.cancelToViewMode();
        }
      } else {
        this.cancelToViewMode();
      }
    } else {
      // In create mode, go back to list
      if (this.form.dirty) {
        if (confirm('You have unsaved changes. Are you sure you want to leave?')) {
          this.router.navigate(['/admin/products']);
        }
      } else {
        this.router.navigate(['/admin/products']);
      }
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.form.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  private markFormGroupTouched(): void {
    Object.keys(this.form.controls).forEach(key => {
      const control = this.form.get(key);
      control?.markAsTouched();
    });
  }

  private handleHttpError(error: any, action: 'create' | 'update') {
    if (error.status === 401) {
      this.error = 'Unauthorized. Please log in again.';
      setTimeout(() => this.router.navigate(['/login']), 1500);
    } else if (error.status === 403) {
      this.error = `You do not have permission to ${action} products.`;
    } else if (error.status === 400) {
      this.error = error.error?.message || 'Invalid data provided. Please check your input.';
    } else if (error.status === 0) {
      this.error = 'Unable to connect to the server. Please check if the API is running.';
    } else {
      this.error = error.error?.message || error.message || 'An error occurred. Please try again.';
    }
  }
}

