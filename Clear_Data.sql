-- Disable foreign key constraints temporarily
EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

-- Delete data from tables in a specific order (order of dependency matters)
BEGIN TRANSACTION;

-- Replace the table names below with the actual table names you want to clear
DELETE FROM RoleClaims;
DELETE FROM Roles;
DELETE FROM UserClaims;
DELETE FROM UserLogins;
DELETE FROM UserRoles;
DELETE FROM Users;
DELETE FROM UserTokens;
DELETE FROM Colognes;
DELETE FROM Items;
DELETE FROM ItemTypes;
DELETE FROM Sneakers;
DELETE FROM Watches;


-- Add more DELETE statements for each table you need to clear

COMMIT TRANSACTION;

-- Re-enable foreign key constraints
EXEC sp_msforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL';

-- Optionally, clear identity columns if needed
 DBCC CHECKIDENT ('ItemTypes', RESEED, 0);
 DBCC CHECKIDENT ('Watches', RESEED, 0);
 DBCC CHECKIDENT ('Sneakers', RESEED, 0);
 DBCC CHECKIDENT ('Colognes', RESEED, 0);
 DBCC CHECKIDENT ('Items', RESEED, 0);
-- Add more DBCC CHECKIDENT statements as needed
