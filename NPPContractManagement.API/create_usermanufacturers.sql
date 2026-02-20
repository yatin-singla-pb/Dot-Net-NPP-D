CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE TABLE `Distributors` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
        `ContactPerson` varchar(200) CHARACTER SET utf8mb4 NULL,
        `Email` varchar(255) CHARACTER SET utf8mb4 NULL,
        `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 NULL,
        `Address` varchar(500) CHARACTER SET utf8mb4 NULL,
        `City` varchar(100) CHARACTER SET utf8mb4 NULL,
        `State` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ZipCode` varchar(20) CHARACTER SET utf8mb4 NULL,
        `Country` varchar(100) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Distributors` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE TABLE `Industries` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Industries` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE TABLE `Manufacturers` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
        `ContactPerson` varchar(200) CHARACTER SET utf8mb4 NULL,
        `Email` varchar(255) CHARACTER SET utf8mb4 NULL,
        `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 NULL,
        `Address` varchar(500) CHARACTER SET utf8mb4 NULL,
        `City` varchar(100) CHARACTER SET utf8mb4 NULL,
        `State` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ZipCode` varchar(20) CHARACTER SET utf8mb4 NULL,
        `Country` varchar(100) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Manufacturers` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE TABLE `Roles` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Roles` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE TABLE `Users` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `UserId` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `FirstName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `LastName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Email` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 NULL,
        `PasswordHash` longtext CHARACTER SET utf8mb4 NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `EmailConfirmed` tinyint(1) NOT NULL,
        `LastLoginDate` datetime(6) NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Users` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE TABLE `Contracts` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractNumber` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Title` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `DistributorId` int NOT NULL,
        `IndustryId` int NOT NULL,
        `Status` int NOT NULL,
        `StartDate` datetime(6) NOT NULL,
        `EndDate` datetime(6) NOT NULL,
        `TotalValue` decimal(18,2) NULL,
        `Terms` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `Notes` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Contracts` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Contracts_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_Contracts_Industries_IndustryId` FOREIGN KEY (`IndustryId`) REFERENCES `Industries` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE TABLE `Products` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
        `SKU` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ManufacturerId` int NOT NULL,
        `UnitPrice` decimal(18,2) NULL,
        `UnitOfMeasure` varchar(50) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Products` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Products_Manufacturers_ManufacturerId` FOREIGN KEY (`ManufacturerId`) REFERENCES `Manufacturers` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE TABLE `UserRoles` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `UserId` int NOT NULL,
        `RoleId` int NOT NULL,
        `AssignedDate` datetime(6) NOT NULL,
        `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_UserRoles` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_UserRoles_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_UserRoles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE TABLE `ContractProducts` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `ProductId` int NOT NULL,
        `ContractPrice` decimal(18,2) NOT NULL,
        `MinimumQuantity` int NULL,
        `MaximumQuantity` int NULL,
        `Notes` varchar(500) CHARACTER SET utf8mb4 NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_ContractProducts` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ContractProducts_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ContractProducts_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    INSERT INTO `Industries` (`Id`, `CreatedBy`, `CreatedDate`, `Description`, `IsActive`, `ModifiedBy`, `ModifiedDate`, `Name`)
    VALUES (1, 'System', TIMESTAMP '2025-09-10 22:13:56', 'Higher education institutions', TRUE, NULL, NULL, 'College & University'),
    (2, 'System', TIMESTAMP '2025-09-10 22:13:56', 'Primary and secondary education', TRUE, NULL, NULL, 'K-12'),
    (3, 'System', TIMESTAMP '2025-09-10 22:13:56', 'Fast food and quick service restaurants', TRUE, NULL, NULL, 'Quick Serve Restaurant'),
    (4, 'System', TIMESTAMP '2025-09-10 22:13:56', 'Hospitals and healthcare facilities', TRUE, NULL, NULL, 'Healthcare'),
    (5, 'System', TIMESTAMP '2025-09-10 22:13:56', 'Corporate dining and cafeterias', TRUE, NULL, NULL, 'Corporate');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    INSERT INTO `Roles` (`Id`, `CreatedBy`, `CreatedDate`, `Description`, `IsActive`, `ModifiedBy`, `ModifiedDate`, `Name`)
    VALUES (1, 'System', TIMESTAMP '2025-09-10 22:13:56', 'Full system access', TRUE, NULL, NULL, 'System Administrator'),
    (2, 'System', TIMESTAMP '2025-09-10 22:13:56', 'Manage contracts and related data', TRUE, NULL, NULL, 'Contract Manager'),
    (3, 'System', TIMESTAMP '2025-09-10 22:13:56', 'Manufacturer user access', TRUE, NULL, NULL, 'Manufacturer'),
    (4, 'System', TIMESTAMP '2025-09-10 22:13:56', 'Distributor user access', TRUE, NULL, NULL, 'Distributor'),
    (5, 'System', TIMESTAMP '2025-09-10 22:13:56', 'View contracts and run reports', TRUE, NULL, NULL, 'Contract Viewer');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE INDEX `IX_ContractProducts_ContractId` ON `ContractProducts` (`ContractId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE INDEX `IX_ContractProducts_ProductId` ON `ContractProducts` (`ProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Contracts_ContractNumber` ON `Contracts` (`ContractNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE INDEX `IX_Contracts_DistributorId` ON `Contracts` (`DistributorId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE INDEX `IX_Contracts_IndustryId` ON `Contracts` (`IndustryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE INDEX `IX_Products_ManufacturerId` ON `Products` (`ManufacturerId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Roles_Name` ON `Roles` (`Name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE INDEX `IX_UserRoles_RoleId` ON `UserRoles` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_UserRoles_UserId_RoleId` ON `UserRoles` (`UserId`, `RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Users_Email` ON `Users` (`Email`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Users_UserId` ON `Users` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250910221359_InitialCreate') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250910221359_InitialCreate', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Contracts` DROP FOREIGN KEY `FK_Contracts_Distributors_DistributorId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Contracts` DROP FOREIGN KEY `FK_Contracts_Industries_IndustryId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Contracts` RENAME COLUMN `IndustryId` TO `ManufacturerId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Contracts` RENAME INDEX `IX_Contracts_IndustryId` TO `IX_Contracts_ManufacturerId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Users` ADD `AccountStatus` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Users` ADD `Class` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Users` ADD `Company` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Users` ADD `IndustryId` int NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Users` ADD `JobTitle` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Products` ADD `Category` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Products` ADD `CostPrice` decimal(18,2) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Products` ADD `GTIN` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Products` ADD `ListPrice` decimal(18,2) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Products` ADD `ManufacturerProductCode` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Products` ADD `PackSize` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Products` ADD `Status` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Products` ADD `SubCategory` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Products` ADD `UPC` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Manufacturers` ADD `AKA` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Manufacturers` ADD `PrimaryBroker` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Manufacturers` ADD `Status` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Industries` ADD `Status` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Distributors` ADD `ReceiveContractProposal` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Distributors` ADD `Status` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Contracts` MODIFY COLUMN `DistributorId` int NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Contracts` ADD `CurrentVersionNumber` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Contracts` ADD `IsSuspended` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Contracts` ADD `SendToPerformance` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE TABLE `ContractDistributors` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `DistributorId` int NOT NULL,
        `AssignedDate` datetime(6) NOT NULL,
        `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        CONSTRAINT `PK_ContractDistributors` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ContractDistributors_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ContractDistributors_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE TABLE `ContractIndustries` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `IndustryId` int NOT NULL,
        `AssignedDate` datetime(6) NOT NULL,
        `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        CONSTRAINT `PK_ContractIndustries` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ContractIndustries_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ContractIndustries_Industries_IndustryId` FOREIGN KEY (`IndustryId`) REFERENCES `Industries` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE TABLE `ContractVersions` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `VersionNumber` int NOT NULL,
        `Title` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `StartDate` datetime(6) NOT NULL,
        `EndDate` datetime(6) NOT NULL,
        `TotalValue` decimal(18,2) NULL,
        `Terms` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `Notes` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `ChangeReason` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `IsCurrentVersion` tinyint(1) NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_ContractVersions` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ContractVersions_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE TABLE `DistributorProductCodes` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `DistributorId` int NOT NULL,
        `ProductId` int NOT NULL,
        `DistributorCode` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
        `DistributorPrice` decimal(18,2) NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_DistributorProductCodes` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_DistributorProductCodes_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_DistributorProductCodes_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE TABLE `MemberAccounts` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `MemberNumber` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `FacilityName` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Address` varchar(500) CHARACTER SET utf8mb4 NULL,
        `City` varchar(100) CHARACTER SET utf8mb4 NULL,
        `State` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ZipCode` varchar(20) CHARACTER SET utf8mb4 NULL,
        `Country` varchar(100) CHARACTER SET utf8mb4 NULL,
        `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 NULL,
        `Email` varchar(255) CHARACTER SET utf8mb4 NULL,
        `ContactPerson` varchar(200) CHARACTER SET utf8mb4 NULL,
        `IndustryId` int NULL,
        `W9` tinyint(1) NOT NULL,
        `TaxId` varchar(500) CHARACTER SET utf8mb4 NULL,
        `BusinessType` varchar(200) CHARACTER SET utf8mb4 NULL,
        `Status` int NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_MemberAccounts` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_MemberAccounts_Industries_IndustryId` FOREIGN KEY (`IndustryId`) REFERENCES `Industries` (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE TABLE `OpCos` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `RemoteReferenceCode` varchar(100) CHARACTER SET utf8mb4 NULL,
        `DistributorId` int NOT NULL,
        `Address` varchar(500) CHARACTER SET utf8mb4 NULL,
        `City` varchar(100) CHARACTER SET utf8mb4 NULL,
        `State` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ZipCode` varchar(20) CHARACTER SET utf8mb4 NULL,
        `Country` varchar(100) CHARACTER SET utf8mb4 NULL,
        `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 NULL,
        `Email` varchar(255) CHARACTER SET utf8mb4 NULL,
        `ContactPerson` varchar(200) CHARACTER SET utf8mb4 NULL,
        `Status` int NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_OpCos` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_OpCos_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE TABLE `ContractOpCos` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `OpCoId` int NOT NULL,
        `AssignedDate` datetime(6) NOT NULL,
        `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        CONSTRAINT `PK_ContractOpCos` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ContractOpCos_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ContractOpCos_OpCos_OpCoId` FOREIGN KEY (`OpCoId`) REFERENCES `OpCos` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE TABLE `CustomerAccounts` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `MemberAccountId` int NOT NULL,
        `DistributorId` int NOT NULL,
        `OpCoId` int NULL,
        `CustomerName` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `CustomerAccountNumber` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Address` varchar(500) CHARACTER SET utf8mb4 NULL,
        `City` varchar(100) CHARACTER SET utf8mb4 NULL,
        `State` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ZipCode` varchar(20) CHARACTER SET utf8mb4 NULL,
        `Country` varchar(100) CHARACTER SET utf8mb4 NULL,
        `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 NULL,
        `Email` varchar(255) CHARACTER SET utf8mb4 NULL,
        `ContactPerson` varchar(200) CHARACTER SET utf8mb4 NULL,
        `CreditLimit` decimal(18,2) NULL,
        `CurrentBalance` decimal(18,2) NULL,
        `Status` int NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_CustomerAccounts` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_CustomerAccounts_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_CustomerAccounts_MemberAccounts_MemberAccountId` FOREIGN KEY (`MemberAccountId`) REFERENCES `MemberAccounts` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_CustomerAccounts_OpCos_OpCoId` FOREIGN KEY (`OpCoId`) REFERENCES `OpCos` (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-09-11 00:40:15', `Status` = 1
    WHERE `Id` = 1;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-09-11 00:40:15', `Status` = 1
    WHERE `Id` = 2;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-09-11 00:40:15', `Status` = 1
    WHERE `Id` = 3;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-09-11 00:40:15', `Status` = 1
    WHERE `Id` = 4;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-09-11 00:40:15', `Status` = 1
    WHERE `Id` = 5;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-09-11 00:40:15'
    WHERE `Id` = 1;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-09-11 00:40:15'
    WHERE `Id` = 2;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-09-11 00:40:15'
    WHERE `Id` = 3;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-09-11 00:40:15'
    WHERE `Id` = 4;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-09-11 00:40:15'
    WHERE `Id` = 5;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE INDEX `IX_Users_IndustryId` ON `Users` (`IndustryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE UNIQUE INDEX `IX_ContractDistributors_ContractId_DistributorId` ON `ContractDistributors` (`ContractId`, `DistributorId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE INDEX `IX_ContractDistributors_DistributorId` ON `ContractDistributors` (`DistributorId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE UNIQUE INDEX `IX_ContractIndustries_ContractId_IndustryId` ON `ContractIndustries` (`ContractId`, `IndustryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE INDEX `IX_ContractIndustries_IndustryId` ON `ContractIndustries` (`IndustryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE UNIQUE INDEX `IX_ContractOpCos_ContractId_OpCoId` ON `ContractOpCos` (`ContractId`, `OpCoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE INDEX `IX_ContractOpCos_OpCoId` ON `ContractOpCos` (`OpCoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE UNIQUE INDEX `IX_ContractVersions_ContractId_VersionNumber` ON `ContractVersions` (`ContractId`, `VersionNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE UNIQUE INDEX `IX_CustomerAccounts_DistributorId_CustomerAccountNumber` ON `CustomerAccounts` (`DistributorId`, `CustomerAccountNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE INDEX `IX_CustomerAccounts_MemberAccountId` ON `CustomerAccounts` (`MemberAccountId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE INDEX `IX_CustomerAccounts_OpCoId` ON `CustomerAccounts` (`OpCoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE UNIQUE INDEX `IX_DistributorProductCodes_DistributorId_ProductId_DistributorC~` ON `DistributorProductCodes` (`DistributorId`, `ProductId`, `DistributorCode`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE INDEX `IX_DistributorProductCodes_ProductId` ON `DistributorProductCodes` (`ProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE INDEX `IX_MemberAccounts_IndustryId` ON `MemberAccounts` (`IndustryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE UNIQUE INDEX `IX_MemberAccounts_MemberNumber` ON `MemberAccounts` (`MemberNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    CREATE INDEX `IX_OpCos_DistributorId` ON `OpCos` (`DistributorId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Contracts` ADD CONSTRAINT `FK_Contracts_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Contracts` ADD CONSTRAINT `FK_Contracts_Manufacturers_ManufacturerId` FOREIGN KEY (`ManufacturerId`) REFERENCES `Manufacturers` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    ALTER TABLE `Users` ADD CONSTRAINT `FK_Users_Industries_IndustryId` FOREIGN KEY (`IndustryId`) REFERENCES `Industries` (`Id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911004018_UpdateSchemaForCompleteDataModel') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250911004018_UpdateSchemaForCompleteDataModel', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    ALTER TABLE `ContractOpCos` RENAME COLUMN `AssignedDate` TO `CreatedDate`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    ALTER TABLE `ContractOpCos` RENAME COLUMN `AssignedBy` TO `CreatedBy`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    ALTER TABLE `ContractIndustries` RENAME COLUMN `AssignedDate` TO `CreatedDate`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    ALTER TABLE `ContractIndustries` RENAME COLUMN `AssignedBy` TO `CreatedBy`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    ALTER TABLE `ContractDistributors` RENAME COLUMN `AssignedDate` TO `CreatedDate`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    ALTER TABLE `ContractDistributors` RENAME COLUMN `AssignedBy` TO `CreatedBy`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-09-11 01:17:56'
    WHERE `Id` = 1;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-09-11 01:17:56'
    WHERE `Id` = 2;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-09-11 01:17:56'
    WHERE `Id` = 3;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-09-11 01:17:56'
    WHERE `Id` = 4;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-09-11 01:17:56'
    WHERE `Id` = 5;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-09-11 01:17:56'
    WHERE `Id` = 1;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-09-11 01:17:56'
    WHERE `Id` = 2;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-09-11 01:17:56'
    WHERE `Id` = 3;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-09-11 01:17:56'
    WHERE `Id` = 4;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-09-11 01:17:56'
    WHERE `Id` = 5;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911011758_ForceCreateMissingTables') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250911011758_ForceCreateMissingTables', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    INSERT INTO `Distributors` (`Id`, `Address`, `City`, `ContactPerson`, `Country`, `CreatedBy`, `CreatedDate`, `Description`, `Email`, `IsActive`, `ModifiedBy`, `ModifiedDate`, `Name`, `PhoneNumber`, `ReceiveContractProposal`, `State`, `Status`, `ZipCode`)
    VALUES (1, '123 Distribution Way, Chicago, IL 60601', NULL, NULL, NULL, 'System', TIMESTAMP '2025-01-01 00:00:00', NULL, NULL, TRUE, NULL, NULL, 'Regional Food Services', '(312) 555-0100', TRUE, NULL, 1, NULL),
    (2, '456 Supply Chain Blvd, Atlanta, GA 30309', NULL, NULL, NULL, 'System', TIMESTAMP '2025-01-01 00:00:00', NULL, NULL, TRUE, NULL, NULL, 'Metro Food Distribution', '(404) 555-0200', TRUE, NULL, 1, NULL),
    (3, '789 Logistics Ave, Dallas, TX 75201', NULL, NULL, NULL, 'System', TIMESTAMP '2025-01-01 00:00:00', NULL, NULL, TRUE, NULL, NULL, 'National Food Partners', '(214) 555-0300', FALSE, NULL, 1, NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-01-01 00:00:00'
    WHERE `Id` = 1;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-01-01 00:00:00'
    WHERE `Id` = 2;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-01-01 00:00:00'
    WHERE `Id` = 3;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-01-01 00:00:00'
    WHERE `Id` = 4;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    UPDATE `Industries` SET `CreatedDate` = TIMESTAMP '2025-01-01 00:00:00'
    WHERE `Id` = 5;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    INSERT INTO `Manufacturers` (`Id`, `AKA`, `Address`, `City`, `ContactPerson`, `Country`, `CreatedBy`, `CreatedDate`, `Description`, `Email`, `IsActive`, `ModifiedBy`, `ModifiedDate`, `Name`, `PhoneNumber`, `PrimaryBroker`, `State`, `Status`, `ZipCode`)
    VALUES (1, 'Sysco', '1390 Enclave Pkwy, Houston, TX 77077', NULL, NULL, NULL, 'System', TIMESTAMP '2025-01-01 00:00:00', NULL, NULL, TRUE, NULL, NULL, 'Sysco Corporation', '(281) 584-1390', 'John Smith', NULL, 1, NULL),
    (2, 'USF', '9399 W Higgins Rd, Rosemont, IL 60018', NULL, NULL, NULL, 'System', TIMESTAMP '2025-01-01 00:00:00', NULL, NULL, TRUE, NULL, NULL, 'US Foods', '(847) 720-8000', 'Jane Doe', NULL, 1, NULL),
    (3, 'PFG', '12500 West Creek Pkwy, Richmond, VA 23238', NULL, NULL, NULL, 'System', TIMESTAMP '2025-01-01 00:00:00', NULL, NULL, TRUE, NULL, NULL, 'Performance Food Group', '(804) 484-7700', 'Mike Johnson', NULL, 1, NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    INSERT INTO `MemberAccounts` (`Id`, `Address`, `BusinessType`, `City`, `ContactPerson`, `Country`, `CreatedBy`, `CreatedDate`, `Email`, `FacilityName`, `IndustryId`, `IsActive`, `MemberNumber`, `ModifiedBy`, `ModifiedDate`, `PhoneNumber`, `State`, `Status`, `TaxId`, `W9`, `ZipCode`)
    VALUES (1, '5801 S Ellis Ave, Chicago, IL 60637', 'Educational Institution', 'Chicago', 'Jennifer Adams', 'USA', 'System', TIMESTAMP '2025-01-01 00:00:00', 'dining@uchicago.edu', 'University of Chicago Dining', 1, TRUE, 'MEM001', NULL, NULL, '(773) 702-1234', 'IL', 1, '36-1234567', TRUE, '60637'),
    (2, '123 School St, Atlanta, GA 30309', 'Public School', 'Atlanta', 'Mary Johnson', 'USA', 'System', TIMESTAMP '2025-01-01 00:00:00', 'cafeteria@lincoln.edu', 'Lincoln Elementary School', 2, TRUE, 'MEM002', NULL, NULL, '(404) 555-5000', 'GA', 1, '58-9876543', TRUE, '30309'),
    (3, '456 Fast Food Blvd, Dallas, TX 75201', 'Restaurant', 'Dallas', 'Carlos Rodriguez', 'USA', 'System', TIMESTAMP '2025-01-01 00:00:00', 'manager@quickbite.com', 'Quick Bite Restaurant', 3, TRUE, 'MEM003', NULL, NULL, '(214) 555-6000', 'TX', 1, '75-1122334', FALSE, '75201'),
    (4, '789 Health Way, Chicago, IL 60611', 'Healthcare Facility', 'Chicago', 'Dr. Patricia Lee', 'USA', 'System', TIMESTAMP '2025-01-01 00:00:00', 'food@generalhospital.org', 'General Hospital Cafeteria', 4, TRUE, 'MEM004', NULL, NULL, '(312) 555-7000', 'IL', 1, '36-5566778', TRUE, '60611');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-01-01 00:00:00'
    WHERE `Id` = 1;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-01-01 00:00:00'
    WHERE `Id` = 2;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-01-01 00:00:00'
    WHERE `Id` = 3;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-01-01 00:00:00'
    WHERE `Id` = 4;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    UPDATE `Roles` SET `CreatedDate` = TIMESTAMP '2025-01-01 00:00:00'
    WHERE `Id` = 5;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    INSERT INTO `OpCos` (`Id`, `Address`, `City`, `ContactPerson`, `Country`, `CreatedBy`, `CreatedDate`, `DistributorId`, `Email`, `IsActive`, `ModifiedBy`, `ModifiedDate`, `Name`, `PhoneNumber`, `RemoteReferenceCode`, `State`, `Status`, `ZipCode`)
    VALUES (1, '100 N Michigan Ave, Chicago, IL 60601', 'Chicago', 'Sarah Wilson', 'USA', 'System', TIMESTAMP '2025-01-01 00:00:00', 1, 'chicago@regionalfood.com', TRUE, NULL, NULL, 'Chicago Operations', '(312) 555-1001', 'CHI001', 'IL', 1, '60601'),
    (2, '200 Peachtree St, Atlanta, GA 30303', 'Atlanta', 'Robert Brown', 'USA', 'System', TIMESTAMP '2025-01-01 00:00:00', 2, 'atlanta@metrofood.com', TRUE, NULL, NULL, 'Atlanta Hub', '(404) 555-2001', 'ATL001', 'GA', 1, '30303'),
    (3, '300 Main St, Dallas, TX 75202', 'Dallas', 'Lisa Garcia', 'USA', 'System', TIMESTAMP '2025-01-01 00:00:00', 3, 'dallas@nationalfood.com', TRUE, NULL, NULL, 'Dallas Center', '(214) 555-3001', 'DAL001', 'TX', 1, '75202'),
    (4, '400 W Lake St, Chicago, IL 60606', 'Chicago', 'David Miller', 'USA', 'System', TIMESTAMP '2025-01-01 00:00:00', 1, 'west@regionalfood.com', TRUE, NULL, NULL, 'Chicago West', '(312) 555-1002', 'CHI002', 'IL', 1, '60606');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    INSERT INTO `Products` (`Id`, `Category`, `CostPrice`, `CreatedBy`, `CreatedDate`, `Description`, `GTIN`, `IsActive`, `ListPrice`, `ManufacturerId`, `ManufacturerProductCode`, `ModifiedBy`, `ModifiedDate`, `Name`, `PackSize`, `SKU`, `Status`, `SubCategory`, `UPC`, `UnitOfMeasure`, `UnitPrice`)
    VALUES (1, 'Meat', 38.5, 'System', TIMESTAMP '2025-01-01 00:00:00', 'Premium Ground Beef 80/20', '1234567890123', TRUE, 45.99, 1, 'SYSCO-BEEF-001', NULL, NULL, '', '10 lb', 'SYS001', 1, 'Ground Beef', '123456789012', NULL, NULL),
    (2, 'Meat', 27.5, 'System', TIMESTAMP '2025-01-01 00:00:00', 'Boneless Chicken Breast', '2345678901234', TRUE, 32.99, 2, 'USF-CHICKEN-001', NULL, NULL, '', '5 lb', 'USF001', 1, 'Chicken', '234567890123', NULL, NULL),
    (3, 'Produce', 15.25, 'System', TIMESTAMP '2025-01-01 00:00:00', 'Fresh Romaine Lettuce', '3456789012345', TRUE, 18.99, 3, 'PFG-PRODUCE-001', NULL, NULL, '', '24 count', 'PFG001', 1, 'Lettuce', '345678901234', NULL, NULL),
    (4, 'Dairy', 10.75, 'System', TIMESTAMP '2025-01-01 00:00:00', 'Whole Milk Gallon', '4567890123456', TRUE, 12.99, 1, 'SYSCO-DAIRY-001', NULL, NULL, '', '4 gallons', 'SYS002', 1, 'Milk', '456789012345', NULL, NULL),
    (5, 'Frozen', 20.5, 'System', TIMESTAMP '2025-01-01 00:00:00', 'Frozen French Fries', '5678901234567', TRUE, 24.99, 2, 'USF-FROZEN-001', NULL, NULL, '', '6/5 lb', 'USF002', 1, 'Potato Products', '567890123456', NULL, NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    INSERT INTO `CustomerAccounts` (`Id`, `Address`, `City`, `ContactPerson`, `Country`, `CreatedBy`, `CreatedDate`, `CreditLimit`, `CurrentBalance`, `CustomerAccountNumber`, `CustomerName`, `DistributorId`, `Email`, `IsActive`, `MemberAccountId`, `ModifiedBy`, `ModifiedDate`, `OpCoId`, `PhoneNumber`, `State`, `Status`, `ZipCode`)
    VALUES (1, '5801 S Ellis Ave, Chicago, IL 60637', 'Chicago', 'Jennifer Adams', 'USA', 'System', TIMESTAMP '2025-01-01 00:00:00', 50000.0, 12500.0, 'CUST001', 'UChicago Main Dining', 1, 'dining@uchicago.edu', TRUE, 1, NULL, NULL, 1, '(773) 702-1234', 'IL', 1, '60637'),
    (2, '123 School St, Atlanta, GA 30309', 'Atlanta', 'Mary Johnson', 'USA', 'System', TIMESTAMP '2025-01-01 00:00:00', 25000.0, 5000.0, 'CUST002', 'Lincoln School Cafeteria', 2, 'cafeteria@lincoln.edu', TRUE, 2, NULL, NULL, 2, '(404) 555-5000', 'GA', 1, '30309'),
    (3, '456 Fast Food Blvd, Dallas, TX 75201', 'Dallas', 'Carlos Rodriguez', 'USA', 'System', TIMESTAMP '2025-01-01 00:00:00', 15000.0, 3200.0, 'CUST003', 'Quick Bite Main Location', 3, 'manager@quickbite.com', TRUE, 3, NULL, NULL, 3, '(214) 555-6000', 'TX', 1, '75201'),
    (4, '789 Health Way, Chicago, IL 60611', 'Chicago', 'Dr. Patricia Lee', 'USA', 'System', TIMESTAMP '2025-01-01 00:00:00', 75000.0, 18750.0, 'CUST004', 'General Hospital Food Service', 1, 'food@generalhospital.org', TRUE, 4, NULL, NULL, 4, '(312) 555-7000', 'IL', 1, '60611');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911012256_AddSampleData') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250911012256_AddSampleData', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911013146_ForceRecreateMainTables') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250911013146_ForceRecreateMainTables', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911014502_AddContractSampleData') THEN

    INSERT INTO `Contracts` (`Id`, `ContractNumber`, `CreatedBy`, `CreatedDate`, `CurrentVersionNumber`, `Description`, `DistributorId`, `EndDate`, `IsSuspended`, `ManufacturerId`, `ModifiedBy`, `ModifiedDate`, `Notes`, `SendToPerformance`, `StartDate`, `Status`, `Terms`, `Title`, `TotalValue`)
    VALUES (1, 'CNT-2025-001', 'System', TIMESTAMP '2025-01-01 00:00:00', 1, 'Comprehensive food service contract for university dining facilities', NULL, TIMESTAMP '2025-12-31 00:00:00', FALSE, 1, NULL, NULL, NULL, TRUE, TIMESTAMP '2025-01-01 00:00:00', 3, NULL, 'University Food Service Contract 2025', 500000.0),
    (2, 'CNT-2025-002', 'System', TIMESTAMP '2025-01-01 00:00:00', 1, 'Food service agreement for K-12 school district cafeterias', NULL, TIMESTAMP '2026-01-31 00:00:00', FALSE, 2, NULL, NULL, NULL, TRUE, TIMESTAMP '2025-02-01 00:00:00', 3, NULL, 'K-12 School District Agreement', 750000.0),
    (3, 'CNT-2025-003', 'System', TIMESTAMP '2025-01-01 00:00:00', 1, 'Food service contract for healthcare facilities and hospitals', NULL, TIMESTAMP '2025-08-31 00:00:00', FALSE, 3, NULL, NULL, NULL, FALSE, TIMESTAMP '2025-03-01 00:00:00', 3, NULL, 'Healthcare Facilities Contract', 300000.0);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911014502_AddContractSampleData') THEN

    INSERT INTO `ContractDistributors` (`Id`, `ContractId`, `CreatedBy`, `CreatedDate`, `DistributorId`, `IsActive`)
    VALUES (1, 1, 'System', TIMESTAMP '2025-01-01 00:00:00', 1, TRUE),
    (2, 1, 'System', TIMESTAMP '2025-01-01 00:00:00', 2, TRUE),
    (3, 2, 'System', TIMESTAMP '2025-01-01 00:00:00', 2, TRUE),
    (4, 3, 'System', TIMESTAMP '2025-01-01 00:00:00', 3, TRUE);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911014502_AddContractSampleData') THEN

    INSERT INTO `ContractIndustries` (`Id`, `ContractId`, `CreatedBy`, `CreatedDate`, `IndustryId`, `IsActive`)
    VALUES (1, 1, 'System', TIMESTAMP '2025-01-01 00:00:00', 1, TRUE),
    (2, 2, 'System', TIMESTAMP '2025-01-01 00:00:00', 2, TRUE),
    (3, 3, 'System', TIMESTAMP '2025-01-01 00:00:00', 4, TRUE);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911014502_AddContractSampleData') THEN

    INSERT INTO `ContractOpCos` (`Id`, `ContractId`, `CreatedBy`, `CreatedDate`, `IsActive`, `OpCoId`)
    VALUES (1, 1, 'System', TIMESTAMP '2025-01-01 00:00:00', TRUE, 1),
    (2, 1, 'System', TIMESTAMP '2025-01-01 00:00:00', TRUE, 4),
    (3, 2, 'System', TIMESTAMP '2025-01-01 00:00:00', TRUE, 2),
    (4, 3, 'System', TIMESTAMP '2025-01-01 00:00:00', TRUE, 3);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250911014502_AddContractSampleData') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250911014502_AddContractSampleData', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `MemberAccounts` ADD `AuditDate` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `MemberAccounts` ADD `ClientGroupEnrollment` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `MemberAccounts` ADD `ClientGroupNumber` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `MemberAccounts` ADD `EntegraGPONumber` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `MemberAccounts` ADD `EntegraIdNumber` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `MemberAccounts` ADD `InternalNotes` text CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `MemberAccounts` ADD `LopDate` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `MemberAccounts` ADD `ParentMemberAccountNumber` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `MemberAccounts` ADD `PayType` int NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `MemberAccounts` ADD `SalesforceAccountName` varchar(200) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `MemberAccounts` ADD `VMAPNumber` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `MemberAccounts` ADD `VMSupplierName` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `MemberAccounts` ADD `VMSupplierSite` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `CustomerAccounts` ADD `Association` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `CustomerAccounts` ADD `AuditDate` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `CustomerAccounts` ADD `CombinedUniqueID` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `CustomerAccounts` ADD `DSO` int NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `CustomerAccounts` ADD `DateToEntegra` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `CustomerAccounts` ADD `EndDate` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `CustomerAccounts` ADD `InternalNotes` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `CustomerAccounts` ADD `Markup` decimal(18,2) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `CustomerAccounts` ADD `SalesRep` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `CustomerAccounts` ADD `StartDate` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `CustomerAccounts` ADD `TRACSAccess` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    ALTER TABLE `CustomerAccounts` ADD `ToEntegra` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    UPDATE `CustomerAccounts` SET `Association` = 1, `AuditDate` = NULL, `CombinedUniqueID` = NULL, `DSO` = NULL, `DateToEntegra` = NULL, `EndDate` = NULL, `InternalNotes` = NULL, `Markup` = NULL, `SalesRep` = NULL, `StartDate` = NULL, `TRACSAccess` = FALSE, `ToEntegra` = FALSE
    WHERE `Id` = 1;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    UPDATE `CustomerAccounts` SET `Association` = 1, `AuditDate` = NULL, `CombinedUniqueID` = NULL, `DSO` = NULL, `DateToEntegra` = NULL, `EndDate` = NULL, `InternalNotes` = NULL, `Markup` = NULL, `SalesRep` = NULL, `StartDate` = NULL, `TRACSAccess` = FALSE, `ToEntegra` = FALSE
    WHERE `Id` = 2;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    UPDATE `CustomerAccounts` SET `Association` = 1, `AuditDate` = NULL, `CombinedUniqueID` = NULL, `DSO` = NULL, `DateToEntegra` = NULL, `EndDate` = NULL, `InternalNotes` = NULL, `Markup` = NULL, `SalesRep` = NULL, `StartDate` = NULL, `TRACSAccess` = FALSE, `ToEntegra` = FALSE
    WHERE `Id` = 3;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    UPDATE `CustomerAccounts` SET `Association` = 1, `AuditDate` = NULL, `CombinedUniqueID` = NULL, `DSO` = NULL, `DateToEntegra` = NULL, `EndDate` = NULL, `InternalNotes` = NULL, `Markup` = NULL, `SalesRep` = NULL, `StartDate` = NULL, `TRACSAccess` = FALSE, `ToEntegra` = FALSE
    WHERE `Id` = 4;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    UPDATE `MemberAccounts` SET `AuditDate` = NULL, `ClientGroupEnrollment` = FALSE, `ClientGroupNumber` = NULL, `EntegraGPONumber` = NULL, `EntegraIdNumber` = NULL, `InternalNotes` = NULL, `LopDate` = NULL, `ParentMemberAccountNumber` = NULL, `PayType` = 1, `SalesforceAccountName` = '', `VMAPNumber` = NULL, `VMSupplierName` = NULL, `VMSupplierSite` = NULL
    WHERE `Id` = 1;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    UPDATE `MemberAccounts` SET `AuditDate` = NULL, `ClientGroupEnrollment` = FALSE, `ClientGroupNumber` = NULL, `EntegraGPONumber` = NULL, `EntegraIdNumber` = NULL, `InternalNotes` = NULL, `LopDate` = NULL, `ParentMemberAccountNumber` = NULL, `PayType` = 1, `SalesforceAccountName` = '', `VMAPNumber` = NULL, `VMSupplierName` = NULL, `VMSupplierSite` = NULL
    WHERE `Id` = 2;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    UPDATE `MemberAccounts` SET `AuditDate` = NULL, `ClientGroupEnrollment` = FALSE, `ClientGroupNumber` = NULL, `EntegraGPONumber` = NULL, `EntegraIdNumber` = NULL, `InternalNotes` = NULL, `LopDate` = NULL, `ParentMemberAccountNumber` = NULL, `PayType` = 1, `SalesforceAccountName` = '', `VMAPNumber` = NULL, `VMSupplierName` = NULL, `VMSupplierSite` = NULL
    WHERE `Id` = 3;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    UPDATE `MemberAccounts` SET `AuditDate` = NULL, `ClientGroupEnrollment` = FALSE, `ClientGroupNumber` = NULL, `EntegraGPONumber` = NULL, `EntegraIdNumber` = NULL, `InternalNotes` = NULL, `LopDate` = NULL, `ParentMemberAccountNumber` = NULL, `PayType` = 1, `SalesforceAccountName` = '', `VMAPNumber` = NULL, `VMSupplierName` = NULL, `VMSupplierSite` = NULL
    WHERE `Id` = 4;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912115917_EnsureCustomerAccountExtendedFields') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250912115917_EnsureCustomerAccountExtendedFields', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912122826_AddContractsIndustryForeignSuspended') THEN

    ALTER TABLE `Contracts` MODIFY COLUMN `CreatedBy` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912122826_AddContractsIndustryForeignSuspended') THEN

    ALTER TABLE `Contracts` ADD `ForeignContractID` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912122826_AddContractsIndustryForeignSuspended') THEN

    ALTER TABLE `Contracts` ADD `IndustryId` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912122826_AddContractsIndustryForeignSuspended') THEN

    ALTER TABLE `Contracts` ADD `SuspendedDate` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912122826_AddContractsIndustryForeignSuspended') THEN

    UPDATE `Contracts` SET `ForeignContractID` = NULL, `IndustryId` = 1, `SuspendedDate` = NULL
    WHERE `Id` = 1;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912122826_AddContractsIndustryForeignSuspended') THEN

    UPDATE `Contracts` SET `ForeignContractID` = NULL, `IndustryId` = 2, `SuspendedDate` = NULL
    WHERE `Id` = 2;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912122826_AddContractsIndustryForeignSuspended') THEN

    UPDATE `Contracts` SET `ForeignContractID` = NULL, `IndustryId` = 4, `SuspendedDate` = NULL
    WHERE `Id` = 3;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912122826_AddContractsIndustryForeignSuspended') THEN

    CREATE INDEX `IX_Contracts_IndustryId` ON `Contracts` (`IndustryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912122826_AddContractsIndustryForeignSuspended') THEN

    ALTER TABLE `Contracts` ADD CONSTRAINT `FK_Contracts_Industries_IndustryId` FOREIGN KEY (`IndustryId`) REFERENCES `Industries` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912122826_AddContractsIndustryForeignSuspended') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250912122826_AddContractsIndustryForeignSuspended', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912134941_AddUserContactStatusFields') THEN

    ALTER TABLE `Users` ADD `Address` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912134941_AddUserContactStatusFields') THEN

    ALTER TABLE `Users` ADD `City` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912134941_AddUserContactStatusFields') THEN

    ALTER TABLE `Users` ADD `FailedAuthAttempts` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912134941_AddUserContactStatusFields') THEN

    ALTER TABLE `Users` ADD `Notes` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912134941_AddUserContactStatusFields') THEN

    ALTER TABLE `Users` ADD `PostCode` varchar(20) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912134941_AddUserContactStatusFields') THEN

    ALTER TABLE `Users` ADD `State` varchar(50) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912134941_AddUserContactStatusFields') THEN

    ALTER TABLE `Users` ADD `Status` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250912134941_AddUserContactStatusFields') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250912134941_AddUserContactStatusFields', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917120000_AddDistributorProductCodeExtensions') THEN

    ALTER TABLE `DistributorProductCodes` ADD `CatchWeight` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917120000_AddDistributorProductCodeExtensions') THEN

    ALTER TABLE `DistributorProductCodes` ADD `EBrand` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917120000_AddDistributorProductCodeExtensions') THEN

    SET @idx := (SELECT INDEX_NAME FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND INDEX_NAME = 'IX_DistributorProductCodes_DistributorId_ProductId_DistributorCode' LIMIT 1);
                                      SET @sql := IF(@idx IS NOT NULL, 'DROP INDEX IX_DistributorProductCodes_DistributorId_ProductId_DistributorCode ON DistributorProductCodes;', 'SELECT 1');
                                      PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917120000_AddDistributorProductCodeExtensions') THEN

    CREATE UNIQUE INDEX `IX_DistributorProductCodes_DistributorId_DistributorCode` ON `DistributorProductCodes` (`DistributorId`, `DistributorCode`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917120000_AddDistributorProductCodeExtensions') THEN

    CREATE UNIQUE INDEX `IX_DistributorProductCodes_DistributorId_ProductId` ON `DistributorProductCodes` (`DistributorId`, `ProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917120000_AddDistributorProductCodeExtensions') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250917120000_AddDistributorProductCodeExtensions', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917121500_AddAuditAliasesForDistributorProductCodes') THEN

    SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'CreatedAt');
                                      SET @sql := IF(@col = 0, 'ALTER TABLE DistributorProductCodes ADD COLUMN CreatedAt datetime(6) NULL;', 'SELECT 1');
                                      PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917121500_AddAuditAliasesForDistributorProductCodes') THEN

    SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'CreatedAt');
                                      SET @sql := IF(@col = 1, 'UPDATE DistributorProductCodes SET CreatedAt = IFNULL(CreatedDate, NOW()) WHERE CreatedAt IS NULL;', 'SELECT 1');
                                      PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917121500_AddAuditAliasesForDistributorProductCodes') THEN

    SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'UpdatedAt');
                                      SET @sql := IF(@col = 0, 'ALTER TABLE DistributorProductCodes ADD COLUMN UpdatedAt datetime(6) NULL;', 'SELECT 1');
                                      PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917121500_AddAuditAliasesForDistributorProductCodes') THEN

    SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'UpdatedAt');
                                      SET @sql := IF(@col = 1, 'UPDATE DistributorProductCodes SET UpdatedAt = ModifiedDate WHERE UpdatedAt IS NULL;', 'SELECT 1');
                                      PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917121500_AddAuditAliasesForDistributorProductCodes') THEN

    SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'UpdatedBy');
                                      SET @sql := IF(@col = 0, 'ALTER TABLE DistributorProductCodes ADD COLUMN UpdatedBy varchar(100) NULL;', 'SELECT 1');
                                      PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917121500_AddAuditAliasesForDistributorProductCodes') THEN

    SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'UpdatedBy');
                                      SET @sql := IF(@col = 1, 'UPDATE DistributorProductCodes SET UpdatedBy = ModifiedBy WHERE UpdatedBy IS NULL;', 'SELECT 1');
                                      PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917121500_AddAuditAliasesForDistributorProductCodes') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250917121500_AddAuditAliasesForDistributorProductCodes', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917152223_AddUserManufacturersTable') THEN

    CREATE TABLE `UserManufacturers` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `AssignedDate` datetime(6) NOT NULL DEFAULT NOW(6),
        `ManufacturerId` int NOT NULL,
        `UserId` int NOT NULL,
        CONSTRAINT `PK_UserManufacturers` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_UserManufacturers_Manufacturers_ManufacturerId` FOREIGN KEY (`ManufacturerId`) REFERENCES `Manufacturers` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_UserManufacturers_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917152223_AddUserManufacturersTable') THEN

    CREATE INDEX `IX_UserManufacturers_ManufacturerId` ON `UserManufacturers` (`ManufacturerId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917152223_AddUserManufacturersTable') THEN

    CREATE INDEX `IX_UserManufacturers_UserId` ON `UserManufacturers` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250917152223_AddUserManufacturersTable') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250917152223_AddUserManufacturersTable', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

