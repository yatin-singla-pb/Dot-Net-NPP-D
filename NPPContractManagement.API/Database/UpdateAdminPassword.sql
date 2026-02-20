-- Update admin user password hash
-- This script updates the admin user's password to 'Admin@123'

USE InterflexNPP;

-- Update the admin user's password hash
UPDATE Users 
SET PasswordHash = '$2a$12$A2njs.w2dAu/Lamur/KBFuRb71mU/a0qHMIJOxOLJ1LZw..1SyMqG'
WHERE UserId = 'admin';

-- Verify the update
SELECT UserId, FirstName, LastName, Email, IsActive, EmailConfirmed, CreatedDate
FROM Users 
WHERE UserId = 'admin';

-- Check user roles
SELECT u.UserId, u.FirstName, u.LastName, r.Name as RoleName
FROM Users u
JOIN UserRoles ur ON u.Id = ur.UserId
JOIN Roles r ON ur.RoleId = r.Id
WHERE u.UserId = 'admin';
