DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID TINYINT(1);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `Extra` = 'auto_increment'
			AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS `POMELO_AFTER_ADD_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID INT(11);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
			AND `COLUMN_TYPE` LIKE '%int%'
			AND `COLUMN_KEY` = 'PRI';
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

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

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923142931_UpdateContractsSchema_20250923') THEN

    ALTER TABLE `Contracts` RENAME COLUMN `ForeignContractID` TO `ForeignContractId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923142931_UpdateContractsSchema_20250923') THEN

    ALTER TABLE `Contracts` ADD `InternalNotes` varchar(2000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923142931_UpdateContractsSchema_20250923') THEN

    ALTER TABLE `Contracts` ADD `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923142931_UpdateContractsSchema_20250923') THEN

    UPDATE `Contracts` SET `InternalNotes` = NULL, `Name` = ''
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923142931_UpdateContractsSchema_20250923') THEN

    UPDATE `Contracts` SET `InternalNotes` = NULL, `Name` = ''
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923142931_UpdateContractsSchema_20250923') THEN

    UPDATE `Contracts` SET `InternalNotes` = NULL, `Name` = ''
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923142931_UpdateContractsSchema_20250923') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250923142931_UpdateContractsSchema_20250923', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

    ALTER TABLE `Contracts` DROP FOREIGN KEY `FK_Contracts_Manufacturers_ManufacturerId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

    ALTER TABLE `Contracts` DROP INDEX `IX_Contracts_DistributorId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

    ALTER TABLE `Contracts` DROP INDEX `IX_Contracts_ManufacturerId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

    ALTER TABLE `Contracts` DROP INDEX `IX_Contracts_ContractNumber`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

    ALTER TABLE `Contracts` DROP COLUMN `ContractNumber`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

    ALTER TABLE `Contracts` DROP COLUMN `Title`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

    ALTER TABLE `Contracts` DROP COLUMN `DistributorId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

    ALTER TABLE `Contracts` DROP COLUMN `ManufacturerId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

    ALTER TABLE `Contracts` DROP COLUMN `TotalValue`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

    ALTER TABLE `Contracts` DROP COLUMN `Terms`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

    ALTER TABLE `Contracts` DROP COLUMN `Notes`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

    ALTER TABLE `Contracts` DROP COLUMN `Status`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923152942_RemoveObsoleteColumnsFromContracts') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250923152942_RemoveObsoleteColumnsFromContracts', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    ALTER TABLE `ContractOpCos` ADD `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    ALTER TABLE `ContractOpCos` ADD `AssignedDate` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    ALTER TABLE `ContractOpCos` ADD `CurrentVersionNumber` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    ALTER TABLE `ContractIndustries` ADD `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    ALTER TABLE `ContractIndustries` ADD `AssignedDate` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    ALTER TABLE `ContractIndustries` ADD `CurrentVersionNumber` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    ALTER TABLE `ContractDistributors` ADD `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    ALTER TABLE `ContractDistributors` ADD `AssignedDate` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    ALTER TABLE `ContractDistributors` ADD `CurrentVersionNumber` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    CREATE TABLE `ContractManufacturers` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `ManufacturerId` int NOT NULL,
        `CurrentVersionNumber` int NOT NULL,
        `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `AssignedDate` datetime(6) NULL,
        CONSTRAINT `PK_ContractManufacturers` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ContractManufacturers_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ContractManufacturers_Manufacturers_ManufacturerId` FOREIGN KEY (`ManufacturerId`) REFERENCES `Manufacturers` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    UPDATE `ContractDistributors` SET `AssignedBy` = NULL, `AssignedDate` = NULL, `CurrentVersionNumber` = 1
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    UPDATE `ContractDistributors` SET `AssignedBy` = NULL, `AssignedDate` = NULL, `CurrentVersionNumber` = 1
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    UPDATE `ContractDistributors` SET `AssignedBy` = NULL, `AssignedDate` = NULL, `CurrentVersionNumber` = 1
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    UPDATE `ContractDistributors` SET `AssignedBy` = NULL, `AssignedDate` = NULL, `CurrentVersionNumber` = 1
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    UPDATE `ContractIndustries` SET `AssignedBy` = NULL, `AssignedDate` = NULL, `CurrentVersionNumber` = 1
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    UPDATE `ContractIndustries` SET `AssignedBy` = NULL, `AssignedDate` = NULL, `CurrentVersionNumber` = 1
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    UPDATE `ContractIndustries` SET `AssignedBy` = NULL, `AssignedDate` = NULL, `CurrentVersionNumber` = 1
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    UPDATE `ContractOpCos` SET `AssignedBy` = NULL, `AssignedDate` = NULL, `CurrentVersionNumber` = 1
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    UPDATE `ContractOpCos` SET `AssignedBy` = NULL, `AssignedDate` = NULL, `CurrentVersionNumber` = 1
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    UPDATE `ContractOpCos` SET `AssignedBy` = NULL, `AssignedDate` = NULL, `CurrentVersionNumber` = 1
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    UPDATE `ContractOpCos` SET `AssignedBy` = NULL, `AssignedDate` = NULL, `CurrentVersionNumber` = 1
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    CREATE UNIQUE INDEX `IX_ContractManufacturers_ContractId_ManufacturerId` ON `ContractManufacturers` (`ContractId`, `ManufacturerId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    CREATE INDEX `IX_ContractManufacturers_ManufacturerId` ON `ContractManufacturers` (`ManufacturerId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923160332_AddContractRelationshipTables') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250923160332_AddContractRelationshipTables', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    ALTER TABLE `ContractVersions` DROP COLUMN `ChangeReason`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    ALTER TABLE `ContractVersions` DROP COLUMN `Description`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    ALTER TABLE `ContractVersions` DROP COLUMN `Notes`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    ALTER TABLE `ContractVersions` DROP COLUMN `Terms`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    ALTER TABLE `ContractVersions` DROP COLUMN `TotalValue`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    ALTER TABLE `ContractVersions` RENAME COLUMN `Title` TO `Name`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    ALTER TABLE `ContractVersions` DROP COLUMN `IsCurrentVersion`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    ALTER TABLE `ContractVersions` DROP COLUMN `IsActive`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    ALTER TABLE `ContractVersions` ADD `SendToPerformance` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    ALTER TABLE `ContractVersions` ADD `IsSuspended` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    ALTER TABLE `ContractVersions` ADD `ForeignContractId` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    ALTER TABLE `ContractVersions` ADD `InternalNotes` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    ALTER TABLE `ContractVersions` ADD `SuspendedDate` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923181058_UpdateContractVersionsSchema') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250923181058_UpdateContractVersionsSchema', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923203108_RefactorContractProductAddContractPriceTable') THEN

    ALTER TABLE `ContractProducts` DROP COLUMN `ContractPrice`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923203108_RefactorContractProductAddContractPriceTable') THEN

    CREATE TABLE `ContractPrices` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `VersionNumber` int NOT NULL,
        `ProductId` int NOT NULL,
        `PriceType` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Allowance` decimal(18,4) NULL,
        `CommercialDelPrice` decimal(18,4) NULL,
        `CommercialFobPrice` decimal(18,4) NULL,
        `CommodityDelPrice` decimal(18,4) NULL,
        `CommodityFobPrice` decimal(18,4) NULL,
        `UOM` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `EstimatedQty` decimal(18,4) NULL,
        `BillbacksAllowed` tinyint(1) NOT NULL,
        `PUA` decimal(18,4) NULL,
        `FFSPrice` decimal(18,4) NULL,
        `NOIPrice` decimal(18,4) NULL,
        `PTV` decimal(18,4) NULL,
        `InternalNotes` longtext CHARACTER SET utf8mb4 NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_ContractPrices` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ContractPrices_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923203108_RefactorContractProductAddContractPriceTable') THEN

    CREATE INDEX `IX_ContractPrices_ProductId_VersionNumber` ON `ContractPrices` (`ProductId`, `VersionNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923203108_RefactorContractProductAddContractPriceTable') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250923203108_RefactorContractProductAddContractPriceTable', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    ALTER TABLE `ContractVersions` DROP COLUMN `CreatedBy`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    ALTER TABLE `ContractVersions` DROP COLUMN `CreatedDate`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    ALTER TABLE `ContractVersions` RENAME COLUMN `ModifiedDate` TO `AssignedDate`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    ALTER TABLE `ContractVersions` RENAME COLUMN `ModifiedBy` TO `AssignedBy`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE TABLE `ContractDistributorsVersion` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `DistributorId` int NOT NULL,
        `VersionNumber` int NOT NULL,
        `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `AssignedDate` datetime(6) NULL,
        CONSTRAINT `PK_ContractDistributorsVersion` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ContractDistributorsVersion_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ContractDistributorsVersion_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE TABLE `ContractIndustriesVersion` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `IndustryId` int NOT NULL,
        `VersionNumber` int NOT NULL,
        `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `AssignedDate` datetime(6) NULL,
        CONSTRAINT `PK_ContractIndustriesVersion` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ContractIndustriesVersion_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ContractIndustriesVersion_Industries_IndustryId` FOREIGN KEY (`IndustryId`) REFERENCES `Industries` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE TABLE `ContractManufacturersVersion` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `ManufacturerId` int NOT NULL,
        `VersionNumber` int NOT NULL,
        `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `AssignedDate` datetime(6) NULL,
        CONSTRAINT `PK_ContractManufacturersVersion` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ContractManufacturersVersion_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ContractManufacturersVersion_Manufacturers_ManufacturerId` FOREIGN KEY (`ManufacturerId`) REFERENCES `Manufacturers` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE TABLE `ContractOpCosVersion` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `OpCoId` int NOT NULL,
        `VersionNumber` int NOT NULL,
        `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `AssignedDate` datetime(6) NULL,
        CONSTRAINT `PK_ContractOpCosVersion` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ContractOpCosVersion_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ContractOpCosVersion_OpCos_OpCoId` FOREIGN KEY (`OpCoId`) REFERENCES `OpCos` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE TABLE `ContractVersionPrice` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `PriceId` int NOT NULL,
        `PriceType` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Allowance` decimal(18,4) NULL,
        `CommercialDelPrice` decimal(18,4) NULL,
        `CommercialFobPrice` decimal(18,4) NULL,
        `CommodityDelPrice` decimal(18,4) NULL,
        `CommodityFobPrice` decimal(18,4) NULL,
        `UOM` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `EstimatedQty` decimal(18,4) NULL,
        `BillbacksAllowed` tinyint(1) NOT NULL DEFAULT FALSE,
        `PUA` decimal(18,4) NULL,
        `FFSPrice` decimal(18,4) NULL,
        `NOIPrice` decimal(18,4) NULL,
        `PTV` decimal(18,4) NULL,
        `InternalNotes` longtext CHARACTER SET utf8mb4 NULL,
        `VersionNumber` int NOT NULL,
        `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `AssignedDate` datetime(6) NULL,
        CONSTRAINT `PK_ContractVersionPrice` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ContractVersionPrice_ContractPrices_PriceId` FOREIGN KEY (`PriceId`) REFERENCES `ContractPrices` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ContractVersionPrice_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE TABLE `ContractVersionProduct` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ContractId` int NOT NULL,
        `ProductId` int NOT NULL,
        `VersionNumber` int NOT NULL,
        `AssignedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `AssignedDate` datetime(6) NULL,
        CONSTRAINT `PK_ContractVersionProduct` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ContractVersionProduct_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ContractVersionProduct_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE INDEX `IX_ContractDistributorsVersion_ContractId_DistributorId_Version~` ON `ContractDistributorsVersion` (`ContractId`, `DistributorId`, `VersionNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE INDEX `IX_ContractDistributorsVersion_DistributorId` ON `ContractDistributorsVersion` (`DistributorId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE INDEX `IX_ContractIndustriesVersion_ContractId_IndustryId_VersionNumber` ON `ContractIndustriesVersion` (`ContractId`, `IndustryId`, `VersionNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE INDEX `IX_ContractIndustriesVersion_IndustryId` ON `ContractIndustriesVersion` (`IndustryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE INDEX `IX_ContractManufacturersVersion_ContractId_ManufacturerId_Versi~` ON `ContractManufacturersVersion` (`ContractId`, `ManufacturerId`, `VersionNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE INDEX `IX_ContractManufacturersVersion_ManufacturerId` ON `ContractManufacturersVersion` (`ManufacturerId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE INDEX `IX_ContractOpCosVersion_ContractId_OpCoId_VersionNumber` ON `ContractOpCosVersion` (`ContractId`, `OpCoId`, `VersionNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE INDEX `IX_ContractOpCosVersion_OpCoId` ON `ContractOpCosVersion` (`OpCoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE INDEX `IX_ContractVersionPrice_ContractId_PriceId_VersionNumber` ON `ContractVersionPrice` (`ContractId`, `PriceId`, `VersionNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE INDEX `IX_ContractVersionPrice_PriceId` ON `ContractVersionPrice` (`PriceId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE INDEX `IX_ContractVersionProduct_ContractId_ProductId_VersionNumber` ON `ContractVersionProduct` (`ContractId`, `ProductId`, `VersionNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    CREATE INDEX `IX_ContractVersionProduct_ProductId` ON `ContractVersionProduct` (`ProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250923235108_AddContractVersionRelationshipTables') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250923235108_AddContractVersionRelationshipTables', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250924174303_RemoveDescriptionAndIndustryFromContracts') THEN

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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250924174303_RemoveDescriptionAndIndustryFromContracts') THEN

    ALTER TABLE `Contracts` DROP INDEX `IX_Contracts_IndustryId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250924174303_RemoveDescriptionAndIndustryFromContracts') THEN

    ALTER TABLE `Contracts` DROP COLUMN `Description`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250924174303_RemoveDescriptionAndIndustryFromContracts') THEN

    ALTER TABLE `Contracts` DROP COLUMN `IndustryId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250924174303_RemoveDescriptionAndIndustryFromContracts') THEN

    ALTER TABLE `Contracts` MODIFY COLUMN `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250924174303_RemoveDescriptionAndIndustryFromContracts') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250924174303_RemoveDescriptionAndIndustryFromContracts', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250926052105_AddContractIdToContractPrices') THEN

    ALTER TABLE `ContractPrices` ADD `ContractId` int NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250926052105_AddContractIdToContractPrices') THEN

    UPDATE ContractPrices cp
    JOIN ContractVersionPrice cvp ON cvp.PriceId = cp.Id
    SET cp.ContractId = cvp.ContractId
    WHERE cp.ContractId IS NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250926052105_AddContractIdToContractPrices') THEN

    ALTER TABLE `ContractPrices` MODIFY COLUMN `ContractId` int NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250926052105_AddContractIdToContractPrices') THEN

    CREATE INDEX `IX_ContractPrices_ContractId_VersionNumber` ON `ContractPrices` (`ContractId`, `VersionNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250926052105_AddContractIdToContractPrices') THEN

    ALTER TABLE `ContractPrices` ADD CONSTRAINT `FK_ContractPrices_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250926052105_AddContractIdToContractPrices') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250926052105_AddContractIdToContractPrices', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250926062216_AddProductIdToContractVersionPrice') THEN

    ALTER TABLE `ContractVersionPrice` ADD `ProductId` int NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250926062216_AddProductIdToContractVersionPrice') THEN

    UPDATE ContractVersionPrice cvp
    JOIN ContractPrices cp ON cp.Id = cvp.PriceId
    SET cvp.ProductId = cp.ProductId
    WHERE cvp.ProductId IS NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250926062216_AddProductIdToContractVersionPrice') THEN

    ALTER TABLE `ContractVersionPrice` MODIFY COLUMN `ProductId` int NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250926062216_AddProductIdToContractVersionPrice') THEN

    CREATE INDEX `IX_ContractVersionPrice_ContractId_ProductId_VersionNumber` ON `ContractVersionPrice` (`ContractId`, `ProductId`, `VersionNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250926062216_AddProductIdToContractVersionPrice') THEN

    CREATE INDEX `IX_ContractVersionPrice_ProductId` ON `ContractVersionPrice` (`ProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250926062216_AddProductIdToContractVersionPrice') THEN

    ALTER TABLE `ContractVersionPrice` ADD CONSTRAINT `FK_ContractVersionPrice_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250926062216_AddProductIdToContractVersionPrice') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250926062216_AddProductIdToContractVersionPrice', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250929220305_AddCurrentVersionNumberToContractProducts') THEN

    ALTER TABLE `ContractProducts` ADD `CurrentVersionNumber` int NOT NULL DEFAULT 1;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250929220305_AddCurrentVersionNumberToContractProducts') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250929220305_AddCurrentVersionNumberToContractProducts', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE TABLE `amendment_actions` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        CONSTRAINT `PK_amendment_actions` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE TABLE `price_types` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        CONSTRAINT `PK_price_types` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE TABLE `product_proposal_statuses` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        CONSTRAINT `PK_product_proposal_statuses` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE TABLE `proposal_batch_jobs` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `JobType` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `Status` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `Total` int NOT NULL,
        `Processed` int NOT NULL,
        `Errors` int NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_proposal_batch_jobs` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE TABLE `proposal_statuses` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        CONSTRAINT `PK_proposal_statuses` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE TABLE `proposal_types` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        CONSTRAINT `PK_proposal_types` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE TABLE `proposals` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Title` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `ProposalTypeId` int NOT NULL,
        `ProposalStatusId` int NOT NULL,
        `ManufacturerId` int NULL,
        `StartDate` datetime(6) NULL,
        `EndDate` datetime(6) NULL,
        `InternalNotes` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `DeletedAt` datetime(6) NULL,
        CONSTRAINT `PK_proposals` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_proposals_Manufacturers_ManufacturerId` FOREIGN KEY (`ManufacturerId`) REFERENCES `Manufacturers` (`Id`),
        CONSTRAINT `FK_proposals_proposal_statuses_ProposalStatusId` FOREIGN KEY (`ProposalStatusId`) REFERENCES `proposal_statuses` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_proposals_proposal_types_ProposalTypeId` FOREIGN KEY (`ProposalTypeId`) REFERENCES `proposal_types` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE TABLE `proposal_distributors` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ProposalId` int NOT NULL,
        `DistributorId` int NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `DeletedAt` datetime(6) NULL,
        CONSTRAINT `PK_proposal_distributors` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_proposal_distributors_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_proposal_distributors_proposals_ProposalId` FOREIGN KEY (`ProposalId`) REFERENCES `proposals` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE TABLE `proposal_industries` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ProposalId` int NOT NULL,
        `IndustryId` int NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `DeletedAt` datetime(6) NULL,
        CONSTRAINT `PK_proposal_industries` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_proposal_industries_Industries_IndustryId` FOREIGN KEY (`IndustryId`) REFERENCES `Industries` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_proposal_industries_proposals_ProposalId` FOREIGN KEY (`ProposalId`) REFERENCES `proposals` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE TABLE `proposal_opcos` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ProposalId` int NOT NULL,
        `OpCoId` int NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `DeletedAt` datetime(6) NULL,
        CONSTRAINT `PK_proposal_opcos` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_proposal_opcos_OpCos_OpCoId` FOREIGN KEY (`OpCoId`) REFERENCES `OpCos` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_proposal_opcos_proposals_ProposalId` FOREIGN KEY (`ProposalId`) REFERENCES `proposals` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE TABLE `proposal_products` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ProposalId` int NOT NULL,
        `ProductId` int NOT NULL,
        `PriceTypeId` int NULL,
        `ProposedPrice` decimal(18,4) NULL,
        `Quantity` int NULL,
        `PackingList` varchar(200) CHARACTER SET utf8mb4 NULL,
        `MetaJson` longtext CHARACTER SET utf8mb4 NULL,
        `ProductProposalStatusId` int NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedDate` datetime(6) NULL,
        `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
        `DeletedAt` datetime(6) NULL,
        CONSTRAINT `PK_proposal_products` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_proposal_products_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_proposal_products_price_types_PriceTypeId` FOREIGN KEY (`PriceTypeId`) REFERENCES `price_types` (`Id`),
        CONSTRAINT `FK_proposal_products_product_proposal_statuses_ProductProposalS~` FOREIGN KEY (`ProductProposalStatusId`) REFERENCES `product_proposal_statuses` (`Id`),
        CONSTRAINT `FK_proposal_products_proposals_ProposalId` FOREIGN KEY (`ProposalId`) REFERENCES `proposals` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE TABLE `proposal_status_history` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ProposalId` int NOT NULL,
        `FromStatusId` int NOT NULL,
        `ToStatusId` int NOT NULL,
        `Comment` varchar(500) CHARACTER SET utf8mb4 NULL,
        `ChangedDate` datetime(6) NOT NULL,
        `ChangedBy` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_proposal_status_history` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_proposal_status_history_proposal_statuses_FromStatusId` FOREIGN KEY (`FromStatusId`) REFERENCES `proposal_statuses` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_proposal_status_history_proposal_statuses_ToStatusId` FOREIGN KEY (`ToStatusId`) REFERENCES `proposal_statuses` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_proposal_status_history_proposals_ProposalId` FOREIGN KEY (`ProposalId`) REFERENCES `proposals` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE TABLE `proposal_product_history` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ProposalProductId` int NOT NULL,
        `ChangeType` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `PreviousJson` longtext CHARACTER SET utf8mb4 NULL,
        `CurrentJson` longtext CHARACTER SET utf8mb4 NULL,
        `ChangedDate` datetime(6) NOT NULL,
        `ChangedBy` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_proposal_product_history` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_proposal_product_history_proposal_products_ProposalProductId` FOREIGN KEY (`ProposalProductId`) REFERENCES `proposal_products` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE INDEX `IX_proposal_distributors_DistributorId` ON `proposal_distributors` (`DistributorId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE UNIQUE INDEX `IX_proposal_distributors_ProposalId_DistributorId` ON `proposal_distributors` (`ProposalId`, `DistributorId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE INDEX `IX_proposal_industries_IndustryId` ON `proposal_industries` (`IndustryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE UNIQUE INDEX `IX_proposal_industries_ProposalId_IndustryId` ON `proposal_industries` (`ProposalId`, `IndustryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE INDEX `IX_proposal_opcos_OpCoId` ON `proposal_opcos` (`OpCoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE UNIQUE INDEX `IX_proposal_opcos_ProposalId_OpCoId` ON `proposal_opcos` (`ProposalId`, `OpCoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE INDEX `IX_proposal_product_history_ProposalProductId` ON `proposal_product_history` (`ProposalProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE INDEX `IX_proposal_products_PriceTypeId` ON `proposal_products` (`PriceTypeId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE INDEX `IX_proposal_products_ProductId` ON `proposal_products` (`ProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE INDEX `IX_proposal_products_ProductProposalStatusId` ON `proposal_products` (`ProductProposalStatusId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE UNIQUE INDEX `IX_proposal_products_ProposalId_ProductId` ON `proposal_products` (`ProposalId`, `ProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE INDEX `IX_proposal_status_history_FromStatusId` ON `proposal_status_history` (`FromStatusId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE INDEX `IX_proposal_status_history_ProposalId` ON `proposal_status_history` (`ProposalId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE INDEX `IX_proposal_status_history_ToStatusId` ON `proposal_status_history` (`ToStatusId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE INDEX `IX_proposals_ManufacturerId` ON `proposals` (`ManufacturerId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE INDEX `IX_proposals_ProposalStatusId` ON `proposals` (`ProposalStatusId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    CREATE INDEX `IX_proposals_ProposalTypeId_ProposalStatusId` ON `proposals` (`ProposalTypeId`, `ProposalStatusId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    INSERT INTO proposal_statuses (Id, Name, IsActive) VALUES
                    (1,'Requested',1),(2,'Pending',1),(3,'Saved',1),(4,'Submitted',1),(5,'Completed',1)
                    ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    INSERT INTO proposal_types (Id, Name, IsActive) VALUES
                    (1,'NewContract',1),(2,'Amendment',1)
                    ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    INSERT INTO price_types (Id, Name, IsActive) VALUES
                    (1,'Commercial',1),(2,'Commodity',1),(3,'FFS',1),(4,'NOI',1)
                    ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    INSERT INTO product_proposal_statuses (Id, Name, IsActive) VALUES
                    (1,'Pending',1),(2,'Accepted',1),(3,'Rejected',1)
                    ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    INSERT INTO amendment_actions (Id, Name, IsActive) VALUES
                    (1,'Add',1),(2,'Update',1),(3,'Remove',1)
                    ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251002031101_2025_10_02_Create_Proposals_Module') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20251002031101_2025_10_02_Create_Proposals_Module', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_distributors` DROP FOREIGN KEY `FK_proposal_distributors_Distributors_DistributorId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_distributors` DROP FOREIGN KEY `FK_proposal_distributors_proposals_ProposalId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_industries` DROP FOREIGN KEY `FK_proposal_industries_Industries_IndustryId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_industries` DROP FOREIGN KEY `FK_proposal_industries_proposals_ProposalId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_opcos` DROP FOREIGN KEY `FK_proposal_opcos_OpCos_OpCoId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_opcos` DROP FOREIGN KEY `FK_proposal_opcos_proposals_ProposalId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_product_history` DROP FOREIGN KEY `FK_proposal_product_history_proposal_products_ProposalProductId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_products` DROP FOREIGN KEY `FK_proposal_products_Products_ProductId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_products` DROP FOREIGN KEY `FK_proposal_products_price_types_PriceTypeId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_products` DROP FOREIGN KEY `FK_proposal_products_product_proposal_statuses_ProductProposalS~`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_products` DROP FOREIGN KEY `FK_proposal_products_proposals_ProposalId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_status_history` DROP FOREIGN KEY `FK_proposal_status_history_proposal_statuses_FromStatusId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_status_history` DROP FOREIGN KEY `FK_proposal_status_history_proposal_statuses_ToStatusId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_status_history` DROP FOREIGN KEY `FK_proposal_status_history_proposals_ProposalId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposals` DROP FOREIGN KEY `FK_proposals_Manufacturers_ManufacturerId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposals` DROP FOREIGN KEY `FK_proposals_proposal_statuses_ProposalStatusId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposals` DROP FOREIGN KEY `FK_proposals_proposal_types_ProposalTypeId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'proposals');
    ALTER TABLE `proposals` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'proposal_types');
    ALTER TABLE `proposal_types` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'proposal_statuses');
    ALTER TABLE `proposal_statuses` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'proposal_status_history');
    ALTER TABLE `proposal_status_history` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'proposal_products');
    ALTER TABLE `proposal_products` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'proposal_product_history');
    ALTER TABLE `proposal_product_history` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'proposal_opcos');
    ALTER TABLE `proposal_opcos` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'proposal_industries');
    ALTER TABLE `proposal_industries` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'proposal_distributors');
    ALTER TABLE `proposal_distributors` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'proposal_batch_jobs');
    ALTER TABLE `proposal_batch_jobs` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'product_proposal_statuses');
    ALTER TABLE `product_proposal_statuses` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'price_types');
    ALTER TABLE `price_types` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'amendment_actions');
    ALTER TABLE `amendment_actions` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposals` RENAME `Proposals`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_types` RENAME `ProposalTypes`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_statuses` RENAME `ProposalStatuses`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_status_history` RENAME `ProposalStatusHistory`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_products` RENAME `ProposalProducts`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_product_history` RENAME `ProposalProductHistory`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_opcos` RENAME `ProposalOpCos`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_industries` RENAME `ProposalIndustries`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_distributors` RENAME `ProposalDistributors`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `proposal_batch_jobs` RENAME `ProposalBatchJobs`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `product_proposal_statuses` RENAME `ProductProposalStatuses`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `price_types` RENAME `PriceTypes`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `amendment_actions` RENAME `AmendmentActions`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `Proposals` RENAME INDEX `IX_proposals_ProposalTypeId_ProposalStatusId` TO `IX_Proposals_ProposalTypeId_ProposalStatusId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `Proposals` RENAME INDEX `IX_proposals_ProposalStatusId` TO `IX_Proposals_ProposalStatusId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `Proposals` RENAME INDEX `IX_proposals_ManufacturerId` TO `IX_Proposals_ManufacturerId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalStatusHistory` RENAME INDEX `IX_proposal_status_history_ToStatusId` TO `IX_ProposalStatusHistory_ToStatusId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalStatusHistory` RENAME INDEX `IX_proposal_status_history_ProposalId` TO `IX_ProposalStatusHistory_ProposalId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalStatusHistory` RENAME INDEX `IX_proposal_status_history_FromStatusId` TO `IX_ProposalStatusHistory_FromStatusId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalProducts` RENAME INDEX `IX_proposal_products_ProposalId_ProductId` TO `IX_ProposalProducts_ProposalId_ProductId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalProducts` RENAME INDEX `IX_proposal_products_ProductProposalStatusId` TO `IX_ProposalProducts_ProductProposalStatusId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalProducts` RENAME INDEX `IX_proposal_products_ProductId` TO `IX_ProposalProducts_ProductId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalProducts` RENAME INDEX `IX_proposal_products_PriceTypeId` TO `IX_ProposalProducts_PriceTypeId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalProductHistory` RENAME INDEX `IX_proposal_product_history_ProposalProductId` TO `IX_ProposalProductHistory_ProposalProductId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalOpCos` RENAME INDEX `IX_proposal_opcos_ProposalId_OpCoId` TO `IX_ProposalOpCos_ProposalId_OpCoId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalOpCos` RENAME INDEX `IX_proposal_opcos_OpCoId` TO `IX_ProposalOpCos_OpCoId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalIndustries` RENAME INDEX `IX_proposal_industries_ProposalId_IndustryId` TO `IX_ProposalIndustries_ProposalId_IndustryId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalIndustries` RENAME INDEX `IX_proposal_industries_IndustryId` TO `IX_ProposalIndustries_IndustryId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalDistributors` RENAME INDEX `IX_proposal_distributors_ProposalId_DistributorId` TO `IX_ProposalDistributors_ProposalId_DistributorId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalDistributors` RENAME INDEX `IX_proposal_distributors_DistributorId` TO `IX_ProposalDistributors_DistributorId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `Proposals` ADD CONSTRAINT `PK_Proposals` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'Proposals', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalTypes` ADD CONSTRAINT `PK_ProposalTypes` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'ProposalTypes', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalStatuses` ADD CONSTRAINT `PK_ProposalStatuses` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'ProposalStatuses', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalStatusHistory` ADD CONSTRAINT `PK_ProposalStatusHistory` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'ProposalStatusHistory', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalProducts` ADD CONSTRAINT `PK_ProposalProducts` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'ProposalProducts', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalProductHistory` ADD CONSTRAINT `PK_ProposalProductHistory` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'ProposalProductHistory', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalOpCos` ADD CONSTRAINT `PK_ProposalOpCos` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'ProposalOpCos', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalIndustries` ADD CONSTRAINT `PK_ProposalIndustries` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'ProposalIndustries', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalDistributors` ADD CONSTRAINT `PK_ProposalDistributors` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'ProposalDistributors', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalBatchJobs` ADD CONSTRAINT `PK_ProposalBatchJobs` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'ProposalBatchJobs', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProductProposalStatuses` ADD CONSTRAINT `PK_ProductProposalStatuses` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'ProductProposalStatuses', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `PriceTypes` ADD CONSTRAINT `PK_PriceTypes` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'PriceTypes', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `AmendmentActions` ADD CONSTRAINT `PK_AmendmentActions` PRIMARY KEY (`Id`);
    CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'AmendmentActions', 'Id');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalDistributors` ADD CONSTRAINT `FK_ProposalDistributors_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalDistributors` ADD CONSTRAINT `FK_ProposalDistributors_Proposals_ProposalId` FOREIGN KEY (`ProposalId`) REFERENCES `Proposals` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalIndustries` ADD CONSTRAINT `FK_ProposalIndustries_Industries_IndustryId` FOREIGN KEY (`IndustryId`) REFERENCES `Industries` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalIndustries` ADD CONSTRAINT `FK_ProposalIndustries_Proposals_ProposalId` FOREIGN KEY (`ProposalId`) REFERENCES `Proposals` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalOpCos` ADD CONSTRAINT `FK_ProposalOpCos_OpCos_OpCoId` FOREIGN KEY (`OpCoId`) REFERENCES `OpCos` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalOpCos` ADD CONSTRAINT `FK_ProposalOpCos_Proposals_ProposalId` FOREIGN KEY (`ProposalId`) REFERENCES `Proposals` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalProductHistory` ADD CONSTRAINT `FK_ProposalProductHistory_ProposalProducts_ProposalProductId` FOREIGN KEY (`ProposalProductId`) REFERENCES `ProposalProducts` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalProducts` ADD CONSTRAINT `FK_ProposalProducts_PriceTypes_PriceTypeId` FOREIGN KEY (`PriceTypeId`) REFERENCES `PriceTypes` (`Id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalProducts` ADD CONSTRAINT `FK_ProposalProducts_ProductProposalStatuses_ProductProposalStat~` FOREIGN KEY (`ProductProposalStatusId`) REFERENCES `ProductProposalStatuses` (`Id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalProducts` ADD CONSTRAINT `FK_ProposalProducts_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalProducts` ADD CONSTRAINT `FK_ProposalProducts_Proposals_ProposalId` FOREIGN KEY (`ProposalId`) REFERENCES `Proposals` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `Proposals` ADD CONSTRAINT `FK_Proposals_Manufacturers_ManufacturerId` FOREIGN KEY (`ManufacturerId`) REFERENCES `Manufacturers` (`Id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `Proposals` ADD CONSTRAINT `FK_Proposals_ProposalStatuses_ProposalStatusId` FOREIGN KEY (`ProposalStatusId`) REFERENCES `ProposalStatuses` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `Proposals` ADD CONSTRAINT `FK_Proposals_ProposalTypes_ProposalTypeId` FOREIGN KEY (`ProposalTypeId`) REFERENCES `ProposalTypes` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalStatusHistory` ADD CONSTRAINT `FK_ProposalStatusHistory_ProposalStatuses_FromStatusId` FOREIGN KEY (`FromStatusId`) REFERENCES `ProposalStatuses` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalStatusHistory` ADD CONSTRAINT `FK_ProposalStatusHistory_ProposalStatuses_ToStatusId` FOREIGN KEY (`ToStatusId`) REFERENCES `ProposalStatuses` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    ALTER TABLE `ProposalStatusHistory` ADD CONSTRAINT `FK_ProposalStatusHistory_Proposals_ProposalId` FOREIGN KEY (`ProposalId`) REFERENCES `Proposals` (`Id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20251003225245_2025_10_04_Rename_Proposals_Tables_To_PascalCase', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251009141159_UpdatePriceAndProductProposalSeed_20251009') THEN

    INSERT INTO PriceTypes (Id, Name, IsActive) VALUES
                    (1,'Guaranteed Price',1),
                    (2,'Published List Price at Time of Purchase',1),
                    (3,'Product Suspended',1),
                    (4,'Product Discontinued',1)
                    ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251009141159_UpdatePriceAndProductProposalSeed_20251009') THEN

    INSERT INTO ProductProposalStatuses (Id, Name, IsActive) VALUES
                    (1,'Requested',1),
                    (2,'Accepted',1),
                    (3,'Rejected',1),
                    (4,'Proposed',1)
                    ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251009141159_UpdatePriceAndProductProposalSeed_20251009') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20251009141159_UpdatePriceAndProductProposalSeed_20251009', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013064921_AddProposalProductExtraFields') THEN

    ALTER TABLE `ProposalProducts` ADD `FfsPrice` decimal(18,4) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013064921_AddProposalProductExtraFields') THEN

    ALTER TABLE `ProposalProducts` ADD `InternalNotes` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013064921_AddProposalProductExtraFields') THEN

    ALTER TABLE `ProposalProducts` ADD `NoiPrice` decimal(18,4) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013064921_AddProposalProductExtraFields') THEN

    ALTER TABLE `ProposalProducts` ADD `Ptv` decimal(18,4) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013064921_AddProposalProductExtraFields') THEN

    ALTER TABLE `ProposalProducts` ADD `Pua` decimal(18,4) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013064921_AddProposalProductExtraFields') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20251013064921_AddProposalProductExtraFields', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `ContractVersions` ADD `ContactPerson` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `ContractVersions` ADD `EntegraContractType` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `ContractVersions` ADD `EntegraVdaProgram` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `ContractVersions` ADD `ManufacturerBillbackName` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `ContractVersions` ADD `ManufacturerNotes` varchar(2000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `ContractVersions` ADD `ManufacturerReferenceNumber` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `ContractVersions` ADD `ManufacturerTermsAndConditions` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `Contracts` ADD `ContactPerson` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `Contracts` ADD `EntegraContractType` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `Contracts` ADD `EntegraVdaProgram` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `Contracts` ADD `ManufacturerBillbackName` varchar(200) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `Contracts` ADD `ManufacturerNotes` varchar(2000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `Contracts` ADD `ManufacturerReferenceNumber` varchar(100) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    ALTER TABLE `Contracts` ADD `ManufacturerTermsAndConditions` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    UPDATE `Contracts` SET `ContactPerson` = NULL, `EntegraContractType` = NULL, `EntegraVdaProgram` = NULL, `ManufacturerBillbackName` = NULL, `ManufacturerNotes` = NULL, `ManufacturerReferenceNumber` = NULL, `ManufacturerTermsAndConditions` = NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    UPDATE `Contracts` SET `ContactPerson` = NULL, `EntegraContractType` = NULL, `EntegraVdaProgram` = NULL, `ManufacturerBillbackName` = NULL, `ManufacturerNotes` = NULL, `ManufacturerReferenceNumber` = NULL, `ManufacturerTermsAndConditions` = NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    UPDATE `Contracts` SET `ContactPerson` = NULL, `EntegraContractType` = NULL, `EntegraVdaProgram` = NULL, `ManufacturerBillbackName` = NULL, `ManufacturerNotes` = NULL, `ManufacturerReferenceNumber` = NULL, `ManufacturerTermsAndConditions` = NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251013073524_AddContractManufacturerEntegraFields') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20251013073524_AddContractManufacturerEntegraFields', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;

DROP PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`;

