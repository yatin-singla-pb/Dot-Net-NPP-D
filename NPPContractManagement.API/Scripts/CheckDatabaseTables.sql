-- Check Database Tables Script
-- Run this in your MySQL client to verify table existence

USE NPPContractManagment;

-- Show all tables in the database
SELECT 'All Tables in Database:' as Info;
SHOW TABLES;

-- Check if specific tables exist
SELECT 
    TABLE_NAME,
    TABLE_ROWS,
    CREATE_TIME,
    UPDATE_TIME
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'NPPContractManagment' 
    AND TABLE_NAME IN ('OpCos', 'MemberAccounts', 'CustomerAccounts', 'Products', 'Manufacturers', 'Distributors')
ORDER BY TABLE_NAME;

-- Check OpCos table specifically
SELECT 'OpCos Table Check:' as Info;
SELECT COUNT(*) as OpCos_Count FROM OpCos;
SELECT * FROM OpCos LIMIT 3;

-- Check MemberAccounts table specifically  
SELECT 'MemberAccounts Table Check:' as Info;
SELECT COUNT(*) as MemberAccounts_Count FROM MemberAccounts;
SELECT * FROM MemberAccounts LIMIT 3;

-- Check CustomerAccounts table specifically
SELECT 'CustomerAccounts Table Check:' as Info;
SELECT COUNT(*) as CustomerAccounts_Count FROM CustomerAccounts;
SELECT * FROM CustomerAccounts LIMIT 3;

-- Check Products table specifically
SELECT 'Products Table Check:' as Info;
SELECT COUNT(*) as Products_Count FROM Products;
SELECT * FROM Products LIMIT 3;

-- Check Manufacturers table specifically
SELECT 'Manufacturers Table Check:' as Info;
SELECT COUNT(*) as Manufacturers_Count FROM Manufacturers;
SELECT * FROM Manufacturers LIMIT 3;

-- Check Distributors table specifically
SELECT 'Distributors Table Check:' as Info;
SELECT COUNT(*) as Distributors_Count FROM Distributors;
SELECT * FROM Distributors LIMIT 3;
