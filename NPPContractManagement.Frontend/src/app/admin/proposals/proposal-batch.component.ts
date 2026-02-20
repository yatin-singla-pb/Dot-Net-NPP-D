import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ProposalService, ProposalCreateDto } from '../../services/proposal.service';

interface BatchProposal {
  title: string;
  proposalTypeId: number;
  manufacturerId?: number;
  startDate?: string;
  endDate?: string;
  internalNotes?: string;
  products: {
    productId: number;
    priceTypeId?: number;
    quantity?: number;
    uom?: string;
    billbacksAllowed?: boolean;
    allowance?: number;
    commercialDelPrice?: number;
    commercialFobPrice?: number;
    commodityDelPrice?: number;
    commodityFobPrice?: number;
    pua?: number;
    ffsPrice?: number;
    noiPrice?: boolean;
    ptv?: number;
    manufacturerNotes?: string;
  }[];
  distributorIds: number[];
  industryIds: number[];
  opcoIds: number[];
}

interface BatchProgress {
  total: number;
  processed: number;
  successful: number;
  failed: number;
  errors: string[];
  isProcessing: boolean;
  isComplete: boolean;
}

@Component({
  selector: 'app-proposal-batch',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './proposal-batch.component.html'
})
export class ProposalBatchComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  error: string | null = null;
  
  csvFile: File | null = null;
  csvData: any[] = [];
  parsedProposals: BatchProposal[] = [];
  progress: BatchProgress = {
    total: 0,
    processed: 0,
    successful: 0,
    failed: 0,
    errors: [],
    isProcessing: false,
    isComplete: false
  };

  // Sample CSV template
  csvTemplate = `title,proposalTypeId,manufacturerId,startDate,endDate,internalNotes,productId,priceTypeId,uom,billbacksAllowed,allowance,commercialDelPrice,commercialFobPrice,commodityDelPrice,commodityFobPrice,pua,ffsPrice,noiPrice,ptv,manufacturerNotes,quantity,distributorIds,industryIds,opcoIds
"Q1 2024 Pricing Proposal",1,1,"2024-01-01","2024-03-31","First quarter pricing",101,1,Cases,true,0.50,25.99,,24.75,,0.10,1.25,true,0.05,"Seasonal allowance",100,1;2,1,1;2
"Q1 2024 Pricing Proposal",1,1,"2024-01-01","2024-03-31","First quarter pricing",102,1,Pounds,false,,15.50,,,,,0.05,false,0.40,,200,1;2,1,1;2
"Special Discount Proposal",2,2,"2024-02-01","2024-04-30","Special pricing for bulk orders",201,2,Cases,true,1.00,45.00,44.50,43.25,42.90,0.15,1.50,true,0.10,"Promo notes",50,3,2,3`;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private proposalService: ProposalService
  ) {
    this.createForm();
  }

  ngOnInit(): void {}

  private createForm(): void {
    this.form = this.fb.group({
      csvFile: [null, [Validators.required]]
    });
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file && file.type === 'text/csv') {
      this.csvFile = file;
      this.form.patchValue({ csvFile: file });
      this.parseCSV(file);
    } else {
      this.error = 'Please select a valid CSV file';
      this.csvFile = null;
      this.csvData = [];
      this.parsedProposals = [];
    }
  }

  private parseCSV(file: File): void {
    this.loading = true;
    this.error = null;

    const reader = new FileReader();
    reader.onload = (e) => {
      try {
        const csv = e.target?.result as string;
        const lines = csv.split('\n').filter(line => line.trim());
        
        if (lines.length < 2) {
          throw new Error('CSV file must contain at least a header row and one data row');
        }

        const headers = this.parseCSVLine(lines[0]);
        const data = lines.slice(1).map(line => {
          const values = this.parseCSVLine(line);
          const row: any = {};
          headers.forEach((header, index) => {
            row[header.trim()] = values[index]?.trim() || '';
          });
          return row;
        });

        this.csvData = data;
        this.parsedProposals = this.groupProposalData(data);
        this.loading = false;
      } catch (error) {
        this.error = `Failed to parse CSV: ${error}`;
        this.loading = false;
      }
    };

    reader.onerror = () => {
      this.error = 'Failed to read CSV file';
      this.loading = false;
    };

    reader.readAsText(file);
  }

  private parseCSVLine(line: string): string[] {
    const result = [];
    let current = '';
    let inQuotes = false;
    
    for (let i = 0; i < line.length; i++) {
      const char = line[i];
      
      if (char === '"') {
        inQuotes = !inQuotes;
      } else if (char === ',' && !inQuotes) {
        result.push(current);
        current = '';
      } else {
        current += char;
      }
    }
    
    result.push(current);
    return result;
  }

  private groupProposalData(data: any[]): BatchProposal[] {
    const proposalMap = new Map<string, BatchProposal>();

    data.forEach(row => {
      const key = `${row.title}_${row.proposalTypeId}_${row.manufacturerId}`;
      
      if (!proposalMap.has(key)) {
        proposalMap.set(key, {
          title: row.title,
          proposalTypeId: parseInt(row.proposalTypeId) || 1,
          manufacturerId: row.manufacturerId ? parseInt(row.manufacturerId) : undefined,
          startDate: row.startDate || undefined,
          endDate: row.endDate || undefined,
          internalNotes: row.internalNotes || undefined,
          products: [],
          distributorIds: this.parseIdList(row.distributorIds),
          industryIds: this.parseIdList(row.industryIds),
          opcoIds: this.parseIdList(row.opcoIds)
        });
      }

      const proposal = proposalMap.get(key)!;
      if (row.productId) {
        proposal.products.push({
          productId: parseInt(row.productId),
          priceTypeId: row.priceTypeId ? parseInt(row.priceTypeId) : undefined,
          quantity: row.quantity ? parseInt(row.quantity) : undefined,
          uom: row.uom || undefined,
          billbacksAllowed: row.billbacksAllowed ? String(row.billbacksAllowed).toLowerCase() === 'true' : undefined,
          allowance: row.allowance ? parseFloat(row.allowance) : undefined,
          commercialDelPrice: row.commercialDelPrice ? parseFloat(row.commercialDelPrice) : undefined,
          commercialFobPrice: row.commercialFobPrice ? parseFloat(row.commercialFobPrice) : undefined,
          commodityDelPrice: row.commodityDelPrice ? parseFloat(row.commodityDelPrice) : undefined,
          commodityFobPrice: row.commodityFobPrice ? parseFloat(row.commodityFobPrice) : undefined,
          pua: row.pua ? parseFloat(row.pua) : undefined,
          ffsPrice: row.ffsPrice ? parseFloat(row.ffsPrice) : undefined,
          noiPrice: row.noiPrice ? String(row.noiPrice).toLowerCase() === 'true' : undefined,
          ptv: row.ptv ? parseFloat(row.ptv) : undefined,
          manufacturerNotes: row.manufacturerNotes || undefined
        });
      }
    });

    return Array.from(proposalMap.values());
  }

  private parseIdList(idString: string): number[] {
    if (!idString) return [];
    return idString.split(';').map(id => parseInt(id.trim())).filter(id => !isNaN(id));
  }

  downloadTemplate(): void {
    const blob = new Blob([this.csvTemplate], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'proposal-batch-template.csv';
    a.click();
    window.URL.revokeObjectURL(url);
  }

  async processBatch(): Promise<void> {
    if (this.parsedProposals.length === 0) {
      this.error = 'No proposals to process';
      return;
    }

    this.progress = {
      total: this.parsedProposals.length,
      processed: 0,
      successful: 0,
      failed: 0,
      errors: [],
      isProcessing: true,
      isComplete: false
    };

    for (let i = 0; i < this.parsedProposals.length; i++) {
      const proposal = this.parsedProposals[i];
      
      try {
        // Set default status to "Requested"
        const payload: ProposalCreateDto = {
          ...proposal,
          proposalStatusId: 1 // Assuming 1 is "Requested" status
        };

        await this.proposalService.create(payload).toPromise();
        this.progress.successful++;
      } catch (error: any) {
        this.progress.failed++;
        this.progress.errors.push(`Proposal "${proposal.title}": ${error?.error?.message || error?.message || 'Unknown error'}`);
      }

      this.progress.processed++;
      
      // Small delay to prevent overwhelming the server
      await new Promise(resolve => setTimeout(resolve, 100));
    }

    this.progress.isProcessing = false;
    this.progress.isComplete = true;
  }

  resetBatch(): void {
    this.csvFile = null;
    this.csvData = [];
    this.parsedProposals = [];
    this.progress = {
      total: 0,
      processed: 0,
      successful: 0,
      failed: 0,
      errors: [],
      isProcessing: false,
      isComplete: false
    };
    this.form.reset();
    this.error = null;
  }

  onCancel(): void {
    this.router.navigate(['/admin/proposals']);
  }

  getProgressPercentage(): number {
    if (this.progress.total === 0) return 0;
    return Math.round((this.progress.processed / this.progress.total) * 100);
  }

  isValidProposal(proposal: BatchProposal): boolean {
    return !!(proposal.title && proposal.proposalTypeId && proposal.products.length > 0);
  }

  getValidProposals(): BatchProposal[] {
    return this.parsedProposals.filter(p => this.isValidProposal(p));
  }

  getInvalidProposals(): BatchProposal[] {
    return this.parsedProposals.filter(p => !this.isValidProposal(p));
  }
}
