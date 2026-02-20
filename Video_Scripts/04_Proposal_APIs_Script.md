# Video Script 4: Proposal Management APIs

## SLIDE 1: Proposal Overview (45 seconds)

**[Show ProposalsController in Swagger]**

Welcome to Video 4, covering Proposal Management.

Proposals are the heart of the NPP system. They represent pricing submissions from manufacturers that go through an approval workflow before becoming contracts.

**Proposal Workflow:**
1. **Draft** - Manufacturer creates proposal with products and pricing
2. **Submitted** - Manufacturer submits for review
3. **Under Review** - NPP reviews the proposal
4. **Accepted** - NPP accepts the proposal
5. **Awarded** - Proposal is converted to a contract

All proposal endpoints are at `/api/v1/proposals`.

---

## SLIDE 2: List Proposals (1 minute)

**[Show GET /api/v1/proposals in Swagger]**

**Endpoint:** `GET /api/v1/proposals`

**Purpose:** Get paginated list of proposals

**Query Parameters:**
- `page` (default: 1)
- `pageSize` (default: 10)
- `search` (searches title, manufacturer name)
- `proposalStatusId` (filter by status)
- `proposalTypeId` (filter by type)
- `manufacturerId` (filter by manufacturer)

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": 1,
      "title": "Q1 2024 Poultry Pricing",
      "proposalTypeId": 1,
      "proposalTypeName": "New Contract",
      "proposalStatusId": 2,
      "proposalStatusName": "Submitted",
      "manufacturerId": 1,
      "manufacturerName": "Tyson Foods",
      "startDate": "2024-01-01",
      "endDate": "2024-12-31",
      "productCount": 25,
      "createdDate": "2024-01-05T10:00:00Z",
      "createdBy": "john.manufacturer"
    }
  ],
  "totalCount": 45,
  "page": 1,
  "pageSize": 10
}
```

---

## SLIDE 3: Get Proposal by ID (1 minute)

**[Show GET /api/v1/proposals/{id} in Swagger]**

**Endpoint:** `GET /api/v1/proposals/{id}`

**Purpose:** Get complete proposal details including products

**Response (200 OK):**
```json
{
  "id": 1,
  "title": "Q1 2024 Poultry Pricing",
  "proposalTypeId": 1,
  "proposalTypeName": "New Contract",
  "proposalStatusId": 2,
  "proposalStatusName": "Submitted",
  "manufacturerId": 1,
  "manufacturerName": "Tyson Foods",
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "internalNotes": "Competitive pricing for Q1",
  "products": [
    {
      "id": 100,
      "productId": 50,
      "productName": "Chicken Breast",
      "sku": "TYS-12345",
      "quantity": 1000,
      "commercialDelPrice": 2.50,
      "uom": "Cases",
      "billbacksAllowed": true
    }
  ],
  "distributors": [1, 2],
  "industries": [1],
  "opcos": [1, 2, 3]
}
```

---

## SLIDE 4: Create Proposal (1 minute 30 seconds)

**[Show POST /api/v1/proposals in Swagger]**

**Endpoint:** `POST /api/v1/proposals`

**Purpose:** Create a new proposal

**Request Body:**
```json
{
  "title": "Q2 2024 Beef Pricing",
  "proposalTypeId": 1,
  "manufacturerId": 2,
  "startDate": "2024-04-01",
  "endDate": "2024-06-30",
  "internalNotes": "New pricing for Q2",
  "products": [
    {
      "productId": 75,
      "quantity": 500,
      "priceTypeId": 1,
      "commercialDelPrice": 5.25,
      "uom": "Cases",
      "billbacksAllowed": true,
      "internalNotes": "Competitive price"
    }
  ],
  "distributorIds": [1, 2],
  "industryIds": [1, 2],
  "opcoIds": [1, 2, 3, 4]
}
```

**Response (201 Created):**
Returns the created proposal with ID and status = "Draft"

---

## SLIDE 5: Update Proposal (1 minute)

**[Show PUT /api/v1/proposals/{id} in Swagger]**

**Endpoint:** `PUT /api/v1/proposals/{id}`

**Purpose:** Update an existing proposal (only in Draft status)

**Request Body:** Same structure as Create

**Response (200 OK):** Returns updated proposal

**Important Notes:**
- Can only update proposals in "Draft" status
- Submitted proposals cannot be edited
- Products are completely replaced (not merged)
- Distributors, Industries, OpCos are replaced

---

## SLIDE 6: Proposal Workflow Actions (2 minutes)

**[Show Workflow Endpoints in Swagger]**

**1. Submit Proposal**

**Endpoint:** `POST /api/v1/proposals/{id}/submit`

**Purpose:** Submit proposal for review (Draft → Submitted)

**Response:** 200 OK with message "Submitted"

---

**2. Accept Proposal**

**Endpoint:** `POST /api/v1/proposals/{id}/accept`

**Purpose:** Accept proposal (Submitted → Accepted)

**Required Role:** Contract Manager or System Administrator

**Response:** 200 OK with message "Accepted"

---

**3. Reject Proposal**

**Endpoint:** `POST /api/v1/proposals/{id}/reject`

**Purpose:** Reject proposal with reason

**Request Body:**
```json
{
  "rejectReason": "Pricing not competitive for current market"
}
```

**Response:** 200 OK with message "Rejected"

The proposal status changes to "Rejected" and the reason is stored.

---

## SLIDE 7: Clone Proposal (1 minute)

**[Show POST /api/v1/proposals/{id}/clone in Swagger]**

**Endpoint:** `POST /api/v1/proposals/{id}/clone`

**Purpose:** Create a copy of an existing proposal

**Response (201 Created):**
```json
{
  "id": 25,
  "title": "Q1 2024 Poultry Pricing (Copy)",
  "proposalStatusId": 1,
  "proposalStatusName": "Draft",
  "manufacturerId": 1,
  "products": [...],
  "distributors": [...],
  "industries": [...],
  "opcos": [...]
}
```

**Use Cases:**
- Create renewal proposals
- Create similar proposals for different time periods
- Resubmit rejected proposals with changes

---

## SLIDE 8: Excel Import/Export (2 minutes)

**[Show Excel Endpoints in Swagger]**

**1. Download Excel Template**

**Endpoint:** `GET /api/v1/proposals/products/excel-template/{manufacturerId}`

**Purpose:** Download Excel template with all products for a manufacturer

**Response:** Excel file (.xlsx)

**Template Columns:**
- SKU, Product Name, UOM
- Billbacks Allowed
- Allowance, Commercial Del Price, Commercial FOB Price
- Commodity Del Price, Commodity FOB Price
- PUA, FFS Price, NOI Price, PTV
- Internal Notes, Manufacturer Notes

---

**2. Upload Excel File**

**Endpoint:** `POST /api/v1/proposals/products/excel-import/{manufacturerId}`

**Purpose:** Import product pricing from Excel file

**Request:** Multipart form data with Excel file

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Import completed successfully",
  "totalRows": 100,
  "validRows": 95,
  "invalidRows": 5,
  "importedProducts": [...],
  "validationErrors": [
    "Row 10: SKU 'ABC-123' not found",
    "Row 25: Commercial Del Price is required"
  ]
}
```

The frontend can then populate the proposal form with the imported products.

---

## SLIDE 9: Proposal Products Management (1 minute 30 seconds)

**[Show Product Endpoints in Swagger]**

**Get Proposal Products**

**Endpoint:** `GET /api/v1/proposals/{id}/products`

**Purpose:** Get all products for a proposal

**Response:** Array of proposal products

---

**Add Product to Proposal**

**Endpoint:** `POST /api/v1/proposals/{id}/products`

**Request Body:**
```json
{
  "productId": 100,
  "quantity": 250,
  "priceTypeId": 1,
  "commercialDelPrice": 3.75,
  "uom": "Cases",
  "billbacksAllowed": true
}
```

---

**Update Product**

**Endpoint:** `PUT /api/v1/proposals/{proposalId}/products/{productId}`

---

**Delete Product**

**Endpoint:** `DELETE /api/v1/proposals/{proposalId}/products/{productId}`

---

## SLIDE 10: Proposal Status History (45 seconds)

**[Show GET /api/v1/proposals/{id}/history in Swagger]**

**Endpoint:** `GET /api/v1/proposals/{id}/history`

**Purpose:** Get complete status change history

**Response:**
```json
[
  {
    "id": 1,
    "proposalId": 1,
    "fromStatusId": 1,
    "fromStatusName": "Draft",
    "toStatusId": 2,
    "toStatusName": "Submitted",
    "changedBy": "john.manufacturer",
    "changedDate": "2024-01-10T14:30:00Z",
    "notes": "Ready for review"
  },
  {
    "id": 2,
    "proposalId": 1,
    "fromStatusId": 2,
    "toStatusId": 3,
    "toStatusName": "Accepted",
    "changedBy": "admin",
    "changedDate": "2024-01-12T09:15:00Z"
  }
]
```

---

## SLIDE 11: Summary (30 seconds)

**[Show Summary Slide]**

We've covered Proposal Management APIs:

✅ **CRUD Operations** - Create, read, update proposals  
✅ **Workflow Actions** - Submit, accept, reject  
✅ **Excel Import/Export** - Bulk product pricing  
✅ **Product Management** - Add/update/delete products  
✅ **Status History** - Audit trail  

**Key Points:**
- Proposals follow a strict workflow
- Only Draft proposals can be edited
- Excel import supports bulk pricing
- Complete audit trail via status history

In the next video, we'll cover Contract Management APIs.

---

**[TOTAL TIME: ~12 minutes]**

