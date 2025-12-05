-- Quick fix to activate user manually
-- Run this in SQL Server Management Studio or via sqlcmd

-- Activate user by email
UPDATE Users 
SET IsActive = 1 
WHERE Email = 'ladhu123@gmail.com' AND IsDeleted = 0;

-- Verify the update
SELECT 
    u.Id,
    u.Name,
    u.Email,
    u.IsActive,
    u.OrganizationId,
    o.Name AS OrganizationName,
    o.IsActive AS OrganizationIsActive
FROM Users u
LEFT JOIN Organizations o ON u.OrganizationId = o.Id
WHERE u.Email = 'ladhu123@gmail.com';

