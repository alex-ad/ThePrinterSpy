
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/18/2020 19:12:06
-- Generated from EDMX file: C:\Users\is5493\source\repos\ThePrinterSpy\ThePrinterSpyControl\DataBase\PrintSpyDb.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [PrintSpy];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Computers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Computers];
GO
IF OBJECT_ID(N'[dbo].[PrintDatas]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PrintDatas];
GO
IF OBJECT_ID(N'[dbo].[Printers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Printers];
GO
IF OBJECT_ID(N'[dbo].[Servers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Servers];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[Configs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configs];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Computers'
CREATE TABLE [dbo].[Computers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL
);
GO

-- Creating table 'PrintDatas'
CREATE TABLE [dbo].[PrintDatas] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PrinterId] int  NOT NULL,
    [UserId] int  NOT NULL,
    [ComputerId] int  NOT NULL,
    [ServerId] int  NOT NULL,
    [DocName] nvarchar(max)  NULL,
    [Pages] int  NOT NULL,
    [TimeStamp] datetime  NOT NULL,
    [JobId] int  NOT NULL
);
GO

-- Creating table 'Printers'
CREATE TABLE [dbo].[Printers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [UserId] int  NOT NULL,
    [ComputerId] int  NOT NULL,
    [ServerId] int  NOT NULL,
    [Enabled] bit  NOT NULL
);
GO

-- Creating table 'Servers'
CREATE TABLE [dbo].[Servers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AccountName] nvarchar(max)  NULL,
    [FullName] nvarchar(max)  NULL,
    [Department] nvarchar(max)  NULL,
    [Position] nvarchar(max)  NULL,
    [Company] nvarchar(max)  NULL,
    [Sid] nvarchar(max)  NULL
);
GO

-- Creating table 'Configs'
CREATE TABLE [dbo].[Configs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AdEnabled] tinyint  NOT NULL,
    [AdServer] nvarchar(max)  NULL,
    [AdUser] nvarchar(max)  NULL,
    [AdPassword] nvarchar(max)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Computers'
ALTER TABLE [dbo].[Computers]
ADD CONSTRAINT [PK_Computers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PrintDatas'
ALTER TABLE [dbo].[PrintDatas]
ADD CONSTRAINT [PK_PrintDatas]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Printers'
ALTER TABLE [dbo].[Printers]
ADD CONSTRAINT [PK_Printers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Servers'
ALTER TABLE [dbo].[Servers]
ADD CONSTRAINT [PK_Servers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Configs'
ALTER TABLE [dbo].[Configs]
ADD CONSTRAINT [PK_Configs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------