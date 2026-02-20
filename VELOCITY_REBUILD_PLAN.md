# Velocity Import Rebuild - Implementation Plan

## 🎯 Overview

This document outlines the plan to rebuild the Velocity import feature with the new database schema and CSV format.

---

## 📊 New Database Schema

### Tables to Create:

1. **IngestedFiles** - File tracking
2. **VelocityJobs** - Job tracking with distributor selection
3. **VelocityShipments** - Canonical shipment data
4. **VelocityJobRows** - Row-level audit
5. **VelocityErrors** - Aggregated errors

### Key Changes:
- ✅ All primary keys are auto-increment integers
- ✅ No required fields except primary keys
- ✅ Distributor selection required at upload time
- ✅ New CSV format with 20 fields

---

## 📋 New CSV Format (20 Fields)

**Sequence:**
1. OPCO
2. Customer #
3. Customer Name
4. Address One
5. Address Two
6. City
7. Zip Code
8. Invoice #
9. Invoice Date
10. Product #
11. Brand
12. Pack Size
13. Description
14. Corp Manuf #
15. GTIN
16. Manufacturer Name
17. Qty
18. Sales
19. Landed Cost
20. Allowances

---

## 🔄 Migration Strategy

### Option 1: Clean Slate (Recommended)
1. Drop all existing Velocity tables
2. Create new tables with new schema
3. Update all models, services, repositories
4. Update frontend with distributor dropdown

### Option 2: Gradual Migration
1. Create new tables alongside old ones
2. Migrate data from old to new
3. Update code to use new tables
4. Drop old tables

---

## 🚧 Implementation Steps

### Step 1: Database Migration
```sql
-- Drop old tables
DROP TABLE IF EXISTS VelocityJobRows;
DROP TABLE IF EXISTS VelocityShipments;
DROP TABLE IF EXISTS VelocityJobs;

-- Create new tables (see schema above)
```

### Step 2: Update Models
- Create `IngestedFile.cs`
- Update `VelocityJob.cs` with new fields
- Update `VelocityShipment.cs` with new fields
- Update `VelocityJobRow.cs` with new fields
- Create `VelocityError.cs`

### Step 3: Update DTOs
- Create DTOs for new CSV format (20 fields)
- Update response DTOs

### Step 4: Update Parsers
- Update CSV parser for new field sequence
- Update Excel parser for new field sequence

### Step 5: Update Service
- Add distributor ID parameter to import methods
- Update validation logic
- Update shipment creation logic

### Step 6: Update Controller
- Add distributor ID to upload endpoint
- Update response format

### Step 7: Update Frontend
- Add distributor dropdown with search
- Update file upload to include distributor
- Update results display

---

## ⚠️ Breaking Changes

1. **Database Schema** - Complete rebuild
2. **CSV Format** - New 20-field format
3. **API Endpoints** - Distributor ID required
4. **Frontend** - New distributor selection UI

---

## 🎯 Recommended Approach

Given the extensive changes required, I recommend:

1. **Create a new feature branch**
2. **Implement clean slate migration**
3. **Test thoroughly before deploying**
4. **Provide migration script for existing data** (if needed)

---

## 📝 Next Steps

1. Confirm approach (clean slate vs gradual)
2. Backup existing Velocity data (if any)
3. Create migration script
4. Update all backend code
5. Update all frontend code
6. Test end-to-end
7. Deploy

---

## ⏱️ Estimated Time

- Database migration: 30 minutes
- Backend updates: 2-3 hours
- Frontend updates: 1-2 hours
- Testing: 1-2 hours
- **Total: 5-8 hours**

---

## 🚀 Quick Start (Clean Slate)

If you want to proceed with clean slate approach:

1. I'll create a migration to drop old tables
2. Create new tables with new schema
3. Update all models
4. Update all services/repositories
5. Update frontend with distributor dropdown
6. Test the complete flow

**Ready to proceed?**

