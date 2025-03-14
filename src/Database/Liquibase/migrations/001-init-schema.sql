USE [OrderManagementDb]
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'app')
BEGIN
    EXEC ('CREATE SCHEMA app AUTHORIZATION dbo;');
    PRINT 'Schema app created.';
END
ELSE
BEGIN
    PRINT 'Schema app already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'users')
BEGIN
    EXEC ('CREATE SCHEMA users AUTHORIZATION dbo;');
    PRINT 'Schema users created.';
END
ELSE
BEGIN
    PRINT 'Schema users already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'registrations')
BEGIN
    EXEC ('CREATE SCHEMA registrations AUTHORIZATION dbo;');
    PRINT 'Schema registrations created.';
END
ELSE
BEGIN
    PRINT 'Schema registrations already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'products')
BEGIN
    EXEC ('CREATE SCHEMA products AUTHORIZATION dbo;');
    PRINT 'Schema products created.';
END
ELSE
BEGIN
    PRINT 'Schema products already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'orders')
BEGIN
    EXEC ('CREATE SCHEMA orders AUTHORIZATION dbo;');
    PRINT 'Schema orders created.';
END
ELSE
BEGIN
    PRINT 'Schema orders already exists.';
END
GO