IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OrderManagementDb')
BEGIN
    CREATE DATABASE [OrderManagementDb]
    CONTAINMENT = NONE;
    PRINT 'Database OrderManagementDb created.';
END
ELSE
BEGIN
    PRINT 'Database OrderManagementDb already exists.';
END
GO

USE [OrderManagementDb]
GO