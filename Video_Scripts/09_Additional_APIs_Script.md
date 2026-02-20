# Video Script 9: Additional APIs - Customer Accounts, Member Accounts & More

## SLIDE 1: Additional APIs Overview (30 seconds)

**[Show Additional Controllers in Swagger]**

Welcome to Video 9, covering Additional APIs.

Beyond the core proposal and contract management, the NPP system includes several supporting APIs:

- **Customer Accounts** - End customer management
- **Member Accounts** - Member organization management
- **Distributor Product Codes** - Product code mapping
- **Test Controller** - Health checks and diagnostics

Let's explore each one.

---

## SLIDE 2: Customer Accounts - List (1 minute 30 seconds)

**[Show GET /api/CustomerAccounts in Swagger]**

**Endpoint:** `GET /api/CustomerAccounts`

**Purpose:** Get paginated list of customer accounts

**Query Parameters:**
- `pageNumber`, `pageSize` (pagination)
- `sortBy`, `sortDirection` (sorting)
- `searchTerm` (searches customer name, number, address)
- `status` (filter by status)

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": 1,
      "customerNumber": "CUST-001",
      "customerName": "ABC Hospital",
      "address": "123 Main St",
      "city": "Chicago",
      "state": "IL",
      "zipCode": "60601",
      "opCoId": 1,
      "opCoName": "OpCo East",
      "memberAccountId": 5,
      "memberAccountName": "Healthcare Group",
      "industryId": 1,
      "industryName": "Healthcare",
      "status": "Active",
      "tracsAccess": true,
      "toEntegra": true,
      "isActive": true
    }
  ],
  "totalCount": 500,
  "pageNumber": 1,
  "pageSize": 10
}
```

---

## SLIDE 3: Customer Accounts - Get by ID (1 minute)

**[Show GET /api/CustomerAccounts/{id} in Swagger]**

**Endpoint:** `GET /api/CustomerAccounts/{id}`

**Purpose:** Get detailed customer account information

**Response (200 OK):**
```json
{
  "id": 1,
  "customerNumber": "CUST-001",
  "customerName": "ABC Hospital",
  "address": "123 Main St",
  "city": "Chicago",
  "state": "IL",
  "zipCode": "60601",
  "phoneNumber": "555-1234",
  "email": "purchasing@abchospital.com",
  "opCoId": 1,
  "opCoName": "OpCo East",
  "memberAccountId": 5,
  "memberAccountName": "Healthcare Group",
  "industryId": 1,
  "industryName": "Healthcare",
  "distributorId": 1,
  "distributorName": "US Foods",
  "status": "Active",
  "tracsAccess": true,
  "toEntegra": true,
  "association": 1,
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "isActive": true,
  "createdDate": "2024-01-01T00:00:00Z"
}
```

---

## SLIDE 4: Customer Accounts - Create & Update (1 minute 30 seconds)

**[Show POST /api/CustomerAccounts in Swagger]**

**Create Customer Account**

**Endpoint:** `POST /api/CustomerAccounts`

**Request Body:**
```json
{
  "customerNumber": "CUST-002",
  "customerName": "XYZ Medical Center",
  "address": "456 Oak Ave",
  "city": "New York",
  "state": "NY",
  "zipCode": "10001",
  "phoneNumber": "555-5678",
  "email": "purchasing@xyzmedical.com",
  "opCoId": 2,
  "memberAccountId": 6,
  "industryId": 1,
  "distributorId": 1,
  "status": "Active",
  "tracsAccess": true,
  "toEntegra": false,
  "association": 1,
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "isActive": true
}
```

**Response (201 Created):** Returns created customer account with ID

---

**Update Customer Account**

**Endpoint:** `PUT /api/CustomerAccounts/{id}`

**Request Body:** Same as create

**Response (200 OK):** Returns updated customer account

---

**Delete Customer Account**

**Endpoint:** `DELETE /api/CustomerAccounts/{id}`

**Response (204 No Content)**

Note: Soft delete - sets IsActive = false

---

## SLIDE 5: Customer Accounts - Additional Endpoints (1 minute 30 seconds)

**[Show Additional Customer Account Endpoints in Swagger]**

**Get by OpCo**

**Endpoint:** `GET /api/CustomerAccounts/opco/{opCoId}`

**Purpose:** Get all customer accounts for a specific OpCo

---

**Get by Member Account**

**Endpoint:** `GET /api/CustomerAccounts/member/{memberAccountId}`

**Purpose:** Get all customer accounts for a specific member account

---

**Advanced Search**

**Endpoint:** `GET /api/CustomerAccounts/search`

**Query Parameters:**
- `searchTerm` (searches name, number, address)
- `memberAccountId` (filter by member)
- `distributorId` (filter by distributor)
- `opCoId` (filter by OpCo)
- `status` (filter by status)
- `isActive` (filter by active/inactive)
- `industryId` (filter by industry)
- `association` (filter by association type)
- `startDate`, `endDate` (date range)
- `tracsAccess` (filter by TRACS access)
- `toEntegra` (filter by Entegra flag)
- `page`, `pageSize` (pagination)

**Use Case:** Complex filtering for reporting and analysis

---

## SLIDE 6: Member Accounts - Overview (1 minute 30 seconds)

**[Show MemberAccountsController in Swagger]**

**Endpoint Base:** `/api/MemberAccounts`

**Purpose:** Manage member organizations (groups of customer accounts)

**List Member Accounts**

**Endpoint:** `GET /api/MemberAccounts`

**Query Parameters:** Same pagination as Customer Accounts

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "memberNumber": "MEM-001",
      "memberName": "Healthcare Group",
      "address": "789 Corporate Blvd",
      "city": "Boston",
      "state": "MA",
      "zipCode": "02101",
      "industryId": 1,
      "industryName": "Healthcare",
      "status": "Active",
      "customerAccountCount": 25,
      "isActive": true
    }
  ],
  "totalCount": 50,
  "pageNumber": 1,
  "pageSize": 10
}
```

---

**CRUD Operations:** Same pattern as Customer Accounts

**Endpoint:** `GET /api/MemberAccounts/{id}`  
**Endpoint:** `POST /api/MemberAccounts`  
**Endpoint:** `PUT /api/MemberAccounts/{id}`  
**Endpoint:** `DELETE /api/MemberAccounts/{id}`

---

## SLIDE 7: Distributor Product Codes (1 minute)

**[Show DistributorProductCodesController in Swagger]**

**Endpoint Base:** `/api/DistributorProductCodes`

**Purpose:** Map distributor-specific product codes to NPP products

**List Distributor Product Codes**

**Endpoint:** `GET /api/DistributorProductCodes`

**Query Parameters:**
- `distributorId` (filter by distributor)
- `productId` (filter by product)

**Response:**
```json
[
  {
    "id": 1,
    "distributorId": 1,
    "distributorName": "US Foods",
    "productId": 50,
    "productName": "Chicken Breast",
    "distributorProductCode": "USF-12345",
    "distributorProductName": "Chicken Breast Boneless",
    "isActive": true
  }
]
```

---

**CRUD Operations:**

**Endpoint:** `GET /api/DistributorProductCodes/{id}`  
**Endpoint:** `POST /api/DistributorProductCodes`  
**Endpoint:** `PUT /api/DistributorProductCodes/{id}`  
**Endpoint:** `DELETE /api/DistributorProductCodes/{id}`

**Use Case:** When distributors use different product codes than manufacturers

---

## SLIDE 8: Test Controller (45 seconds)

**[Show TestController in Swagger]**

**Endpoint Base:** `/api/Test`

**Purpose:** Health checks and diagnostics

**Health Check**

**Endpoint:** `GET /api/Test/health`

**Response:**
```json
{
  "status": "Healthy",
  "timestamp": "2024-12-09T10:00:00Z",
  "version": "1.0.0"
}
```

---

**Database Connection Test**

**Endpoint:** `GET /api/Test/db`

**Response:**
```json
{
  "status": "Connected",
  "database": "NPPContractManagement",
  "server": "localhost"
}
```

---

**Echo Test**

**Endpoint:** `POST /api/Test/echo`

**Request Body:** Any JSON

**Response:** Returns the same JSON (useful for testing)

---

## SLIDE 9: Summary (30 seconds)

**[Show Summary Slide]**

We've covered Additional APIs:

✅ **Customer Accounts** - End customer management with advanced search  
✅ **Member Accounts** - Member organization management  
✅ **Distributor Product Codes** - Product code mapping  
✅ **Test Controller** - Health checks and diagnostics  

**Key Points:**
- Customer accounts link to member accounts, OpCos, and industries
- Advanced search supports complex filtering
- Distributor product codes enable cross-reference
- Test endpoints help with monitoring and troubleshooting

This completes our API overview series!

---

**[TOTAL TIME: ~9 minutes]**

