USE [master]
GO
/****** Object:  Database [ECommerce]    Script Date: 11/14/2025 11:54:49 AM ******/
CREATE DATABASE [ECommerce]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ECommerce', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\ECommerce.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ECommerce_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\ECommerce_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [ECommerce] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ECommerce].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ECommerce] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ECommerce] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ECommerce] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ECommerce] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ECommerce] SET ARITHABORT OFF 
GO
ALTER DATABASE [ECommerce] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ECommerce] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ECommerce] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ECommerce] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ECommerce] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ECommerce] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ECommerce] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ECommerce] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ECommerce] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ECommerce] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ECommerce] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ECommerce] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ECommerce] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ECommerce] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ECommerce] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ECommerce] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ECommerce] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ECommerce] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [ECommerce] SET  MULTI_USER 
GO
ALTER DATABASE [ECommerce] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ECommerce] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ECommerce] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ECommerce] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ECommerce] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [ECommerce] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [ECommerce] SET QUERY_STORE = ON
GO
ALTER DATABASE [ECommerce] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [ECommerce]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11/14/2025 11:54:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 11/14/2025 11:54:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 11/14/2025 11:54:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[Id] [uniqueidentifier] NOT NULL,
	[OrderId] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Quantity] [int] NOT NULL,
	[PriceAtOrder] [decimal](18, 2) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 11/14/2025 11:54:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [uniqueidentifier] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[OrderDate] [datetime2](7) NOT NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 11/14/2025 11:54:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[CategoryId] [uniqueidentifier] NOT NULL,
	[StockQuantity] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RefreshTokens]    Script Date: 11/14/2025 11:54:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefreshTokens](
	[Id] [uniqueidentifier] NOT NULL,
	[Token] [nvarchar](512) NOT NULL,
	[ExpiresAt] [datetime2](7) NOT NULL,
	[RevokedAt] [datetime2](7) NULL,
	[ReplacedByToken] [nvarchar](512) NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 11/14/2025 11:54:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Email] [nvarchar](200) NOT NULL,
	[PasswordHash] [nvarchar](512) NOT NULL,
	[Role] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251113183149_InitMigration', N'8.0.2')
GO
INSERT [dbo].[Categories] ([Id], [Name], [Description], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', N'Electronics', N'Electronic devices and accessories', CAST(N'2025-11-13T18:49:03.4570143' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Categories] ([Id], [Name], [Description], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', N'Clothing', N'Men''s and women''s clothing', CAST(N'2025-11-13T18:49:03.4572023' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Categories] ([Id], [Name], [Description], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'cccccccc-cccc-cccc-cccc-cccccccccccc', N'Books', N'Books and reading materials', CAST(N'2025-11-13T18:49:03.4572038' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Categories] ([Id], [Name], [Description], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'dddddddd-dddd-dddd-dddd-dddddddddddd', N'Home & Garden', N'Home improvement and garden supplies', CAST(N'2025-11-13T18:49:03.4572045' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Categories] ([Id], [Name], [Description], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', N'Sports & Outdoors', N'Sports equipment and outdoor gear', CAST(N'2025-11-13T18:49:03.4572064' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b1111111-1111-1111-1111-111111111111', N'a1111111-1111-1111-1111-111111111111', N'10000000-0000-0000-0000-000000000001', 1, CAST(999.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0715308' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b1111111-1111-1111-1111-111111111112', N'a1111111-1111-1111-1111-111111111111', N'10000000-0000-0000-0000-000000000002', 1, CAST(249.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0718978' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b2222222-2222-2222-2222-222222222221', N'a2222222-2222-2222-2222-222222222222', N'20000000-0000-0000-0000-000000000002', 2, CAST(79.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0719861' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b2222222-2222-2222-2222-222222222222', N'a2222222-2222-2222-2222-222222222222', N'30000000-0000-0000-0000-000000000002', 3, CAST(14.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0719889' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b2222222-2222-2222-2222-222222222223', N'a2222222-2222-2222-2222-222222222222', N'30000000-0000-0000-0000-000000000003', 1, CAST(29.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0719897' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b3333333-3333-3333-3333-333333333331', N'a3333333-3333-3333-3333-333333333333', N'10000000-0000-0000-0000-000000000003', 1, CAST(1299.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0719914' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b4444444-4444-4444-4444-444444444441', N'a4444444-4444-4444-4444-444444444444', N'50000000-0000-0000-0000-000000000001', 1, CAST(119.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0719931' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b4444444-4444-4444-4444-444444444442', N'a4444444-4444-4444-4444-444444444444', N'20000000-0000-0000-0000-000000000001', 3, CAST(19.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0719944' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b5555555-5555-5555-5555-555555555551', N'a5555555-5555-5555-5555-555555555555', N'10000000-0000-0000-0000-000000000004', 1, CAST(299.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0720012' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b5555555-5555-5555-5555-555555555552', N'a5555555-5555-5555-5555-555555555555', N'50000000-0000-0000-0000-000000000002', 2, CAST(34.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0720022' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b6666666-6666-6666-6666-666666666661', N'a6666666-6666-6666-6666-666666666666', N'20000000-0000-0000-0000-000000000003', 1, CAST(149.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0720313' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b6666666-6666-6666-6666-666666666662', N'a6666666-6666-6666-6666-666666666666', N'20000000-0000-0000-0000-000000000001', 2, CAST(19.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0720322' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b7777777-7777-7777-7777-777777777771', N'a7777777-7777-7777-7777-777777777777', N'30000000-0000-0000-0000-000000000001', 2, CAST(39.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0720345' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b7777777-7777-7777-7777-777777777772', N'a7777777-7777-7777-7777-777777777777', N'30000000-0000-0000-0000-000000000002', 1, CAST(14.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0720352' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [PriceAtOrder], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b8888888-8888-8888-8888-888888888881', N'a8888888-8888-8888-8888-888888888888', N'10000000-0000-0000-0000-000000000002', 2, CAST(249.99 AS Decimal(18, 2)), CAST(N'2025-11-13T18:49:04.0720375' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Orders] ([Id], [CustomerId], [OrderDate], [TotalAmount], [Status], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'a1111111-1111-1111-1111-111111111111', N'22222222-2222-2222-2222-222222222222', CAST(N'2025-11-03T18:49:04.0710301' AS DateTime2), CAST(1249.98 AS Decimal(18, 2)), 2, CAST(N'2025-11-13T18:49:04.0709315' AS DateTime2), CAST(N'2025-11-03T18:49:04.0710301' AS DateTime2), NULL, 0)
GO
INSERT [dbo].[Orders] ([Id], [CustomerId], [OrderDate], [TotalAmount], [Status], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'a2222222-2222-2222-2222-222222222222', N'33333333-3333-3333-3333-333333333333', CAST(N'2025-11-10T18:49:04.0719842' AS DateTime2), CAST(234.94 AS Decimal(18, 2)), 1, CAST(N'2025-11-13T18:49:04.0719833' AS DateTime2), CAST(N'2025-11-10T18:49:04.0719842' AS DateTime2), NULL, 0)
GO
INSERT [dbo].[Orders] ([Id], [CustomerId], [OrderDate], [TotalAmount], [Status], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'a3333333-3333-3333-3333-333333333333', N'44444444-4444-4444-4444-444444444444', CAST(N'2025-11-06T18:49:04.0719909' AS DateTime2), CAST(1299.99 AS Decimal(18, 2)), 3, CAST(N'2025-11-13T18:49:04.0719907' AS DateTime2), CAST(N'2025-11-07T18:49:04.0719909' AS DateTime2), NULL, 0)
GO
INSERT [dbo].[Orders] ([Id], [CustomerId], [OrderDate], [TotalAmount], [Status], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'a4444444-4444-4444-4444-444444444444', N'22222222-2222-2222-2222-222222222222', CAST(N'2025-11-12T18:49:04.0719927' AS DateTime2), CAST(179.96 AS Decimal(18, 2)), 1, CAST(N'2025-11-13T18:49:04.0719925' AS DateTime2), CAST(N'2025-11-12T18:49:04.0719927' AS DateTime2), NULL, 0)
GO
INSERT [dbo].[Orders] ([Id], [CustomerId], [OrderDate], [TotalAmount], [Status], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'a5555555-5555-5555-5555-555555555555', N'33333333-3333-3333-3333-333333333333', CAST(N'2025-10-29T18:49:04.0719954' AS DateTime2), CAST(369.97 AS Decimal(18, 2)), 2, CAST(N'2025-11-13T18:49:04.0719952' AS DateTime2), CAST(N'2025-10-29T18:49:04.0719954' AS DateTime2), NULL, 0)
GO
INSERT [dbo].[Orders] ([Id], [CustomerId], [OrderDate], [TotalAmount], [Status], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'a6666666-6666-6666-6666-666666666666', N'44444444-4444-4444-4444-444444444444', CAST(N'2025-11-13T13:49:04.0720040' AS DateTime2), CAST(189.97 AS Decimal(18, 2)), 1, CAST(N'2025-11-13T18:49:04.0720038' AS DateTime2), CAST(N'2025-11-13T13:49:04.0720040' AS DateTime2), NULL, 0)
GO
INSERT [dbo].[Orders] ([Id], [CustomerId], [OrderDate], [TotalAmount], [Status], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'a7777777-7777-7777-7777-777777777777', N'22222222-2222-2222-2222-222222222222', CAST(N'2025-10-24T18:49:04.0720334' AS DateTime2), CAST(94.97 AS Decimal(18, 2)), 2, CAST(N'2025-11-13T18:49:04.0720332' AS DateTime2), CAST(N'2025-10-24T18:49:04.0720334' AS DateTime2), NULL, 0)
GO
INSERT [dbo].[Orders] ([Id], [CustomerId], [OrderDate], [TotalAmount], [Status], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'a8888888-8888-8888-8888-888888888888', N'33333333-3333-3333-3333-333333333333', CAST(N'2025-11-08T18:49:04.0720371' AS DateTime2), CAST(499.98 AS Decimal(18, 2)), 3, CAST(N'2025-11-13T18:49:04.0720369' AS DateTime2), CAST(N'2025-11-09T18:49:04.0720371' AS DateTime2), NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'10000000-0000-0000-0000-000000000001', N'Smartphone Pro Max', N'Latest generation smartphone with advanced features', CAST(999.99 AS Decimal(18, 2)), N'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 49, 1, CAST(N'2025-11-13T18:49:03.5405094' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'20000000-0000-0000-0000-000000000001', N'Classic T-Shirt', N'Comfortable cotton t-shirt in multiple colors', CAST(19.99 AS Decimal(18, 2)), N'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 195, 1, CAST(N'2025-11-13T18:49:03.5410735' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'30000000-0000-0000-0000-000000000001', N'Programming Guide', N'Comprehensive guide to modern programming', CAST(39.99 AS Decimal(18, 2)), N'cccccccc-cccc-cccc-cccc-cccccccccccc', 118, 1, CAST(N'2025-11-13T18:49:03.5410831' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'40000000-0000-0000-0000-000000000001', N'Garden Tool Set', N'Complete set of gardening tools', CAST(49.99 AS Decimal(18, 2)), N'dddddddd-dddd-dddd-dddd-dddddddddddd', 60, 1, CAST(N'2025-11-13T18:49:03.5410856' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'50000000-0000-0000-0000-000000000001', N'Running Shoes', N'Professional running shoes for athletes', CAST(119.99 AS Decimal(18, 2)), N'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 99, 1, CAST(N'2025-11-13T18:49:03.5410868' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'10000000-0000-0000-0000-000000000002', N'Wireless Headphones', N'Premium noise-cancelling wireless headphones', CAST(249.99 AS Decimal(18, 2)), N'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 99, 1, CAST(N'2025-11-13T18:49:03.5410700' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'20000000-0000-0000-0000-000000000002', N'Denim Jeans', N'Premium quality denim jeans', CAST(79.99 AS Decimal(18, 2)), N'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 148, 1, CAST(N'2025-11-13T18:49:03.5410802' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'30000000-0000-0000-0000-000000000002', N'Mystery Novel', N'Bestselling mystery thriller', CAST(14.99 AS Decimal(18, 2)), N'cccccccc-cccc-cccc-cccc-cccccccccccc', 196, 1, CAST(N'2025-11-13T18:49:03.5410838' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'40000000-0000-0000-0000-000000000002', N'Coffee Maker', N'Programmable coffee maker with timer', CAST(89.99 AS Decimal(18, 2)), N'dddddddd-dddd-dddd-dddd-dddddddddddd', 40, 1, CAST(N'2025-11-13T18:49:03.5410862' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'50000000-0000-0000-0000-000000000002', N'Yoga Mat', N'Non-slip premium yoga mat', CAST(34.99 AS Decimal(18, 2)), N'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 148, 1, CAST(N'2025-11-13T18:49:03.5410875' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'10000000-0000-0000-0000-000000000003', N'Laptop Ultrabook', N'High-performance laptop for professionals', CAST(1299.99 AS Decimal(18, 2)), N'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 30, 1, CAST(N'2025-11-13T18:49:03.5410720' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'20000000-0000-0000-0000-000000000003', N'Winter Jacket', N'Warm and stylish winter jacket', CAST(149.99 AS Decimal(18, 2)), N'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 79, 1, CAST(N'2025-11-13T18:49:03.5410810' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'30000000-0000-0000-0000-000000000003', N'Cookbook Collection', N'Delicious recipes from around the world', CAST(29.99 AS Decimal(18, 2)), N'cccccccc-cccc-cccc-cccc-cccccccccccc', 89, 1, CAST(N'2025-11-13T18:49:03.5410849' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'50000000-0000-0000-0000-000000000003', N'Dumbbell Set', N'Adjustable dumbbell set for home workouts', CAST(159.99 AS Decimal(18, 2)), N'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 25, 1, CAST(N'2025-11-13T18:49:03.5410882' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [CategoryId], [StockQuantity], [IsActive], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'10000000-0000-0000-0000-000000000004', N'Smart Watch', N'Fitness tracking smartwatch with health monitoring', CAST(299.99 AS Decimal(18, 2)), N'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 74, 1, CAST(N'2025-11-13T18:49:03.5410728' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[RefreshTokens] ([Id], [Token], [ExpiresAt], [RevokedAt], [ReplacedByToken], [UserId], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b7c40c53-8078-4ef2-ae36-0fc5be9dea98', N'rb0Jt2sZ3nbH03MXtacnOMeryWP5JUE0AtKl8rDfvv62OsX7MiWa52QIgrUkrDvYqJdgKs1+aW1jJNzHS6QL3Q==', CAST(N'2025-11-20T19:32:39.1510900' AS DateTime2), NULL, NULL, N'22222222-2222-2222-2222-222222222222', CAST(N'2025-11-13T19:32:39.1511820' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[RefreshTokens] ([Id], [Token], [ExpiresAt], [RevokedAt], [ReplacedByToken], [UserId], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'52e6dd2b-d2b4-46bb-95e0-2363debfe84f', N'46vv1hDXJULQ/xQamZYvU/+SggcZYFVLOynKLV3GYcX7zxA7TsSJauT6KHweDfvT1QTKLo0G0Zc6y0MvWFlTPQ==', CAST(N'2025-11-20T18:51:39.2822970' AS DateTime2), NULL, NULL, N'22222222-2222-2222-2222-222222222222', CAST(N'2025-11-13T18:51:39.2845250' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[RefreshTokens] ([Id], [Token], [ExpiresAt], [RevokedAt], [ReplacedByToken], [UserId], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'09b2c358-d65a-498a-b281-2f143ff2a996', N'h8ybXfaCKWvL3gOn1KFou/l84cToKAr+OFJUHW1aqrZ1o+WN0wsqp/75uh8ycGiU8YQIJaLeaI7JnrrWuNgglw==', CAST(N'2025-11-20T20:00:05.8642074' AS DateTime2), NULL, NULL, N'22222222-2222-2222-2222-222222222222', CAST(N'2025-11-13T20:00:05.8677289' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[RefreshTokens] ([Id], [Token], [ExpiresAt], [RevokedAt], [ReplacedByToken], [UserId], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'f308562b-fdd3-4a76-afda-4b344be55f8c', N'wTauaToF+nO7tr4r9Hf0LALrB/h4kxNrhUuGnP3/eq2khNglmiBxoKqYmO5davJJ+doeSyf1DnxtioOvoP28/w==', CAST(N'2025-11-20T19:49:18.1988369' AS DateTime2), NULL, NULL, N'22222222-2222-2222-2222-222222222222', CAST(N'2025-11-13T19:49:18.1989462' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[RefreshTokens] ([Id], [Token], [ExpiresAt], [RevokedAt], [ReplacedByToken], [UserId], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'542b9972-8fde-4dea-947a-568fab4b312c', N'4S1w91V9OOf0wrJZTndu5TvJ5Hk/WGSzFFg6r1qFXTEyibHgB1OenDoMYa40bwy1NZBhu3DgkX+Bzqi07ZdUlA==', CAST(N'2025-11-20T19:52:48.1069802' AS DateTime2), NULL, NULL, N'22222222-2222-2222-2222-222222222222', CAST(N'2025-11-13T19:52:48.1158114' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[RefreshTokens] ([Id], [Token], [ExpiresAt], [RevokedAt], [ReplacedByToken], [UserId], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b804e7a0-9603-4845-89c5-572ebb7ff43d', N'oSepoFw4iGlFVC19I11H5EVfe/awZSkEHJg3nHmW1HoUc8cEJS0bfI8ZgV5RSX+VopkUWCuSovJIXr3YpWjl8w==', CAST(N'2025-11-20T20:11:05.8992742' AS DateTime2), NULL, NULL, N'22222222-2222-2222-2222-222222222222', CAST(N'2025-11-13T20:11:05.9033901' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[RefreshTokens] ([Id], [Token], [ExpiresAt], [RevokedAt], [ReplacedByToken], [UserId], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'434a132b-bbef-44c1-8432-7b982d08412d', N'N4TEse5K28Jmd7w7tSJAtmVQNANjMaJtPLDAePOUYjvbbCltf+yYQfbZpUjEan2Pm9kul8ZAfeuP6YEPoSM6sQ==', CAST(N'2025-11-20T20:12:42.0431346' AS DateTime2), NULL, NULL, N'11111111-1111-1111-1111-111111111111', CAST(N'2025-11-13T20:12:42.0432209' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[RefreshTokens] ([Id], [Token], [ExpiresAt], [RevokedAt], [ReplacedByToken], [UserId], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'ef0d56e7-a54d-4829-8668-7ced2ac08e69', N'LGYw+J4zWCmoEFJj1kwNydzWn+8HihMUhGDQ6TpFI5RjyQfv4lkgSXXwXkofwzADOhS5Sw7SfGzdq7FQYWHmdg==', CAST(N'2025-11-20T19:43:22.3679607' AS DateTime2), NULL, NULL, N'22222222-2222-2222-2222-222222222222', CAST(N'2025-11-13T19:43:22.3720115' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[RefreshTokens] ([Id], [Token], [ExpiresAt], [RevokedAt], [ReplacedByToken], [UserId], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'b4950d9c-0ff9-43a1-9a81-9b6efe25f1e8', N'ytwtUSMPbekCj6dJTTcd9gfMiPBeSeDhyy516r+UpV6/+Td7RvPOloXTquw9Tvhi31mAtw5M+K5UUMnqEFnzxw==', CAST(N'2025-11-20T19:29:54.5802402' AS DateTime2), NULL, NULL, N'22222222-2222-2222-2222-222222222222', CAST(N'2025-11-13T19:29:54.5863326' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[RefreshTokens] ([Id], [Token], [ExpiresAt], [RevokedAt], [ReplacedByToken], [UserId], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'3536664c-f5df-483d-8fc7-c970dead5d0c', N'4qTgyq4q2QDWTbrlYuPsXr8vXf9I/JzF0azkoZDZu1W2brl9+8r9tpcofo81knV9Mn7epUhCfa4qjBjlvw8YJg==', CAST(N'2025-11-20T20:05:04.7479946' AS DateTime2), NULL, NULL, N'22222222-2222-2222-2222-222222222222', CAST(N'2025-11-13T20:05:04.7512872' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[RefreshTokens] ([Id], [Token], [ExpiresAt], [RevokedAt], [ReplacedByToken], [UserId], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'493f2db4-8f3f-4f9d-819b-fe9382f5dcb8', N'8ttieleLkA1pospDbmri84ogC8r1Nbu7qzFOqkycC2oxe9EiFY/qmxyt1PUGqiHbpzL3JoWf/ZuaQ0Tk8s6njg==', CAST(N'2025-11-20T19:10:29.9264310' AS DateTime2), NULL, NULL, N'22222222-2222-2222-2222-222222222222', CAST(N'2025-11-13T19:10:29.9314034' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [Role], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'11111111-1111-1111-1111-111111111111', N'Admin User', N'admin@ecommerce.com', N'AQAAAAIAAYagAAAAEBsuFISrnqXaMhPkgRfkY8F9PQd2vfZr6XIN/MTBfM09HZOgy2LtE0VcQRYXYV00Cw==', 1, CAST(N'2025-11-13T18:49:02.4440088' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [Role], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'22222222-2222-2222-2222-222222222222', N'John Doe', N'john.doe@example.com', N'AQAAAAIAAYagAAAAEKPBIabnJlWN4JJ56pGj1OEumwlU1mhqr5lKya46W2KAfwAooJTlaHXpDv3J8RxfcQ==', 2, CAST(N'2025-11-13T18:49:02.4444529' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [Role], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'33333333-3333-3333-3333-333333333333', N'Jane Smith', N'jane.smith@example.com', N'AQAAAAIAAYagAAAAECKAR7H4GmJj6BBatT/e+z7RnqvStyF2IjqTr/FL2cCCUCAHX28w7lUEE11LkDGgcw==', 2, CAST(N'2025-11-13T18:49:02.4444554' AS DateTime2), NULL, NULL, 0)
GO
INSERT [dbo].[Users] ([Id], [Name], [Email], [PasswordHash], [Role], [CreatedDate], [UpdatedDate], [DeletedDate], [IsDeleted]) VALUES (N'44444444-4444-4444-4444-444444444444', N'Bob Johnson', N'bob.johnson@example.com', N'AQAAAAIAAYagAAAAEKSb4Oh5DSLhQsixw+2CQGDoJ6qcv9xt3ydVm7hbPgV28ea9Lj6l0gbUDLPiePZTeg==', 2, CAST(N'2025-11-13T18:49:02.4444561' AS DateTime2), NULL, NULL, 0)
GO
/****** Object:  Index [IX_OrderItems_OrderId]    Script Date: 11/14/2025 11:54:49 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderId] ON [dbo].[OrderItems]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_ProductId]    Script Date: 11/14/2025 11:54:49 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_ProductId] ON [dbo].[OrderItems]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_CustomerId]    Script Date: 11/14/2025 11:54:49 AM ******/
CREATE NONCLUSTERED INDEX [IX_Orders_CustomerId] ON [dbo].[Orders]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Products_CategoryId]    Script Date: 11/14/2025 11:54:49 AM ******/
CREATE NONCLUSTERED INDEX [IX_Products_CategoryId] ON [dbo].[Products]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Products_Name_CategoryId]    Script Date: 11/14/2025 11:54:49 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Products_Name_CategoryId] ON [dbo].[Products]
(
	[Name] ASC,
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_RefreshTokens_Token]    Script Date: 11/14/2025 11:54:49 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_RefreshTokens_Token] ON [dbo].[RefreshTokens]
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RefreshTokens_UserId]    Script Date: 11/14/2025 11:54:49 AM ******/
CREATE NONCLUSTERED INDEX [IX_RefreshTokens_UserId] ON [dbo].[RefreshTokens]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_Email]    Script Date: 11/14/2025 11:54:49 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getutcdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [StockQuantity]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_OrderItems_Orders_OrderId]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_OrderItems_Products_ProductId]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Users_CustomerId] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Users_CustomerId]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Categories_CategoryId]
GO
ALTER TABLE [dbo].[RefreshTokens]  WITH CHECK ADD  CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RefreshTokens] CHECK CONSTRAINT [FK_RefreshTokens_Users_UserId]
GO
USE [master]
GO
ALTER DATABASE [ECommerce] SET  READ_WRITE 
GO
