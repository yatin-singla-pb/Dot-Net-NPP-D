# Video Script 6: Velocity Integration APIs

## SLIDE 1: Velocity Overview (1 minute)

**[Show VelocityController in Swagger]**

Welcome to Video 6, covering Velocity Integration.

Velocity is the shipment tracking system that records actual product deliveries to customers.

**Purpose:**
- Import shipment data from CSV files
- Match shipments to contracts
- Compare actual prices vs contracted prices
- Identify pricing discrepancies
- Generate velocity reports

**Data Flow:**
1. CSV file uploaded or fetched from SFTP
2. File parsed and validated
3. Shipments matched to contracts
4. Results stored for reporting

All velocity endpoints are at `/api/Velocity`.

---

## SLIDE 2: Ingest Velocity Data (1 minute 30 seconds)

**[Show POST /api/Velocity/ingest in Swagger]**

**Endpoint:** `POST /api/Velocity/ingest`

**Purpose:** Upload and process a velocity CSV file

**Request:** Multipart form data with CSV file

**CSV Format (20 columns):**
- OpCo, Customer Number, Customer Name
- Address One, Address Two, City, Zip Code
- Invoice Number, Invoice Date
- Product Number, Brand, Pack Size, Description
- Corp Manuf Number, GTIN, Manufacturer Name
- Qty, Sales, Landed Cost, Allowances

**Response (200 OK):**
```json
{
  "success": true,
  "jobId": "VEL-20241209-001",
  "message": "File uploaded successfully. Processing started.",
  "fileName": "velocity_data_20241209.csv",
  "fileSize": 2048576,
  "estimatedRows": 5000
}
```

**Processing:**
The file is processed asynchronously in the background.
Use the jobId to check status.

---

## SLIDE 3: Get Velocity Jobs (1 minute)

**[Show GET /api/Velocity/jobs in Swagger]**

**Endpoint:** `GET /api/Velocity/jobs`

**Purpose:** Get list of all velocity import jobs

**Query Parameters:**
- `page` (default: 1)
- `pageSize` (default: 10)
- `status` (Pending, Processing, Completed, Failed)

**Response (200 OK):**
```json
{
  "items": [
    {
      "jobId": "VEL-20241209-001",
      "fileName": "velocity_data_20241209.csv",
      "status": "Completed",
      "totalRows": 5000,
      "validRows": 4850,
      "invalidRows": 150,
      "matchedShipments": 4500,
      "unmatchedShipments": 350,
      "startedAt": "2024-12-09T10:00:00Z",
      "completedAt": "2024-12-09T10:15:00Z",
      "uploadedBy": "admin"
    }
  ],
  "totalCount": 25,
  "page": 1,
  "pageSize": 10
}
```

---

## SLIDE 4: Get Job Details (1 minute 30 seconds)

**[Show GET /api/Velocity/jobs/{jobId} in Swagger]**

**Endpoint:** `GET /api/Velocity/jobs/{jobId}`

**Purpose:** Get detailed information about a specific job

**Response (200 OK):**
```json
{
  "jobId": "VEL-20241209-001",
  "fileName": "velocity_data_20241209.csv",
  "status": "Completed",
  "totalRows": 5000,
  "validRows": 4850,
  "invalidRows": 150,
  "matchedShipments": 4500,
  "unmatchedShipments": 350,
  "startedAt": "2024-12-09T10:00:00Z",
  "completedAt": "2024-12-09T10:15:00Z",
  "uploadedBy": "admin",
  "errors": [
    {
      "rowNumber": 10,
      "error": "Invalid GTIN format",
      "rawData": "OpCo1,12345,..."
    },
    {
      "rowNumber": 25,
      "error": "Missing required field: Product Number",
      "rawData": "OpCo2,67890,..."
    }
  ],
  "summary": {
    "totalSales": 125000.50,
    "totalQty": 10000,
    "uniqueProducts": 250,
    "uniqueCustomers": 150
  }
}
```

---

## SLIDE 5: Get Velocity Shipments (1 minute)

**[Show GET /api/Velocity/shipments in Swagger]**

**Endpoint:** `GET /api/Velocity/shipments`

**Purpose:** Get list of processed shipments

**Query Parameters:**
- `page`, `pageSize`
- `jobId` (filter by import job)
- `contractId` (filter by contract)
- `productId` (filter by product)
- `startDate`, `endDate` (date range)
- `matched` (true/false - filter by match status)

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": 1000,
      "jobId": "VEL-20241209-001",
      "opCo": "OpCo East",
      "customerNumber": "12345",
      "customerName": "ABC Hospital",
      "invoiceNumber": "INV-001",
      "invoiceDate": "2024-12-01",
      "productNumber": "TYS-12345",
      "gtin": "00012345678905",
      "manufacturerName": "Tyson Foods",
      "qty": 100,
      "sales": 275.00,
      "landedCost": 250.00,
      "allowances": 25.00,
      "contractId": 1,
      "contractName": "Tyson Foods 2024 Contract",
      "contractPrice": 2.50,
      "actualPrice": 2.75,
      "priceDifference": 0.25,
      "matched": true
    }
  ],
  "totalCount": 4500,
  "page": 1,
  "pageSize": 10
}
```

---

## SLIDE 6: Get Unmatched Shipments (1 minute)

**[Show GET /api/Velocity/shipments/unmatched in Swagger]**

**Endpoint:** `GET /api/Velocity/shipments/unmatched`

**Purpose:** Get shipments that couldn't be matched to contracts

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": 1500,
      "jobId": "VEL-20241209-001",
      "productNumber": "UNK-999",
      "gtin": "00099999999999",
      "manufacturerName": "Unknown Manufacturer",
      "qty": 50,
      "sales": 100.00,
      "reason": "Product not found in catalog",
      "invoiceDate": "2024-12-01"
    },
    {
      "id": 1501,
      "productNumber": "TYS-99999",
      "gtin": "00012345999999",
      "manufacturerName": "Tyson Foods",
      "qty": 25,
      "sales": 75.00,
      "reason": "No active contract for this product",
      "invoiceDate": "2024-12-02"
    }
  ],
  "totalCount": 350
}
```

**Use Case:**
Review unmatched shipments to identify:
- Missing products in catalog
- Products without contracts
- Data quality issues

---

## SLIDE 7: Retry Failed Job (45 seconds)

**[Show POST /api/Velocity/jobs/{jobId}/retry in Swagger]**

**Endpoint:** `POST /api/Velocity/jobs/{jobId}/retry`

**Purpose:** Retry a failed velocity import job

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Job queued for retry",
  "jobId": "VEL-20241209-001",
  "newStatus": "Pending"
}
```

**Use Case:**
If a job fails due to temporary issues (database timeout, etc.), retry it without re-uploading the file.

---

## SLIDE 8: Download CSV Template (30 seconds)

**[Show GET /api/Velocity/template in Swagger]**

**Endpoint:** `GET /api/Velocity/template`

**Purpose:** Download a sample CSV template

**Response:** CSV file with headers and sample data

**Template Headers:**
```
OpCo,Customer Number,Customer Name,Address One,Address Two,City,Zip Code,
Invoice Number,Invoice Date,Product Number,Brand,Pack Size,Description,
Corp Manuf Number,GTIN,Manufacturer Name,Qty,Sales,Landed Cost,Allowances
```

---

## SLIDE 9: SFTP Configuration (1 minute)

**[Show SFTP Endpoints in Swagger]**

**Get SFTP Configs**

**Endpoint:** `GET /api/Velocity/sftp-configs`

**Purpose:** Get list of SFTP server configurations

**Response:**
```json
[
  {
    "id": 1,
    "name": "Production SFTP",
    "host": "sftp.example.com",
    "port": 22,
    "username": "npp_user",
    "remotePath": "/velocity/incoming",
    "isActive": true,
    "lastProbeDate": "2024-12-09T06:00:00Z"
  }
]
```

---

**Create/Update SFTP Config**

**Endpoint:** `POST /api/Velocity/sftp-configs`

**Request Body:**
```json
{
  "name": "Production SFTP",
  "host": "sftp.example.com",
  "port": 22,
  "username": "npp_user",
  "password": "encrypted_password",
  "remotePath": "/velocity/incoming",
  "filePattern": "*.csv",
  "isActive": true
}
```

---

## SLIDE 10: Velocity Reports (1 minute)

**[Show Report Endpoints in Swagger]**

**Get Velocity Summary Report**

**Endpoint:** `GET /api/Velocity/reports/summary`

**Query Parameters:**
- `startDate`, `endDate`
- `manufacturerId`
- `contractId`

**Response:**
```json
{
  "period": "2024-12-01 to 2024-12-31",
  "totalShipments": 10000,
  "totalSales": 500000.00,
  "totalQty": 50000,
  "matchedShipments": 9500,
  "unmatchedShipments": 500,
  "matchRate": 95.0,
  "averagePriceDifference": 0.15,
  "topProducts": [
    {
      "productId": 50,
      "productName": "Chicken Breast",
      "shipments": 500,
      "sales": 25000.00
    }
  ]
}
```

---

**Export to Excel**

**Endpoint:** `GET /api/Velocity/reports/export`

**Response:** Excel file with detailed velocity data

---

## SLIDE 11: Summary (30 seconds)

**[Show Summary Slide]**

We've covered Velocity Integration APIs:

✅ **Data Ingestion** - Upload CSV files  
✅ **Job Management** - Track import jobs  
✅ **Shipment Tracking** - View matched/unmatched shipments  
✅ **SFTP Integration** - Automated file retrieval  
✅ **Reporting** - Velocity analysis and exports  

**Key Points:**
- Asynchronous processing for large files
- Automatic contract matching
- Identifies pricing discrepancies
- SFTP support for automation

In the next video, we'll cover Reporting and Lookup APIs.

---

**[TOTAL TIME: ~11 minutes]**

