# Proposals Module - NPP Contract Management System

## Overview

The Proposals module enables users to create, manage, and process pricing proposals with support for multiple products, manufacturers, distributors, and operational companies. The module includes comprehensive workflow management, batch processing capabilities, and deduplication logic for handling identical product lines.

## Features

- **CRUD Operations**: Create, read, update, and delete proposals
- **Workflow Management**: Status-based workflow (Requested → Pending → Saved → Submitted → Completed)
- **Multi-Entity Support**: Associate proposals with distributors, industries, OpCos, and manufacturers
- **Product Management**: Inline product management with pricing and packing list details
- **Batch Processing**: CSV upload for bulk proposal creation
- **Manufacturer Review**: Dedicated interface for manufacturer users to review and submit proposals
- **Deduplication**: Configurable strategies for handling duplicate product lines
- **Feature Flag**: Gradual rollout using `FEATURE_PROPOSALS_V1` flag

## Database Schema

### Core Tables

- **Proposals**: Main proposal entity with metadata
- **ProposalProducts**: Product lines within proposals with pricing details
- **ProposalDistributors**: Many-to-many relationship with distributors
- **ProposalIndustries**: Many-to-many relationship with industries
- **ProposalOpCos**: Many-to-many relationship with OpCos

### Lookup Tables

- **ProposalTypes**: NewContract, Amendment, etc.
- **ProposalStatuses**: Requested, Pending, Saved, Submitted, Completed
- **PriceTypes**: List Price, Contract Price, etc.
- **ProductProposalStatuses**: Pending, Approved, Rejected

## API Endpoints

### Base URL: `/api/v1/proposals`

#### GET /api/v1/proposals
Get paginated list of proposals

**Query Parameters:**
- `page` (int): Page number (default: 1)
- `pageSize` (int): Items per page (default: 10)
- `search` (string): Search term for title/notes

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "title": "Q1 2024 Pricing Proposal",
      "proposalTypeId": 1,
      "proposalStatusId": 1,
      "manufacturerId": 1,
      "startDate": "2024-01-01",
      "endDate": "2024-03-31",
      "internalNotes": "First quarter pricing update",
      "isActive": true,
      "products": [...],
      "distributorIds": [1, 2],
      "industryIds": [1],
      "opcoIds": [1, 2]
    }
  ],
  "totalCount": 25,
  "page": 1,
  "pageSize": 10
}
```

#### POST /api/v1/proposals
Create new proposal

**Request Body:**
```json
{
  "title": "New Pricing Proposal",
  "proposalTypeId": 1,
  "proposalStatusId": 1,
  "manufacturerId": 1,
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "internalNotes": "Annual pricing review",
  "products": [
    {
      "productId": 101,
      "priceTypeId": 1,
      "proposedPrice": 25.99,
      "quantity": 100,
      "packingList": "Case of 12"
    }
  ],
  "distributorIds": [1, 2],
  "industryIds": [1],
  "opcoIds": [1, 2]
}
```

#### POST /api/v1/proposals/batch
Create multiple proposals from array

**Request Body:**
```json
[
  {
    "title": "Batch Proposal 1",
    "proposalTypeId": 1,
    "proposalStatusId": 1,
    "products": [...],
    "distributorIds": [],
    "industryIds": [],
    "opcoIds": []
  }
]
```

**Response:** Number of successfully created proposals

### Lookup Endpoints

- `GET /api/v1/lookup/proposal-types`
- `GET /api/v1/lookup/proposal-statuses`
- `GET /api/v1/lookup/price-types`
- `GET /api/v1/lookup/product-proposal-statuses`

## Frontend Components

### Core Components

1. **ProposalsListComponent** (`/admin/proposals`)
   - Paginated list with search functionality
   - Action buttons for create, edit, view, clone
   - Status-based filtering

2. **ProposalCreateComponent** (`/admin/proposals/create`)
   - Reactive form with validation
   - Inline product management
   - Multi-select for associations

3. **ProposalEditComponent** (`/admin/proposals/:id/edit`)
   - Pre-populated form for editing
   - Same functionality as create component

4. **ProposalDetailComponent** (`/admin/proposals/:id`)
   - Read-only view with formatted data
   - Status-based action buttons
   - Product list with pricing details

5. **ProposalCloneComponent** (`/admin/proposals/:id/clone`)
   - Pre-fills create form with cloned data
   - Resets status and modifies title

6. **ProposalManufacturerReviewComponent** (`/admin/proposals/:id/manufacturer-review`)
   - Manufacturer-specific review interface
   - Filtered product view
   - Save and submit actions

7. **ProposalBatchComponent** (`/admin/proposals/batch`)
   - CSV file upload
   - Data validation and preview
   - Progress tracking during batch creation

### Services

1. **ProposalService**
   - HTTP client wrapper for API calls
   - TypeScript interfaces for type safety
   - Lookup data methods

2. **DeduplicationService**
   - Configurable deduplication strategies
   - Metadata preservation for packing lists
   - Configuration management

## Deduplication Strategies

### 1. Collapse with Metadata (Default)
- Collapses duplicate product lines
- Preserves original packing lists in `metaJson` field
- Maintains first packing list as primary

### 2. Sum Quantities
- Sums quantities for duplicate products
- Combines packing lists with semicolon separator
- Stores original quantities in metadata

### 3. Keep All
- No deduplication performed
- All product lines preserved as-is

### Configuration

Set deduplication strategy via localStorage:
```javascript
// Set strategy
localStorage.setItem('PROPOSAL_DEDUPE_STRATEGY', 'collapse_with_metadata');

// Or set full configuration
localStorage.setItem('PROPOSAL_DEDUPE_CONFIG', JSON.stringify({
  strategy: 'sum_quantities',
  preserveFields: ['productId', 'priceTypeId', 'proposedPrice'],
  metadataField: 'metaJson'
}));
```

## Feature Flag

The Proposals module is controlled by the `FEATURE_PROPOSALS_V1` feature flag.

### Enable Feature Flag
```javascript
localStorage.setItem('FEATURE_PROPOSALS_V1', 'true');
```

### Disable Feature Flag
```javascript
localStorage.removeItem('FEATURE_PROPOSALS_V1');
```

When enabled, the "Proposals" menu item appears in the admin navigation.

## Deployment Instructions

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+ and npm
- MySQL 8.0+
- Entity Framework Core CLI tools

### Backend Deployment

1. **Apply Database Migration**
   ```bash
   cd NPPContractManagement.API
   dotnet ef database update --context ApplicationDbContext
   ```

2. **Verify Migration**
   ```sql
   -- Check if tables exist
   SHOW TABLES LIKE 'Proposal%';

   -- Verify seed data
   SELECT * FROM ProposalTypes;
   SELECT * FROM ProposalStatuses;
   SELECT * FROM PriceTypes;
   SELECT * FROM ProductProposalStatuses;
   ```

3. **Build and Deploy API**
   ```bash
   dotnet build --configuration Release
   dotnet publish --configuration Release --output ./publish
   ```

### Frontend Deployment

1. **Install Dependencies**
   ```bash
   cd NPPContractManagement.Frontend
   npm install
   ```

2. **Build for Production**
   ```bash
   npm run build --prod
   ```

3. **Deploy Static Files**
   Copy `dist/` contents to web server

### Verification Steps

1. **API Health Check**
   ```bash
   curl -X GET "https://your-api-url/api/v1/proposals" \
        -H "Authorization: Bearer YOUR_JWT_TOKEN"
   ```

2. **Frontend Access**
   - Navigate to `/admin/proposals`
   - Enable feature flag in browser console
   - Verify menu item appears

3. **Database Verification**
   ```sql
   -- Check migration applied
   SELECT * FROM __EFMigrationsHistory WHERE MigrationId LIKE '%Proposal%';

   -- Verify lookup data
   SELECT COUNT(*) FROM ProposalTypes; -- Should be > 0
   SELECT COUNT(*) FROM ProposalStatuses; -- Should be > 0
   ```

## Rollback Instructions

### Database Rollback
```bash
# Find previous migration
dotnet ef migrations list

# Rollback to previous migration
dotnet ef database update PreviousMigrationName --context ApplicationDbContext
```

### Frontend Rollback
1. Disable feature flag:
   ```javascript
   localStorage.removeItem('FEATURE_PROPOSALS_V1');
   ```

2. Deploy previous frontend version

### API Rollback
1. Deploy previous API version
2. Restart application services

## Testing

### Unit Tests
```bash
# Frontend tests
cd NPPContractManagement.Frontend
npm test

# Backend tests
cd NPPContractManagement.API
dotnet test
```

### Integration Tests
```bash
# Run with test database
dotnet test --configuration Release --logger "console;verbosity=detailed"
```

### Manual Testing Checklist

- [ ] Create new proposal with products
- [ ] Edit existing proposal
- [ ] Clone proposal
- [ ] Submit proposal workflow
- [ ] Manufacturer review interface
- [ ] Batch CSV upload
- [ ] Deduplication functionality
- [ ] Feature flag toggle
- [ ] API endpoint responses
- [ ] Database constraints and relationships

## Troubleshooting

### Common Issues

1. **Migration Fails**
   - Check database connection string
   - Verify MySQL server is running
   - Check user permissions

2. **Feature Flag Not Working**
   - Clear browser cache
   - Check localStorage in developer tools
   - Verify navigation component logic

3. **API 401 Unauthorized**
   - Check JWT token validity
   - Verify authentication middleware
   - Check user permissions

4. **Frontend Build Errors**
   - Clear node_modules and reinstall
   - Check TypeScript version compatibility
   - Verify Angular CLI version

### Support Contacts

- **Development Team**: dev-team@company.com
- **Database Admin**: dba@company.com
- **DevOps**: devops@company.com

## Change Log

### Version 1.0.0 (Initial Release)
- Complete CRUD operations for proposals
- Workflow management with status transitions
- Multi-entity associations (distributors, industries, OpCos)
- Batch processing with CSV upload
- Manufacturer review interface
- Configurable deduplication strategies
- Feature flag implementation
- Comprehensive test coverage

