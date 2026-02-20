-- Baseline __EFMigrationsHistory to match existing schema, then apply the latest change
-- Target DB: NPPContractManagement (Dev)
-- Safe to run multiple times (uses INSERT IGNORE and checks column existence)

-- Ensure migrations history table exists
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
  CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

-- Baseline all prior migrations as applied
INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES
('20250910221359_InitialCreate','9.0.9'),
('20250911004018_UpdateSchemaForCompleteDataModel','9.0.9'),
('20250911011758_ForceCreateMissingTables','9.0.9'),
('20250911012256_AddSampleData','9.0.9'),
('20250911013146_ForceRecreateMainTables','9.0.9'),
('20250911014502_AddContractSampleData','9.0.9'),
('20250912115917_EnsureCustomerAccountExtendedFields','9.0.9'),
('20250912122826_AddContractsIndustryForeignSuspended','9.0.9'),
('20250912134941_AddUserContactStatusFields','9.0.9'),
('20250917120000_AddDistributorProductCodeExtensions','9.0.9'),
('20250917121500_AddAuditAliasesForDistributorProductCodes','9.0.9'),
('20250917152223_AddUserManufacturersTable','9.0.9'),
('20250923142931_UpdateContractsSchema_20250923','9.0.9'),
('20250923152942_RemoveObsoleteColumnsFromContracts','9.0.9'),
('20250923160332_AddContractRelationshipTables','9.0.9'),
('20250923181058_UpdateContractVersionsSchema','9.0.9'),
('20250923203108_RefactorContractProductAddContractPriceTable','9.0.9'),
('20250923235108_AddContractVersionRelationshipTables','9.0.9'),
('20250924174303_RemoveDescriptionAndIndustryFromContracts','9.0.9'),
('20250926052105_AddContractIdToContractPrices','9.0.9'),
('20250926062216_AddProductIdToContractVersionPrice','9.0.9'),
('20250929220305_AddCurrentVersionNumberToContractProducts','9.0.9'),
('20251002031101_2025_10_02_Create_Proposals_Module','9.0.9'),
('20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase','9.0.9'),
('20251009141159_UpdatePriceAndProductProposalSeed_20251009','9.0.9'),
('20251013064921_AddProposalProductExtraFields','9.0.9'),
('20251013073524_AddContractManufacturerEntegraFields','9.0.9'),
('20251015110528_UpdateProposalProductFields','9.0.9'),
('20251028000100_AddRejectReasonToProposals','9.0.9');

-- Apply the latest change if the column still exists
SET @col_exists := (
  SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'DistributorProductCodes'
    AND COLUMN_NAME = 'DistributorPrice'
);
SET @drop_sql := IF(@col_exists = 1,
  'ALTER TABLE `DistributorProductCodes` DROP COLUMN `DistributorPrice`',
  'SELECT 1');
PREPARE stmt FROM @drop_sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Mark the latest migration as applied
INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES
('20251030130414_RemoveDistributorPriceFromDistributorProductCodes','9.0.9');


-- Apply removal of Description if the column still exists
SET @col_exists_desc := (
  SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'DistributorProductCodes'
    AND COLUMN_NAME = 'Description'
);
SET @drop_sql_desc := IF(@col_exists_desc = 1,
  'ALTER TABLE `DistributorProductCodes` DROP COLUMN `Description`',
  'SELECT 1');
PREPARE stmt_desc FROM @drop_sql_desc; EXECUTE stmt_desc; DEALLOCATE PREPARE stmt_desc;

-- Mark the RemoveDescription migration as applied
INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES
('20251030171415_RemoveDescriptionFromDistributorProductCodes','9.0.9');


-- Apply removal of IsActive if the column still exists
SET @col_exists_isactive := (
  SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'DistributorProductCodes'
    AND COLUMN_NAME = 'IsActive'
);
SET @drop_sql_isactive := IF(@col_exists_isactive = 1,
  'ALTER TABLE `DistributorProductCodes` DROP COLUMN `IsActive`',
  'SELECT 1');
PREPARE stmt_isactive FROM @drop_sql_isactive; EXECUTE stmt_isactive; DEALLOCATE PREPARE stmt_isactive;

-- Mark the RemoveIsActive migration as applied
INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES
('20251030222039_RemoveIsActiveFromDistributorProductCodes','9.0.9');


-- Apply addition of new Product fields if they do not exist
ALTER TABLE `Products` ADD COLUMN IF NOT EXISTS `Brand` varchar(100) NULL;
ALTER TABLE `Products` ADD COLUMN IF NOT EXISTS `TertiaryCategory` varchar(100) NULL;
ALTER TABLE `Products` ADD COLUMN IF NOT EXISTS `AlwaysList` tinyint(1) NOT NULL DEFAULT 0;
ALTER TABLE `Products` ADD COLUMN IF NOT EXISTS `Notes` varchar(1000) NULL;

-- Mark the Product fields migration as applied
INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES
('20251031141708_AddBrandTertiaryCategoryAlwaysListNotesToProducts','9.0.9');


-- Apply removal of ContactPerson, CreditLimit, CurrentBalance from CustomerAccounts if columns still exist
SET @col_exists_contact := (
  SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'CustomerAccounts'
    AND COLUMN_NAME = 'ContactPerson'
);
SET @drop_sql_contact := IF(@col_exists_contact = 1,
  'ALTER TABLE `CustomerAccounts` DROP COLUMN `ContactPerson`',
  'SELECT 1');
PREPARE stmt_contact FROM @drop_sql_contact; EXECUTE stmt_contact; DEALLOCATE PREPARE stmt_contact;

SET @col_exists_creditlimit := (
  SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'CustomerAccounts'
    AND COLUMN_NAME = 'CreditLimit'
);
SET @drop_sql_creditlimit := IF(@col_exists_creditlimit = 1,
  'ALTER TABLE `CustomerAccounts` DROP COLUMN `CreditLimit`',
  'SELECT 1');
PREPARE stmt_creditlimit FROM @drop_sql_creditlimit; EXECUTE stmt_creditlimit; DEALLOCATE PREPARE stmt_creditlimit;

SET @col_exists_currentbalance := (
  SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'CustomerAccounts'
    AND COLUMN_NAME = 'CurrentBalance'
);
SET @drop_sql_currentbalance := IF(@col_exists_currentbalance = 1,
  'ALTER TABLE `CustomerAccounts` DROP COLUMN `CurrentBalance`',
  'SELECT 1');
PREPARE stmt_currentbalance FROM @drop_sql_currentbalance; EXECUTE stmt_currentbalance; DEALLOCATE PREPARE stmt_currentbalance;

-- Mark the CustomerAccounts column removal migration as applied
INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES
('20251031153923_RemoveCreditLimitCurrentBalanceContactPersonFromCustomerAccounts','9.0.9');


-- Apply removal of Product pricing/unit columns if they still exist
SET @col_exists_unitprice := (
  SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'Products'
    AND COLUMN_NAME = 'UnitPrice'
);
SET @drop_sql_unitprice := IF(@col_exists_unitprice = 1,
  'ALTER TABLE `Products` DROP COLUMN `UnitPrice`',
  'SELECT 1');
PREPARE stmt_unitprice FROM @drop_sql_unitprice; EXECUTE stmt_unitprice; DEALLOCATE PREPARE stmt_unitprice;

SET @col_exists_uom := (
  SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'Products'
    AND COLUMN_NAME = 'UnitOfMeasure'
);
SET @drop_sql_uom := IF(@col_exists_uom = 1,
  'ALTER TABLE `Products` DROP COLUMN `UnitOfMeasure`',
  'SELECT 1');
PREPARE stmt_uom FROM @drop_sql_uom; EXECUTE stmt_uom; DEALLOCATE PREPARE stmt_uom;

SET @col_exists_listprice := (
  SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'Products'
    AND COLUMN_NAME = 'ListPrice'
);
SET @drop_sql_listprice := IF(@col_exists_listprice = 1,
  'ALTER TABLE `Products` DROP COLUMN `ListPrice`',
  'SELECT 1');
PREPARE stmt_listprice FROM @drop_sql_listprice; EXECUTE stmt_listprice; DEALLOCATE PREPARE stmt_listprice;

SET @col_exists_costprice := (
  SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'Products'
    AND COLUMN_NAME = 'CostPrice'
);
SET @drop_sql_costprice := IF(@col_exists_costprice = 1,
  'ALTER TABLE `Products` DROP COLUMN `CostPrice`',
  'SELECT 1');
PREPARE stmt_costprice FROM @drop_sql_costprice; EXECUTE stmt_costprice; DEALLOCATE PREPARE stmt_costprice;

-- Mark the RemovePricingFieldsFromProducts migration as applied
INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES
('20251101211350_RemovePricingFieldsFromProducts','9.0.9');
