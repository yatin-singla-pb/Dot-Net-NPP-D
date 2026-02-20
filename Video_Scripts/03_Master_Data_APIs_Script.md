# Video Script 3: Master Data Management APIs

## SLIDE 1: Master Data Overview (30 seconds)

**[Show Master Data Controllers in Swagger]**

Welcome to Video 3, covering Master Data Management.

Master data includes the core reference entities used throughout the system:
- **Manufacturers** - Companies that produce products
- **Products** - Items with SKU, GTIN, pricing
- **Distributors** - Companies that distribute products
- **Industries** - Industry classifications
- **OpCos** - Operating companies

Let's explore each API.

---

## SLIDE 2: Manufacturers API - List (1 minute)

**[Show GET /api/Manufacturers in Swagger]**

**Endpoint:** `GET /api/Manufacturers`

**Purpose:** Get paginated list of manufacturers

**Query Parameters:**
- `pageNumber` (default: 1)
- `pageSize` (default: 10)
- `sortBy` (default: "Name")
- `sortDirection` ("asc" or "desc")
- `searchTerm` (searches name, AKA)
- `status` ("active", "inactive", or "all")

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": 1,
      "name": "Tyson Foods Inc",
      "aka": "Tyson",
      "status": "Active",
      "primaryBrokerId": 5,
      "primaryBrokerName": "John Broker",
      "isActive": true,
      "productCount": 245
    }
  ],
  "totalCount": 50,
  "pageNumber": 1,
  "pageSize": 10
}
```

---

## SLIDE 3: Manufacturers API - Get by ID (45 seconds)

**[Show GET /api/Manufacturers/{id} in Swagger]**

**Endpoint:** `GET /api/Manufacturers/{id}`

**Purpose:** Get detailed manufacturer information

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "Tyson Foods Inc",
  "aka": "Tyson",
  "status": "Active",
  "primaryBrokerId": 5,
  "primaryBrokerName": "John Broker",
  "contactEmail": "contracts@tyson.com",
  "contactPhone": "555-1234",
  "address": "123 Main St",
  "city": "Springdale",
  "state": "AR",
  "zipCode": "72762",
  "isActive": true,
  "createdDate": "2024-01-01T00:00:00Z",
  "products": []
}
```

---

## SLIDE 4: Manufacturers API - Create & Update (1 minute 30 seconds)

**[Show POST /api/Manufacturers in Swagger]**

**Create Manufacturer**

**Endpoint:** `POST /api/Manufacturers`

**Request Body:**
```json
{
  "name": "New Food Company",
  "aka": "NFC",
  "status": "Active",
  "primaryBrokerId": 3,
  "contactEmail": "info@newfood.com",
  "contactPhone": "555-9999",
  "address": "456 Oak Ave",
  "city": "Chicago",
  "state": "IL",
  "zipCode": "60601",
  "isActive": true
}
```

**Response (201 Created):** Returns created manufacturer with ID

---

**Update Manufacturer**

**Endpoint:** `PUT /api/Manufacturers/{id}`

**Request Body:** Same as create

**Response (200 OK):** Returns updated manufacturer

---

**Delete Manufacturer**

**Endpoint:** `DELETE /api/Manufacturers/{id}`

**Response (204 No Content)**

Note: This is a soft delete - sets IsActive = false

---

## SLIDE 5: Products API - List (1 minute)

**[Show GET /api/Products in Swagger]**

**Endpoint:** `GET /api/Products`

**Purpose:** Get paginated list of products

**Query Parameters:**
- `pageNumber`, `pageSize`, `sortBy`, `sortDirection`
- `searchTerm` (searches SKU, name, GTIN, UPC)
- `status` ("active", "inactive", "all")
- `manufacturerId` (filter by manufacturer)

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": 100,
      "sku": "TYS-12345",
      "name": "Chicken Breast Boneless",
      "description": "Fresh chicken breast, boneless skinless",
      "gtin": "00012345678905",
      "upc": "012345678905",
      "brand": "Tyson",
      "packSize": "10 LB",
      "manufacturerId": 1,
      "manufacturerName": "Tyson Foods",
      "isActive": true
    }
  ],
  "totalCount": 1250,
  "pageNumber": 1,
  "pageSize": 10
}
```

---

## SLIDE 6: Products API - Get by ID (45 seconds)

**[Show GET /api/Products/{id} in Swagger]**

**Endpoint:** `GET /api/Products/{id}`

**Purpose:** Get detailed product information

**Response (200 OK):**
```json
{
  "id": 100,
  "sku": "TYS-12345",
  "name": "Chicken Breast Boneless",
  "description": "Fresh chicken breast, boneless skinless, individually wrapped",
  "gtin": "00012345678905",
  "upc": "012345678905",
  "brand": "Tyson",
  "packSize": "10 LB",
  "caseCount": 4,
  "manufacturerId": 1,
  "manufacturerName": "Tyson Foods",
  "category": "Poultry",
  "isActive": true,
  "createdDate": "2024-01-15T00:00:00Z"
}
```

---

## SLIDE 7: Products API - Create & Update (1 minute 30 seconds)

**[Show POST /api/Products in Swagger]**

**Create Product**

**Endpoint:** `POST /api/Products`

**Request Body:**
```json
{
  "sku": "TYS-67890",
  "name": "Chicken Tenders",
  "description": "Breaded chicken tenders",
  "gtin": "00012345678912",
  "upc": "012345678912",
  "brand": "Tyson",
  "packSize": "5 LB",
  "caseCount": 6,
  "manufacturerId": 1,
  "category": "Poultry",
  "isActive": true
}
```

**Response (201 Created):** Returns created product with ID

---

**Update Product**

**Endpoint:** `PUT /api/Products/{id}`

**Request Body:** Same as create

**Response (200 OK):** Returns updated product

---

**Delete Product**

**Endpoint:** `DELETE /api/Products/{id}`

**Response (204 No Content)**

Note: Soft delete - sets IsActive = false

---

## SLIDE 8: Distributors API (1 minute 30 seconds)

**[Show DistributorsController in Swagger]**

**List Distributors**

**Endpoint:** `GET /api/Distributors`

**Query Parameters:** Same pagination as Manufacturers

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "name": "US Foods",
      "receiveContractProposal": true,
      "status": "Active",
      "contactEmail": "contracts@usfoods.com",
      "isActive": true
    }
  ]
}
```

---

**Get Distributor by ID**

**Endpoint:** `GET /api/Distributors/{id}`

---

**Create Distributor**

**Endpoint:** `POST /api/Distributors`

**Request Body:**
```json
{
  "name": "Sysco Corporation",
  "receiveContractProposal": true,
  "status": "Active",
  "contactEmail": "contracts@sysco.com",
  "contactPhone": "555-2222",
  "isActive": true
}
```

---

**Update/Delete:** Same pattern as Manufacturers

---

## SLIDE 9: Industries API (1 minute)

**[Show IndustriesController in Swagger]**

**List Industries**

**Endpoint:** `GET /api/Industries`

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "name": "Healthcare",
      "description": "Hospitals, nursing homes, medical facilities",
      "status": "Active",
      "isActive": true
    },
    {
      "id": 2,
      "name": "Education",
      "description": "Schools, universities, colleges",
      "status": "Active",
      "isActive": true
    }
  ]
}
```

---

**CRUD Operations:** Same pattern as other master data

**Endpoint:** `GET /api/Industries/{id}`  
**Endpoint:** `POST /api/Industries`  
**Endpoint:** `PUT /api/Industries/{id}`  
**Endpoint:** `DELETE /api/Industries/{id}`

---

## SLIDE 10: OpCos API (1 minute)

**[Show OpCosController in Swagger]**

**List OpCos (Operating Companies)**

**Endpoint:** `GET /api/OpCos`

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "name": "OpCo East",
      "code": "EAST",
      "distributorId": 1,
      "distributorName": "US Foods",
      "isActive": true
    },
    {
      "id": 2,
      "name": "OpCo West",
      "code": "WEST",
      "distributorId": 1,
      "distributorName": "US Foods",
      "isActive": true
    }
  ]
}
```

---

**CRUD Operations:** Same pattern

**Endpoint:** `GET /api/OpCos/{id}`  
**Endpoint:** `POST /api/OpCos`  
**Endpoint:** `PUT /api/OpCos/{id}`  
**Endpoint:** `DELETE /api/OpCos/{id}`

---

## SLIDE 11: Summary (30 seconds)

**[Show Summary Slide]**

We've covered Master Data Management APIs:

✅ **Manufacturers** - Product suppliers  
✅ **Products** - Catalog with SKU, GTIN, UPC  
✅ **Distributors** - Distribution companies  
✅ **Industries** - Industry classifications  
✅ **OpCos** - Operating companies  

**Common Patterns:**
- All support pagination, sorting, filtering
- All use soft deletes (IsActive flag)
- All return standardized JSON responses
- All require authentication

In the next video, we'll cover Proposal Management APIs.

---

**[TOTAL TIME: ~10 minutes]**

