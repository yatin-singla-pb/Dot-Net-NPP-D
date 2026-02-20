# 🧪 Velocity Rebuild - Testing Instructions

## ✅ Pre-Testing Checklist

- [x] Database migration applied: `20251203132322_RebuildVelocityTables`
- [x] Backend builds successfully (no errors)
- [x] Frontend compiles successfully (no errors)
- [x] Sample CSV file created: `sample_velocity_data_new_format.csv`

---

## 🚀 Step-by-Step Testing Guide

### Step 1: Start the Backend

```bash
cd NPPContractManagement.API
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
      Now listening on: http://localhost:5001
```

---

### Step 2: Start the Frontend

```bash
cd NPPContractManagement.Frontend
npm start
```

**Expected Output:**
```
** Angular Live Development Server is listening on localhost:4200 **
✔ Compiled successfully.
```

---

### Step 3: Login to the Application

1. Navigate to `http://localhost:4200`
2. Login with admin credentials
3. Navigate to **Reporting → Velocity**

---

### Step 4: Test Distributor Dropdown

**Test Case 1: Load Distributors**
- ✅ Distributor dropdown should be visible
- ✅ Click on the distributor input field
- ✅ Dropdown should show list of active distributors
- ✅ Type to search by name or code
- ✅ Filtered list should update

**Test Case 2: Select Distributor**
- ✅ Click on a distributor from the list
- ✅ Distributor name should appear in the input field
- ✅ Dropdown should close
- ✅ Clear button (X) should appear

**Test Case 3: Clear Distributor**
- ✅ Click the clear button (X)
- ✅ Input field should clear
- ✅ Dropdown should show all distributors again

---

### Step 5: Test File Upload

**Test Case 1: Upload Without Distributor**
- ✅ Select a CSV file
- ✅ Do NOT select a distributor
- ✅ "Import Data" button should be DISABLED
- ✅ Error message should appear: "Please select a distributor"

**Test Case 2: Upload With Distributor**
- ✅ Select a distributor
- ✅ Select the file: `sample_velocity_data_new_format.csv`
- ✅ Preview should show first 10 rows (CSV only)
- ✅ "Import Data" button should be ENABLED
- ✅ Click "Import Data"
- ✅ Upload progress should show
- ✅ Success message should appear
- ✅ Job should appear in "Recent Jobs" list

---

### Step 6: Verify Database Records

**Check IngestedFiles Table:**
```sql
SELECT * FROM IngestedFiles ORDER BY created_at DESC;
```

**Expected:**
- ✅ New record with `original_filename` = "sample_velocity_data_new_format.csv"
- ✅ `uploaded_by` = your username
- ✅ `source_type` = "upload"
- ✅ `bytes` > 0

**Check VelocityJobs Table:**
```sql
SELECT * FROM VelocityJobs ORDER BY created_at DESC;
```

**Expected:**
- ✅ New record with `file_id` matching IngestedFiles
- ✅ `distributor_id` = selected distributor ID
- ✅ `status` = "completed" or "completed_with_errors"
- ✅ `totals` JSON contains row counts

**Check VelocityShipments Table:**
```sql
SELECT * FROM VelocityShipments WHERE job_id = (SELECT TOP 1 job_id FROM VelocityJobs ORDER BY created_at DESC);
```

**Expected:**
- ✅ 5 records (matching sample CSV)
- ✅ `manifest_line` JSON contains all 20 fields
- ✅ `job_id` and `file_id` are populated

**Check VelocityJobRows Table:**
```sql
SELECT * FROM VelocityJobRows WHERE job_id = (SELECT TOP 1 job_id FROM VelocityJobs ORDER BY created_at DESC);
```

**Expected:**
- ✅ 5 records (one per CSV row)
- ✅ `raw_values` JSON contains all 20 fields
- ✅ `status` = "success" for all rows

---

### Step 7: Test Sample Template Download

**Test Case:**
- ✅ Click "Download Sample Template" button
- ✅ File should download: `velocity_template.csv`
- ✅ Open the file
- ✅ Should have 20 columns in header
- ✅ Should have 3 sample data rows

---

### Step 8: Test Excel Upload

**Create Excel File:**
1. Open Excel
2. Copy data from `sample_velocity_data_new_format.csv`
3. Paste into Excel
4. Save as `test_velocity.xlsx`

**Test Upload:**
- ✅ Select a distributor
- ✅ Select the Excel file
- ✅ Click "Import Data"
- ✅ Upload should succeed
- ✅ Verify database records (same as Step 6)

---

### Step 9: Test Error Handling

**Test Case 1: Invalid File Type**
- ✅ Try to upload a .txt file
- ✅ Error message should appear: "Only CSV (.csv) and Excel (.xlsx, .xls) files are allowed"

**Test Case 2: File Too Large**
- ✅ Try to upload a file > 10 MB
- ✅ Error message should appear: "File size exceeds 10 MB limit"

**Test Case 3: Empty File**
- ✅ Create an empty CSV file
- ✅ Try to upload
- ✅ Error message should appear: "CSV file is empty"

---

### Step 10: Test Job Details

**Test Case:**
- ✅ Click "View Details" on a job in the Recent Jobs list
- ✅ Should navigate to job details page
- ✅ Should show job status, row counts, errors
- ✅ Should show list of processed rows

---

## 🎯 Success Criteria

All test cases should pass:
- ✅ Distributor dropdown works
- ✅ File upload requires distributor selection
- ✅ CSV upload creates correct database records
- ✅ Excel upload works
- ✅ Sample template downloads
- ✅ Error handling works
- ✅ Job details page works

---

## 🐛 Known Issues / Limitations

1. **Excel Preview**: Excel files don't show preview (only CSV files)
2. **Large Files**: Files > 10 MB are rejected
3. **Validation**: All fields are optional (minimal validation)

---

## 📞 Support

If you encounter any issues:
1. Check browser console for errors
2. Check backend logs for errors
3. Verify database migration was applied
4. Verify all services are running

---

## ✅ Testing Complete!

Once all test cases pass, the Velocity rebuild is **READY FOR PRODUCTION**! 🎉

