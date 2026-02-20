# Velocity Reporting Feature - Implementation Guide

## Overview
Complete feature for consuming and logging distributor shipment (velocity) data with CSV upload, sFTP integration, job processing, and cloud scheduling.

## Database Schema

### Tables Created
1. **VelocityShipments** - Stores processed shipment records
2. **VelocityJobs** - Tracks import job metadata
3. **VelocityJobRows** - Stores row-level processing results
4. **SftpProbeConfigs** - Stores sFTP connection configurations

### Migration Command
```bash
dotnet ef migrations add AddVelocityTables --project NPPContractManagement.API
dotnet ef database update --project NPPContractManagement.API
```

## API Endpoints

### 1. POST /api/velocity/ingest
**Purpose**: Ingest velocity data from file upload or sFTP

**Request (Multipart File Upload)**:
```http
POST /api/velocity/ingest
Content-Type: multipart/form-data
Authorization: Bearer <token>

file: [CSV file]
```

**Request (JSON with sFTP URL)**:
```http
POST /api/velocity/ingest
Content-Type: application/json
Authorization: Bearer <token>

{
  "sftpFileUrl": "sftp://host/path/to/file.csv",
  "jobMeta": {
    "source": "scheduled_probe",
    "probeConfigId": "1"
  }
}
```

**Response**:
```json
{
  "jobId": "abc123-def456-ghi789",
  "status": "queued",
  "detailsUrl": "/api/velocity/jobs/abc123-def456-ghi789",
  "message": "Job queued for processing"
}
```

### 2. GET /api/velocity/jobs/{jobId}
**Purpose**: Get job details and processing results

**Response**:
```json
{
  "id": 1,
  "jobId": "abc123-def456-ghi789",
  "status": "completed",
  "fileName": "shipments_2024-12-02.csv",
  "totalRows": 100,
  "processedRows": 100,
  "successRows": 95,
  "failedRows": 5,
  "createdAt": "2024-12-02T10:00:00Z",
  "startedAt": "2024-12-02T10:00:05Z",
  "completedAt": "2024-12-02T10:00:30Z",
  "createdBy": "scheduler@system",
  "detailsUrl": "/api/velocity/jobs/abc123-def456-ghi789",
  "rows": [
    {
      "rowIndex": 2,
      "status": "failed",
      "errorMessage": "Row 2: quantity must be an integer greater than 0",
      "rawData": "{\"distributor_id\":\"1\",\"shipment_id\":\"SH001\",\"sku\":\"SKU123\",\"quantity\":\"-5\",\"shipped_at\":\"2024-12-01T10:00:00Z\",\"origin\":\"Warehouse A\",\"destination\":\"Store B\"}"
    }
  ]
}
```

### 3. GET /api/velocity/jobs
**Purpose**: List recent jobs with pagination

**Query Parameters**:
- `page` (default: 1)
- `pageSize` (default: 20)
- `status` (optional): queued, processing, completed, failed, partial_success

### 4. GET /api/velocity/template
**Purpose**: Download sample CSV template

**Response**: CSV file with headers and example rows

### 5. POST /api/velocity/sftp-configs
**Purpose**: Create sFTP probe configuration

### 6. GET /api/velocity/sftp-configs
**Purpose**: List sFTP probe configurations

### 7. PUT /api/velocity/sftp-configs/{id}
**Purpose**: Update sFTP probe configuration

### 8. POST /api/velocity/sftp-configs/{id}/test
**Purpose**: Test sFTP connection

### 9. POST /api/velocity/sftp-configs/{id}/probe
**Purpose**: Manually trigger sFTP probe

## CSV Format

### Required Columns
- `distributor_id` - Integer, required
- `shipment_id` - String, required, max 100 chars
- `sku` - String, required, max 100 chars
- `quantity` - Integer > 0, required
- `shipped_at` - ISO8601 datetime, required
- `origin` - String, optional, max 200 chars
- `destination` - String, optional, max 200 chars

### Sample CSV
```csv
distributor_id,shipment_id,sku,quantity,shipped_at,origin,destination
1,SH001,SKU123,50,2024-12-01T10:00:00Z,Warehouse A,Store B
1,SH002,SKU456,25,2024-12-01T11:30:00Z,Warehouse A,Store C
2,SH003,SKU789,100,2024-12-01T14:00:00Z,Warehouse B,Store D
```

### Validation Rules
1. **distributor_id**: Must be valid integer
2. **shipment_id**: Required, non-empty
3. **sku**: Required, non-empty
4. **quantity**: Must be integer > 0
5. **shipped_at**: Must parse as ISO8601 datetime
6. **origin/destination**: Optional

### Error Handling
- Invalid rows are logged with specific error messages
- Processing continues for remaining rows (partial success allowed)
- Job status becomes "partial_success" if some rows fail
- Job status becomes "failed" if all rows fail or critical error occurs

## Cloud Scheduling

### AWS EventBridge + Lambda
```json
{
  "schedule": "rate(1 hour)",
  "target": {
    "arn": "arn:aws:lambda:region:account:function:velocity-ingest",
    "input": {
      "url": "https://api.example.com/api/velocity/ingest",
      "method": "POST",
      "headers": {
        "Authorization": "Bearer YOUR_STATIC_SECRET_TOKEN",
        "Content-Type": "application/json"
      },
      "body": {
        "sftpFileUrl": "probe://config/1",
        "jobMeta": {
          "source": "aws_eventbridge",
          "schedule": "hourly"
        }
      }
    }
  }
}
```

### Azure Function Timer Trigger
```csharp
[FunctionName("VelocityIngestScheduler")]
public static async Task Run(
    [TimerTrigger("0 0 * * * *")] TimerInfo myTimer, // Every hour
    ILogger log)
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", "YOUR_STATIC_SECRET_TOKEN");
    
    var response = await client.PostAsJsonAsync(
        "https://api.example.com/api/velocity/ingest",
        new { sftpFileUrl = "probe://config/1" });
}
```

### Cron Job (Linux)
```bash
# /etc/cron.d/velocity-ingest
0 * * * * curl -X POST https://api.example.com/api/velocity/ingest \
  -H "Authorization: Bearer YOUR_STATIC_SECRET_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"sftpFileUrl":"probe://config/1"}'
```

## Security

### Authentication
- All endpoints require `Authorization: Bearer <token>` header
- Returns 401 if missing or invalid
- Scheduler uses static secret token (configured in appsettings.json)

### File Upload Limits
- Maximum file size: 10 MB
- Allowed content types: text/csv, application/csv
- Filename sanitization to prevent directory traversal

### sFTP Credentials
- Passwords and private keys encrypted at rest
- Never returned in API responses
- Stored using ASP.NET Core Data Protection

## Logging

### Structured JSON Logs
```json
{
  "timestamp": "2024-12-02T10:00:30Z",
  "level": "Information",
  "jobId": "abc123",
  "rowIndex": 5,
  "status": "success",
  "message": "Row processed successfully",
  "distributorId": 1,
  "shipmentId": "SH005"
}
```

### Error Logs
```json
{
  "timestamp": "2024-12-02T10:00:35Z",
  "level": "Error",
  "jobId": "abc123",
  "rowIndex": 10,
  "status": "failed",
  "error": "quantity must be an integer greater than 0",
  "rawData": "{...}"
}
```

## Next Steps

1. Run database migration
2. Configure sFTP credentials (if using sFTP)
3. Set up cloud scheduler
4. Test with sample CSV
5. Monitor job processing logs
6. Set up alerts for failed jobs

