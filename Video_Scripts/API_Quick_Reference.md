# NPP Contract Management API - Quick Reference Card

## 🔗 Base URLs

- **API:** `http://localhost:5110`
- **Swagger:** `http://localhost:5110/swagger`
- **Frontend:** `http://localhost:4200`

---

## 🔐 Authentication

### Login
```
POST /api/Auth/login
Body: { "userId": "admin", "password": "Admin@123" }
Response: { "accessToken": "...", "refreshToken": "..." }
```

### Use Token
```
Authorization: Bearer {accessToken}
```

---

## 👥 User Management

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/Users` | GET | List users (paginated) |
| `/api/Users/{id}` | GET | Get user by ID |
| `/api/Users` | POST | Create user |
| `/api/Users/{id}` | PUT | Update user |
| `/api/Users/{id}` | DELETE | Delete user (soft) |
| `/api/Users/{id}/manufacturers` | GET | Get user's manufacturers |
| `/api/Users/{id}/manufacturers` | POST | Sync manufacturers |

---

## 🏭 Master Data

### Manufacturers
```
GET    /api/Manufacturers
GET    /api/Manufacturers/{id}
POST   /api/Manufacturers
PUT    /api/Manufacturers/{id}
DELETE /api/Manufacturers/{id}
```

### Products
```
GET    /api/Products
GET    /api/Products/{id}
POST   /api/Products
PUT    /api/Products/{id}
DELETE /api/Products/{id}
```

### Distributors, Industries, OpCos
Same pattern as Manufacturers

---

## 📋 Proposals

### CRUD
```
GET    /api/v1/proposals
GET    /api/v1/proposals/{id}
POST   /api/v1/proposals
PUT    /api/v1/proposals/{id}
DELETE /api/v1/proposals/{id}
```

### Workflow
```
POST   /api/v1/proposals/{id}/submit
POST   /api/v1/proposals/{id}/accept
POST   /api/v1/proposals/{id}/reject
POST   /api/v1/proposals/{id}/clone
```

### Excel
```
GET    /api/v1/proposals/products/excel-template/{manufacturerId}
POST   /api/v1/proposals/products/excel-import/{manufacturerId}
```

### Products
```
GET    /api/v1/proposals/{id}/products
POST   /api/v1/proposals/{id}/products
PUT    /api/v1/proposals/{proposalId}/products/{productId}
DELETE /api/v1/proposals/{proposalId}/products/{productId}
```

---

## 📄 Contracts

### CRUD
```
GET    /api/Contracts
GET    /api/Contracts/{id}
POST   /api/Contracts
PUT    /api/Contracts/{id}
DELETE /api/Contracts/{id}
```

### Versioning
```
GET    /api/Contracts/{id}/versions
GET    /api/Contracts/{id}/versions/{versionNumber}
POST   /api/Contracts/{id}/versions
```

### Pricing
```
GET    /api/Contracts/{id}/prices
POST   /api/Contracts/{id}/prices
PUT    /api/Contracts/{contractId}/prices/{priceId}
DELETE /api/Contracts/{contractId}/prices/{priceId}
```

### Bulk Operations
```
POST   /api/Contracts/bulk-renewal
```

### Status
```
POST   /api/Contracts/{id}/suspend
POST   /api/Contracts/{id}/reactivate
POST   /api/Contracts/{id}/terminate
```

---

## 🚚 Velocity

### Ingestion
```
POST   /api/Velocity/ingest
GET    /api/Velocity/template
```

### Jobs
```
GET    /api/Velocity/jobs
GET    /api/Velocity/jobs/{jobId}
POST   /api/Velocity/jobs/{jobId}/retry
```

### Shipments
```
GET    /api/Velocity/shipments
GET    /api/Velocity/shipments/unmatched
```

### SFTP
```
GET    /api/Velocity/sftp-configs
POST   /api/Velocity/sftp-configs
PUT    /api/Velocity/sftp-configs/{id}
```

---

## 📊 Reports

```
GET    /api/Reports/contract-over-term?contractId={id}&format=json
GET    /api/Reports/contract-pricing?manufacturerId={id}&format=excel
GET    /api/Reports/proposal-summary?startDate={date}&endDate={date}
GET    /api/Reports/dashboard
```

---

## 🔍 Lookups (No Auth Required)

```
GET    /api/v1/lookup/proposal-types
GET    /api/v1/lookup/proposal-statuses
GET    /api/v1/lookup/product-proposal-statuses
GET    /api/v1/lookup/amendment-actions
GET    /api/v1/lookup/price-types
GET    /api/v1/lookup/manufacturers
GET    /api/v1/lookup/distributors
GET    /api/v1/lookup/industries
GET    /api/v1/lookup/opcos
```

---

## 📦 Common Request Bodies

### Create Proposal
```json
{
  "title": "Q1 2024 Pricing",
  "proposalTypeId": 1,
  "manufacturerId": 1,
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "products": [
    {
      "productId": 50,
      "quantity": 1000,
      "commercialDelPrice": 2.50,
      "uom": "Cases",
      "billbacksAllowed": true
    }
  ],
  "distributorIds": [1, 2],
  "industryIds": [1],
  "opcoIds": [1, 2, 3]
}
```

### Create Contract
```json
{
  "name": "2024 Contract",
  "manufacturerId": 1,
  "proposalId": 10,
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "status": "Active",
  "prices": [
    {
      "productId": 50,
      "priceType": "Delivered",
      "commercialDelPrice": 2.50,
      "uom": "Cases"
    }
  ]
}
```

---

## 🎯 Common Query Parameters

### Pagination
```
?pageNumber=1&pageSize=10
```

### Sorting
```
?sortBy=Name&sortDirection=asc
```

### Filtering
```
?searchTerm=chicken
?status=active
?manufacturerId=1
?startDate=2024-01-01&endDate=2024-12-31
```

---

## 📋 Response Codes

| Code | Meaning |
|------|---------|
| 200 | OK - Success |
| 201 | Created - Resource created |
| 204 | No Content - Deleted successfully |
| 400 | Bad Request - Invalid data |
| 401 | Unauthorized - Login required |
| 403 | Forbidden - Insufficient permissions |
| 404 | Not Found - Resource doesn't exist |
| 500 | Internal Server Error |

---

## 🔑 Sample Test Data

### Users
- **Admin:** `userId: "admin"`, `password: "Admin@123"`
- **Manager:** `userId: "manager"`, `password: "Manager@123"`
- **Manufacturer:** `userId: "mfg.user"`, `password: "Mfg@123"`

### Manufacturers
- ID 1: Tyson Foods
- ID 2: Sysco Corporation
- ID 3: US Foods

### Products
- ID 50: Chicken Breast (Tyson)
- ID 75: Ground Beef (Sysco)
- ID 100: French Fries (US Foods)

---

## 🛠️ Useful Swagger Features

### Try It Out
1. Click "Try it out" button
2. Fill in parameters
3. Click "Execute"
4. View response

### Authorize
1. Click "Authorize" button (top right)
2. Enter: `Bearer {your-access-token}`
3. Click "Authorize"
4. All requests now include token

### Schemas
- Scroll to bottom of Swagger UI
- View all DTOs and models
- See required fields and data types

---

## 🎬 Recording Shortcuts

### Swagger Navigation
- **Expand all:** Click controller name
- **Collapse all:** Click again
- **Expand endpoint:** Click endpoint
- **Try it out:** Click button
- **Execute:** Click button
- **Copy response:** Click copy icon

### Browser
- **Full screen:** F11
- **Zoom in:** Ctrl + Plus
- **Zoom out:** Ctrl + Minus
- **Reset zoom:** Ctrl + 0

---

## 📝 Script Markers

When reading scripts, look for these markers:

- **[Show ...]** - What to display on screen
- **Endpoint:** - API endpoint to highlight
- **Purpose:** - Why this endpoint exists
- **Request Body:** - Example request
- **Response:** - Example response
- **Use Case:** - Real-world scenario

---

## ✅ Quick Checklist

Before each recording:
- [ ] API running
- [ ] Swagger open
- [ ] Logged in (token ready)
- [ ] Sample data loaded
- [ ] Script open
- [ ] Recording software ready
- [ ] Microphone tested
- [ ] Notifications off

---

**Print this page and keep it handy during recording!** 📄

