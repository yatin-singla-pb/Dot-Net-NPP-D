# Video Script 7: Reporting & Lookup APIs

## SLIDE 1: Overview (30 seconds)

**[Show ReportsController and LookupController in Swagger]**

Welcome to Video 7, covering Reporting and Lookup APIs.

**Reporting APIs** provide business intelligence and data exports.

**Lookup APIs** provide reference data for dropdowns and forms.

These are essential for the frontend UI and data analysis.

---

## SLIDE 2: Contract Over Term Report (1 minute 30 seconds)

**[Show GET /api/Reports/contract-over-term in Swagger]**

**Endpoint:** `GET /api/Reports/contract-over-term`

**Purpose:** Generate comprehensive contract pricing report

**Query Parameters:**
- `contractId` (required)
- `startDate`, `endDate` (optional date range)
- `format` ("json" or "excel")

**Response (JSON):**
```json
{
  "contractId": 1,
  "contractName": "Tyson Foods 2024 Contract",
  "manufacturerName": "Tyson Foods",
  "reportDate": "2024-12-09",
  "period": "2024-01-01 to 2024-12-31",
  "products": [
    {
      "productId": 50,
      "sku": "TYS-12345",
      "productName": "Chicken Breast Boneless",
      "gtin": "00012345678905",
      "brand": "Tyson",
      "packSize": "10 LB",
      "priceType": "Delivered",
      "uom": "Cases",
      "contractPrice": 2.50,
      "estimatedQty": 1000,
      "estimatedValue": 2500.00,
      "actualQty": 950,
      "actualSales": 2375.00,
      "variance": -125.00,
      "variancePercent": -5.0
    }
  ],
  "summary": {
    "totalProducts": 25,
    "totalEstimatedValue": 50000.00,
    "totalActualSales": 48500.00,
    "totalVariance": -1500.00
  }
}
```

**Excel Format:** Returns downloadable Excel file

---

## SLIDE 3: Contract Pricing Report (1 minute)

**[Show GET /api/Reports/contract-pricing in Swagger]**

**Endpoint:** `GET /api/Reports/contract-pricing`

**Purpose:** Get current pricing for all contracts

**Query Parameters:**
- `manufacturerId` (optional)
- `status` (optional - Active, Suspended, etc.)
- `format` ("json" or "excel")

**Response (JSON):**
```json
{
  "reportDate": "2024-12-09",
  "contracts": [
    {
      "contractId": 1,
      "contractName": "Tyson Foods 2024 Contract",
      "manufacturerName": "Tyson Foods",
      "status": "Active",
      "versionNumber": 2,
      "productCount": 25,
      "products": [
        {
          "productId": 50,
          "sku": "TYS-12345",
          "productName": "Chicken Breast",
          "priceType": "Delivered",
          "price": 2.50,
          "uom": "Cases"
        }
      ]
    }
  ]
}
```

---

## SLIDE 4: Proposal Summary Report (1 minute)

**[Show GET /api/Reports/proposal-summary in Swagger]**

**Endpoint:** `GET /api/Reports/proposal-summary`

**Purpose:** Get summary of all proposals by status

**Query Parameters:**
- `startDate`, `endDate`
- `manufacturerId`
- `proposalStatusId`

**Response:**
```json
{
  "reportDate": "2024-12-09",
  "period": "2024-01-01 to 2024-12-31",
  "summary": {
    "totalProposals": 100,
    "byStatus": {
      "Draft": 15,
      "Submitted": 25,
      "Under Review": 10,
      "Accepted": 40,
      "Rejected": 5,
      "Awarded": 5
    },
    "byManufacturer": [
      {
        "manufacturerId": 1,
        "manufacturerName": "Tyson Foods",
        "proposalCount": 20,
        "acceptedCount": 15,
        "rejectedCount": 2
      }
    ]
  },
  "proposals": [...]
}
```

---

## SLIDE 5: Dashboard Statistics (1 minute)

**[Show GET /api/Reports/dashboard in Swagger]**

**Endpoint:** `GET /api/Reports/dashboard`

**Purpose:** Get key metrics for dashboard

**Response:**
```json
{
  "proposals": {
    "total": 100,
    "draft": 15,
    "submitted": 25,
    "underReview": 10,
    "accepted": 40,
    "rejected": 5,
    "awarded": 5
  },
  "contracts": {
    "total": 50,
    "active": 40,
    "suspended": 2,
    "expired": 5,
    "terminated": 3
  },
  "recentActivity": [
    {
      "type": "Proposal Submitted",
      "id": 100,
      "title": "Q1 2024 Pricing",
      "date": "2024-12-09T10:00:00Z",
      "user": "john.manufacturer"
    },
    {
      "type": "Contract Created",
      "id": 50,
      "title": "New Contract 2024",
      "date": "2024-12-08T15:30:00Z",
      "user": "admin"
    }
  ],
  "expiringContracts": [
    {
      "contractId": 10,
      "contractName": "Expiring Contract",
      "endDate": "2024-12-31",
      "daysRemaining": 22
    }
  ]
}
```

---

## SLIDE 6: Lookup APIs - Overview (30 seconds)

**[Show LookupController in Swagger]**

Lookup APIs provide reference data for dropdowns and forms.

All lookup endpoints are at `/api/v1/lookup` and do NOT require authentication.

**Available Lookups:**
- Proposal Types
- Proposal Statuses
- Product Proposal Statuses
- Amendment Actions
- Price Types
- Manufacturers
- Distributors
- Industries
- OpCos

---

## SLIDE 7: Proposal Type Lookup (45 seconds)

**[Show GET /api/v1/lookup/proposal-types in Swagger]**

**Endpoint:** `GET /api/v1/lookup/proposal-types`

**Purpose:** Get list of proposal types for dropdown

**Response:**
```json
[
  {
    "id": 1,
    "name": "New Contract",
    "isActive": true
  },
  {
    "id": 2,
    "name": "Contract Amendment",
    "isActive": true
  },
  {
    "id": 3,
    "name": "Contract Renewal",
    "isActive": true
  },
  {
    "id": 4,
    "name": "Price Update",
    "isActive": true
  }
]
```

**Use Case:** Populate "Proposal Type" dropdown when creating a proposal

---

## SLIDE 8: Proposal Status Lookup (45 seconds)

**[Show GET /api/v1/lookup/proposal-statuses in Swagger]**

**Endpoint:** `GET /api/v1/lookup/proposal-statuses`

**Purpose:** Get list of proposal statuses

**Response:**
```json
[
  {
    "id": 1,
    "name": "Draft",
    "isActive": true
  },
  {
    "id": 2,
    "name": "Submitted",
    "isActive": true
  },
  {
    "id": 3,
    "name": "Under Review",
    "isActive": true
  },
  {
    "id": 4,
    "name": "Accepted",
    "isActive": true
  },
  {
    "id": 5,
    "name": "Rejected",
    "isActive": true
  },
  {
    "id": 6,
    "name": "Awarded",
    "isActive": true
  }
]
```

---

## SLIDE 9: Product Proposal Status Lookup (45 seconds)

**[Show GET /api/v1/lookup/product-proposal-statuses in Swagger]**

**Endpoint:** `GET /api/v1/lookup/product-proposal-statuses`

**Purpose:** Get product-level statuses

**Response:**
```json
[
  {
    "id": 1,
    "name": "Pending",
    "isActive": true
  },
  {
    "id": 2,
    "name": "Accepted",
    "isActive": true
  },
  {
    "id": 3,
    "name": "Rejected",
    "isActive": true
  },
  {
    "id": 4,
    "name": "Awarded",
    "isActive": true
  }
]
```

**Use Case:** Each product in a proposal can have its own status

---

## SLIDE 10: Amendment Action Lookup (45 seconds)

**[Show GET /api/v1/lookup/amendment-actions in Swagger]**

**Endpoint:** `GET /api/v1/lookup/amendment-actions`

**Purpose:** Get amendment action types

**Response:**
```json
[
  {
    "id": 1,
    "name": "Add",
    "isActive": true
  },
  {
    "id": 2,
    "name": "Modify",
    "isActive": true
  },
  {
    "id": 3,
    "name": "Delete",
    "isActive": true
  }
]
```

**Use Case:** When amending a contract, specify if products are being added, modified, or deleted

---

## SLIDE 11: Price Type Lookup (45 seconds)

**[Show GET /api/v1/lookup/price-types in Swagger]**

**Endpoint:** `GET /api/v1/lookup/price-types`

**Purpose:** Get price type options

**Response:**
```json
[
  {
    "id": 1,
    "name": "Delivered",
    "isActive": true
  },
  {
    "id": 2,
    "name": "FOB",
    "isActive": true
  },
  {
    "id": 3,
    "name": "Allowance",
    "isActive": true
  }
]
```

---

## SLIDE 12: Master Data Lookups (1 minute)

**[Show Master Data Lookup Endpoints in Swagger]**

**Manufacturers**

**Endpoint:** `GET /api/v1/lookup/manufacturers`

**Response:** List of all active manufacturers

---

**Distributors**

**Endpoint:** `GET /api/v1/lookup/distributors`

**Response:** List of all active distributors

---

**Industries**

**Endpoint:** `GET /api/v1/lookup/industries`

**Response:** List of all active industries

---

**OpCos**

**Endpoint:** `GET /api/v1/lookup/opcos`

**Response:** List of all active operating companies

**Note:** These are simplified versions without pagination, perfect for dropdowns.

---

## SLIDE 13: Summary (30 seconds)

**[Show Summary Slide]**

We've covered Reporting and Lookup APIs:

✅ **Reports** - Contract over term, pricing, proposals, dashboard  
✅ **Lookups** - Reference data for forms and dropdowns  

**Key Points:**
- Reports support JSON and Excel formats
- Lookup APIs don't require authentication
- Dashboard provides real-time metrics
- All lookups return active items only

In the next video, we'll cover Core Domain Objects and Data Models.

---

**[TOTAL TIME: ~10 minutes]**

