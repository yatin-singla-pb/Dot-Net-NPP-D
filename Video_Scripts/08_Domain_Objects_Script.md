# Video Script 8: Core Domain Objects & Data Models

## SLIDE 1: Domain Model Overview (45 seconds)

**[Show Entity Relationship Diagram]**

Welcome to Video 8, our final video covering Core Domain Objects and Data Models.

Understanding the data model is crucial for working with the API effectively.

**Core Entities:**
1. **User & Authentication** - Users, Roles, UserRoles
2. **Master Data** - Manufacturers, Products, Distributors, Industries, OpCos
3. **Proposals** - Proposal, ProposalProduct, ProposalDistributor, etc.
4. **Contracts** - Contract, ContractPrice, ContractVersion, ContractVersionPrice
5. **Velocity** - VelocityJob, VelocityShipment, IngestedFile

Let's explore each domain.

---

## SLIDE 2: User & Authentication Objects (1 minute 30 seconds)

**[Show User Entity Diagram]**

**User Entity**

```typescript
{
  id: number,
  userId: string,              // Login username
  email: string,
  firstName: string,
  lastName: string,
  phoneNumber: string,
  jobTitle: string,
  address: string,
  city: string,
  state: string,
  postCode: string,
  notes: string,
  industryId: number,          // Optional industry association
  accountStatus: number,       // 1=Active, 2=Inactive, 3=Locked
  status: number,              // Legacy status field
  class: string,               // User classification
  passwordHash: string,        // Encrypted password
  isActive: boolean,
  emailConfirmed: boolean,
  failedAuthAttempts: number,
  lastLoginDate: Date,
  createdDate: Date,
  modifiedDate: Date,
  createdBy: string,
  modifiedBy: string
}
```

**Relationships:**
- User → UserRoles → Roles (many-to-many)
- User → UserManufacturers → Manufacturers (many-to-many)
- User → Industry (many-to-one)

---

## SLIDE 3: Role Object (45 seconds)

**[Show Role Entity]**

**Role Entity**

```typescript
{
  id: number,
  name: string,                // "System Administrator", "Contract Manager", etc.
  description: string,
  isActive: boolean,
  createdDate: Date,
  modifiedDate: Date,
  createdBy: string,
  modifiedBy: string
}
```

**Standard Roles:**
1. **System Administrator** - Full system access
2. **Contract Manager** - Manage contracts and proposals
3. **Manufacturer User** - Create and manage proposals
4. **Read Only** - View-only access

**UserRole Join Table:**
```typescript
{
  userId: number,
  roleId: number
}
```

---

## SLIDE 4: Manufacturer Object (1 minute)

**[Show Manufacturer Entity]**

**Manufacturer Entity**

```typescript
{
  id: number,
  name: string,                // "Tyson Foods Inc"
  aka: string,                 // "Tyson" (also known as)
  status: string,              // "Active", "Inactive"
  primaryBrokerId: number,     // User ID of primary broker
  contactEmail: string,
  contactPhone: string,
  address: string,
  city: string,
  state: string,
  zipCode: string,
  isActive: boolean,
  createdDate: Date,
  modifiedDate: Date,
  createdBy: string,
  modifiedBy: string
}
```

**Relationships:**
- Manufacturer → Products (one-to-many)
- Manufacturer → Proposals (one-to-many)
- Manufacturer → Contracts (one-to-many)
- Manufacturer → UserManufacturers → Users (many-to-many)

---

## SLIDE 5: Product Object (1 minute)

**[Show Product Entity]**

**Product Entity**

```typescript
{
  id: number,
  sku: string,                 // "TYS-12345"
  name: string,                // "Chicken Breast Boneless"
  description: string,
  gtin: string,                // "00012345678905" (14-digit)
  upc: string,                 // "012345678905" (12-digit)
  brand: string,               // "Tyson"
  packSize: string,            // "10 LB"
  caseCount: number,           // Units per case
  manufacturerId: number,
  category: string,            // "Poultry", "Beef", etc.
  isActive: boolean,
  createdDate: Date,
  modifiedDate: Date,
  createdBy: string,
  modifiedBy: string
}
```

**Relationships:**
- Product → Manufacturer (many-to-one)
- Product → ProposalProducts (one-to-many)
- Product → ContractPrices (one-to-many)

---

## SLIDE 6: Distributor, Industry, OpCo Objects (1 minute)

**[Show Distributor, Industry, OpCo Entities]**

**Distributor Entity**

```typescript
{
  id: number,
  name: string,                // "US Foods"
  receiveContractProposal: boolean,
  status: string,
  contactEmail: string,
  contactPhone: string,
  isActive: boolean,
  createdDate: Date
}
```

**Industry Entity**

```typescript
{
  id: number,
  name: string,                // "Healthcare"
  description: string,
  status: string,
  isActive: boolean
}
```

**OpCo Entity**

```typescript
{
  id: number,
  name: string,                // "OpCo East"
  code: string,                // "EAST"
  distributorId: number,
  isActive: boolean
}
```

---

## SLIDE 7: Proposal Object (1 minute 30 seconds)

**[Show Proposal Entity]**

**Proposal Entity**

```typescript
{
  id: number,
  title: string,               // "Q1 2024 Poultry Pricing"
  proposalTypeId: number,      // 1=New Contract, 2=Amendment, etc.
  proposalStatusId: number,    // 1=Draft, 2=Submitted, etc.
  manufacturerId: number,
  amendedContractId: number,   // If this is an amendment
  startDate: Date,
  endDate: Date,
  internalNotes: string,
  rejectReason: string,        // If rejected
  isActive: boolean,
  createdDate: Date,
  modifiedDate: Date,
  createdBy: string,
  modifiedBy: string
}
```

**Relationships:**
- Proposal → Manufacturer (many-to-one)
- Proposal → ProposalType (many-to-one)
- Proposal → ProposalStatus (many-to-one)
- Proposal → Contract (one-to-one, if amended)
- Proposal → ProposalProducts (one-to-many)
- Proposal → ProposalDistributors (one-to-many)
- Proposal → ProposalIndustries (one-to-many)
- Proposal → ProposalOpcos (one-to-many)
- Proposal → ProposalStatusHistory (one-to-many)

---

## SLIDE 8: ProposalProduct Object (1 minute 30 seconds)

**[Show ProposalProduct Entity]**

**ProposalProduct Entity**

```typescript
{
  id: number,
  proposalId: number,
  productId: number,
  priceTypeId: number,         // 1=Delivered, 2=FOB, 3=Allowance
  quantity: number,            // Estimated quantity
  amendmentActionId: number,   // 1=Add, 2=Modify, 3=Delete (for amendments)
  
  // Pricing fields
  allowance: decimal,
  commercialDelPrice: decimal,
  commercialFobPrice: decimal,
  commodityDelPrice: decimal,
  commodityFobPrice: decimal,
  uom: string,                 // "Cases", "Pounds"
  billbacksAllowed: boolean,
  pua: decimal,                // Pickup Allowance
  ffsPrice: decimal,           // Freight From Source
  noiPrice: decimal,           // Net of Invoice
  ptv: decimal,                // Price to Value
  
  internalNotes: string,
  manufacturerNotes: string,
  metaJson: string,            // Legacy JSON field
  productProposalStatusId: number,  // Product-level status
  isActive: boolean,
  createdDate: Date,
  modifiedDate: Date,
  createdBy: string,
  modifiedBy: string
}
```

**Relationships:**
- ProposalProduct → Proposal (many-to-one)
- ProposalProduct → Product (many-to-one)
- ProposalProduct → PriceType (many-to-one)
- ProposalProduct → ProductProposalStatus (many-to-one)
- ProposalProduct → ProposalProductHistory (one-to-many)

---

## SLIDE 9: Contract Object (1 minute 30 seconds)

**[Show Contract Entity]**

**Contract Entity**

```typescript
{
  id: number,
  name: string,                // "Tyson Foods 2024 Contract"
  foreignContractId: string,   // External reference ID
  manufacturerId: number,
  proposalId: number,          // Source proposal
  status: string,              // "Active", "Suspended", "Expired", "Terminated"
  startDate: Date,
  endDate: Date,
  suspendedDate: Date,
  isSuspended: boolean,
  sendToPerformance: boolean,
  currentVersionNumber: number,
  
  // Metadata
  internalNotes: string,
  manufacturerReferenceNumber: string,
  manufacturerBillbackName: string,
  manufacturerTermsAndConditions: string,
  manufacturerNotes: string,
  contactPerson: string,
  entegraContractType: string,
  entegraVdaProgram: string,
  
  isActive: boolean,
  createdDate: Date,
  modifiedDate: Date,
  createdBy: string,
  modifiedBy: string
}
```

**Relationships:**
- Contract → Manufacturer (many-to-one)
- Contract → Proposal (many-to-one)
- Contract → ContractPrices (one-to-many)
- Contract → ContractVersions (one-to-many)
- Contract → ContractDistributors (one-to-many)
- Contract → ContractIndustries (one-to-many)
- Contract → ContractOpCos (one-to-many)

---

## SLIDE 10: ContractPrice Object (1 minute)

**[Show ContractPrice Entity]**

**ContractPrice Entity**

```typescript
{
  id: number,
  contractId: number,
  productId: number,
  priceType: string,           // "Delivered", "FOB", "Allowance"
  uom: string,                 // "Cases", "Pounds"
  estimatedQty: number,
  billbacksAllowed: boolean,
  
  // Pricing fields (same as ProposalProduct)
  allowance: decimal,
  commercialDelPrice: decimal,
  commercialFobPrice: decimal,
  commodityDelPrice: decimal,
  commodityFobPrice: decimal,
  pua: decimal,
  ffsPrice: decimal,
  noiPrice: decimal,
  ptv: decimal,
  
  internalNotes: string,
  manufacturerNotes: string,
  isActive: boolean,
  createdDate: Date,
  modifiedDate: Date,
  createdBy: string,
  modifiedBy: string
}
```

---

## SLIDE 11: ContractVersion & ContractVersionPrice (1 minute 30 seconds)

**[Show ContractVersion Entity]**

**ContractVersion Entity**

```typescript
{
  id: number,
  contractId: number,
  versionNumber: number,       // 1, 2, 3, etc.
  name: string,                // "Tyson Foods 2024 Contract - V2"
  foreignContractId: string,
  sendToPerformance: boolean,
  isSuspended: boolean,
  suspendedDate: Date,
  internalNotes: string,
  
  // Snapshot of contract metadata
  manufacturerReferenceNumber: string,
  manufacturerBillbackName: string,
  manufacturerTermsAndConditions: string,
  manufacturerNotes: string,
  contactPerson: string,
  entegraContractType: string,
  entegraVdaProgram: string,
  
  startDate: Date,
  endDate: Date,
  createdDate: Date,
  createdBy: string
}
```

**ContractVersionPrice Entity**

```typescript
{
  id: number,
  contractVersionId: number,
  contractId: number,
  priceId: number,             // Reference to ContractPrice
  productId: number,
  
  // Snapshot of pricing at this version
  priceType: string,
  uom: string,
  estimatedQty: number,
  billbacksAllowed: boolean,
  price: decimal,              // Calculated primary price
  
  // All pricing fields...
  commercialDelPrice: decimal,
  commercialFobPrice: decimal,
  // etc.
}
```

**Purpose:** Track all pricing changes over time

---

## SLIDE 12: Velocity Objects (1 minute 30 seconds)

**[Show Velocity Entities]**

**VelocityJob Entity**

```typescript
{
  jobId: string,               // "VEL-20241209-001"
  fileName: string,
  status: string,              // "Pending", "Processing", "Completed", "Failed"
  totalRows: number,
  validRows: number,
  invalidRows: number,
  matchedShipments: number,
  unmatchedShipments: number,
  startedAt: Date,
  completedAt: Date,
  uploadedBy: string
}
```

**VelocityShipment Entity**

```typescript
{
  id: number,
  jobId: string,
  opCo: string,
  customerNumber: string,
  customerName: string,
  invoiceNumber: string,
  invoiceDate: Date,
  productNumber: string,
  gtin: string,
  manufacturerName: string,
  qty: number,
  sales: decimal,
  landedCost: decimal,
  allowances: decimal,
  
  // Matching results
  contractId: number,
  contractPrice: decimal,
  actualPrice: decimal,
  priceDifference: decimal,
  matched: boolean
}
```

**IngestedFile Entity**

```typescript
{
  id: number,
  fileName: string,
  fileSize: number,
  uploadedBy: string,
  uploadedAt: Date,
  jobId: string
}
```

---

## SLIDE 13: Summary (1 minute)

**[Show Complete ERD]**

We've covered all Core Domain Objects:

✅ **User & Auth** - Users, Roles, UserRoles  
✅ **Master Data** - Manufacturers, Products, Distributors, Industries, OpCos  
✅ **Proposals** - Proposal, ProposalProduct, ProposalDistributor, etc.  
✅ **Contracts** - Contract, ContractPrice, ContractVersion, ContractVersionPrice  
✅ **Velocity** - VelocityJob, VelocityShipment, IngestedFile  

**Key Relationships:**
- Proposals → Contracts (one-to-one when awarded)
- ProposalProducts → ContractPrices (copied during conversion)
- Contracts → ContractVersions (one-to-many for change tracking)
- VelocityShipments → Contracts (many-to-one for matching)

**Data Model Principles:**
- Soft deletes (IsActive flag)
- Audit fields (CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
- Versioning for contracts
- Complete history tracking

---

## SLIDE 14: Conclusion (1 minute)

**[Show Thank You Slide]**

Thank you for watching this complete API overview series!

**What We Covered:**
1. Introduction & System Architecture
2. Authentication & User Management APIs
3. Master Data Management APIs
4. Proposal Management APIs
5. Contract Management APIs
6. Velocity Integration APIs
7. Reporting & Lookup APIs
8. Core Domain Objects & Data Models

**Resources:**
- Swagger UI: `http://localhost:5110/swagger`
- API Documentation: Available in Swagger
- Database Schema: Entity Framework migrations

**Next Steps:**
- Explore the Swagger UI
- Test endpoints with sample data
- Review the frontend Angular application
- Check out the database schema

If you have questions, please reach out to the development team.

Happy coding!

---

**[TOTAL TIME: ~12 minutes]**

