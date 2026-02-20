# Bulk Contract Renewal Feature - Implementation Guide

## 🎯 Overview

The Bulk Contract Renewal feature allows authorized users to create multiple proposal requests from existing contracts in an efficient manner. Users can select multiple contracts, apply pricing adjustments, and generate renewal proposals with a single action.

---

## ✅ Features Implemented

### **Backend (API)**

1. ✅ **DTOs Created** (`BulkRenewalDto.cs`)
   - `BulkRenewalRequest` - Request model with contract IDs, pricing adjustments, and options
   - `PricingAdjustment` - Percentage change and quantity threshold configuration
   - `BulkRenewalResponse` - Response with success/failure counts and detailed results
   - `ContractRenewalResult` - Individual contract renewal result
   - `ContractRenewalSummary` - Contract summary for selection

2. ✅ **Service Layer** (`BulkRenewalService.cs`)
   - `CreateBulkRenewalProposalsAsync` - Creates multiple proposals from selected contracts
   - `ValidateContractsForRenewalAsync` - Validates contracts can be renewed
   - `GetContractsForRenewalAsync` - Gets contracts available for renewal
   - Automatic term date calculation (starts 1 day after contract end)
   - Pricing adjustment logic with quantity thresholds
   - Only includes non-discontinued products
   - Copies industries, op-cos, manufacturer, and distributor from source contract

3. ✅ **API Controller** (`BulkRenewalController.cs`)
   - `POST /api/bulkRenewal/create` - Create bulk renewal proposals
   - `POST /api/bulkRenewal/validate` - Validate contracts for renewal
   - `POST /api/bulkRenewal/available` - Get available contracts
   - **Authorization**: Restricted to "System Administrator" and "Contract Manager" roles

4. ✅ **Service Registration**
   - `IBulkRenewalService` registered in DI container

### **Frontend (Angular)**

1. ✅ **Models** (`bulk-renewal.model.ts`)
   - TypeScript interfaces for all bulk renewal data types

2. ✅ **Service** (`bulk-renewal.service.ts`)
   - API client for bulk renewal operations

3. ✅ **Dialog Component** (`bulk-renewal-dialog.component`)
   - User-friendly dialog for configuring bulk renewal
   - Pricing adjustment configuration
   - Proposal due date selection
   - Real-time adjustment preview
   - Error handling and validation

---

## 🔧 How It Works

### **Workflow**

1. **User selects contracts** from the contract listing page
2. **User clicks "Create Renewal Proposals"** button
3. **Dialog opens** with configuration options:
   - Percentage increase/decrease
   - Minimum quantity threshold (optional)
   - Proposal due date
4. **User submits** the request
5. **System creates proposals** for each selected contract:
   - Copies manufacturer, distributor, industries, op-cos
   - Calculates new term dates (starts 1 day after contract end)
   - Includes only non-discontinued products
   - Applies pricing adjustments based on configuration
6. **User receives results** showing success/failure for each contract

---

## 📊 Pricing Adjustment Logic

### **Percentage Change**
- **Positive value** = Price increase (e.g., 5 = 5% increase)
- **Negative value** = Price decrease (e.g., -3 = 3% decrease)
- **Zero** = No price adjustment

### **Quantity Threshold**
- **Apply to all products**: Adjustment applies regardless of quantity
- **Minimum threshold**: Adjustment only applies to products with quantity >= threshold

### **Example**
```
Current Price: $100
Percentage Change: 5%
Result: $105

Current Price: $100
Percentage Change: -3%
Result: $97
```

---

## 🔐 Authorization

**Required Roles:**
- System Administrator
- Contract Manager

**Enforcement:**
- Backend: `[Authorize(Roles = "System Administrator,Contract Manager")]`
- Frontend: Role-based UI visibility (to be implemented)

---

## 📋 API Endpoints

### **1. Create Bulk Renewal**
```http
POST /api/bulkRenewal/create
Authorization: Bearer <token>
Content-Type: application/json

{
  "contractIds": [1, 2, 3],
  "pricingAdjustment": {
    "percentageChange": 5.0,
    "minimumQuantityThreshold": 100,
    "applyToAllProducts": false
  },
  "proposalDueDate": "2024-12-31T00:00:00Z",
  "additionalProductIds": []
}
```

**Response:**
```json
{
  "totalContracts": 3,
  "successfulProposals": 3,
  "failedProposals": 0,
  "createdProposalIds": [101, 102, 103],
  "results": [
    {
      "contractId": 1,
      "contractNumber": "C-2024-001",
      "success": true,
      "proposalId": 101,
      "productCount": 25,
      "additionalProductCount": 0
    }
  ],
  "success": true,
  "message": "Created 3 of 3 renewal proposals"
}
```

### **2. Validate Contracts**
```http
POST /api/bulkRenewal/validate
Authorization: Bearer <token>
Content-Type: application/json

[1, 2, 3]
```

**Response:**
```json
{
  "1": "",
  "2": "",
  "3": "No contract versions found"
}
```

---

## 🎯 Business Rules

### **Proposal Creation**
1. ✅ **Term Dates**: Start 1 day after contract end date, same duration as original
2. ✅ **Products**: Only non-discontinued products included
3. ✅ **Relationships**: Inherits industries, op-cos, manufacturer, distributor
4. ✅ **Pricing**: Adjustments applied based on configuration
5. ✅ **Status**: All proposals created in "Draft" status
6. ✅ **Type**: All proposals marked as "Renewal" type

### **Validation**
1. ✅ Contract must exist
2. ✅ Contract must have at least one version
3. ✅ User must have appropriate role

---

## 📁 Files Created

### **Backend**
- ✅ `DTOs/BulkRenewalDto.cs`
- ✅ `Services/IBulkRenewalService.cs`
- ✅ `Services/BulkRenewalService.cs`
- ✅ `Controllers/BulkRenewalController.cs`
- ✅ `Program.cs` (updated)

### **Frontend**
- ✅ `models/bulk-renewal.model.ts`
- ✅ `services/bulk-renewal.service.ts`
- ✅ `components/bulk-renewal-dialog/bulk-renewal-dialog.component.ts`
- ✅ `components/bulk-renewal-dialog/bulk-renewal-dialog.component.html`
- ✅ `components/bulk-renewal-dialog/bulk-renewal-dialog.component.css`

---

## 🚧 Remaining Tasks

### **Frontend Integration**
1. ⏳ Update contract listing component to add multi-select capability
2. ⏳ Add "Create Renewal Proposals" button to contract list
3. ⏳ Integrate bulk renewal dialog with contract list
4. ⏳ Add role-based visibility for the feature
5. ⏳ Add product selection feature (optional additional products)
6. ⏳ Display results after bulk renewal completion

### **Testing**
1. ⏳ Test with single contract
2. ⏳ Test with multiple contracts
3. ⏳ Test pricing adjustments
4. ⏳ Test quantity thresholds
5. ⏳ Test authorization (role-based access)
6. ⏳ Test error handling

---

## 💡 Usage Example

### **Scenario: Renew 10 contracts with 5% price increase**

1. Navigate to Contracts page
2. Filter contracts (e.g., ending in next 30 days)
3. Select 10 contracts using checkboxes
4. Click "Create Renewal Proposals"
5. Configure:
   - Percentage Change: 5
   - Apply to all products: Yes
   - Due Date: 30 days from now
6. Click "Create 10 Proposal(s)"
7. View results showing success for each contract
8. Navigate to Proposals page to see created proposals

---

## 🎊 Summary

✅ **Backend Complete** - Service, controller, DTOs implemented  
✅ **Frontend Components** - Dialog and service created  
✅ **Authorization** - Role-based access control  
✅ **Pricing Logic** - Flexible adjustment configuration  
✅ **Business Rules** - All requirements implemented  
⏳ **UI Integration** - Needs to be added to contract listing page  
⏳ **Testing** - End-to-end testing required  

The core functionality is complete and ready for integration with the contract listing page!

