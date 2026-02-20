-- NPP Contract Management Database Schema
-- MySQL Database Creation Script

CREATE DATABASE IF NOT EXISTS NPPContractManagement;
USE NPPContractManagement;

-- Create Roles table
CREATE TABLE Roles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Description VARCHAR(500),
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME,
    CreatedBy VARCHAR(100),
    ModifiedBy VARCHAR(100)
);

-- Create Users table
CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId VARCHAR(100) NOT NULL UNIQUE,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PhoneNumber VARCHAR(20),
    PasswordHash TEXT NOT NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    EmailConfirmed BOOLEAN NOT NULL DEFAULT FALSE,
    LastLoginDate DATETIME,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME,
    CreatedBy VARCHAR(100),
    ModifiedBy VARCHAR(100)
);

-- Create UserRoles table
CREATE TABLE UserRoles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    AssignedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    AssignedBy VARCHAR(100),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
    UNIQUE KEY unique_user_role (UserId, RoleId)
);

-- Create Industries table
CREATE TABLE Industries (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(200) NOT NULL UNIQUE,
    Description VARCHAR(500),
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME,
    CreatedBy VARCHAR(100),
    ModifiedBy VARCHAR(100)
);

-- Create Manufacturers table
CREATE TABLE Manufacturers (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Description VARCHAR(500),
    ContactPerson VARCHAR(200),
    Email VARCHAR(255),
    PhoneNumber VARCHAR(20),
    Address VARCHAR(500),
    City VARCHAR(100),
    State VARCHAR(100),
    ZipCode VARCHAR(20),
    Country VARCHAR(100),
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME,
    CreatedBy VARCHAR(100),
    ModifiedBy VARCHAR(100)
);

-- Create Distributors table
CREATE TABLE Distributors (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Description VARCHAR(500),
    ContactPerson VARCHAR(200),
    Email VARCHAR(255),
    PhoneNumber VARCHAR(20),
    Address VARCHAR(500),
    City VARCHAR(100),
    State VARCHAR(100),
    ZipCode VARCHAR(20),
    Country VARCHAR(100),
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME,
    CreatedBy VARCHAR(100),
    ModifiedBy VARCHAR(100)
);

-- Create Products table
CREATE TABLE Products (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Description VARCHAR(500),
    SKU VARCHAR(100),
    ManufacturerId INT NOT NULL,
    UnitPrice DECIMAL(18,2),
    UnitOfMeasure VARCHAR(50),
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME,
    CreatedBy VARCHAR(100),
    ModifiedBy VARCHAR(100),
    FOREIGN KEY (ManufacturerId) REFERENCES Manufacturers(Id) ON DELETE CASCADE
);

-- Create Contracts table
CREATE TABLE Contracts (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ContractNumber VARCHAR(100) NOT NULL UNIQUE,
    Title VARCHAR(200) NOT NULL,
    Description VARCHAR(1000),
    DistributorId INT NOT NULL,
    IndustryId INT NOT NULL,
    Status INT NOT NULL DEFAULT 1, -- 1=Draft, 2=Pending, 3=Active, 4=Expired, 5=Terminated, 6=Suspended
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    TotalValue DECIMAL(18,2),
    Notes VARCHAR(1000),
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME,
    CreatedBy VARCHAR(100),
    ModifiedBy VARCHAR(100),
    FOREIGN KEY (DistributorId) REFERENCES Distributors(Id) ON DELETE CASCADE,
    FOREIGN KEY (IndustryId) REFERENCES Industries(Id) ON DELETE CASCADE
);

-- Create ContractProducts table
CREATE TABLE ContractProducts (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ContractId INT NOT NULL,
    ProductId INT NOT NULL,
    ContractPrice DECIMAL(18,2) NOT NULL,
    MinimumQuantity INT,
    MaximumQuantity INT,
    Notes VARCHAR(500),
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME,
    CreatedBy VARCHAR(100),
    ModifiedBy VARCHAR(100),
    FOREIGN KEY (ContractId) REFERENCES Contracts(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);

-- Insert default roles
INSERT INTO Roles (Id, Name, Description, CreatedBy) VALUES
(1, 'System Administrator', 'Full system access', 'System'),
(2, 'Contract Manager', 'Manage contracts and related data', 'System'),
(3, 'Manufacturer', 'Manufacturer user access', 'System'),
(4, 'Distributor', 'Distributor user access', 'System'),
(5, 'Contract Viewer', 'View contracts and run reports', 'System');

-- Insert default industries
INSERT INTO Industries (Id, Name, Description, CreatedBy) VALUES
(1, 'College & University', 'Higher education institutions', 'System'),
(2, 'K-12', 'Primary and secondary education', 'System'),
(3, 'Quick Serve Restaurant', 'Fast food and quick service restaurants', 'System'),
(4, 'Healthcare', 'Hospitals and healthcare facilities', 'System'),
(5, 'Corporate', 'Corporate dining and cafeterias', 'System');

-- Create default admin user (password: Admin@123)
-- Password hash for 'Admin@123' using BCrypt
INSERT INTO Users (UserId, FirstName, LastName, Email, PasswordHash, IsActive, EmailConfirmed, CreatedBy) VALUES
('admin', 'System', 'Administrator', 'admin@nppcontractmanagement.com', '$2a$12$A2njs.w2dAu/Lamur/KBFuRb71mU/a0qHMIJOxOLJ1LZw..1SyMqG', TRUE, TRUE, 'System');

-- Assign admin role to default user
INSERT INTO UserRoles (UserId, RoleId, AssignedBy) VALUES
(1, 1, 'System');


-- Create ContractVersionPriceEntry table (used for versioned price rows)
CREATE TABLE IF NOT EXISTS ContractVersionPriceEntry (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ContractId INT NOT NULL,
    PriceId INT NOT NULL,
    ProductId INT NOT NULL,
    PriceType VARCHAR(100) NOT NULL,
    Allowance DECIMAL(18,4) NULL,
    CommercialDelPrice DECIMAL(18,4) NULL,
    CommercialFobPrice DECIMAL(18,4) NULL,
    CommodityDelPrice DECIMAL(18,4) NULL,
    CommodityFobPrice DECIMAL(18,4) NULL,
    UOM VARCHAR(50) NOT NULL,
    EstimatedQty DECIMAL(18,4) NULL,
    BillbacksAllowed TINYINT(1) NOT NULL DEFAULT 0,
    PUA DECIMAL(18,4) NULL,
    FFSPrice DECIMAL(18,4) NULL,
    NOIPrice DECIMAL(18,4) NULL,
    PTV DECIMAL(18,4) NULL,
    InternalNotes LONGTEXT NULL,
    VersionNumber INT NOT NULL,
    AssignedBy VARCHAR(100) NULL,
    AssignedDate DATETIME(6) NULL,
    CONSTRAINT FK_CVPE_Contracts FOREIGN KEY (ContractId) REFERENCES Contracts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CVPE_ContractPrices FOREIGN KEY (PriceId) REFERENCES ContractPrices(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CVPE_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    INDEX IX_CVPE_Contract_Price_Version (ContractId, PriceId, VersionNumber),
    INDEX IX_CVPE_Contract_Product_Version (ContractId, ProductId, VersionNumber)
);
