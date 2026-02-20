import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ProposalService, ProposalCreateDto, Proposal } from './proposal.service';
import { AppConfigService } from '../config/app.config.service';

describe('ProposalService', () => {
  let service: ProposalService;
  let httpMock: HttpTestingController;
  let appConfigService: jasmine.SpyObj<AppConfigService>;

  const mockApiUrl = 'https://api.test.com';

  beforeEach(() => {
    const appConfigSpy = jasmine.createSpyObj('AppConfigService', [], {
      apiUrl: mockApiUrl
    });

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        ProposalService,
        { provide: AppConfigService, useValue: appConfigSpy }
      ]
    });

    service = TestBed.inject(ProposalService);
    httpMock = TestBed.inject(HttpTestingController);
    appConfigService = TestBed.inject(AppConfigService) as jasmine.SpyObj<AppConfigService>;
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getPaginated', () => {
    it('should fetch paginated proposals and map DTOs', () => {
      const mockResponse = {
        data: [
          { id: 1, title: 'Test Proposal 1', proposalTypeId: 1, proposalStatusId: 1 },
          { id: 2, title: 'Test Proposal 2', proposalTypeId: 2, proposalStatusId: 2 }
        ],
        totalCount: 2,
        page: 1,
        pageSize: 10
      };

      service.getPaginated(1, 10, 'test').subscribe(response => {
        expect(response.totalCount).toBe(2);
        expect(response.page).toBe(1);
        expect(response.pageSize).toBe(10);
        expect(response.data.length).toBe(2);
        expect(response.data[0].id).toBe(1);
        expect(response.data[0].title).toBe('Test Proposal 1');
      });

      const req = httpMock.expectOne(`${mockApiUrl}/v1/proposals?page=1&pageSize=10&search=test`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });

    it('should handle pagination without search term', () => {
      const mockResponse = { data: [], totalCount: 0 };

      service.getPaginated(1, 10).subscribe(response => {
        expect(response).toEqual(mockResponse);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/v1/proposals?page=1&pageSize=10`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });
  });

  describe('getById', () => {
    it('should fetch a proposal by id', () => {
      const mockProposal: Proposal = {
        id: 1,
        title: 'Test Proposal',
        proposalTypeId: 1,
        proposalStatusId: 1,
        manufacturerId: 1,
        startDate: '2024-01-01',
        endDate: '2024-12-31',
        internalNotes: 'Test notes',
        isActive: true,
        products: [],
        distributorIds: [1, 2],
        industryIds: [1],
        opcoIds: [1, 2]
      };

      service.getById(1).subscribe(proposal => {
        expect(proposal).toEqual(mockProposal);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/v1/proposals/1`);
      expect(req.request.method).toBe('GET');
      req.flush(mockProposal);
    });
  });

  describe('create', () => {
    it('should create a new proposal', () => {
      const createDto: ProposalCreateDto = {
        title: 'New Proposal',
        proposalTypeId: 1,
        proposalStatusId: 1,
        manufacturerId: 1,
        startDate: '2024-01-01',
        endDate: '2024-12-31',
        internalNotes: 'New proposal notes',
        products: [
          {
            productId: 1,
            priceTypeId: 1,
            proposedPrice: 25.99,
            quantity: 100,
            packingList: 'Case of 12'
          }
        ],
        distributorIds: [1],
        industryIds: [1],
        opcoIds: [1]
      };

      const mockResponse: Proposal = {
        id: 1,
        ...createDto,
        isActive: true
      };

      service.create(createDto).subscribe(proposal => {
        expect(proposal).toEqual(mockResponse);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/v1/proposals`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(createDto);
      req.flush(mockResponse);
    });
  });

  describe('update', () => {
    it('should update an existing proposal', () => {
      const updateDto: ProposalCreateDto = {
        title: 'Updated Proposal',
        proposalTypeId: 1,
        proposalStatusId: 2,
        manufacturerId: 1,
        startDate: '2024-01-01',
        endDate: '2024-12-31',
        internalNotes: 'Updated notes',
        products: [],
        distributorIds: [],
        industryIds: [],
        opcoIds: []
      };

      const mockResponse: Proposal = {
        id: 1,
        ...updateDto,
        isActive: true
      };

      service.update(1, updateDto).subscribe(proposal => {
        expect(proposal).toEqual(mockResponse);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/v1/proposals/1`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updateDto);
      req.flush(mockResponse);
    });
  });

  describe('clone', () => {
    it('should clone an existing proposal', () => {
      const mockResponse: Proposal = {
        id: 2,
        title: 'Copy of Test Proposal',
        proposalTypeId: 1,
        proposalStatusId: 1,
        manufacturerId: 1,
        startDate: '2024-01-01',
        endDate: '2024-12-31',
        internalNotes: 'Cloned proposal',
        isActive: true,
        products: [],
        distributorIds: [],
        industryIds: [],
        opcoIds: []
      };

      service.clone(1).subscribe(proposal => {
        expect(proposal).toEqual(mockResponse);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/v1/proposals/1/clone`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({});
      req.flush(mockResponse);
    });
  });

  describe('submit', () => {
    it('should submit a proposal', () => {
      service.submit(1).subscribe(result => {
        expect(result).toBe(true);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/v1/proposals/1/submit`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({});
      req.flush(true);
    });
  });

  describe('accept', () => {
    it('should accept a proposal', () => {
      service.accept(1).subscribe(result => {
        expect(result).toBe(true);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/v1/proposals/1/accept`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({});
      req.flush(true);
    });
  });

  describe('batch', () => {
    it('should create proposals in batch', () => {
      const proposals: ProposalCreateDto[] = [
        {
          title: 'Batch Proposal 1',
          proposalTypeId: 1,
          proposalStatusId: 1,
          products: [],
          distributorIds: [],
          industryIds: [],
          opcoIds: []
        },
        {
          title: 'Batch Proposal 2',
          proposalTypeId: 1,
          proposalStatusId: 1,
          products: [],
          distributorIds: [],
          industryIds: [],
          opcoIds: []
        }
      ];

      service.batch(proposals).subscribe(count => {
        expect(count).toBe(2);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/v1/proposals/batch`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(proposals);
      req.flush(2);
    });
  });

  describe('lookup methods', () => {
    it('should fetch proposal types', () => {
      const mockTypes = [
        { id: 1, name: 'NewContract', isActive: true },
        { id: 2, name: 'Amendment', isActive: true }
      ];

      service.getProposalTypes().subscribe(types => {
        expect(types).toEqual(mockTypes);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/v1/lookup/proposal-types`);
      expect(req.request.method).toBe('GET');
      req.flush(mockTypes);
    });

    it('should fetch proposal statuses', () => {
      const mockStatuses = [
        { id: 1, name: 'Requested', isActive: true },
        { id: 2, name: 'Pending', isActive: true }
      ];

      service.getProposalStatuses().subscribe(statuses => {
        expect(statuses).toEqual(mockStatuses);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/v1/lookup/proposal-statuses`);
      expect(req.request.method).toBe('GET');
      req.flush(mockStatuses);
    });

    it('should fetch price types', () => {
      const mockPriceTypes = [
        { id: 1, name: 'List Price', isActive: true },
        { id: 2, name: 'Contract Price', isActive: true }
      ];

      service.getPriceTypes().subscribe(priceTypes => {
        expect(priceTypes).toEqual(mockPriceTypes);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/v1/lookup/price-types`);
      expect(req.request.method).toBe('GET');
      req.flush(mockPriceTypes);
    });

    it('should fetch product proposal statuses', () => {
      const mockStatuses = [
        { id: 1, name: 'Pending', isActive: true },
        { id: 2, name: 'Approved', isActive: true }
      ];

      service.getProductProposalStatuses().subscribe(statuses => {
        expect(statuses).toEqual(mockStatuses);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/v1/lookup/product-proposal-statuses`);
      expect(req.request.method).toBe('GET');
      req.flush(mockStatuses);
    });
  });
});
