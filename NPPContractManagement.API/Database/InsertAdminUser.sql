-- Insert admin user into the database
-- This script adds the default admin user with correct password hash

USE NPPContractManagment;

-- Insert the admin user (password: Admin@123)
INSERT INTO Users (UserId, FirstName, LastName, Email, PasswordHash, IsActive, EmailConfirmed, CreatedDate, CreatedBy) 
VALUES (
    'admin', 
    'System', 
    'Administrator', 
    'admin@nppcontractmanagement.com', 
    '$2a$12$A2njs.w2dAu/Lamur/KBFuRb71mU/a0qHMIJOxOLJ1LZw..1SyMqG', 
    TRUE, 
    TRUE, 
    NOW(), 
    'System'
);

-- Get the user ID for role assignment
SET @userId = LAST_INSERT_ID();

-- Assign System Administrator role to the admin user
INSERT INTO UserRoles (UserId, RoleId, AssignedDate, AssignedBy) 
VALUES (@userId, 1, NOW(), 'System');

-- Verify the user was created
SELECT u.Id, u.UserId, u.FirstName, u.LastName, u.Email, u.IsActive, u.EmailConfirmed
FROM Users u 
WHERE u.UserId = 'admin';

-- Verify the role assignment
SELECT u.UserId, u.FirstName, u.LastName, r.Name as RoleName
FROM Users u
JOIN UserRoles ur ON u.Id = ur.UserId
JOIN Roles r ON ur.RoleId = r.Id
WHERE u.UserId = 'admin';
