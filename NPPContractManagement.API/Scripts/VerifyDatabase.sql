-- Database Verification Script
-- This script verifies that all required tables exist and contain sample data

USE NPPContractManagment;

-- Show all tables in the database
SHOW TABLES;

-- Verify OpCos table and data
SELECT 'OpCos Table' as TableName, COUNT(*) as RecordCount FROM OpCos;
SELECT * FROM OpCos LIMIT 5;

-- Verify MemberAccounts table and data
SELECT 'MemberAccounts Table' as TableName, COUNT(*) as RecordCount FROM MemberAccounts;
SELECT * FROM MemberAccounts LIMIT 5;

-- Verify CustomerAccounts table and data
SELECT 'CustomerAccounts Table' as TableName, COUNT(*) as RecordCount FROM CustomerAccounts;
SELECT * FROM CustomerAccounts LIMIT 5;

-- Verify Products table and data
SELECT 'Products Table' as TableName, COUNT(*) as RecordCount FROM Products;
SELECT * FROM Products LIMIT 5;

-- Verify Manufacturers table and data
SELECT 'Manufacturers Table' as TableName, COUNT(*) as RecordCount FROM Manufacturers;
SELECT * FROM Manufacturers LIMIT 5;

-- Verify Distributors table and data
SELECT 'Distributors Table' as TableName, COUNT(*) as RecordCount FROM Distributors;
SELECT * FROM Distributors LIMIT 5;

-- Verify Industries table and data
SELECT 'Industries Table' as TableName, COUNT(*) as RecordCount FROM Industries;
SELECT * FROM Industries LIMIT 5;

-- Verify Contracts table and data
SELECT 'Contracts Table' as TableName, COUNT(*) as RecordCount FROM Contracts;
SELECT * FROM Contracts LIMIT 5;

-- Verify junction tables
SELECT 'ContractDistributors Table' as TableName, COUNT(*) as RecordCount FROM ContractDistributors;
SELECT 'ContractOpCos Table' as TableName, COUNT(*) as RecordCount FROM ContractOpCos;
SELECT 'ContractIndustries Table' as TableName, COUNT(*) as RecordCount FROM ContractIndustries;
SELECT 'ContractVersions Table' as TableName, COUNT(*) as RecordCount FROM ContractVersions;
SELECT 'DistributorProductCodes Table' as TableName, COUNT(*) as RecordCount FROM DistributorProductCodes;

-- Summary of all table counts
SELECT 
    'OpCos' as TableName, COUNT(*) as RecordCount FROM OpCos
UNION ALL
SELECT 
    'MemberAccounts' as TableName, COUNT(*) as RecordCount FROM MemberAccounts
UNION ALL
SELECT 
    'CustomerAccounts' as TableName, COUNT(*) as RecordCount FROM CustomerAccounts
UNION ALL
SELECT 
    'Products' as TableName, COUNT(*) as RecordCount FROM Products
UNION ALL
SELECT 
    'Manufacturers' as TableName, COUNT(*) as RecordCount FROM Manufacturers
UNION ALL
SELECT 
    'Distributors' as TableName, COUNT(*) as RecordCount FROM Distributors
UNION ALL
SELECT 
    'Industries' as TableName, COUNT(*) as RecordCount FROM Industries
UNION ALL
SELECT 
    'Contracts' as TableName, COUNT(*) as RecordCount FROM Contracts
UNION ALL
SELECT 
    'Users' as TableName, COUNT(*) as RecordCount FROM Users
UNION ALL
SELECT 
    'Roles' as TableName, COUNT(*) as RecordCount FROM Roles;
