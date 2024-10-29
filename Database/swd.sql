USE master
GO
CREATE DATABASE [CampusFoodSystem]
Go
USE [CampusFoodSystem]
GO

CREATE TABLE [User] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [UserId] [varchar] (50) UNIQUE NOT NULL,
  [Name] [nvarchar](max) NOT NULL,
  [UserName] [varchar](50) NOT NULL,
  [Password] [varchar] (50) NOT NULL,
  [Email] [varchar] (max) NOT NULL,
  [CampusId] [varchar] (50) NOT NULL,
  [Phone] [int] NOT NULL,
  [Role] [int] NOT NULL,
  [Balance] [int] NOT NULL,
  [Status] [int] NOT NULL,
  [AccessToken] [varchar] (max) NULL,
  [RefreshToken] [varchar] (max) NULL
);

CREATE TABLE [Campus] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [CampusId] [varchar] (50) UNIQUE NOT NULL,
  [AreaId] [varchar] (50) NOT NULL,
  [Name] [nvarchar] (max) NOT NULL,
  [Status] [int] NOT NULL

);

CREATE TABLE [Area] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [AreaId] [varchar] (50) UNIQUE NOT NULL,
  [Name] [nvarchar] (max) NOT NULL,
  [Status] [int] NOT NULL
);

CREATE TABLE [Session] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [SessionId] [varchar] (50) UNIQUE NOT NULL,
  [StartTime] [time] NOT NULL,
  [EndTime] [time] NOT NULL
);

CREATE TABLE [AreaSession] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [AreaSessionId] [varchar] (50) UNIQUE NOT NULL,
  [SessionId] [varchar] (50) NOT NULL,
  [AreaId] [varchar] (50) NOT NULL
);

CREATE TABLE [Transaction] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [TransationId] [varchar] (50) UNIQUE NOT NULL,
  [UserId] [varchar] (50) NOT NULL,
  [Type] [int] NOT NULL,
  [Amonut] [int] NOT NULL,
  [Status] [int] NOT NULL,
  [Time] [datetime] NOT NULL
);

CREATE TABLE [Order] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [OrderId] [varchar] (50) UNIQUE NOT NULL,
  [UserId] [varchar] (50) NOT NULL,
  [AreaSessionId] [varchar] (50) NOT NULL,
  [Price] [int] NOT NULL,
  [Quantity] [int] NOT NULL,
  [StoreId] [varchar] (50) NOT NULL,
  [TransationId] [varchar] (50) NOT NULL,
  [Status] [int] NOT NULL
);

CREATE TABLE [Store] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [StoreId] [varchar] (50) UNIQUE NOT NULL,
  [AreaId] [varchar] (50) NOT NULL,
  [Name] [nvarchar]  (max) NOT NULL,
  [Address] [nvarchar] (max) NOT NULL,
  [Status] [int] NOT NULL,
  [Phone] [int] NOT NULL
);

CREATE TABLE [OrderDetail] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [OrderDetailId] [varchar] (50) UNIQUE NOT NULL,
  [OrderId] [varchar]  (50) NOT NULL,
  [FoodId] [varchar] (50) NOT NULL,
  [Status] [int] NOT NULL,
  [Price] [int] NOT NULL,
  [Quantity] [int] NOT NULL,
  [Note] [nvarchar] (max) NULL
);

CREATE TABLE [Food] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [FoodId] [varchar] (50) UNIQUE NOT NULL,
  [Name] [nvarchar] (50)NOT NULL,
  [StoreId] [varchar] (50)NOT NULL,
  [Price] [int] NOT NULL,
  [Title] [nvarchar] (max) NOT NULL,
  [Description] [nvarchar] (max) NOT NULL,
  [Cate] [int]  NOT NULL,
  [Image] [varchar] NULL,
  [Status] [int] NOT NULL
);

/*--------------------------------------------------------------------------------------------------------------------------------------------------*/

/*User and Transaction*/
ALTER TABLE [Transaction] ADD CONSTRAINT FK_Transaction_User FOREIGN KEY ([UserId]) REFERENCES [User]([UserId]);

/*User and Campus*/
ALTER TABLE [User] ADD CONSTRAINT FK_User_Campus FOREIGN KEY ([CampusId]) REFERENCES [Campus]([CampusId]);

/*Campus and Area*/
ALTER TABLE [Campus] ADD CONSTRAINT FK_Campus_Area FOREIGN KEY ([AreaId]) REFERENCES [Area]([AreaId]);

/* Area, AreaSession and Session*/
ALTER TABLE [AreaSession] ADD CONSTRAINT FK_AreaSession_Area FOREIGN KEY ([AreaId]) REFERENCES [Area]([AreaId]);
ALTER TABLE [AreaSession] ADD CONSTRAINT FK_AreaSession_Session FOREIGN KEY ([SessionId]) REFERENCES [Session]([SessionId]);

/*AreaSession and Order*/
ALTER TABLE [Order] ADD CONSTRAINT FK_Order_AreaSession FOREIGN KEY ([AreaSessionId]) REFERENCES [AreaSession]([AreaSessionId]);

/* Transaction and Order*/
ALTER TABLE [Order] ADD CONSTRAINT FK_Order_Transaction FOREIGN KEY ([TransationId]) REFERENCES [Transaction]([TransationId]);

/*Order and Store*/
ALTER TABLE [Order] ADD CONSTRAINT FK_Order_Store FOREIGN KEY ([StoreId]) REFERENCES [Store]([StoreId]);

/*Store and Area*/
ALTER TABLE [Store] ADD CONSTRAINT FK_Store_Area FOREIGN KEY ([AreaId]) REFERENCES [Area]([AreaId]);

/*Store and Food*/
ALTER TABLE [Food] ADD CONSTRAINT FK_Food_Store FOREIGN KEY ([StoreId]) REFERENCES [Store]([StoreId]);

/*Order and OrderDetail*/
ALTER TABLE [OrderDetail] ADD CONSTRAINT FK_OrderDetail_Order FOREIGN KEY ([OrderId]) REFERENCES [Order]([OrderId]);

/*OrderDetail and Food*/
ALTER TABLE [OrderDetail] ADD CONSTRAINT FK_OrderDetail_Food FOREIGN KEY ([FoodId]) REFERENCES [Food]([FoodId]);

/*User and Order*/
ALTER TABLE [Order] ADD CONSTRAINT FK_Order_User FOREIGN KEY ([UserId]) REFERENCES [User]([UserId]);

/*--------------------------------------------------------------------------------------------------------------------------------------------------*/

/*Check Status Campus 1-active 2-inactive*/
ALTER TABLE [Campus] ADD CONSTRAINT CHK_Campus_Status CHECK (Status IN (1, 2));

/*Check Status Area 1-active 2-inactive*/
ALTER TABLE [Area] ADD CONSTRAINT CHK_Area_Status CHECK (Status IN (1, 2));

/*Check Status Store 1-active 2-inactive*/
ALTER TABLE [Store] ADD CONSTRAINT CHK_Store_Status CHECK (Status IN (1, 2));

/*Check Status Food 1-Yes 2-No*/
ALTER TABLE [Food] ADD CONSTRAINT CHK_Food_Status CHECK (Status IN (1, 2));

/*Check Status User 1-active 2-inactive*/
ALTER TABLE [User] ADD CONSTRAINT CHK_User_Status CHECK (Status IN (1, 2));

/*Check Status User 1-admin 2-customer 3-shipper*/
ALTER TABLE [User] ADD CONSTRAINT CHK_User_Role CHECK (Status IN (1, 2, 3));

/*Check Status Order 1-Done 2-Waite 3-Cancel*/
ALTER TABLE [Order] ADD CONSTRAINT CHK_Order_Status CHECK (Status IN (1, 2, 3));

/*Check Status Transation 1-Order 2-Load 3-Wait*/
ALTER TABLE [Transaction] ADD CONSTRAINT CHK_Transation_Status CHECK (Status IN (1, 2, 3));

