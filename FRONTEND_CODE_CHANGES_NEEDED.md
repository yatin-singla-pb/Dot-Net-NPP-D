# Frontend Code Changes Required for "Guaranteed Price" → "Bid Amount"

## 📋 FILES TO UPDATE

After updating the database, these frontend files need to be updated to change validation logic from "Guaranteed" to "Bid Amount":

---

## 1. proposal-create.component.ts

### Location: Line 946
**Current Code:**
```typescript
if (priceTypeName === 'Guaranteed') {
```

**Change To:**
```typescript
if (priceTypeName === 'Bid Amount') {
```

---

## 2. proposal-edit.component.ts

### Location: Line 41
**Current Code:**
```typescript
if (pt === 'guaranteed price') return 'Contract Price at Time of Purchase';
```

**Change To:**
```typescript
if (pt === 'bid amount') return 'Contract Price at Time of Purchase';
```

### Location: Line 663
**Current Code:**
```typescript
if (priceTypeName === 'Guaranteed') {
```

**Change To:**
```typescript
if (priceTypeName === 'Bid Amount') {
```

### Location: Line 904
**Current Code:**
```typescript
if (priceTypeName === 'Guaranteed') {
```

**Change To:**
```typescript
if (priceTypeName === 'Bid Amount') {
```

### Location: Line 909
**Current Code:**
```typescript
return `${field.name} is required for Guaranteed price type on ${productName}.`;
```

**Change To:**
```typescript
return `${field.name} is required for Bid Amount price type on ${productName}.`;
```

---

## 3. proposal-awards.component.ts

### Location: Line 551
**Current Code:**
```typescript
if (pt === 'guaranteed price') return 'Contract Price at Time of Purchase';
```

**Change To:**
```typescript
if (pt === 'bid amount') return 'Contract Price at Time of Purchase';
```

---

## 4. proposal-detail.component.ts

### Location: Line 137
**Current Code:**
```typescript
if (pt === 'guaranteed price') return 'Contract Price at Time of Purchase';
```

**Change To:**
```typescript
if (pt === 'bid amount') return 'Contract Price at Time of Purchase';
```

---

## 5. contract-form.component.ts

### Location: Line 97
**Current Code:**
```typescript
if (s === 'contract price at time of purchase' || s === 'guaranteed price') return 'Contract Price at Time of Purchase';
```

**Change To:**
```typescript
if (s === 'contract price at time of purchase' || s === 'bid amount') return 'Contract Price at Time of Purchase';
```

---

## 6. ContractService.cs (Backend)

### Location: Line 1468
**Current Code:**
```csharp
("guaranteedprice", "Contract Price at Time of Purchase", "guaranteed"),
```

**Change To:**
```csharp
("bidamount", "Contract Price at Time of Purchase", "bid-amount"),
```

**Note:** This is for fuzzy matching during contract imports/migrations. Update to recognize "Bid Amount" as a valid price type.

---

## 🎯 VALIDATION LOGIC EXPLANATION

The validation logic checks the price type name to determine required fields:

- **"Bid Amount"** (formerly "Guaranteed Price"): ALL pricing fields are required
- **Other price types**: At least ONE pricing field is required

This ensures data quality and prevents incomplete pricing information.

---

## ✅ TESTING CHECKLIST

After making these changes:

1. [ ] Create new proposal with "Bid Amount" price type
2. [ ] Verify all pricing fields are required
3. [ ] Try to submit without all fields - should show validation error
4. [ ] Edit existing proposal with "Bid Amount" price type
5. [ ] Verify validation works correctly
6. [ ] Check proposal awards page displays correctly
7. [ ] Check proposal detail page displays correctly
8. [ ] Verify contract form handles "Bid Amount" correctly

---

## 🔄 DEPLOYMENT SEQUENCE

**Option 1: Code First (Recommended for Production)**
1. Deploy frontend code changes
2. Update database
3. Test thoroughly

**Option 2: Database First (Faster for Development)**
1. Update database
2. Deploy frontend code changes immediately
3. Test thoroughly

**Option 1 is safer** because the code will handle both "Guaranteed Price" and "Bid Amount" during the transition.

