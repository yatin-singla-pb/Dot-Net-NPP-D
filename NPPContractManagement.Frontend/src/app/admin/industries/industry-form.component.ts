import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { IndustryService } from '../../services/industry.service';
import { AuthService } from '../../services/auth.service';
import { Industry, IndustryStatus, CreateIndustryRequest, UpdateIndustryRequest } from '../../models/industry.model';

@Component({
  selector: 'app-industry-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './industry-form.component.html',
  styleUrls: ['./industry-form.component.css']
})
export class IndustryFormComponent implements OnInit {
  industryForm: FormGroup;
  industry: Industry | null = null;
  isEditMode = false;
  isViewMode = false;
  loading = false;
  submitting = false;
  error: string | null = null;
  successMessage: string | null = null;
  nameValidationError: string | null = null;

  constructor(
    private fb: FormBuilder,
    private industryService: IndustryService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.industryForm = this.createForm();
  }

  ngOnInit(): void {
    // Check authentication first
    if (!this.authService.isAuthenticated()) {
      this.error = 'You must be logged in to access this page.';
      setTimeout(() => {
        this.router.navigate(['/login']);
      }, 2000);
      return;
    }

    // Check if we're in view mode or edit mode
    const url = this.router.url;
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      if (url.includes('/view/')) {
        this.isViewMode = true;
        this.isEditMode = true; // We're viewing an existing industry
      } else if (url.includes('/edit/')) {
        this.isViewMode = false;
        this.isEditMode = true;
      }
      this.loadIndustry(+id);
    }

    // Setup name validation
    this.industryForm.get('name')?.valueChanges.subscribe(value => {
      if (value && value.length >= 2) {
        this.validateName(value);
      } else {
        this.nameValidationError = null;
      }
    });
  }

  private createForm(): FormGroup {
    return this.fb.group({
      name: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(200),
        Validators.pattern(/^[a-zA-Z0-9\s\-&.,()]+$/)
      ]],
      description: ['', [
        Validators.maxLength(1000)
      ]],
      status: ['Active', [Validators.required]]
    });
  }

  private loadIndustry(id: number): void {
    this.loading = true;
    this.error = null;

    this.industryService.getById(id).subscribe({
      next: (industry) => {
        this.industry = industry;
        this.populateForm(industry);

        // Disable form if in view mode
        if (this.isViewMode) {
          this.industryForm.disable();
        }

        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load industry. Please try again.';
        this.loading = false;
        console.error('Error loading industry:', error);
      }
    });
  }

  private populateForm(industry: Industry): void {
    this.industryForm.patchValue({
      name: industry.name,
      description: industry.description || '',
      status: industry.status
    });
  }

  private validateName(name: string): void {
    const excludeId = this.isEditMode && this.industry ? this.industry.id : undefined;
    
    this.industryService.validateName(name, excludeId).subscribe({
      next: (result) => {
        this.nameValidationError = result.isValid ? null : result.message || 'Name is not available';
      },
      error: (error) => {
        console.error('Name validation error:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.industryForm.invalid || this.nameValidationError) {
      this.markFormGroupTouched();
      return;
    }

    this.submitting = true;
    this.error = null;
    this.successMessage = null;

    const formValue = this.industryForm.value;

    if (this.isEditMode && this.industry) {
      this.updateIndustry(formValue);
    } else {
      this.createIndustry(formValue);
    }
  }

  private createIndustry(formValue: any): void {
    const request: CreateIndustryRequest = {
      name: formValue.name.trim(),
      description: formValue.description?.trim() || undefined,
      status: formValue.status as IndustryStatus
    };

    this.industryService.create(request).subscribe({
      next: (industry) => {
        this.successMessage = 'Industry created successfully!';
        this.submitting = false;

        // Redirect to edit mode or list after a delay
        setTimeout(() => {
          this.router.navigate(['/admin/industries']);
        }, 1500);
      },
      error: (error) => {
        console.error('Create industry error:', error);

        if (error.status === 401) {
          this.error = 'Unauthorized. Please log in again.';
          // Redirect to login after a delay
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 2000);
        } else if (error.status === 403) {
          this.error = 'You do not have permission to create industries.';
        } else if (error.status === 400) {
          this.error = error.error?.message || 'Invalid data provided. Please check your input.';
        } else {
          if (error.error && typeof error.error === 'string') {
            this.error = error.error;
          } else if (error.error?.message) {
            this.error = error.error.message;
          } else if (error.message) {
            this.error = error.message;
          } else if (error.status === 0) {
            this.error = 'Unable to connect to the server. Please check if the API is running.';
          } else {
            this.error = `An error occurred (Status: ${error.status}). Please try again or contact support.`;
          }
        }

        this.submitting = false;
      }
    });
  }

  private updateIndustry(formValue: any): void {
    if (!this.industry) return;

    const request: UpdateIndustryRequest = {
      name: formValue.name.trim(),
      description: formValue.description?.trim() || undefined,
      status: formValue.status as IndustryStatus,
      isActive: this.industry.isActive  // Preserve the existing isActive value
    };

    this.industryService.update(this.industry.id, request).subscribe({
      next: (updatedIndustry) => {
        this.submitting = false;
        this.successMessage = 'Industry updated successfully!';
        this.industry = updatedIndustry;

        // Switch back to view mode after successful update
        setTimeout(() => {
          this.isViewMode = true;
          this.industryForm.disable();
          this.populateForm(updatedIndustry);
          this.router.navigate(['/admin/industries/view', this.industry!.id], { replaceUrl: true });
          this.successMessage = null;
        }, 1000);
      },
      error: (error) => {
        this.error = error.message || 'Failed to update industry. Please try again.';
        this.submitting = false;
      }
    });
  }

  onDelete(): void {
    if (!this.industry) return;

    const confirmMessage = `Are you sure you want to delete "${this.industry.name}"? This action cannot be undone.`;
    
    if (confirm(confirmMessage)) {
      this.submitting = true;
      
      this.industryService.delete(this.industry.id).subscribe({
        next: () => {
          this.successMessage = 'Industry deleted successfully!';
          
          setTimeout(() => {
            this.router.navigate(['/admin/industries']);
          }, 1500);
        },
        error: (error) => {
          this.error = error.message || 'Failed to delete industry. Please try again.';
          this.submitting = false;
        }
      });
    }
  }

  onCancel(): void {
    // If we're in edit mode (not view mode), cancel back to view mode
    if (!this.isViewMode && this.industry) {
      if (this.industryForm.dirty) {
        if (confirm('You have unsaved changes. Are you sure you want to cancel?')) {
          this.cancelToViewMode();
        }
      } else {
        this.cancelToViewMode();
      }
    } else {
      // If we're in view mode or creating new, go back to list
      if (this.industryForm.dirty) {
        if (confirm('You have unsaved changes. Are you sure you want to leave?')) {
          this.router.navigate(['/admin/industries']);
        }
      } else {
        this.router.navigate(['/admin/industries']);
      }
    }
  }

  toggleEditMode(): void {
    this.isViewMode = false;
    this.industryForm.enable();
    // Update the URL to reflect edit mode
    if (this.industry) {
      this.router.navigate(['/admin/industries/edit', this.industry.id], { replaceUrl: true });
    }
  }

  private cancelToViewMode(): void {
    this.isViewMode = true;
    this.industryForm.disable();
    // Reset form to original values
    if (this.industry) {
      this.populateForm(this.industry);
      this.router.navigate(['/admin/industries/view', this.industry.id], { replaceUrl: true });
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.industryForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  private markFormGroupTouched(): void {
    Object.keys(this.industryForm.controls).forEach(key => {
      const control = this.industryForm.get(key);
      control?.markAsTouched();
    });
  }
}
