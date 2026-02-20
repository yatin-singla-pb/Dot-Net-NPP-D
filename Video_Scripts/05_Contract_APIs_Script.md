# Video Script 5: Contract Management APIs

## SLIDE 1: Contract Overview (45 seconds)

**[Show ContractsController in Swagger]**

Welcome to Video 5, covering Contract Management.

Contracts are created from accepted proposals and represent active pricing agreements between manufacturers and NPP.

**Key Features:**
- **Contract Versioning** - Track changes over time
- **Pricing Management** - Multiple price types per product
- **Bulk Renewals** - Create renewal proposals from expiring contracts
- **Status Management** - Active, Suspended, Expired, Terminated

All contract endpoints are at `/api/Contracts`.

---

## SLIDE 2: List Contracts (1 minute)

**[Show GET /api/Contracts in Swagger]**

**Endpoint:** `GET /api/Contracts`

**Purpose:** Get all contracts with filtering

**Query Parameters:**
- `search` (searches name, manufacturer)
- `status` (Active, Suspended, Expired, Terminated)
- `manufacturerId` (filter by manufacturer)
- `startDate`, `endDate` (date range filters)

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "name": "Tyson Foods 2024 Contract",
    "foreignContractId": "TYS-2024-001",
    "manufacturerId": 1,
    "manufacturerName": "Tyson Foods",
    "status": "Active",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "currentVersionNumber": 2,
    "isSuspended": false,
    "sendToPerformance": true,
    "proposalId": 5,
    "createdDate": "2024-01-15T00:00:00Z"
  }
]
```

---

## SLIDE 3: Get Contract by ID (1 minute)

**[Show GET /api/Contracts/{id} in Swagger]**

**Endpoint:** `GET /api/Contracts/{id}`

**Purpose:** Get complete contract details

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "Tyson Foods 2024 Contract",
  "foreignContractId": "TYS-2024-001",
  "manufacturerId": 1,
  "manufacturerName": "Tyson Foods",
  "status": "Active",
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "currentVersionNumber": 2,
  "isSuspended": false,
  "sendToPerformance": true,
  "internalNotes": "Standard annual contract",
  "manufacturerReferenceNumber": "REF-2024-001",
  "manufacturerBillbackName": "Tyson Billback Program",
  "contactPerson": "John Smith",
  "entegraContractType": "GPO",
  "entegraVdaProgram": "VDA-2024",
  "distributors": [
    { "distributorId": 1, "distributorName": "US Foods" }
  ],
  "opCos": [
    { "opCoId": 1, "opCoName": "OpCo East" }
  ],
  "industries": [
    { "industryId": 1, "industryName": "Healthcare" }
  ],
  "contractProducts": [
    { "productId": 50, "productName": "Chicken Breast" }
  ]
}
```

---

## SLIDE 4: Create Contract from Proposal (1 minute 30 seconds)

**[Show POST /api/Contracts in Swagger]**

**Endpoint:** `POST /api/Contracts`

**Purpose:** Create a new contract (usually from an accepted proposal)

**Request Body:**
```json
{
  "name": "New Manufacturer 2024 Contract",
  "foreignContractId": "NM-2024-001",
  "manufacturerId": 2,
  "proposalId": 10,
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "status": "Active",
  "sendToPerformance": true,
  "internalNotes": "Created from proposal #10",
  "manufacturerReferenceNumber": "MFG-REF-001",
  "contactPerson": "Jane Doe",
  "distributorIds": [1, 2],
  "opCoIds": [1, 2, 3],
  "industryIds": [1],
  "prices": [
    {
      "productId": 75,
      "priceType": "Delivered",
      "uom": "Cases",
      "estimatedQty": 1000,
      "billbacksAllowed": true,
      "commercialDelPrice": 5.50,
      "internalNotes": "Standard pricing"
    }
  ]
}
```

**Response (201 Created):**
Returns the created contract with ID and version 1.

---

## SLIDE 5: Update Contract (1 minute)

**[Show PUT /api/Contracts/{id} in Swagger]**

**Endpoint:** `PUT /api/Contracts/{id}`

**Purpose:** Update contract metadata (not pricing)

**Request Body:**
```json
{
  "name": "Updated Contract Name",
  "foreignContractId": "TYS-2024-001-V2",
  "status": "Active",
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "isSuspended": false,
  "sendToPerformance": true,
  "internalNotes": "Updated notes",
  "manufacturerReferenceNumber": "REF-2024-001-V2",
  "contactPerson": "John Smith Jr"
}
```

**Response (200 OK):** Returns updated contract

**Note:** To update pricing, use the versioning endpoints.

---

## SLIDE 6: Contract Versioning - Get Versions (1 minute)

**[Show GET /api/Contracts/{id}/versions in Swagger]**

**Endpoint:** `GET /api/Contracts/{id}/versions`

**Purpose:** Get all versions of a contract

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "contractId": 1,
    "versionNumber": 1,
    "name": "Tyson Foods 2024 Contract - V1",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "createdDate": "2024-01-15T00:00:00Z",
    "createdBy": "admin"
  },
  {
    "id": 2,
    "contractId": 1,
    "versionNumber": 2,
    "name": "Tyson Foods 2024 Contract - V2",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "createdDate": "2024-03-10T00:00:00Z",
    "createdBy": "admin"
  }
]
```

---

## SLIDE 7: Contract Versioning - Get Version Details (1 minute 30 seconds)

**[Show GET /api/Contracts/{id}/versions/{versionNumber} in Swagger]**

**Endpoint:** `GET /api/Contracts/{id}/versions/{versionNumber}`

**Purpose:** Get complete details for a specific version including pricing

**Response (200 OK):**
```json
{
  "id": 2,
  "contractId": 1,
  "versionNumber": 2,
  "name": "Tyson Foods 2024 Contract - V2",
  "foreignContractId": "TYS-2024-001-V2",
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "sendToPerformance": true,
  "isSuspended": false,
  "internalNotes": "Price update for Q2",
  "createdDate": "2024-03-10T00:00:00Z",
  "createdBy": "admin",
  "prices": [
    {
      "priceId": 100,
      "productId": 50,
      "productName": "Chicken Breast",
      "sku": "TYS-12345",
      "priceType": "Delivered",
      "uom": "Cases",
      "estimatedQty": 1000,
      "billbacksAllowed": true,
      "commercialDelPrice": 2.75,
      "price": 2.75
    }
  ]
}
```

---

## SLIDE 8: Contract Versioning - Create New Version (1 minute 30 seconds)

**[Show POST /api/Contracts/{id}/versions in Swagger]**

**Endpoint:** `POST /api/Contracts/{id}/versions`

**Purpose:** Create a new version with updated pricing

**Request Body:**
```json
{
  "name": "Tyson Foods 2024 Contract - V3",
  "foreignContractId": "TYS-2024-001-V3",
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "sendToPerformance": true,
  "internalNotes": "Q3 price adjustments",
  "prices": [
    {
      "productId": 50,
      "priceType": "Delivered",
      "uom": "Cases",
      "estimatedQty": 1200,
      "billbacksAllowed": true,
      "commercialDelPrice": 2.85,
      "internalNotes": "Price increase due to market conditions"
    }
  ]
}
```

**Response (201 Created):**
Returns the new version with incremented version number.

**Note:** The contract's currentVersionNumber is automatically updated.

---

## SLIDE 9: Contract Pricing Management (1 minute 30 seconds)

**[Show Pricing Endpoints in Swagger]**

**Get Contract Prices**

**Endpoint:** `GET /api/Contracts/{id}/prices`

**Purpose:** Get all current pricing for a contract

**Response:** Array of contract prices

---

**Add Price to Contract**

**Endpoint:** `POST /api/Contracts/{id}/prices`

**Request Body:**
```json
{
  "productId": 100,
  "priceType": "FOB",
  "uom": "Pounds",
  "estimatedQty": 5000,
  "billbacksAllowed": false,
  "commercialFobPrice": 1.25,
  "internalNotes": "New product added"
}
```

---

**Update Price**

**Endpoint:** `PUT /api/Contracts/{contractId}/prices/{priceId}`

---

**Delete Price**

**Endpoint:** `DELETE /api/Contracts/{contractId}/prices/{priceId}`

**Note:** Deleting a price creates a new version automatically.

---

## SLIDE 10: Bulk Renewal (1 minute 30 seconds)

**[Show POST /api/Contracts/bulk-renewal in Swagger]**

**Endpoint:** `POST /api/Contracts/bulk-renewal`

**Purpose:** Create renewal proposals for expiring contracts

**Request Body:**
```json
{
  "contractIds": [1, 2, 3, 5, 7],
  "newStartDate": "2025-01-01",
  "newEndDate": "2025-12-31",
  "priceAdjustmentPercent": 2.5,
  "proposalTitle": "2025 Renewal Proposals"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Created 5 renewal proposals",
  "createdProposals": [
    {
      "proposalId": 50,
      "contractId": 1,
      "title": "2025 Renewal - Tyson Foods",
      "manufacturerId": 1
    },
    {
      "proposalId": 51,
      "contractId": 2,
      "title": "2025 Renewal - Sysco",
      "manufacturerId": 2
    }
  ]
}
```

**Use Case:**
At year-end, select all expiring contracts and create renewal proposals with a 2.5% price increase.

---

## SLIDE 11: Contract Status Management (1 minute)

**[Show Status Endpoints in Swagger]**

**Suspend Contract**

**Endpoint:** `POST /api/Contracts/{id}/suspend`

**Request Body:**
```json
{
  "reason": "Quality issues reported",
  "suspendedDate": "2024-06-15"
}
```

**Response:** 200 OK

Sets `isSuspended = true` and `status = "Suspended"`

---

**Reactivate Contract**

**Endpoint:** `POST /api/Contracts/{id}/reactivate`

**Response:** 200 OK

Sets `isSuspended = false` and `status = "Active"`

---

**Terminate Contract**

**Endpoint:** `POST /api/Contracts/{id}/terminate`

**Request Body:**
```json
{
  "reason": "Contract breach",
  "terminationDate": "2024-08-01"
}
```

**Response:** 200 OK

Sets `status = "Terminated"`

---

## SLIDE 12: Contract Assignments (2 minutes)

**[Show GET /api/contracts/{contractId}/assignments in Swagger]**

**Endpoint Base:** `/api/contracts/{contractId}/assignments`

**Purpose:** Manage distributors, manufacturers, OpCos, and industries assigned to a contract

**Get Distributors**

**Endpoint:** `GET /api/contracts/{contractId}/assignments/distributors`

**Response:**
```json
[
  {
    "id": 1,
    "contractId": 1,
    "distributorId": 1,
    "currentVersionNumber": 2,
    "assignedBy": "admin",
    "assignedDate": "2024-01-15T00:00:00Z"
  }
]
```

---

**Add Distributor**

**Endpoint:** `POST /api/contracts/{contractId}/assignments/distributors`

**Request Body:**
```json
{
  "distributorId": 2
}
```

---

**Remove Distributor**

**Endpoint:** `DELETE /api/contracts/{contractId}/assignments/distributors/{id}`

**Same pattern for:**
- `/assignments/manufacturers`
- `/assignments/opcos`
- `/assignments/industries`

---

## SLIDE 13: Contract Prices API (1 minute 30 seconds)

**[Show ContractPricesController in Swagger]**

**Endpoint Base:** `/api/contract-prices`

**Purpose:** Direct access to contract pricing data

**Get All Prices**

**Endpoint:** `GET /api/contract-prices`

**Query Parameters:**
- `productId` (optional)
- `versionNumber` (optional)

**Response:**
```json
[
  {
    "id": 100,
    "contractId": 1,
    "productId": 50,
    "priceType": "Delivered",
    "commercialDelPrice": 2.50,
    "uom": "Cases",
    "estimatedQty": 1000
  }
]
```

---

**Get Price by ID**

**Endpoint:** `GET /api/contract-prices/{id}`

---

**Create Price**

**Endpoint:** `POST /api/contract-prices`

---

**Update Price**

**Endpoint:** `PUT /api/contract-prices/{id}`

---

**Delete Price**

**Endpoint:** `DELETE /api/contract-prices/{id}`

---

## SLIDE 14: Contract Version Associations (1 minute 30 seconds)

**[Show Contract Version Controllers in Swagger]**

**Purpose:** Track which distributors, manufacturers, OpCos, industries, and products are associated with each contract version

**Contract Version Distributors**

**Endpoint:** `GET /api/contract-version/distributors`

**Query Parameters:**
- `contractId` (optional)
- `versionNumber` (optional)
- `distributorId` (optional)

---

**Contract Version Manufacturers**

**Endpoint:** `GET /api/contract-version/manufacturers`

---

**Contract Version OpCos**

**Endpoint:** `GET /api/contract-version/opcos`

---

**Contract Version Industries**

**Endpoint:** `GET /api/contract-version/industries`

---

**Contract Version Products**

**Endpoint:** `GET /api/contract-version/products`

**Use Case:** See which products were in version 1 vs version 2 of a contract

---

## SLIDE 15: Bulk Renewal API (1 minute)

**[Show POST /api/BulkRenewal/create in Swagger]**

**Endpoint:** `POST /api/BulkRenewal/create`

**Purpose:** Create renewal proposals for multiple expiring contracts

**Request Body:**
```json
{
  "contractIds": [1, 2, 3, 5, 7],
  "newStartDate": "2025-01-01",
  "newEndDate": "2025-12-31",
  "pricingAdjustments": [
    {
      "adjustmentType": "Percentage",
      "adjustmentValue": 2.5,
      "applyToAllProducts": true
    }
  ],
  "createdBy": "admin"
}
```

**Response:**
```json
{
  "success": true,
  "totalContracts": 5,
  "successfulRenewals": 5,
  "failedRenewals": 0,
  "results": [
    {
      "contractId": 1,
      "contractName": "Tyson Foods 2024",
      "success": true,
      "proposalId": 50,
      "message": "Renewal proposal created successfully"
    }
  ]
}
```

---

## SLIDE 16: Summary (30 seconds)

**[Show Summary Slide]**

We've covered Contract Management APIs:

✅ **CRUD Operations** - Create, read, update contracts
✅ **Versioning** - Track pricing changes over time
✅ **Pricing Management** - Add/update/delete prices
✅ **Contract Assignments** - Distributors, manufacturers, OpCos, industries
✅ **Contract Prices API** - Direct price access
✅ **Version Associations** - Track changes across versions
✅ **Bulk Renewal** - Create renewal proposals
✅ **Status Management** - Suspend, reactivate, terminate

**Key Points:**
- Contracts are created from proposals
- Versioning tracks all pricing changes
- Bulk renewal simplifies year-end process
- Complete audit trail via versions
- Flexible assignment management

In the next video, we'll cover Velocity Integration APIs.

---

**[TOTAL TIME: ~19 minutes]**

