-- MySQL dump 10.13  Distrib 8.4.6, for Win64 (x86_64)
--
-- Host: DESKTOP-0EM04K6    Database: NPPContractManagment
-- ------------------------------------------------------
-- Server version	8.0.41

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `NPPContractManagment`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `NPPContractManagment` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `NPPContractManagment`;

--
-- Table structure for table `__EFMigrationsHistory`
--

DROP TABLE IF EXISTS `__EFMigrationsHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__EFMigrationsHistory`
--

LOCK TABLES `__EFMigrationsHistory` WRITE;
/*!40000 ALTER TABLE `__EFMigrationsHistory` DISABLE KEYS */;
INSERT INTO `__EFMigrationsHistory` VALUES ('20250910221359_InitialCreate','9.0.9'),('20250911004018_UpdateSchemaForCompleteDataModel','9.0.9'),('20250911011758_ForceCreateMissingTables','9.0.9'),('20250911012256_AddSampleData','9.0.9'),('20250911013146_ForceRecreateMainTables','9.0.9'),('20250911014502_AddContractSampleData','9.0.9'),('20250912115917_EnsureCustomerAccountExtendedFields','9.0.9'),('20250912122826_AddContractsIndustryForeignSuspended','9.0.9'),('20250912134941_AddUserContactStatusFields','9.0.9');
/*!40000 ALTER TABLE `__EFMigrationsHistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ContractDistributors`
--

DROP TABLE IF EXISTS `ContractDistributors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ContractDistributors` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ContractId` int NOT NULL,
  `DistributorId` int NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ContractDistributors_ContractId_DistributorId` (`ContractId`,`DistributorId`),
  KEY `IX_ContractDistributors_DistributorId` (`DistributorId`),
  CONSTRAINT `FK_ContractDistributors_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ContractDistributors_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ContractDistributors`
--

LOCK TABLES `ContractDistributors` WRITE;
/*!40000 ALTER TABLE `ContractDistributors` DISABLE KEYS */;
/*!40000 ALTER TABLE `ContractDistributors` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ContractIndustries`
--

DROP TABLE IF EXISTS `ContractIndustries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ContractIndustries` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ContractId` int NOT NULL,
  `IndustryId` int NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ContractIndustries_ContractId_IndustryId` (`ContractId`,`IndustryId`),
  KEY `IX_ContractIndustries_IndustryId` (`IndustryId`),
  CONSTRAINT `FK_ContractIndustries_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ContractIndustries_Industries_IndustryId` FOREIGN KEY (`IndustryId`) REFERENCES `Industries` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ContractIndustries`
--

LOCK TABLES `ContractIndustries` WRITE;
/*!40000 ALTER TABLE `ContractIndustries` DISABLE KEYS */;
INSERT INTO `ContractIndustries` VALUES (6,3,4,'2025-09-12 22:49:57.081808','1',1),(7,4,5,'2025-09-12 23:20:20.382038','1',1),(8,1,1,'2025-09-12 23:20:33.935652','1',1);
/*!40000 ALTER TABLE `ContractIndustries` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ContractOpCos`
--

DROP TABLE IF EXISTS `ContractOpCos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ContractOpCos` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ContractId` int NOT NULL,
  `OpCoId` int NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ContractOpCos_ContractId_OpCoId` (`ContractId`,`OpCoId`),
  KEY `IX_ContractOpCos_OpCoId` (`OpCoId`),
  CONSTRAINT `FK_ContractOpCos_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ContractOpCos_OpCos_OpCoId` FOREIGN KEY (`OpCoId`) REFERENCES `OpCos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ContractOpCos`
--

LOCK TABLES `ContractOpCos` WRITE;
/*!40000 ALTER TABLE `ContractOpCos` DISABLE KEYS */;
/*!40000 ALTER TABLE `ContractOpCos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ContractProducts`
--

DROP TABLE IF EXISTS `ContractProducts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ContractProducts` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ContractId` int NOT NULL,
  `ProductId` int NOT NULL,
  `ContractPrice` decimal(18,2) NOT NULL,
  `MinimumQuantity` int DEFAULT NULL,
  `MaximumQuantity` int DEFAULT NULL,
  `Notes` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ContractProducts_ContractId` (`ContractId`),
  KEY `IX_ContractProducts_ProductId` (`ProductId`),
  CONSTRAINT `FK_ContractProducts_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ContractProducts_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ContractProducts`
--

LOCK TABLES `ContractProducts` WRITE;
/*!40000 ALTER TABLE `ContractProducts` DISABLE KEYS */;
/*!40000 ALTER TABLE `ContractProducts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Contracts`
--

DROP TABLE IF EXISTS `Contracts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Contracts` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ContractNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Title` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DistributorId` int DEFAULT NULL,
  `ManufacturerId` int NOT NULL,
  `Status` int NOT NULL,
  `StartDate` datetime(6) NOT NULL,
  `EndDate` datetime(6) NOT NULL,
  `TotalValue` decimal(18,2) DEFAULT NULL,
  `Terms` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Notes` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CurrentVersionNumber` int NOT NULL DEFAULT '0',
  `IsSuspended` tinyint(1) NOT NULL DEFAULT '0',
  `SendToPerformance` tinyint(1) NOT NULL DEFAULT '0',
  `ForeignContractID` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IndustryId` int NOT NULL DEFAULT '0',
  `SuspendedDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Contracts_ContractNumber` (`ContractNumber`),
  KEY `IX_Contracts_DistributorId` (`DistributorId`),
  KEY `IX_Contracts_ManufacturerId` (`ManufacturerId`),
  KEY `IX_Contracts_IndustryId` (`IndustryId`),
  CONSTRAINT `FK_Contracts_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`),
  CONSTRAINT `FK_Contracts_Industries_IndustryId` FOREIGN KEY (`IndustryId`) REFERENCES `Industries` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Contracts_Manufacturers_ManufacturerId` FOREIGN KEY (`ManufacturerId`) REFERENCES `Manufacturers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Contracts`
--

LOCK TABLES `Contracts` WRITE;
/*!40000 ALTER TABLE `Contracts` DISABLE KEYS */;
INSERT INTO `Contracts` VALUES (1,'CNT-2025-001','University Food Service Contract 2025','Comprehensive food service contract for university dining facilities',NULL,1,3,'2025-01-01 00:00:00.000000','2025-09-10 00:00:00.000000',500000.00,NULL,NULL,'2025-01-01 00:00:00.000000','2025-09-12 23:20:33.935647','System','1',1,1,1,NULL,1,NULL),(3,'CNT-2025-003','Healthcare Facilities Contract','Food service contract for healthcare facilities and hospitals',NULL,3,3,'2025-03-01 00:00:00.000000','2025-12-31 00:00:00.000000',300000.00,NULL,NULL,'2025-01-01 00:00:00.000000','2025-09-12 22:49:57.081802','System','1',1,0,0,NULL,4,NULL),(4,'CN-789','Test',NULL,NULL,3,3,'2025-09-01 00:00:00.000000','2025-09-30 00:00:00.000000',NULL,NULL,NULL,'2025-09-12 22:42:22.669456','2025-09-12 23:20:20.381793','1','1',1,0,1,NULL,5,NULL);
/*!40000 ALTER TABLE `Contracts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ContractVersions`
--

DROP TABLE IF EXISTS `ContractVersions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ContractVersions` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ContractId` int NOT NULL,
  `VersionNumber` int NOT NULL,
  `Title` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `StartDate` datetime(6) NOT NULL,
  `EndDate` datetime(6) NOT NULL,
  `TotalValue` decimal(18,2) DEFAULT NULL,
  `Terms` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Notes` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ChangeReason` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsCurrentVersion` tinyint(1) NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ContractVersions_ContractId_VersionNumber` (`ContractId`,`VersionNumber`),
  CONSTRAINT `FK_ContractVersions_Contracts_ContractId` FOREIGN KEY (`ContractId`) REFERENCES `Contracts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ContractVersions`
--

LOCK TABLES `ContractVersions` WRITE;
/*!40000 ALTER TABLE `ContractVersions` DISABLE KEYS */;
INSERT INTO `ContractVersions` VALUES (1,4,1,'',NULL,'0001-01-01 00:00:00.000000','0001-01-01 00:00:00.000000',NULL,NULL,NULL,'Initial version',1,1,'2025-09-12 22:42:22.903448',NULL,'1',NULL);
/*!40000 ALTER TABLE `ContractVersions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CustomerAccounts`
--

DROP TABLE IF EXISTS `CustomerAccounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CustomerAccounts` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `MemberAccountId` int NOT NULL,
  `DistributorId` int NOT NULL,
  `OpCoId` int DEFAULT NULL,
  `CustomerName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CustomerAccountNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Address` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `City` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `State` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ZipCode` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Country` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ContactPerson` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreditLimit` decimal(18,2) DEFAULT NULL,
  `CurrentBalance` decimal(18,2) DEFAULT NULL,
  `Status` int NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Association` int NOT NULL DEFAULT '0',
  `AuditDate` datetime(6) DEFAULT NULL,
  `CombinedUniqueID` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DSO` int DEFAULT NULL,
  `DateToEntegra` datetime(6) DEFAULT NULL,
  `EndDate` datetime(6) DEFAULT NULL,
  `InternalNotes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Markup` decimal(18,2) DEFAULT NULL,
  `SalesRep` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `StartDate` datetime(6) DEFAULT NULL,
  `TRACSAccess` tinyint(1) NOT NULL DEFAULT '0',
  `ToEntegra` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_CustomerAccounts_DistributorId_CustomerAccountNumber` (`DistributorId`,`CustomerAccountNumber`),
  KEY `IX_CustomerAccounts_MemberAccountId` (`MemberAccountId`),
  KEY `IX_CustomerAccounts_OpCoId` (`OpCoId`),
  CONSTRAINT `FK_CustomerAccounts_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CustomerAccounts_MemberAccounts_MemberAccountId` FOREIGN KEY (`MemberAccountId`) REFERENCES `MemberAccounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CustomerAccounts_OpCos_OpCoId` FOREIGN KEY (`OpCoId`) REFERENCES `OpCos` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CustomerAccounts`
--

LOCK TABLES `CustomerAccounts` WRITE;
/*!40000 ALTER TABLE `CustomerAccounts` DISABLE KEYS */;
INSERT INTO `CustomerAccounts` VALUES (1,1,1,1,'UChicago Main Dining','CUST001','5801 S Ellis Ave, Chicago, IL 60637','Chicago','IL','60637','USA','(773) 702-1234','dining@uchicago.edu','Jennifer Adams',50000.00,12500.00,1,1,'2025-01-01 00:00:00.000000','2025-09-12 20:03:29.218801','System','admin',1,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,0),(2,2,2,2,'Lincoln School Cafeteria','CUST002','123 School St, Atlanta, GA 30309','Atlanta','GA','30309','USA','(404) 555-5000','cafeteria@lincoln.edu','Mary Johnson',25000.00,5000.00,1,1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,1,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,0),(3,3,3,3,'Quick Bite Main Location','CUST003','456 Fast Food Blvd, Dallas, TX 75201','Dallas','TX','75201','USA','(214) 555-6000','manager@quickbite.com','Carlos Rodriguez',15000.00,3200.00,1,1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,1,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,0),(4,4,1,4,'General Hospital Food Service','CUST004','789 Health Way, Chicago, IL 60611','Chicago','IL','60611','USA','(312) 555-7000','food@generalhospital.org','Dr. Patricia Lee',75000.00,18750.00,2,1,'2025-01-01 00:00:00.000000','2025-09-12 19:35:53.677170','System','admin',1,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,0),(5,2,2,2,'TEST','TEST',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0.00,1,1,'2025-09-12 20:30:54.432845','2025-09-12 21:08:20.447410','1','1',1,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,1,0);
/*!40000 ALTER TABLE `CustomerAccounts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `DistributorProductCodes`
--

DROP TABLE IF EXISTS `DistributorProductCodes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DistributorProductCodes` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `DistributorId` int NOT NULL,
  `ProductId` int NOT NULL,
  `DistributorCode` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DistributorPrice` decimal(18,2) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_DistributorProductCodes_DistributorId_ProductId_DistributorC~` (`DistributorId`,`ProductId`,`DistributorCode`),
  KEY `IX_DistributorProductCodes_ProductId` (`ProductId`),
  CONSTRAINT `FK_DistributorProductCodes_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_DistributorProductCodes_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `DistributorProductCodes`
--

LOCK TABLES `DistributorProductCodes` WRITE;
/*!40000 ALTER TABLE `DistributorProductCodes` DISABLE KEYS */;
/*!40000 ALTER TABLE `DistributorProductCodes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Distributors`
--

DROP TABLE IF EXISTS `Distributors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Distributors` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ContactPerson` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Address` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `City` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `State` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ZipCode` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Country` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ReceiveContractProposal` tinyint(1) NOT NULL DEFAULT '0',
  `Status` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Distributors`
--

LOCK TABLES `Distributors` WRITE;
/*!40000 ALTER TABLE `Distributors` DISABLE KEYS */;
INSERT INTO `Distributors` VALUES (1,'Regional Food Services',NULL,NULL,NULL,'(312) 555-0100','123 Distribution Way, Chicago, IL 60601',NULL,NULL,NULL,NULL,1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,1,1),(2,'Metro Food Distribution1','Metro Food Distribution1','Metro Food Distribution1','riteshoct7@gmail.com','(404) 555-0200','456 Supply Chain Blvd, Atlanta, GA 30309','Ahmedbad','GUJARAT','380058','USA',1,'2025-01-01 00:00:00.000000','2025-09-11 20:35:45.369092','System','admin',1,1),(3,'National Food Partners',NULL,NULL,NULL,'(214) 555-0300','789 Logistics Ave, Dallas, TX 75201',NULL,NULL,NULL,NULL,1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,0,1),(4,'NextWave Distribution1','NextWave Distribution','','riteshoct7@gmail.com','','','','','','',1,'2025-09-11 20:17:26.876162','2025-09-11 20:36:16.359127','admin','admin',1,1),(5,'PrimePath Distributors','PrimePath Distributors',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,1,'2025-09-11 20:17:54.969346',NULL,'admin',NULL,1,1),(6,'Unity Supply Chain',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,1,'2025-09-11 20:18:40.634955',NULL,'admin',NULL,1,1),(7,'Test1',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,1,'2025-09-11 20:36:29.951484','2025-09-11 20:41:11.398100','admin','admin',1,1);
/*!40000 ALTER TABLE `Distributors` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Industries`
--

DROP TABLE IF EXISTS `Industries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Industries` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Status` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Industries`
--

LOCK TABLES `Industries` WRITE;
/*!40000 ALTER TABLE `Industries` DISABLE KEYS */;
INSERT INTO `Industries` VALUES (1,'College & University','Higher education institutions',1,'2025-01-01 00:00:00.000000','2025-09-11 18:40:01.625121','System','1',2),(2,'K-12','Primary and secondary education',1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,1),(3,'Quick Serve Restaurant','Fast food and quick service restaurants',1,'2025-01-01 00:00:00.000000','2025-09-11 18:22:43.608310','System','1',3),(4,'Healthcare','Hospitals and healthcare facilities',1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,1),(5,'Corporate','Corporate dining and cafeterias',1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,1),(6,'Test','Test',1,'2025-09-11 18:17:40.988967','2025-09-11 18:22:31.253096','1','1',2);
/*!40000 ALTER TABLE `Industries` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Manufacturers`
--

DROP TABLE IF EXISTS `Manufacturers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Manufacturers` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ContactPerson` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Address` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `City` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `State` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ZipCode` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Country` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `AKA` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PrimaryBroker` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Status` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Manufacturers`
--

LOCK TABLES `Manufacturers` WRITE;
/*!40000 ALTER TABLE `Manufacturers` DISABLE KEYS */;
INSERT INTO `Manufacturers` VALUES (1,'Sysco Corporation',NULL,NULL,NULL,'(281) 584-1390','1390 Enclave Pkwy, Houston, TX 77077',NULL,NULL,NULL,NULL,1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,'Sysco','John Smith',1),(3,'Performance Food Group',NULL,NULL,NULL,'(804) 484-7700','12500 West Creek Pkwy, Richmond, VA 23238',NULL,NULL,NULL,NULL,1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,'PFG','Mike Johnson',1),(7,'Test1',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,'2025-09-12 00:53:38.272288','2025-09-12 00:54:37.761216','admin','admin','TT`',NULL,1);
/*!40000 ALTER TABLE `Manufacturers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MemberAccounts`
--

DROP TABLE IF EXISTS `MemberAccounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MemberAccounts` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `MemberNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FacilityName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Address` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `City` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `State` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ZipCode` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Country` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ContactPerson` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IndustryId` int DEFAULT NULL,
  `W9` tinyint(1) NOT NULL,
  `TaxId` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `BusinessType` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Status` int NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `AuditDate` datetime(6) DEFAULT NULL,
  `ClientGroupEnrollment` tinyint(1) NOT NULL DEFAULT '0',
  `ClientGroupNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EntegraGPONumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EntegraIdNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `InternalNotes` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `LopDate` datetime(6) DEFAULT NULL,
  `ParentMemberAccountNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PayType` int DEFAULT NULL,
  `SalesforceAccountName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `VMAPNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `VMSupplierName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `VMSupplierSite` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_MemberAccounts_MemberNumber` (`MemberNumber`),
  KEY `IX_MemberAccounts_IndustryId` (`IndustryId`),
  CONSTRAINT `FK_MemberAccounts_Industries_IndustryId` FOREIGN KEY (`IndustryId`) REFERENCES `Industries` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MemberAccounts`
--

LOCK TABLES `MemberAccounts` WRITE;
/*!40000 ALTER TABLE `MemberAccounts` DISABLE KEYS */;
INSERT INTO `MemberAccounts` VALUES (1,'MEM001','University of Chicago Dining','5801 S Ellis Ave, Chicago, IL 60637','Chicago','IL','60637','USA','(773) 702-1234','dining@uchicago.edu','Jennifer Adams',1,1,'36-1234567','Educational Institution',1,1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,1,'',NULL,NULL,NULL),(2,'MEM002','Lincoln Elementary School','123 School St, Atlanta, GA 30309','Atlanta','GA','30309','USA','(404) 555-5000','cafeteria@lincoln.edu','Mary Johnson',2,1,'58-9876543','Public School',1,1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,1,'',NULL,NULL,NULL),(3,'MEM003','Quick Bite Restaurant','456 Fast Food Blvd, Dallas, TX 75201','Dallas','TX','75201','USA','(214) 555-6000','manager@quickbite.com','Carlos Rodriguez',3,0,'75-1122334','Restaurant',1,1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,1,'',NULL,NULL,NULL),(4,'MEM004','General Hospital Cafeteria','789 Health Way, Chicago, IL 60611','Chicago','IL','60611','USA','(312) 555-7000','food@generalhospital.org','Dr. Patricia Lee',4,1,'36-5566778','Healthcare Facility',2,1,'2025-01-01 00:00:00.000000','2025-09-12 18:55:01.293164','System','1',NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,1,'',NULL,NULL,NULL),(5,'Test1','Test1',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,4,0,NULL,NULL,1,1,'2025-09-12 19:15:02.268785','2025-09-12 19:15:23.264491','1','1','2025-09-12 19:15:23.264491',0,NULL,NULL,NULL,NULL,'2025-09-11 00:00:00.000000',NULL,2,'Test1',NULL,NULL,NULL);
/*!40000 ALTER TABLE `MemberAccounts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `OpCos`
--

DROP TABLE IF EXISTS `OpCos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `OpCos` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RemoteReferenceCode` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DistributorId` int NOT NULL,
  `Address` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `City` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `State` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ZipCode` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Country` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ContactPerson` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Status` int NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_OpCos_DistributorId` (`DistributorId`),
  CONSTRAINT `FK_OpCos_Distributors_DistributorId` FOREIGN KEY (`DistributorId`) REFERENCES `Distributors` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `OpCos`
--

LOCK TABLES `OpCos` WRITE;
/*!40000 ALTER TABLE `OpCos` DISABLE KEYS */;
INSERT INTO `OpCos` VALUES (1,'Chicago Operations','CHI001',1,'100 N Michigan Ave, Chicago, IL 60601','Chicago','IL','60601','USA','(312) 555-1001','chicago@regionalfood.com','Sarah Wilson',3,1,'2025-01-01 00:00:00.000000','2025-09-11 23:52:26.657890','System','1'),(2,'Atlanta Hub','ATL001',2,'200 Peachtree St, Atlanta, GA 30303','Atlanta','GA','30303','USA','(404) 555-2001','atlanta@metrofood.com','Robert Brown',1,1,'2025-01-01 00:00:00.000000','2025-09-11 23:44:04.510855','System','1'),(3,'Dallas Center','DAL001',3,'300 Main St, Dallas, TX 75202','Dallas','TX','75202','USA','(214) 555-3001','dallas@nationalfood.com','Lisa Garcia',1,1,'2025-01-01 00:00:00.000000',NULL,'System',NULL),(4,'Chicago West','CHI002',1,'400 W Lake St, Chicago, IL 60606','Chicago','IL','60606','USA','(312) 555-1002','west@regionalfood.com','David Miller',1,1,'2025-01-01 00:00:00.000000','2025-09-11 23:51:15.722366','System','1'),(5,'AlphaCore Operations','ALPH',7,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,2,1,'2025-09-11 23:31:30.729118','2025-09-11 23:52:19.732853','1','1');
/*!40000 ALTER TABLE `OpCos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Products`
--

DROP TABLE IF EXISTS `Products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Products` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `SKU` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ManufacturerId` int NOT NULL,
  `UnitPrice` decimal(18,2) DEFAULT NULL,
  `UnitOfMeasure` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Category` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CostPrice` decimal(18,2) DEFAULT NULL,
  `GTIN` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ListPrice` decimal(18,2) DEFAULT NULL,
  `ManufacturerProductCode` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PackSize` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Status` int NOT NULL DEFAULT '0',
  `SubCategory` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `UPC` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Products_ManufacturerId` (`ManufacturerId`),
  CONSTRAINT `FK_Products_Manufacturers_ManufacturerId` FOREIGN KEY (`ManufacturerId`) REFERENCES `Manufacturers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Products`
--

LOCK TABLES `Products` WRITE;
/*!40000 ALTER TABLE `Products` DISABLE KEYS */;
INSERT INTO `Products` VALUES (1,'Beef','Premium Ground Beef 80/20','SYS001',1,NULL,NULL,1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,'Meat',38.50,'1234567890123',45.99,'SYSCO-BEEF-001','10 lb',1,'Ground Beef','123456789012'),(3,'Fresh Romaine Lettuce','Fresh Romaine Lettuce',NULL,3,NULL,NULL,1,'2025-01-01 00:00:00.000000','2025-09-12 10:40:27.805304','System','1','Produce',NULL,'3456789012345',18.99,'PFG-PRODUCE-001','24 count',1,'Lettuce','345678901234'),(4,'Milk','Whole Milk Gallon','SYS002',1,NULL,NULL,1,'2025-01-01 00:00:00.000000',NULL,'System',NULL,'Dairy',10.75,'4567890123456',12.99,'SYSCO-DAIRY-001','4 gallons',1,'Milk','456789012345'),(6,'Test','Test',NULL,3,NULL,NULL,1,'2025-09-12 10:50:34.357068','2025-09-12 11:00:09.562515','1','1','',NULL,'',NULL,'Test','',2,'','');
/*!40000 ALTER TABLE `Products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Roles`
--

DROP TABLE IF EXISTS `Roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Roles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Roles_Name` (`Name`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Roles`
--

LOCK TABLES `Roles` WRITE;
/*!40000 ALTER TABLE `Roles` DISABLE KEYS */;
INSERT INTO `Roles` VALUES (1,'System Administrator','Full system access',1,'2025-01-01 00:00:00.000000',NULL,'System',NULL),(2,'Contract Manager','Manage contracts and related data',1,'2025-01-01 00:00:00.000000',NULL,'System',NULL),(3,'Manufacturer','Manufacturer user access',1,'2025-01-01 00:00:00.000000',NULL,'System',NULL),(4,'Distributor','Distributor user access',1,'2025-01-01 00:00:00.000000',NULL,'System',NULL),(5,'Contract Viewer','View contracts and run reports',1,'2025-01-01 00:00:00.000000',NULL,'System',NULL);
/*!40000 ALTER TABLE `Roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `UserRoles`
--

DROP TABLE IF EXISTS `UserRoles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `UserRoles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `RoleId` int NOT NULL,
  `AssignedDate` datetime(6) NOT NULL,
  `AssignedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_UserRoles_UserId_RoleId` (`UserId`,`RoleId`),
  KEY `IX_UserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_UserRoles_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_UserRoles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `UserRoles`
--

LOCK TABLES `UserRoles` WRITE;
/*!40000 ALTER TABLE `UserRoles` DISABLE KEYS */;
INSERT INTO `UserRoles` VALUES (1,1,1,'2025-09-11 03:46:58.000000','System');
/*!40000 ALTER TABLE `UserRoles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Users`
--

DROP TABLE IF EXISTS `Users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FirstName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LastName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PhoneNumber` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `LastLoginDate` datetime(6) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) DEFAULT NULL,
  `CreatedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `AccountStatus` int NOT NULL DEFAULT '0',
  `Class` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Company` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IndustryId` int DEFAULT NULL,
  `JobTitle` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Address` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `City` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `FailedAuthAttempts` int NOT NULL DEFAULT '0',
  `Notes` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PostCode` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `State` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Status` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Users_Email` (`Email`),
  UNIQUE KEY `IX_Users_UserId` (`UserId`),
  KEY `IX_Users_IndustryId` (`IndustryId`),
  CONSTRAINT `FK_Users_Industries_IndustryId` FOREIGN KEY (`IndustryId`) REFERENCES `Industries` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Users`
--

LOCK TABLES `Users` WRITE;
/*!40000 ALTER TABLE `Users` DISABLE KEYS */;
INSERT INTO `Users` VALUES (1,'admin','Ritesh','Parekh','admin@nppcontractmanagement.com',NULL,'$2a$12$A2njs.w2dAu/Lamur/KBFuRb71mU/a0qHMIJOxOLJ1LZw..1SyMqG',1,1,'2025-09-13 00:51:18.700678','2025-09-11 03:46:58.000000',NULL,'System',NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,0);
/*!40000 ALTER TABLE `Users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'NPPContractManagment'
--

--
-- Dumping routines for database 'NPPContractManagment'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-09-13  7:44:35
