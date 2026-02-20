# Complete API Endpoints Breakdown for Java Migration

## 📊 **SUMMARY**
- **Total Controllers:** 26
- **Total Endpoints:** 194
- **Database:** MySQL 8.0
- **Authentication:** JWT Bearer Token

---

## 🔐 **1. AUTHENTICATION MODULE**

**Controller:** `AuthController.cs`  
**Base Route:** `/api/auth`  
**Endpoints:** 7

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/login` | User login with credentials | ❌ No |
| POST | `/logout` | User logout | ✅ Yes |
| POST | `/refresh-token` | Refresh JWT token | ❌ No |
| POST | `/forgot-password` | Request password reset | ❌ No |
| POST | `/reset-password` | Reset password with token | ❌ No |
| POST | `/change-password` | Change password (logged in) | ✅ Yes |
| GET | `/validate-token` | Validate reset token | ❌ No |

**Key Features:**
- JWT token generation (access + refresh tokens)
- Password hashing (BCrypt)
- Email-based password reset
- Token expiry: 60 minutes (configurable)

---

## 👥 **2. USERS MANAGEMENT**

**Controller:** `UsersController.cs`  
**Base Route:** `/api/users`  
**Endpoints:** 13

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | Get all users | ✅ Yes |
| GET | `/{id}` | Get user by ID | ✅ Yes |
| POST | `/` | Create new user | ✅ Yes (Admin) |
| PUT | `/{id}` | Update user | ✅ Yes (Admin) |
| DELETE | `/{id}` | Delete user | ✅ Yes (Admin) |
| GET | `/{id}/roles` | Get user roles | ✅ Yes |
| POST | `/{id}/roles` | Assign role to user | ✅ Yes (Admin) |
| DELETE | `/{id}/roles/{roleId}` | Remove role from user | ✅ Yes (Admin) |
| GET | `/{id}/manufacturers` | Get user manufacturers | ✅ Yes |
| POST | `/{id}/manufacturers` | Assign manufacturer to user | ✅ Yes (Admin) |
| DELETE | `/{id}/manufacturers/{manufacturerId}` | Remove manufacturer from user | ✅ Yes (Admin) |
| PUT | `/{id}/activate` | Activate user | ✅ Yes (Admin) |
| PUT | `/{id}/deactivate` | Deactivate user | ✅ Yes (Admin) |

---

## 🔑 **3. ROLES & PERMISSIONS**

**Controller:** `RolesController.cs`  
**Base Route:** `/api/roles`  
**Endpoints:** 5

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | Get all roles | ✅ Yes |
| GET | `/{id}` | Get role by ID | ✅ Yes |
| POST | `/` | Create new role | ✅ Yes (Admin) |
| PUT | `/{id}` | Update role | ✅ Yes (Admin) |
| DELETE | `/{id}` | Delete role | ✅ Yes (Admin) |

**Default Roles:**
1. Admin
2. Contract Manager
3. Proposal Manager
4. Contract Viewer
5. Manufacturer User

---

## 📄 **4. CONTRACTS MODULE (CORE)**

**Controller:** `ContractsController.cs`  
**Base Route:** `/api/contracts`  
**Endpoints:** 24

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | Get all contracts | ✅ Yes |
| GET | `/{id}` | Get contract by ID | ✅ Yes |
| POST | `/` | Create new contract | ✅ Yes |
| PUT | `/{id}` | Update contract | ✅ Yes |
| DELETE | `/{id}` | Delete contract | ✅ Yes |
| GET | `/{id}/versions` | Get contract versions | ✅ Yes |
| GET | `/{id}/versions/{versionId}` | Get specific version | ✅ Yes |
| POST | `/{id}/versions` | Create new version | ✅ Yes |
| PUT | `/{id}/versions/{versionId}` | Update version | ✅ Yes |
| DELETE | `/{id}/versions/{versionId}` | Delete version | ✅ Yes |
| GET | `/{id}/products` | Get contract products | ✅ Yes |
| POST | `/{id}/products` | Add product to contract | ✅ Yes |
| DELETE | `/{id}/products/{productId}` | Remove product | ✅ Yes |
| GET | `/{id}/prices` | Get contract prices | ✅ Yes |
| POST | `/{id}/prices` | Add price | ✅ Yes |
| PUT | `/{id}/prices/{priceId}` | Update price | ✅ Yes |
| DELETE | `/{id}/prices/{priceId}` | Delete price | ✅ Yes |
| GET | `/{id}/assignments` | Get contract assignments | ✅ Yes |
| POST | `/{id}/assign` | Assign contract | ✅ Yes |
| DELETE | `/{id}/assignments/{assignmentId}` | Remove assignment | ✅ Yes |
| PUT | `/{id}/activate` | Activate contract | ✅ Yes |
| PUT | `/{id}/suspend` | Suspend contract | ✅ Yes |
| GET | `/search` | Search contracts | ✅ Yes |
| GET | `/export` | Export contracts to Excel | ✅ Yes |

**Complex Features:**
- Multi-version support
- Product-price relationships
- Assignment to customers/members
- Status workflow (Draft → Active → Suspended → Expired)

---

## 📦 **5. PROPOSALS MODULE**

**Controller:** `ProposalsController.cs`  
**Base Route:** `/api/proposals`  
**Endpoints:** 11

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | Get all proposals | ✅ Yes |
| GET | `/{id}` | Get proposal by ID | ✅ Yes |
| POST | `/` | Create new proposal | ✅ Yes |
| PUT | `/{id}` | Update proposal | ✅ Yes |
| DELETE | `/{id}` | Delete proposal | ✅ Yes |
| GET | `/{id}/products` | Get proposal products | ✅ Yes |
| POST | `/{id}/products` | Add product to proposal | ✅ Yes |
| PUT | `/{id}/products/{productId}` | Update proposal product | ✅ Yes |
| DELETE | `/{id}/products/{productId}` | Remove product | ✅ Yes |
| POST | `/{id}/products/import` | Import products from Excel | ✅ Yes |
| GET | `/{id}/products/export` | Export products to Excel | ✅ Yes |

**Key Features:**
- Excel import/export (Apache POI equivalent needed)
- Product pricing with multiple price types
- Allowance calculations
- Award status tracking

---

## 🏢 **7. MANUFACTURERS MODULE**

**Controller:** `ManufacturersController.cs`
**Base Route:** `/api/manufacturers`
**Endpoints:** 5

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | Get all manufacturers | ✅ Yes |
| GET | `/{id}` | Get manufacturer by ID | ✅ Yes |
| POST | `/` | Create new manufacturer | ✅ Yes |
| PUT | `/{id}` | Update manufacturer | ✅ Yes |
| DELETE | `/{id}` | Delete manufacturer | ✅ Yes |

---

## 🚚 **8. DISTRIBUTORS MODULE**

**Controller:** `DistributorsController.cs`
**Base Route:** `/api/distributors`
**Endpoints:** 7

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | Get all distributors | ✅ Yes |
| GET | `/{id}` | Get distributor by ID | ✅ Yes |
| POST | `/` | Create new distributor | ✅ Yes |
| PUT | `/{id}` | Update distributor | ✅ Yes |
| DELETE | `/{id}` | Delete distributor | ✅ Yes |
| GET | `/{id}/product-codes` | Get distributor product codes | ✅ Yes |
| POST | `/{id}/product-codes` | Add product code mapping | ✅ Yes |

---

## 🏭 **9. INDUSTRIES MODULE**

**Controller:** `IndustriesController.cs`
**Base Route:** `/api/industries`
**Endpoints:** 10

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | Get all industries | ✅ Yes |
| GET | `/{id}` | Get industry by ID | ✅ Yes |
| POST | `/` | Create new industry | ✅ Yes |
| PUT | `/{id}` | Update industry | ✅ Yes |
| DELETE | `/{id}` | Delete industry | ✅ Yes |
| GET | `/{id}/contracts` | Get contracts by industry | ✅ Yes |
| GET | `/{id}/customers` | Get customers in industry | ✅ Yes |
| PUT | `/{id}/activate` | Activate industry | ✅ Yes |
| PUT | `/{id}/deactivate` | Deactivate industry | ✅ Yes |
| GET | `/hierarchy` | Get industry hierarchy | ✅ Yes |

---

## 🏢 **10. OPCOS (OPERATING COMPANIES)**

**Controller:** `OpCosController.cs`
**Base Route:** `/api/opcos`
**Endpoints:** 11

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | Get all OpCos | ✅ Yes |
| GET | `/{id}` | Get OpCo by ID | ✅ Yes |
| POST | `/` | Create new OpCo | ✅ Yes |
| PUT | `/{id}` | Update OpCo | ✅ Yes |
| DELETE | `/{id}` | Delete OpCo | ✅ Yes |
| GET | `/{id}/contracts` | Get contracts by OpCo | ✅ Yes |
| GET | `/{id}/customers` | Get customers by OpCo | ✅ Yes |
| GET | `/{id}/members` | Get members by OpCo | ✅ Yes |
| PUT | `/{id}/activate` | Activate OpCo | ✅ Yes |
| PUT | `/{id}/deactivate` | Deactivate OpCo | ✅ Yes |
| GET | `/hierarchy` | Get OpCo hierarchy | ✅ Yes |

---

## 👥 **11. CUSTOMER ACCOUNTS**

**Controller:** `CustomerAccountsController.cs`
**Base Route:** `/api/customer-accounts`
**Endpoints:** 11

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | Get all customer accounts | ✅ Yes |
| GET | `/{id}` | Get customer by ID | ✅ Yes |
| POST | `/` | Create new customer | ✅ Yes |
| PUT | `/{id}` | Update customer | ✅ Yes |
| DELETE | `/{id}` | Delete customer | ✅ Yes |
| GET | `/search` | Search customers | ✅ Yes |
| GET | `/{id}/contracts` | Get customer contracts | ✅ Yes |
| GET | `/{id}/assignments` | Get contract assignments | ✅ Yes |
| POST | `/import` | Import customers from Excel | ✅ Yes |
| GET | `/export` | Export customers to Excel | ✅ Yes |
| GET | `/by-opco/{opcoId}` | Get customers by OpCo | ✅ Yes |

---

## 👤 **12. MEMBER ACCOUNTS**

**Controller:** `MemberAccountsController.cs`
**Base Route:** `/api/member-accounts`
**Endpoints:** 9

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | Get all member accounts | ✅ Yes |
| GET | `/{id}` | Get member by ID | ✅ Yes |
| POST | `/` | Create new member | ✅ Yes |
| PUT | `/{id}` | Update member | ✅ Yes |
| DELETE | `/{id}` | Delete member | ✅ Yes |
| GET | `/search` | Search members | ✅ Yes |
| GET | `/{id}/contracts` | Get member contracts | ✅ Yes |
| POST | `/import` | Import members from Excel | ✅ Yes |
| GET | `/export` | Export members to Excel | ✅ Yes |

---

## 📊 **13. VELOCITY (USAGE TRACKING)**

**Controller:** `VelocityController.cs`
**Base Route:** `/api/velocity`
**Endpoints:** 11

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/jobs` | Get all velocity jobs | ✅ Yes |
| GET | `/jobs/{id}` | Get job by ID | ✅ Yes |
| POST | `/jobs` | Create new velocity job | ✅ Yes |
| POST | `/jobs/{id}/upload` | Upload velocity file (CSV/Excel) | ✅ Yes |
| POST | `/jobs/{id}/process` | Process velocity data | ✅ Yes |
| GET | `/jobs/{id}/status` | Get job processing status | ✅ Yes |
| GET | `/jobs/{id}/errors` | Get job errors | ✅ Yes |
| GET | `/shipments` | Get all shipments | ✅ Yes |
| GET | `/shipments/search` | Search shipments | ✅ Yes |
| GET | `/usage-report` | Generate usage report | ✅ Yes |
| GET | `/usage-report/export` | Export usage report to Excel | ✅ Yes |

**Key Features:**
- Background job processing
- CSV/Excel file parsing
- Large file handling (chunked processing)
- Error tracking and reporting

---

## 📈 **14. REPORTS MODULE**

**Controller:** `ReportsController.cs`
**Base Route:** `/api/reports`
**Endpoints:** 4

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/contract-pricing` | Contract pricing report | ✅ Yes |
| GET | `/contract-over-term` | Contracts over term report | ✅ Yes |
| GET | `/velocity-usage` | Velocity usage report | ✅ Yes |
| GET | `/proposal-summary` | Proposal summary report | ✅ Yes |

**All reports support:**
- Excel export
- Date range filtering
- Multiple filter criteria
- Pagination

---

## 🔄 **15. BULK RENEWAL**

**Controller:** `BulkRenewalController.cs`
**Base Route:** `/api/bulk-renewal`
**Endpoints:** 2

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/preview` | Preview bulk renewal | ✅ Yes |
| POST | `/execute` | Execute bulk renewal | ✅ Yes |

---

## 📋 **16. LOOKUP/DROPDOWN DATA**

**Controller:** `LookupController.cs`
**Base Route:** `/api/lookup`
**Endpoints:** 10

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/price-types` | Get all price types | ✅ Yes |
| GET | `/contract-statuses` | Get contract statuses | ✅ Yes |
| GET | `/proposal-statuses` | Get proposal statuses | ✅ Yes |
| GET | `/product-categories` | Get product categories | ✅ Yes |
| GET | `/uom` | Get units of measure | ✅ Yes |
| GET | `/states` | Get US states | ✅ Yes |
| GET | `/countries` | Get countries | ✅ Yes |
| GET | `/currencies` | Get currencies | ✅ Yes |
| GET | `/payment-terms` | Get payment terms | ✅ Yes |
| GET | `/shipping-terms` | Get shipping terms | ✅ Yes |

---

## 🔧 **ADDITIONAL TECHNICAL DETAILS**

### **Authentication Flow:**
```
1. POST /api/auth/login
   → Returns: { token, refreshToken, user }

2. All subsequent requests:
   → Header: Authorization: Bearer {token}

3. Token expires (60 min):
   → POST /api/auth/refresh-token
   → Returns: { token, refreshToken }
```

### **Error Response Format:**
```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": [
    {
      "field": "email",
      "message": "Email is required"
    }
  ]
}
```

### **Pagination Format:**
```
Request: GET /api/contracts?page=1&pageSize=20
Response: {
  "data": [...],
  "totalCount": 150,
  "page": 1,
  "pageSize": 20,
  "totalPages": 8
}
```

### **File Upload Format:**
```
Content-Type: multipart/form-data
Field: file (binary)
Additional fields: metadata (JSON)
```

---

## 📦 **JAVA DEPENDENCIES NEEDED**

```xml
<!-- Spring Boot Starter -->
<dependency>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-web</artifactId>
</dependency>

<!-- Spring Security + JWT -->
<dependency>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-security</artifactId>
</dependency>
<dependency>
    <groupId>io.jsonwebtoken</groupId>
    <artifactId>jjwt-api</artifactId>
</dependency>

<!-- JPA/Hibernate -->
<dependency>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-data-jpa</artifactId>
</dependency>

<!-- MySQL Driver -->
<dependency>
    <groupId>com.mysql</groupId>
    <artifactId>mysql-connector-j</artifactId>
</dependency>

<!-- Excel Processing (Apache POI) -->
<dependency>
    <groupId>org.apache.poi</groupId>
    <artifactId>poi-ooxml</artifactId>
</dependency>

<!-- CSV Processing -->
<dependency>
    <groupId>com.opencsv</groupId>
    <artifactId>opencsv</artifactId>
</dependency>

<!-- SendGrid Email -->
<dependency>
    <groupId>com.sendgrid</groupId>
    <artifactId>sendgrid-java</artifactId>
</dependency>

<!-- Swagger/OpenAPI -->
<dependency>
    <groupId>org.springdoc</groupId>
    <artifactId>springdoc-openapi-starter-webmvc-ui</artifactId>
</dependency>

<!-- BCrypt Password Hashing -->
<dependency>
    <groupId>org.springframework.security</groupId>
    <artifactId>spring-security-crypto</artifactId>
</dependency>
```

---

**Document Version:** 1.0
**Last Updated:** 2026-01-06
**Total Endpoints Documented:** 194
## 🏭 **6. PRODUCTS MODULE**

**Controller:** `ProductsController.cs`  
**Base Route:** `/api/products`  
**Endpoints:** 15

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | Get all products | ✅ Yes |
| GET | `/{id}` | Get product by ID | ✅ Yes |
| POST | `/` | Create new product | ✅ Yes |
| PUT | `/{id}` | Update product | ✅ Yes |
| DELETE | `/{id}` | Delete product | ✅ Yes |
| GET | `/search` | Search products | ✅ Yes |
| GET | `/by-manufacturer/{manufacturerId}` | Get by manufacturer | ✅ Yes |
| GET | `/{id}/contracts` | Get contracts using product | ✅ Yes |
| GET | `/{id}/prices` | Get product price history | ✅ Yes |
| POST | `/import` | Import products from Excel | ✅ Yes |
| GET | `/export` | Export products to Excel | ✅ Yes |
| PUT | `/{id}/activate` | Activate product | ✅ Yes |
| PUT | `/{id}/deactivate` | Deactivate product | ✅ Yes |
| GET | `/duplicates` | Find duplicate products | ✅ Yes |
| POST | `/merge` | Merge duplicate products | ✅ Yes |

---


