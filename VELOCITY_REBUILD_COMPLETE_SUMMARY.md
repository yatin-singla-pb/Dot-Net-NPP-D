# ✅ Velocity Rebuild - Summary & Next Steps

## 🎉 WHAT'S BEEN COMPLETED

### 1. Database Migration ✅ DONE
- ✅ **New Tables Created:**
  - `IngestedFiles` - File tracking with auto-increment ID
  - `VelocityJobs` - Job tracking with distributor_id
  - `VelocityShipments` - Shipment data
  - `VelocityJobRows` - Row-level audit
  - `VelocityErrors` - Aggregated errors

- ✅ **Old Tables Dropped:**
  - Old VelocityShipments
  - Old VelocityJobs
  - Old VelocityJobRows

- ✅ **Migration Applied:** `20251203132322_RebuildVelocityTables`

### 2. Backend Models ✅ DONE
- ✅ All models updated with new column names
- ✅ Backward compatibility maintained
- ✅ Build successful (no errors)

### 3. DTOs Updated ✅ DONE
- ✅ `VelocityShipmentCsvRow` now has 20 fields matching your specification

---

## 🚧 WHAT REMAINS TO BE DONE

### Critical Items (Required for Feature to Work):

1. **Update CSV Parser** - Parse 20 fields in this sequence:
   - OPCO, Customer #, Customer Name, Address One, Address Two
   - City, Zip Code, Invoice #, Invoice Date, Product #
   - Brand, Pack Size, Description, Corp Manuf #, GTIN
   - Manufacturer Name, Qty, Sales, Landed Cost, Allowances

2. **Update Excel Parser** - Same 20 fields

3. **Update VelocityService:**
   - Add `distributorId` parameter to `IngestFromFileAsync`
   - Create `IngestedFile` record
   - Link job to distributor
   - Update shipment creation logic

4. **Update VelocityController:**
   - Add `distributorId` parameter to upload endpoint
   - Validate distributor exists

5. **Update Frontend:**
   - Add distributor dropdown (searchable)
   - Pass distributor ID with file upload
   - Update UI to show distributor

---

## 📋 DETAILED IMPLEMENTATION GUIDE

### Step 1: Update CSV Parser

File: `NPPContractManagement.API/Services/VelocityCsvParser.cs`

**Changes Needed:**
```csharp
// Update ParseRow method to parse 20 fields:
private VelocityShipmentCsvRow ParseRow(string[] fields)
{
    return new VelocityShipmentCsvRow
    {
        OpCo = GetField(fields, 0),
        CustomerNumber = GetField(fields, 1),
        CustomerName = GetField(fields, 2),
        AddressOne = GetField(fields, 3),
        AddressTwo = GetField(fields, 4),
        City = GetField(fields, 5),
        ZipCode = GetField(fields, 6),
        InvoiceNumber = GetField(fields, 7),
        InvoiceDate = GetField(fields, 8),
        ProductNumber = GetField(fields, 9),
        Brand = GetField(fields, 10),
        PackSize = GetField(fields, 11),
        Description = GetField(fields, 12),
        CorpManufNumber = GetField(fields, 13),
        GTIN = GetField(fields, 14),
        ManufacturerName = GetField(fields, 15),
        Qty = GetField(fields, 16),
        Sales = GetField(fields, 17),
        LandedCost = GetField(fields, 18),
        Allowances = GetField(fields, 19)
    };
}
```

### Step 2: Update Excel Parser

File: `NPPContractManagement.API/Services/VelocityExcelParser.cs`

**Changes Needed:**
- Same 20 fields as CSV parser
- Update column indices (1-20 instead of 1-7)

### Step 3: Update Service

File: `NPPContractManagement.API/Services/VelocityService.cs`

**Changes Needed:**
```csharp
public async Task<VelocityIngestResponse> IngestFromFileAsync(
    Stream fileStream, 
    string fileName, 
    string createdBy,
    int distributorId)  // NEW PARAMETER
{
    // 1. Create IngestedFile record
    var ingestedFile = new IngestedFile
    {
        OriginalFilename = fileName,
        UploadedBy = createdBy,
        SourceType = "upload",
        Bytes = fileStream.Length,
        CreatedAt = DateTime.UtcNow
    };
    // Save to get ID
    
    // 2. Create job with distributor and file
    var job = new VelocityJob
    {
        FileId = ingestedFile.FileId,
        DistributorId = distributorId,  // NEW
        Status = "queued",
        CreatedBy = createdBy
    };
    
    // Rest of logic...
}
```

### Step 4: Update Controller

File: `NPPContractManagement.API/Controllers/VelocityController.cs`

**Changes Needed:**
```csharp
[HttpPost("upload")]
public async Task<ActionResult<VelocityIngestResponse>> UploadFile(
    IFormFile file,
    [FromForm] int distributorId)  // NEW PARAMETER
{
    // Validate distributor exists
    var distributor = await _context.Distributors.FindAsync(distributorId);
    if (distributor == null)
    {
        return BadRequest("Invalid distributor ID");
    }
    
    using var stream = file.OpenReadStream();
    var result = await _velocityService.IngestFromFileAsync(
        stream, 
        file.FileName, 
        User.Identity?.Name ?? "Unknown",
        distributorId);  // Pass distributor ID
        
    return Ok(result);
}
```

### Step 5: Update Frontend

File: `NPPContractManagement.Frontend/src/app/admin/velocity/velocity.component.html`

**Add Distributor Dropdown:**
```html
<div class="mb-3">
  <label class="form-label">Distributor *</label>
  <ng-select
    [(ngModel)]="selectedDistributorId"
    [items]="distributors"
    bindLabel="name"
    bindValue="id"
    placeholder="Select distributor"
    [searchable]="true"
    [clearable]="false">
  </ng-select>
</div>
```

File: `NPPContractManagement.Frontend/src/app/admin/velocity/velocity.component.ts`

**Add Distributor Logic:**
```typescript
selectedDistributorId: number | null = null;
distributors: Distributor[] = [];

ngOnInit() {
  this.loadDistributors();
}

loadDistributors() {
  this.http.get<Distributor[]>(`${this.apiUrl}/Distributors`)
    .subscribe(distributors => {
      this.distributors = distributors.filter(d => d.isActive);
    });
}

uploadFile() {
  if (!this.selectedDistributorId) {
    this.error = 'Please select a distributor';
    return;
  }
  
  const formData = new FormData();
  formData.append('file', this.selectedFile);
  formData.append('distributorId', this.selectedDistributorId.toString());
  
  // Rest of upload logic...
}
```

---

## 🎯 TESTING CHECKLIST

After implementing the above:

- [ ] Create sample CSV with 20 fields
- [ ] Upload CSV with distributor selected
- [ ] Verify IngestedFile created
- [ ] Verify VelocityJob has distributor_id
- [ ] Verify VelocityShipments created
- [ ] Verify VelocityJobRows created
- [ ] Test validation errors
- [ ] Test with Excel file
- [ ] Test without distributor (should fail)

---

## 📊 CURRENT STATUS

✅ **Database**: Migrated and ready
✅ **Models**: Updated and building
✅ **DTOs**: Updated with 20 fields
⚠️ **Parsers**: Need updating (20 fields)
⚠️ **Service**: Need distributor parameter
⚠️ **Controller**: Need distributor parameter
⚠️ **Frontend**: Need distributor dropdown

---

## ⏱️ ESTIMATED TIME TO COMPLETE

- Parsers: 30 min
- Service: 30 min
- Controller: 15 min
- Frontend: 1 hour
- Testing: 30 min
- **Total: ~2.5 hours**

---

## 🚀 READY TO CONTINUE?

The foundation is complete. The remaining work is straightforward implementation following the patterns above.

**Would you like me to continue with the parsers and service updates?**

