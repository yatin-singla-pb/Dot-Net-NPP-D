import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { ContractViewComponent } from './contract-view.component';
import { ContractService } from '../../services/contract.service';
import { ActivatedRoute, Router } from '@angular/router';

class ContractServiceStub {
  getById(id: number) { return of({ id, contractNumber: 'CN-1', title: 'T', manufacturerId: 1, industryId: 1, status: 'Active', startDate: new Date(), endDate: new Date(), isSuspended: false, sendToPerformance: false, currentVersionNumber: 1, isActive: true, createdDate: new Date() } as any); }
  getVersions(contractId: number) { return of([{ id: 10, contractId, versionNumber: 2, title: 'v2', startDate: new Date(), endDate: new Date(), isCurrentVersion: true, isActive: true, createdDate: new Date(), prices: [] } as any]); }
}

describe('Contract Version UI Flow', () => {
  it('loads versions and navigates to create version', async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule.withRoutes([]), ContractViewComponent],
      providers: [
        { provide: ContractService, useClass: ContractServiceStub },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: new Map([['id','1']]) } } }
      ]
    }).compileComponents();

    const fixture = TestBed.createComponent(ContractViewComponent);
    const comp = fixture.componentInstance;
    const router = TestBed.inject(Router);
    spyOn(router, 'navigate').and.returnValue(Promise.resolve(true) as any);

    fixture.detectChanges();

    expect(comp.versions.length).toBe(1);
    comp.onDuplicateAmend();
    expect(router.navigate).toHaveBeenCalled();
    expect(typeof comp.onDuplicateAmend).toBe('function');
  });
});

