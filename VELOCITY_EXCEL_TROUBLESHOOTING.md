# Velocity Excel Upload - Troubleshooting Guide

## 🔧 Recent Fixes Applied

### Issue: Excel Upload Error
**Symptom**: "An error occurred while processing the file" when uploading Excel files

### Fixes Applied:

1. ✅ **EPPlus License Context Set Globally**
   - Added `ExcelPackage.LicenseContext = LicenseContext.NonCommercial;` in `Program.cs`
   - This must be set before any EPPlus operations

2. ✅ **Stream Position Reset**
   - Added stream position reset before parsing: `fileStream.Position = 0;`
   - Ensures stream is at the beginning for Excel parsing

3. ✅ **Enhanced Error Handling**
   - Added try-catch block in `VelocityService.IngestFromFileAsync`
   - Logs detailed error messages
   - Updates job status to Failed with error message

4. ✅ **API Restart Required**
   - The API needs to be restarted to pick up the new code changes
   - Process ID: 10168 (currently running)

---

## 🚀 How to Test After Restart

### Step 1: Restart the API
The API is currently running but needs to be restarted to pick up the changes.

**Option A: Manual Restart**
```powershell
# Stop the current API process
Stop-Process -Id 10168 -Force

# Start the API
cd E:\TestAIFixed\NPPContractManagement.API
dotnet run
```

**Option B: Let it auto-restart**
If you're using a development tool that watches for file changes, it should restart automatically.

### Step 2: Test Excel Upload
1. Navigate to **Reporting → Velocity**
2. Upload `sample_velocity_data.xlsx`
3. Check the results

### Step 3: Check Logs if Error Occurs
If you still get an error, check the API console output for detailed error messages.

---

## 🔍 Common Issues and Solutions

### Issue 1: "Excel file contains no worksheets"
**Cause**: The Excel file is corrupted or empty
**Solution**: Regenerate the Excel file using `create_sample_excel.ps1`

### Issue 2: "Excel worksheet is empty"
**Cause**: The first worksheet has no data
**Solution**: Ensure the first worksheet has headers in row 1 and data starting from row 2

### Issue 3: "Missing required columns"
**Cause**: Excel file doesn't have all required column headers
**Solution**: Ensure these columns exist in row 1:
- distributor_id
- shipment_id
- sku
- quantity
- shipped_at
- origin
- destination

### Issue 4: Stream/Memory errors
**Cause**: File stream not properly positioned or closed
**Solution**: The code now resets stream position before parsing

### Issue 5: License context error
**Cause**: EPPlus license context not set
**Solution**: Now set globally in Program.cs

---

## 📋 Verification Checklist

Before testing, verify:

- [ ] API is restarted with latest code
- [ ] EPPlus package is installed (v7.5.0)
- [ ] `sample_velocity_data.xlsx` exists in root directory
- [ ] Excel file has correct structure (headers in row 1, data from row 2)
- [ ] Distributor IDs in Excel file exist in database (1, 2, 3)

---

## 🧪 Test Cases

### Test Case 1: Valid Excel File
**File**: `sample_velocity_data.xlsx`
**Expected**: 
- Job created successfully
- 5 rows processed
- 5 success (if distributors exist)
- 0 failed

### Test Case 2: Valid CSV File
**File**: `sample_velocity_data.csv`
**Expected**:
- Job created successfully
- 5 rows processed
- 5 success (if distributors exist)
- 0 failed

### Test Case 3: Invalid File Type
**File**: `test.txt`
**Expected**: Error message "Only CSV (.csv) and Excel (.xlsx, .xls) files are allowed"

### Test Case 4: File Too Large
**File**: > 10 MB
**Expected**: Error message "File size exceeds maximum of 10 MB"

---

## 📊 Expected Behavior

### Successful Upload Flow:
1. User selects Excel file
2. Frontend validates file type and size
3. File uploaded to API
4. API detects file type (.xlsx)
5. Routes to Excel parser
6. Excel parser reads first worksheet
7. Validates headers
8. Parses each row
9. Validates each row
10. Creates shipment records
11. Updates job status
12. Returns success response

### Error Flow:
1. User selects Excel file
2. Upload starts
3. Error occurs during parsing
4. Error logged to console
5. Job status set to "Failed"
6. Error message stored in job
7. User sees error message

---

## 🔧 Debug Mode

To see detailed logs, check the API console output. You should see:
```
info: NPPContractManagement.API.Services.VelocityService[0]
      Created velocity job abc123-def456 for file sample_velocity_data.xlsx
info: NPPContractManagement.API.Services.VelocityService[0]
      Parsing Excel file sample_velocity_data.xlsx
```

If there's an error, you'll see:
```
fail: NPPContractManagement.API.Services.VelocityService[0]
      Error parsing file sample_velocity_data.xlsx for job abc123-def456
      System.Exception: [Error details here]
```

---

## 🎯 Next Steps

1. **Restart the API** to pick up the code changes
2. **Test Excel upload** with `sample_velocity_data.xlsx`
3. **Check API logs** if error occurs
4. **Verify distributors exist** in database (IDs: 1, 2, 3)

---

## 💡 Additional Notes

- The Excel parser uses EPPlus library (NonCommercial license)
- Only the first worksheet is processed
- Empty rows are automatically skipped
- Excel dates are automatically converted to ISO8601 format
- The same validation rules apply to both CSV and Excel files

---

## 📞 If Issues Persist

If you continue to see errors after restarting the API:

1. Check the API console for the full error stack trace
2. Verify the Excel file opens correctly in Microsoft Excel
3. Try uploading the CSV file instead to verify the core functionality works
4. Check that all required NuGet packages are installed
5. Ensure the database connection is working

The error message in the API logs will provide specific details about what went wrong.

