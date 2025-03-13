﻿IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OrderManagementDb')
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