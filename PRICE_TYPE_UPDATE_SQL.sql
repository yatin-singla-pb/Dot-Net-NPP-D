-- ============================================================================
-- Price Type Update: "Guaranteed Price" → "Bid Amount"
-- ============================================================================
-- 
-- SAFETY: This update is 100% safe because:
-- 1. The system uses priceTypeId (integer) for data storage
-- 2. The Name field is only used for display purposes
-- 3. All existing data will automatically show the new name
-- 4. No foreign key constraints on the Name field
--
-- IMPORTANT: After running this SQL, you must update the frontend code
-- to change validation logic from "Guaranteed" to "Bid Amount"
-- ============================================================================

-- Verify current value before update
SELECT * FROM PriceTypes WHERE Id = 1;

-- Update the price type name
UPDATE PriceTypes 
SET Name = 'Bid Amount' 
WHERE Id = 1 AND Name = 'Guaranteed Price';

-- Verify the update
SELECT * FROM PriceTypes WHERE Id = 1;

-- Expected result:
-- Id | Name       | IsActive
-- 1  | Bid Amount | 1

-- ============================================================================
-- VERIFICATION QUERIES
-- ============================================================================

-- Check how many proposals use this price type
SELECT COUNT(*) as ProposalCount
FROM ProposalProducts 
WHERE PriceTypeId = 1;

-- Check how many contracts use this price type (if applicable)
-- Note: Contracts may store price type as string, not ID
SELECT COUNT(*) as ContractCount
FROM ContractPrices 
WHERE PriceType LIKE '%Guaranteed%';

-- ============================================================================
-- ROLLBACK (if needed)
-- ============================================================================

-- To rollback this change:
-- UPDATE PriceTypes 
-- SET Name = 'Guaranteed Price' 
-- WHERE Id = 1 AND Name = 'Bid Amount';

