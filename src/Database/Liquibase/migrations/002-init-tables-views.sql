USE [OrderManagementDb]
GO

CREATE TABLE [users].[Users]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[Login] NVARCHAR(100) NOT NULL,
	[Email] NVARCHAR (255) NOT NULL,
	[Password] NVARCHAR(255) NOT NULL,
	[IsActive] BIT NOT NULL,
	[FirstName] NVARCHAR(50) NOT NULL,
	[LastName] NVARCHAR(50) NOT NULL,
	[Name] NVARCHAR (255) NOT NULL,
	CONSTRAINT [PK_users_Users_Id] PRIMARY KEY ([Id] ASC)
)
GO

CREATE TABLE [users].[UserRoles]
(
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [RoleCode] NVARCHAR(50)
)
GO

CREATE TABLE [users].[Permissions]
(
	[Code] VARCHAR(50) NOT NULL,
	[Name] VARCHAR(100) NOT NULL,
	[Description] [varchar](255) NULL,
	CONSTRAINT [PK_users_Permissions_Code] PRIMARY KEY ([Code] ASC)
)
GO

CREATE TABLE [users].[RolesToPermissions]
(
	[RoleCode] VARCHAR(50) NOT NULL,
	[PermissionCode] VARCHAR(50) NOT NULL,
	CONSTRAINT [PK_RolesToPermissions_RoleCode_PermissionCode] PRIMARY KEY (RoleCode ASC, PermissionCode ASC)
)
GO

CREATE TABLE [users].[OutboxMessages]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[OccurredOn] DATETIME2 NOT NULL,
	[Type] VARCHAR(255) NOT NULL,
	[Data] VARCHAR(MAX) NOT NULL,
	[ProcessedDate] DATETIME2 NULL,
	CONSTRAINT [PK_users_OutboxMessages_Id] PRIMARY KEY ([Id] ASC)
)
GO

CREATE TABLE [users].[InboxMessages]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[OccurredOn] DATETIME2 NOT NULL,
	[Type] VARCHAR(255) NOT NULL,
	[Data] VARCHAR(MAX) NOT NULL,
	[ProcessedDate] DATETIME2 NULL,
	CONSTRAINT [PK_users_InboxMessages_Id] PRIMARY KEY ([Id] ASC)
)
GO

CREATE TABLE [users].[InternalCommands]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[EnqueueDate] DATETIME2 NOT NULL,
	[Type] VARCHAR(255) NOT NULL,
	[Data] VARCHAR(MAX) NOT NULL,
	[ProcessedDate] DATETIME2 NULL,
	[Error] NVARCHAR(MAX) NULL,
	CONSTRAINT [PK_users_InternalCommands_Id] PRIMARY KEY ([Id] ASC)
)
GO

CREATE TABLE [app].[Emails]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[From] NVARCHAR(255) NOT NULL,
	[To] NVARCHAR(255) NOT NULL,
	[Subject] NVARCHAR(255) NOT NULL,
	[Content] NVARCHAR(MAX) NOT NULL,
	[Date] DATETIME NOT NULL,
	CONSTRAINT [PK_app_Emails_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

CREATE TABLE [registrations].[UserRegistrations]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [Login] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR (255) NOT NULL,
    [Password] NVARCHAR(255) NOT NULL,
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [Name] NVARCHAR (255) NOT NULL,
	[StatusCode] VARCHAR(50) NOT NULL,
	[RegisterDate] DATETIME NOT NULL,
	[ConfirmedDate] DATETIME NULL,
    CONSTRAINT [PK_registrations_UserRegistrations_Id] PRIMARY KEY ([Id] ASC)
)
GO

CREATE TABLE [registrations].[OutboxMessages] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [OccurredOn]    DATETIME2 (7)    NOT NULL,
    [Type]          VARCHAR (255)    NOT NULL,
    [Data]          VARCHAR (MAX)    NOT NULL,
    [ProcessedDate] DATETIME2 (7)    NULL,
    CONSTRAINT [PK_users_OutboxMessages_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
GO

CREATE TABLE [registrations].[InboxMessages] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [OccurredOn]    DATETIME2 (7)    NOT NULL,
    [Type]          VARCHAR (255)    NOT NULL,
    [Data]          VARCHAR (MAX)    NOT NULL,
    [ProcessedDate] DATETIME2 (7)    NULL,
    CONSTRAINT [PK_registrations_InboxMessages_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
GO

CREATE TABLE [registrations].[InternalCommands] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [EnqueueDate]   DATETIME2 (7)    NOT NULL,
    [Type]          VARCHAR (255)    NOT NULL,
    [Data]          VARCHAR (MAX)    NOT NULL,
    [ProcessedDate] DATETIME2 (7)    NULL,
    [Error]         NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_registrations_InternalCommands_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
GO


CREATE VIEW [users].[v_Users]
AS
SELECT
    [User].[Id],
    [User].[IsActive],
    [User].[Login],
    [User].[Password],
    [User].[Email],
    [User].[Name]
FROM [users].[Users] AS [User]
GO

CREATE VIEW [users].[v_UserRoles]
AS
SELECT
    [UserRole].[UserId],
    [UserRole].[RoleCode]
FROM [users].[UserRoles] AS [UserRole]
GO

CREATE VIEW [users].[v_UserPermissions]
AS
SELECT 
	DISTINCT
	[UserRole].UserId,
	[RolesToPermission].PermissionCode
FROM [users].UserRoles AS [UserRole]
	INNER JOIN [users].RolesToPermissions AS [RolesToPermission]
		ON [UserRole].RoleCode = [RolesToPermission].RoleCode
GO

CREATE VIEW [registrations].[v_UserRegistrations]
AS
SELECT
    [UserRegistration].[Id],
    [UserRegistration].[Login],
    [UserRegistration].[Email],
    [UserRegistration].[FirstName],
    [UserRegistration].[LastName],
    [UserRegistration].[Name],
    [UserRegistration].[StatusCode],
    [UserRegistration].[Password]
FROM [registrations].[UserRegistrations] AS [UserRegistration]
GO

INSERT INTO [users].[Users] VALUES ('0beb0b3c-ee09-497f-ba94-cb4f7fee8a90', 'admin', 'luonganh@gmail.com', 'Alo1234567', 1, 'Anh', 'Luong', 'Anh Luong')
GO

INSERT INTO [users].[UserRoles] VALUES ('0beb0b3c-ee09-497f-ba94-cb4f7fee8a90', 'Administrator')
GO