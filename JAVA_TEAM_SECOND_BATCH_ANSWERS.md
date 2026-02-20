# Java Team - Second Batch Questions - Detailed Answers

## 📋 **ANSWERS TO YOUR 4 QUESTIONS**

---

## **Q1: Single Microservice or Multiple Microservices?**

### ✅ **ANSWER: SINGLE MONOLITHIC APPLICATION (NOT MICROSERVICES)**

This is a **single .NET Web API application** - NOT a microservices architecture.

### **Architecture Details:**

```
┌─────────────────────────────────────────────────────────┐
│  SINGLE APPLICATION: NPPContractManagement.API          │
├─────────────────────────────────────────────────────────┤
│  • One codebase                                         │
│  • One deployment unit                                  │
│  • One database                                         │
│  • All 194 endpoints in one API                         │
└─────────────────────────────────────────────────────────┘
```

### **Why Velocity APIs Weren't in the Video:**

The Velocity module **IS PART OF THE SAME API** - it was just not shown in the video demonstration. Here's the proof:

#### **Evidence from Codebase:**

1. **Same Project Structure:**
   ```
   NPPContractManagement.API/
   ├── Controllers/
   │   ├── AuthController.cs
   │   ├── ContractsController.cs
   │   ├── ProposalsController.cs
   │   ├── VelocityController.cs  ← SAME PROJECT
   │   └── ... (23 other controllers)
   ├── Services/
   │   ├── VelocityService.cs
   │   ├── VelocityBackgroundProcessor.cs
   │   └── ... (other services)
   └── Program.cs  ← Single entry point
   ```

2. **Same Swagger Documentation:**
   - All controllers (including Velocity) are in the same Swagger UI
   - URL: `http://34.9.77.60:8081/swagger/index.html`
   - All endpoints share the same base URL: `/api/*`

3. **Same Database:**
   - Velocity tables are in the same MySQL database
   - Tables: `VelocityJobs`, `VelocityShipments`, `VelocityJobRows`, `SftpProbeConfigs`

4. **Same Authentication:**
   - Velocity endpoints use the same JWT authentication
   - Same authorization middleware
   - Same user context

### **Velocity Module Details:**

**Controller:** `VelocityController.cs`  
**Base Route:** `/api/velocity`  
**Endpoints:** 11 endpoints

| Endpoint | Purpose |
|----------|---------|
| `POST /api/velocity/ingest` | Upload CSV/Excel file for processing |
| `GET /api/velocity/jobs` | List all velocity jobs |
| `GET /api/velocity/jobs/{jobId}` | Get job details |
| `POST /api/velocity/jobs/{jobId}/retry` | Retry failed job |
| `POST /api/velocity/jobs/{jobId}/restart` | Restart job |
| `GET /api/velocity/template` | Download CSV template |
| `POST /api/velocity/exceptions` | Get failed job rows |
| `POST /api/velocity/usage-report` | Generate usage report |
| `POST /api/velocity/usage-report/details` | Get usage details |
| `POST /api/velocity/usage-report/check-contracts` | Check existing contracts |
| `POST /api/velocity/usage-report/create-proposals` | Create proposals from velocity data |

### **Why It's NOT Microservices:**

❌ **NOT separate deployments**  
❌ **NOT separate databases**  
❌ **NOT separate authentication**  
❌ **NOT separate scaling**  
❌ **NOT inter-service communication**  

✅ **Single monolithic application**  
✅ **All modules in one codebase**  
✅ **One deployment**  
✅ **Shared database**  

### **For Java Migration:**

**You need to build:** **ONE Spring Boot application** with all 26 controllers (including Velocity)

**NOT:** Multiple microservices

---

## **Q2: Backend Service Deployed to Cloud for Testing?**

### ✅ **ANSWER: YES, BACKEND IS DEPLOYED ON GOOGLE CLOUD**

### **Deployment Details:**

| Component | URL | Status |
|-----------|-----|--------|
| **Backend API** | `http://34.9.77.60:8081/api` | ✅ LIVE |
| **Swagger UI** | `http://34.9.77.60:8081/swagger/index.html` | ✅ LIVE |
| **Frontend** | `http://34.9.77.60:8080` | ✅ LIVE |
| **Database** | `34.9.77.60:3306` (MySQL) | ✅ LIVE |

### **Why Video Showed Localhost:**

The video was recorded during **local development** for demonstration purposes. However, the **same code is deployed to production** on GCP.

### **Testing Access:**

#### **Option 1: Use Deployed API (Recommended)**
```
Base URL: http://34.9.77.60:8081/api
Swagger:  http://34.9.77.60:8081/swagger/index.html
```

**Steps to Test:**
1. Open Swagger UI: `http://34.9.77.60:8081/swagger/index.html`
2. Click "Authorize" button
3. Login to get JWT token:
   ```
   POST /api/auth/login
   {
     "userId": "admin",
     "password": "Admin@123"
   }
   ```
4. Copy the `token` from response
5. Click "Authorize" again, enter: `Bearer {token}`
6. Test any endpoint

#### **Option 2: Request Database Access**
We can provide:
- ✅ Read-only database credentials
- ✅ VPN access to database server
- ✅ Sample data exports
- ✅ Database schema documentation

#### **Option 3: Local Setup**
We can provide:
- ✅ Complete source code
- ✅ Database backup
- ✅ Setup instructions
- ✅ Sample data

### **Verification Commands:**

```bash
# Test API is accessible
curl http://34.9.77.60:8081/api/lookup/price-types

# Test Swagger is accessible
curl http://34.9.77.60:8081/swagger/v1/swagger.json

# Test login endpoint
curl -X POST http://34.9.77.60:8081/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"userId":"admin","password":"Admin@123"}'
```

### **For Java Team:**

**You have access to:**
1. ✅ Live API for testing (http://34.9.77.60:8081)
2. ✅ Swagger documentation (http://34.9.77.60:8081/swagger)
3. ✅ Frontend to see how APIs are used (http://34.9.77.60:8080)
4. ✅ Can request database access if needed

**You can:**
- Test all 194 endpoints
- See request/response formats
- Understand business logic
- Verify data structures
- Test authentication flow

---

## **Q3: Postman JSON Collection of All APIs**

### ✅ **ANSWER: YES, POSTMAN COLLECTION PROVIDED**

I've created a comprehensive Postman collection with all 194 endpoints. See file: `NPP_Contract_Management_API.postman_collection.json`

### **Collection Structure:**

```
NPP Contract Management API/
├── 01. Authentication (7 endpoints)
│   ├── Login
│   ├── Logout
│   ├── Refresh Token
│   ├── Forgot Password
│   ├── Reset Password
│   ├── Change Password
│   └── Validate Token
├── 02. Users (13 endpoints)
├── 03. Roles (5 endpoints)
├── 04. Contracts (24 endpoints)
├── 05. Proposals (11 endpoints)
├── 06. Products (15 endpoints)
├── 07. Manufacturers (5 endpoints)
├── 08. Distributors (7 endpoints)
├── 09. Industries (10 endpoints)
├── 10. OpCos (11 endpoints)
├── 11. Customer Accounts (11 endpoints)
├── 12. Member Accounts (9 endpoints)
├── 13. Velocity (11 endpoints)
├── 14. Reports (4 endpoints)
├── 15. Bulk Renewal (2 endpoints)
└── 16. Lookup (10 endpoints)
```

### **How to Use:**

1. **Import into Postman:**
   - Open Postman
   - Click "Import"
   - Select `NPP_Contract_Management_API.postman_collection.json`

2. **Set Environment Variables:**
   ```
   baseUrl: http://34.9.77.60:8081/api
   token: (will be set automatically after login)
   ```

3. **Login First:**
   - Run: `01. Authentication > Login`
   - Token will be automatically saved to environment
   - All subsequent requests will use this token

4. **Test Any Endpoint:**
   - All requests have pre-filled examples
   - All requests have proper headers
   - All requests have sample request bodies

### **Collection Features:**

✅ **All 194 endpoints included**
✅ **Pre-configured authentication**
✅ **Sample request bodies**
✅ **Environment variables**
✅ **Organized by module**
✅ **Ready to use**

### **Effort Estimation Using Postman:**

**For each endpoint, you can:**
1. See the HTTP method (GET/POST/PUT/DELETE)
2. See the URL structure
3. See request headers required
4. See request body format (JSON)
5. See response format (JSON)
6. Understand business logic from examples

**Estimated time to review:**
- 194 endpoints × 5 minutes = **16 hours** to review all endpoints
- This will give you complete understanding of the API

---

## **Q4: Spring Batch for Data Ingestion - SFTP or Import/Export?**

### ✅ **ANSWER: BOTH - But SFTP is FUTURE FEATURE (Not Implemented Yet)**

### **Current Implementation (What You MUST Build):**

#### **1. File Upload (IMPLEMENTED - HIGH PRIORITY)**

**Endpoint:** `POST /api/velocity/ingest`
**Method:** Multipart file upload
**File Types:** CSV, Excel (.xlsx, .xls)
**Max Size:** 10 MB

**How it works:**
```
User uploads file → API validates → Creates job → Parses file →
Background processing → Saves to database → Updates job status
```

**Spring Batch Equivalent:**
```java
@PostMapping("/velocity/ingest")
public ResponseEntity<VelocityIngestResponse> ingestFile(
    @RequestParam("file") MultipartFile file,
    @RequestParam("distributorId") int distributorId) {

    // 1. Validate file
    // 2. Create job record
    // 3. Parse CSV/Excel
    // 4. Queue for batch processing
    // 5. Return job ID
}

// Spring Batch Job
@Bean
public Job velocityProcessingJob() {
    return jobBuilderFactory.get("velocityProcessingJob")
        .start(parseFileStep())
        .next(validateDataStep())
        .next(saveToDbStep())
        .build();
}
```

**Processing Flow:**
1. **Upload:** User uploads CSV/Excel file
2. **Parse:** Parse file into rows (using Apache POI for Excel, OpenCSV for CSV)
3. **Validate:** Validate each row (required fields, data types, business rules)
4. **Queue:** Create job with status "queued"
5. **Background Processing:** Spring Batch processes rows in batches of 1000
6. **Save:** Save valid rows to `VelocityShipments` table
7. **Error Tracking:** Save failed rows to `VelocityJobRows` table
8. **Update Job:** Update job status to "completed" or "failed"

**Database Tables:**
- `VelocityJobs` - Job metadata (status, counts, timestamps)
- `VelocityJobRows` - Row-level processing results (success/failed)
- `VelocityShipments` - Successfully processed shipment data
- `IngestedFiles` - File metadata

**CSV Format Example:**
```csv
DistributorAccountNumber,CustomerAccountNumber,CustomerName,ShipToAddress1,ShipToAddress2,ShipToCity,ShipToZip,InvoiceNumber,InvoiceDate,ProductCode,BrandName,PackSize,ProductDescription,GTIN,UPC,ManufacturerName,Quantity,ExtendedCost,ExtendedPrice,Allowance
DIST001,CUST001,ABC Restaurant,123 Main St,Suite 100,New York,10001,INV-2024-001,2024-12-01,PROD001,Brand A,12x500ml,First Product,CORP001,1234567890123,Manufacturer A,10,500.00,450.00,25.00
```

**Spring Batch Configuration Needed:**
```java
// Chunk size: 1000 rows
@Bean
public Step processVelocityDataStep() {
    return stepBuilderFactory.get("processVelocityDataStep")
        .<VelocityRow, VelocityShipment>chunk(1000)
        .reader(velocityFileReader())
        .processor(velocityRowProcessor())
        .writer(velocityShipmentWriter())
        .faultTolerant()
        .skip(ValidationException.class)
        .skipLimit(Integer.MAX_VALUE)
        .listener(velocityJobListener())
        .build();
}
```

---

#### **2. SFTP Integration (PLANNED - LOW PRIORITY)**

**Status:** ⚠️ **NOT IMPLEMENTED** - Marked as "Phase 2" in codebase

**Code Evidence:**
```csharp
// From VelocityService.cs line 133-138
public async Task<VelocityIngestResponse> IngestFromSftpAsync(
    string sftpFileUrl, string createdBy, int distributorId,
    Dictionary<string, string>? jobMeta = null)
{
    // For now, return not implemented
    // In production, this would download from sFTP and call IngestFromFileAsync
    throw new NotImplementedException("sFTP ingestion will be implemented in phase 2");
}
```

**Database Table Exists (But Not Used):**
- `SftpProbeConfigs` - SFTP server configurations (host, port, username, password, remote path)

**Intended Design (For Future):**
```
Scheduled Job (Cron/Quartz) → Connect to SFTP →
Download files → Call IngestFromFileAsync → Process like uploaded files
```

**For Java Migration:**

**Option 1: Skip SFTP (Recommended)**
- Only implement file upload functionality
- SFTP can be added later as enhancement
- Reduces scope and timeline

**Option 2: Implement SFTP (If Required)**
- Use Spring Integration SFTP
- Add scheduled job (Spring @Scheduled or Quartz)
- Download files from SFTP server
- Process same as uploaded files

**Spring Integration SFTP Example:**
```java
@Bean
public IntegrationFlow sftpInboundFlow() {
    return IntegrationFlows
        .from(Sftp.inboundAdapter(sftpSessionFactory())
            .remoteDirectory("/velocity/incoming")
            .localDirectory(new File("/tmp/velocity"))
            .autoCreateLocalDirectory(true),
            e -> e.poller(Pollers.fixedDelay(60000))) // Every 1 minute
        .handle(message -> {
            File file = (File) message.getPayload();
            velocityService.ingestFromFile(file, distributorId);
        })
        .get();
}
```

---

### **Data Ingestion Summary:**

| Feature | Status | Priority | Effort |
|---------|--------|----------|--------|
| **File Upload (CSV)** | ✅ Implemented | HIGH | 5 days |
| **File Upload (Excel)** | ✅ Implemented | HIGH | 3 days |
| **Background Processing** | ✅ Implemented | HIGH | 5 days |
| **Batch Processing (1000 rows)** | ✅ Implemented | HIGH | 3 days |
| **Error Tracking** | ✅ Implemented | HIGH | 2 days |
| **Job Status Tracking** | ✅ Implemented | HIGH | 2 days |
| **SFTP Download** | ❌ Not Implemented | LOW | 5 days |
| **SFTP Scheduling** | ❌ Not Implemented | LOW | 3 days |
| **SFTP Configuration UI** | ❌ Not Implemented | LOW | 2 days |

**Total Effort (Without SFTP):** 20 days
**Total Effort (With SFTP):** 30 days

---

### **What You Need to Read:**

**NO SFTP server access needed!** The data ingestion is:

1. ✅ **User uploads file** via web UI
2. ✅ **API receives file** (multipart/form-data)
3. ✅ **Parse file** (CSV or Excel)
4. ✅ **Process in background** (Spring Batch)
5. ✅ **Save to database** (MySQL)
6. ✅ **Track job status** (queued → processing → completed/failed)

**NOT:**
- ❌ Reading from SFTP server
- ❌ Scheduled file downloads
- ❌ Remote server connections

**Exception:** If client specifically requests SFTP in future, it can be added as enhancement.

---

### **Reports Functionality:**

**Endpoint:** `POST /api/velocity/usage-report`

**Purpose:** Generate usage reports from processed velocity data

**How it works:**
```
Query VelocityShipments table →
Aggregate by product/customer/date →
Calculate totals →
Return report data →
Export to Excel (optional)
```

**NOT batch processing** - just SQL queries and aggregation.

**Spring equivalent:**
```java
@PostMapping("/velocity/usage-report")
public VelocityUsageReportResponse generateReport(
    @RequestBody VelocityUsageReportRequest request) {

    // Query database
    List<VelocityShipment> shipments = repository.findByDateRange(
        request.getStartDate(),
        request.getEndDate()
    );

    // Aggregate data
    Map<String, UsageAggregate> aggregates = shipments.stream()
        .collect(Collectors.groupingBy(
            s -> s.getProductCode(),
            Collectors.summarizingDouble(VelocityShipment::getQuantity)
        ));

    // Return report
    return new VelocityUsageReportResponse(aggregates);
}
```

---

## 📊 **SUMMARY FOR JAVA TEAM**

### **Architecture:**
✅ **Single monolithic application** (NOT microservices)
✅ **All 194 endpoints in one Spring Boot app**
✅ **Velocity module is part of the same API**

### **Testing:**
✅ **Live API available:** `http://34.9.77.60:8081`
✅ **Swagger documentation:** `http://34.9.77.60:8081/swagger`
✅ **Can test all endpoints immediately**

### **Postman Collection:**
✅ **Complete collection provided** (194 endpoints)
✅ **Ready to import and test**
✅ **Use for effort estimation**

### **Data Ingestion:**
✅ **File upload ONLY** (no SFTP required)
✅ **Spring Batch for background processing**
✅ **CSV and Excel parsing**
✅ **Batch size: 1000 rows**
✅ **Error tracking and job status**
❌ **SFTP is future feature** (can skip for now)

---

## 📞 **NEXT STEPS**

1. ✅ Review Postman collection
2. ✅ Test live API endpoints
3. ✅ Analyze request/response formats
4. ✅ Provide detailed estimate
5. ✅ Confirm SFTP requirement (or skip it)

---

**Document Created:** 2026-01-06
**Version:** 1.0
**Contact:** [Your contact information]
