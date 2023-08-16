USE [master]
GO
/****** Object:  Database [IMS]    Script Date: 16-Aug-23 2:37:28 PM ******/
CREATE DATABASE [IMS]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'IMS', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\IMS.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'IMS_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\IMS_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [IMS] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [IMS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [IMS] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [IMS] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [IMS] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [IMS] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [IMS] SET ARITHABORT OFF 
GO
ALTER DATABASE [IMS] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [IMS] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [IMS] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [IMS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [IMS] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [IMS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [IMS] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [IMS] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [IMS] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [IMS] SET  DISABLE_BROKER 
GO
ALTER DATABASE [IMS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [IMS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [IMS] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [IMS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [IMS] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [IMS] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [IMS] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [IMS] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [IMS] SET  MULTI_USER 
GO
ALTER DATABASE [IMS] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [IMS] SET DB_CHAINING OFF 
GO
ALTER DATABASE [IMS] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [IMS] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [IMS] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [IMS] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [IMS] SET QUERY_STORE = ON
GO
ALTER DATABASE [IMS] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [IMS]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationUser]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationUser](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](30) NOT NULL,
	[CreateBy] [bigint] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifyBy] [bigint] NULL,
	[ModificationDate] [datetime2](7) NULL,
	[Status] [int] NOT NULL,
	[Rank] [bigint] NULL,
	[VersionNumber] [bigint] NULL,
	[BusinessId] [nvarchar](255) NULL,
	[AspNetUsersId] [bigint] NULL,
	[Email] [nvarchar](255) NULL,
 CONSTRAINT [PK__Applicat__3214EC077C9B296D] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [bigint] NOT NULL,
	[RoleId] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bank]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bank](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[VersionNumber] [bigint] NULL,
	[Status] [int] NOT NULL,
	[Rank] [bigint] NULL,
	[CreateBy] [bigint] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifyBy] [bigint] NULL,
	[ModificationDate] [datetime2](7) NULL,
	[BusinessId] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Brand]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brand](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[VersionNumber] [bigint] NULL,
	[Status] [int] NOT NULL,
	[Rank] [bigint] NULL,
	[CreateBy] [bigint] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifyBy] [bigint] NULL,
	[ModificationDate] [datetime2](7) NULL,
	[BusinessId] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[VersionNumber] [bigint] NULL,
	[Status] [int] NOT NULL,
	[Rank] [bigint] NULL,
	[CreateBy] [bigint] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifyBy] [bigint] NULL,
	[ModificationDate] [datetime2](7) NULL,
	[BusinessId] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[VersionNumber] [bigint] NULL,
	[Status] [int] NOT NULL,
	[Rank] [bigint] NULL,
	[CreateBy] [bigint] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifyBy] [bigint] NULL,
	[ModificationDate] [datetime2](7) NULL,
	[BusinessId] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Address] [nvarchar](255) NULL,
	[ContactNumber] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[VersionNumber] [bigint] NULL,
	[Status] [int] NOT NULL,
	[Rank] [bigint] NULL,
	[CreateBy] [bigint] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifyBy] [bigint] NULL,
	[ModificationDate] [datetime2](7) NULL,
	[BusinessId] [nvarchar](255) NULL,
	[PurchaseId] [bigint] NULL,
	[SaleId] [bigint] NULL,
	[VoucherId] [nvarchar](255) NULL,
	[OperationType] [int] NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[PaidAmount] [decimal](18, 2) NULL,
 CONSTRAINT [PK__Payment__3214EC07E0CE0A6E] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentDetails]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentDetails](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[VersionNumber] [bigint] NULL,
	[Status] [int] NOT NULL,
	[Rank] [bigint] NULL,
	[CreateBy] [bigint] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifyBy] [bigint] NULL,
	[ModificationDate] [datetime2](7) NULL,
	[BusinessId] [nvarchar](255) NULL,
	[PaymentMethod] [int] NULL,
	[TransactionId] [nvarchar](255) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[PaymentDate] [datetime2](7) NOT NULL,
	[PaymentId] [bigint] NOT NULL,
	[BankId] [bigint] NULL,
 CONSTRAINT [PK__PaymentD__3214EC0716DD4C45] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[VersionNumber] [bigint] NULL,
	[Status] [int] NOT NULL,
	[Rank] [bigint] NULL,
	[CreateBy] [bigint] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifyBy] [bigint] NULL,
	[ModificationDate] [datetime2](7) NULL,
	[BusinessId] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[BuyingPrice] [decimal](19, 5) NULL,
	[SellingPrice] [decimal](19, 5) NULL,
	[DiscountPrice] [decimal](19, 5) NULL,
	[ProfitMargin] [decimal](19, 5) NOT NULL,
	[Image] [nvarchar](255) NULL,
	[InStockQuantity] [int] NULL,
	[CategoryId] [bigint] NOT NULL,
	[BrandId] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Purchase]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Purchase](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[VersionNumber] [bigint] NULL,
	[Status] [int] NOT NULL,
	[Rank] [bigint] NULL,
	[CreateBy] [bigint] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifyBy] [bigint] NULL,
	[ModificationDate] [datetime2](7) NULL,
	[BusinessId] [nvarchar](255) NULL,
	[VoucherId] [nvarchar](255) NULL,
	[SupplierId] [bigint] NULL,
	[PurchaseDate] [datetime2](7) NULL,
	[GrandTotalPrice] [decimal](18, 2) NULL,
	[IsPaid] [bit] NULL,
	[PaymentId] [bigint] NULL,
 CONSTRAINT [PK__Purchase__3214EC071090C951] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseDetails]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseDetails](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[VersionNumber] [bigint] NULL,
	[Status] [int] NOT NULL,
	[Rank] [bigint] NULL,
	[CreateBy] [bigint] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifyBy] [bigint] NULL,
	[ModificationDate] [datetime2](7) NULL,
	[BusinessId] [nvarchar](255) NULL,
	[ProductId] [bigint] NULL,
	[Quantity] [int] NULL,
	[TotalPrice] [decimal](18, 2) NULL,
	[PurchaseId] [bigint] NULL,
 CONSTRAINT [PK__Purchase__3214EC07C86008ED] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sale]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sale](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[VersionNumber] [bigint] NULL,
	[Status] [int] NOT NULL,
	[Rank] [bigint] NULL,
	[CreateBy] [bigint] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifyBy] [bigint] NULL,
	[ModificationDate] [datetime2](7) NULL,
	[BusinessId] [nvarchar](255) NULL,
	[VoucherId] [nvarchar](255) NULL,
	[CustomerId] [bigint] NULL,
	[SaleDate] [datetime2](7) NULL,
	[GrandTotalPrice] [decimal](18, 2) NULL,
	[IsPaid] [bit] NULL,
	[PaymentId] [bigint] NULL,
 CONSTRAINT [PK__Sale__3214EC077927691D] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SaleDetails]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SaleDetails](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[VersionNumber] [bigint] NULL,
	[Status] [int] NOT NULL,
	[Rank] [bigint] NULL,
	[CreateBy] [bigint] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifyBy] [bigint] NULL,
	[ModificationDate] [datetime2](7) NULL,
	[BusinessId] [nvarchar](255) NULL,
	[ProductId] [bigint] NULL,
	[Quantity] [int] NULL,
	[TotalPrice] [decimal](18, 2) NULL,
	[SaleId] [bigint] NULL,
 CONSTRAINT [PK__SaleDeta__3214EC07554AB429] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Supplier]    Script Date: 16-Aug-23 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Supplier](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[VersionNumber] [bigint] NULL,
	[Status] [int] NOT NULL,
	[Rank] [bigint] NULL,
	[CreateBy] [bigint] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ModifyBy] [bigint] NULL,
	[ModificationDate] [datetime2](7) NULL,
	[BusinessId] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Address] [nvarchar](255) NULL,
	[ContactNumber] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__MigrationHistory] ([MigrationId], [ContextKey], [Model], [ProductVersion]) VALUES (N'202307160856287_AddIdentityTables', N'IMS.Migrations.Configuration', 0x1F8B0800000000000400ED5C5B6FE3B8157E2FD0FF20E8A92DB251926917DBC0DE45D649DAA0930BC69945DF06B4443BC4489456A2B2098AFD657DE84FEA5FE8A16EE655175BB19D4531C0C0E6E59CC3C38F3C87E4E7FCF7DFFF99FCF01285CE334E3312D3A97B7A7CE23A98FA7140E86AEAE66CF9CD77EE0FDFFFFE7793AB207A717EAADB7DE0EDA027CDA6EE1363C9B9E765FE138E50761C113F8DB378C98EFD38F250107B6727277FF54E4F3D0C225C90E538934F396524C2C517F83A8BA98F1396A3F0360E709855E550332FA43A7728C259827C3C756F6EE7C7652BD7B90809020BE6385CBA0EA2346688817DE79F333C67694C57F3040A50F8F89A6068B74461862BBBCFD7CDFB0EE1E48C0FC15B77AC45F979C6E268A0C0D30F954F3CB5FB469E751B9F81D7AEC0BBEC958FBAF0DCD4FD1487F886B287AFAEA36A3B9F85296F297AF6B8697FE440E95133F1800FFEEFC899E521CB533CA53867290A8F9C877C1112FF1FF8F531FE8AE994E661289A0446419D5400450F699CE094BD7EC2CBCAD09BC0753CB99FA7766CBA097DAA1150F6ED9F5DE70E94A345889B1917463B67718AFF86294E11C3C103620CA794CBC085CF34ED8A2EFE7FAD0D2006ABC4756ED1CB474C57EC69EA9EFDE55BD7B9262F38A84B2A0B3E53028B0A3AB134C7060B15AD77E899AC0A8315FD80EC1480FF0987456DF6449212FFEB29FB52B5B94EE388170AB35F567D99C779EAF361C4E6FA4794AE30938D9A786B50B5428D8B180A37A9CFAE21C795EBB033362D6CDC00A1B58A7694B623AFD6DD5F46EF29BB48127064313DDCD29E93A6F4FAFF4E21EBBA8A100947D82A7A6881F0B92469849BD1FD180358101D8CB1079465BFC469F077943DB5980E1F47307D8EFD3C0547CE198A9237D7F6F014537C97470B8EEFDDE91A6D6A1E7F89AF910F90BCA2BCD7D6F23EC6FED738675734B804787F667E2D907F7D24517F01A39873E1FB38CBAE01CC3898C5901D0A0BF5C3D960717C43DA77A89E858844E658AD6C9D5FEAA6EB906D6EA1456E4B3353006F33F563BC22B49FA97553BBA9658B4E53AB66434DE5C2FA595AB5B41B5A34E8B4B36CB5754A54CCCCD09C68DDE9DD4557E3A21D2FBA8E915115DEE502DE3C20149A7E42613EB6AA41102C56DC5008AE3BED1A828566287E26018FD93DD2F3BA3188EFD5DE9CF977035DB1CC3AA3A767DF8D1262D4F4421CE6AE950F5F78368C5E6459EC93027DEA2D457584950D8154C5E938CF96BE904EC3E013C027E17B3AD8006E71555CDCD34B1C62869D0BBFBCBC99A1CC4781EE091847D0D7A43A5E0826290763D9AC3F69DA00A638E59B21E2F97D064B8B50A6639A509F24286CF78CD2ADE78ECF87DC28506B2E718229DFACDB1DD047B3F954CDB5374A9489E8F2CDC413C0D58E394B2A669BE7AEBC6C3DDDDAA17A2738EC480815388A49C99BE0B1DD5D3B8065BB3FFA1860BD1EDA073AABECBBEFECABA9F8A1A153390328E814F3959DA05376D71ED029FBE3DDA1B33C71F59D7CE5F87568D894CF7D3B8EE3ADBEDA033025671C182ECBEC12FA30E881531D9B970B5E895F98E1D8037656279FAC4A88557C70E173CCE4EB877546ABE79B5EBB04093E26510ABE7A883349D116510F39EB74C066979830F410B8DEC16D02C53D5E11284CB7EC7FE9954B686579085311D87D8E6886D2CCB706E3EECC5F10A24EB9BA2BC923EDE105DBFDA1EE8C3EE9ED9004571955BB6B3A9251459684BED15D54DF5B76BBC894630DC9B2B67291921129B2A4F534BA8B2AAC777BC810E70744FAADFC2347E531D6587D3DD10490A66EE295EC93AA60E259682A935B942484AE04DA4A55E2CC4BCECAEC9BF970524754CAF0FCCCC0ED68AC6D34B138452BACD4826AB0F49AA419BB440C2D10BF9C990591D6CC182E2D1B7CAD528C88FA0CD69B7DDD9A7FAE721303CFC49054541DAF615811CF4C8A1B63656BB6F475387108852835DC50CFE2308FA83D4BB2F72E1F94C4FE65892E61E229C66B5990E61F2D51959DDD6B2A9435B0E19CC8F9C7F079E9E86FF36E9D318AFEB565917629F55D9228C576BFB4D779DA787AD4C46EF804754A789BE5531122440155D14019C29BBA264CA8EB2F55A63D8832E59AFE12156E832852A91A60A5C860908C142B369267F1A8B9457F0D3A674194AED7F6976C602F88A20DD51BC836D8ACD6F5976A203888820DD5FD65AFD90EEAE67980014A4CF3B78850C24974B310D526E06D36C071229CF06A2D0A128A07CAAADEA5356155F94181473C006D011EE1D66133F0B409B0EF2BD2ABB1BCADB43E75DB654A4FC1D2D6DDF6146E97370CA26F0A04ED5CA63669B437E733E51C36A9CE44DDBF29D00E496513D7A9DD0861FB3563383AE60D8EE73F87B39060BE49D70D6E11254B9CB192F2E09E9D9C9E293F4F389C9F0A785916848633A5F1F702F284ED803BB420B016C0B39D04A181C405910E489F51EA3F2113D3F98606F865EAFEABE8755EDC42F04F45F19173937DA6E4E71C2A1ED31C3BBFEA8CC27118D95D24FAC6CA5FDF05E3BD9ED32E6FDFFCF34BD9F1C8B94F61559D3B27DCC75B31E507E82E3B0ED0BD31C3FE37B3AA2496FB7A59D957C5E6A4F60561A310DA6B2BFF10A1973F0E35CD485ADF4AA281983E96BC515C68239E6F22CB4A3A0FE02B2B48E7C3066B26A16F629A95805E2C9B2DE9E7FDE34DDD730FF1C67620DAC5F6F4267BD33E6390C62DDE6A4DEBFCE101E2C6E3086F9775BC333EAF319815A4DAEDE9BAA3C9DE1DC4DF90C5BB57E2AE9269EF8BAFBB0B524FD7C3CE6F8A997B008C329530B367F2EDAE20D67A337BE00CC661FCDA03C19818B3F74CA1DD15C65A2F700F1C638358B20702B1FD45C83D016C589CDC3BDB5567F9589E54B4FBDD562A6B790D0EC7F3450C135FE68617597287999117D9CE75EDA6BADAD5D50DBB556A6BA49B19DBAEB6DF28C560DEC99C6D5758933907F16B7BD06BDBB5D6FCC8FD917035DEAD9116DCB635594948EF89712B0DA383C5DD9A6EDADFBBDF13BB762B77E8EBC3F482FB7E98B45B3963DC85328036AB3FC342FC13FE001C04E08CACD622F89F83A3D897225FD3E6862EE33A002B16D54D943B925BCC500061F1226564897C06D5FCAAB7F8BD7471A7C61F1C1638B8A1F7394B720643C6D12294AE9C78206FD35F7083659B27F709FF968D31043093F02BF27BFA634EC2A0B1FBDA704F6311C13384EA6E95CF25E377ACABD746D25D4C7B0AAADCD724368F384A421096DDD3397AC69BD80618FC8857C87F5DDFC1D984744F84ECF6C92541AB1445592563DD1FBE028683E8E5FBFF01357FF0CE07510000, N'6.4.4')
GO
SET IDENTITY_INSERT [dbo].[ApplicationUser] ON 
GO
INSERT [dbo].[ApplicationUser] ([Id], [Name], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [Status], [Rank], [VersionNumber], [BusinessId], [AspNetUsersId], [Email]) VALUES (2, N'Admin_IMS', 1, CAST(N'2023-07-31T04:25:35.6182041' AS DateTime2), NULL, NULL, 1, 1, NULL, NULL, 1, N'admin@ims')
GO
INSERT [dbo].[ApplicationUser] ([Id], [Name], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [Status], [Rank], [VersionNumber], [BusinessId], [AspNetUsersId], [Email]) VALUES (3, N'Manager_IMS', 1, CAST(N'2023-07-31T04:25:35.6182041' AS DateTime2), NULL, NULL, 1, 2, NULL, NULL, 2, N'manager@ims')
GO
INSERT [dbo].[ApplicationUser] ([Id], [Name], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [Status], [Rank], [VersionNumber], [BusinessId], [AspNetUsersId], [Email]) VALUES (6, N'Selle_IMS', 1, CAST(N'2023-07-31T04:25:35.6182041' AS DateTime2), NULL, NULL, 1, 3, NULL, NULL, 3, N'seller@ims')
GO
SET IDENTITY_INSERT [dbo].[ApplicationUser] OFF
GO
SET IDENTITY_INSERT [dbo].[AspNetRoles] ON 
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (2, N'Manager')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (1, N'SA')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (3, N'Seller')
GO
SET IDENTITY_INSERT [dbo].[AspNetRoles] OFF
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (1, 1)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (1, 2)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (1, 3)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (2, 2)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (2, 3)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (3, 3)
GO
SET IDENTITY_INSERT [dbo].[AspNetUsers] ON 
GO
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (1, N'admin@ims', 0, N'AEL0hvBJr+0e1G+rmoCOAdlmq70+9zLB7Nlemn4fEgU77LWJsvpY374pDsJ9h2dnmg==', N'514fdb19-298c-45bf-88af-5ea1729b2ee3', NULL, 0, 0, NULL, 0, 0, N'admin@ims')
GO
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (2, N'manager@ims', 0, N'AABL1niYfKPvSH+0JWhxoU/pVHqHcn/pxNBuxLWJrj3N6lLErCKXCqcZhWZ1gTz0OQ==', N'e35d75b1-2f8a-40e2-83fe-c204a3383ebb', NULL, 0, 0, NULL, 0, 0, N'manager@ims')
GO
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (3, N'seller@ims', 0, N'AGvinw1QwR3da8fsVDp09eeYawzWBltx4CPDeIzEwhKc23nqD2igl/NRPY+DGeUBTA==', N'fad800a3-4038-44f9-871e-9ad4697e08be', NULL, 0, 0, NULL, 0, 0, N'seller@ims')
GO
SET IDENTITY_INSERT [dbo].[AspNetUsers] OFF
GO
SET IDENTITY_INSERT [dbo].[Bank] ON 
GO
INSERT [dbo].[Bank] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [Name], [Description]) VALUES (1, 1, 1, 1, 1, CAST(N'2023-08-16T14:31:05.5895012' AS DateTime2), 0, NULL, NULL, N'Bank A', NULL)
GO
SET IDENTITY_INSERT [dbo].[Bank] OFF
GO
SET IDENTITY_INSERT [dbo].[Brand] ON 
GO
INSERT [dbo].[Brand] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [Name], [Description]) VALUES (1, 1, 1, 1, 1, CAST(N'2023-08-16T14:20:26.9531136' AS DateTime2), 0, NULL, NULL, N'Vivo', NULL)
GO
INSERT [dbo].[Brand] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [Name], [Description]) VALUES (2, 1, 1, 2, 1, CAST(N'2023-08-16T14:32:31.0972384' AS DateTime2), 0, NULL, NULL, N'Samsung', NULL)
GO
SET IDENTITY_INSERT [dbo].[Brand] OFF
GO
SET IDENTITY_INSERT [dbo].[Category] ON 
GO
INSERT [dbo].[Category] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [Name], [Description]) VALUES (1, 1, 1, 1, 1, CAST(N'2023-08-16T14:20:02.3618149' AS DateTime2), 0, NULL, NULL, N'Android Phone', NULL)
GO
SET IDENTITY_INSERT [dbo].[Category] OFF
GO
SET IDENTITY_INSERT [dbo].[Customer] ON 
GO
INSERT [dbo].[Customer] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [Name], [Address], [ContactNumber], [Email]) VALUES (1, 2, 1, 1, 1, CAST(N'2023-08-16T14:24:20.0490738' AS DateTime2), 1, CAST(N'2023-08-16T14:26:09.3483004' AS DateTime2), NULL, N'Customer A', N'Village: X, District: Y', N'+8801722222222', N'customerA@mail.com')
GO
SET IDENTITY_INSERT [dbo].[Customer] OFF
GO
SET IDENTITY_INSERT [dbo].[Payment] ON 
GO
INSERT [dbo].[Payment] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [PurchaseId], [SaleId], [VoucherId], [OperationType], [TotalAmount], [PaidAmount]) VALUES (1, 1, 1, 1, 1, CAST(N'2023-08-16T14:33:58.3551361' AS DateTime2), 0, NULL, NULL, 1, NULL, N'#P116082023', 10, CAST(250000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Payment] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [PurchaseId], [SaleId], [VoucherId], [OperationType], [TotalAmount], [PaidAmount]) VALUES (2, 1, 1, 2, 1, CAST(N'2023-08-16T14:34:33.7189809' AS DateTime2), 0, NULL, NULL, NULL, 1, N'#S116082023', 20, CAST(190400.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
SET IDENTITY_INSERT [dbo].[Payment] OFF
GO
SET IDENTITY_INSERT [dbo].[Product] ON 
GO
INSERT [dbo].[Product] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [Name], [Description], [BuyingPrice], [SellingPrice], [DiscountPrice], [ProfitMargin], [Image], [InStockQuantity], [CategoryId], [BrandId]) VALUES (1, 4, 1, 1, 1, CAST(N'2023-08-16T14:21:28.4977074' AS DateTime2), 0, NULL, NULL, N'Vivo Y12s', NULL, CAST(12000.00000 AS Decimal(19, 5)), CAST(17900.00000 AS Decimal(19, 5)), CAST(100.00000 AS Decimal(19, 5)), CAST(50.00000 AS Decimal(19, 5)), N'203377b7-94eb-4f51-a527-6cfd9be332be.png', 5, 1, 1)
GO
INSERT [dbo].[Product] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [Name], [Description], [BuyingPrice], [SellingPrice], [DiscountPrice], [ProfitMargin], [Image], [InStockQuantity], [CategoryId], [BrandId]) VALUES (2, 3, 1, 2, 1, CAST(N'2023-08-16T14:33:21.0453318' AS DateTime2), 0, NULL, NULL, N'Samsung M20', NULL, CAST(13000.00000 AS Decimal(19, 5)), CAST(20180.00000 AS Decimal(19, 5)), CAST(100.00000 AS Decimal(19, 5)), CAST(56.00000 AS Decimal(19, 5)), N'd0669ee0-5910-45f6-939c-e578943dac4f.png', 5, 1, 2)
GO
SET IDENTITY_INSERT [dbo].[Product] OFF
GO
SET IDENTITY_INSERT [dbo].[Purchase] ON 
GO
INSERT [dbo].[Purchase] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [VoucherId], [SupplierId], [PurchaseDate], [GrandTotalPrice], [IsPaid], [PaymentId]) VALUES (1, 1, 1, 1, 1, CAST(N'2023-08-16T14:33:58.3531360' AS DateTime2), 0, NULL, NULL, N'#P116082023', 1, CAST(N'2023-08-16T14:33:58.3531360' AS DateTime2), CAST(250000.00 AS Decimal(18, 2)), 0, 1)
GO
SET IDENTITY_INSERT [dbo].[Purchase] OFF
GO
SET IDENTITY_INSERT [dbo].[PurchaseDetails] ON 
GO
INSERT [dbo].[PurchaseDetails] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [ProductId], [Quantity], [TotalPrice], [PurchaseId]) VALUES (1, 1, 1, 0, 1, CAST(N'2023-08-16T14:33:58.3511357' AS DateTime2), 0, NULL, NULL, 1, 10, CAST(120000.00 AS Decimal(18, 2)), 1)
GO
INSERT [dbo].[PurchaseDetails] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [ProductId], [Quantity], [TotalPrice], [PurchaseId]) VALUES (2, 1, 1, 0, 1, CAST(N'2023-08-16T14:33:58.3531360' AS DateTime2), 0, NULL, NULL, 2, 10, CAST(130000.00 AS Decimal(18, 2)), 1)
GO
SET IDENTITY_INSERT [dbo].[PurchaseDetails] OFF
GO
SET IDENTITY_INSERT [dbo].[Sale] ON 
GO
INSERT [dbo].[Sale] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [VoucherId], [CustomerId], [SaleDate], [GrandTotalPrice], [IsPaid], [PaymentId]) VALUES (1, 1, 1, 1, 1, CAST(N'2023-08-16T14:34:33.7169792' AS DateTime2), 0, NULL, NULL, N'#S116082023', 1, CAST(N'2023-08-16T14:34:33.7169792' AS DateTime2), CAST(190400.00 AS Decimal(18, 2)), 0, 2)
GO
SET IDENTITY_INSERT [dbo].[Sale] OFF
GO
SET IDENTITY_INSERT [dbo].[SaleDetails] ON 
GO
INSERT [dbo].[SaleDetails] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [ProductId], [Quantity], [TotalPrice], [SaleId]) VALUES (1, 1, 1, 1, 1, CAST(N'2023-08-16T14:34:33.7139815' AS DateTime2), 0, NULL, NULL, 1, 5, CAST(89500.00 AS Decimal(18, 2)), 1)
GO
INSERT [dbo].[SaleDetails] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [ProductId], [Quantity], [TotalPrice], [SaleId]) VALUES (2, 1, 1, 1, 1, CAST(N'2023-08-16T14:34:33.7169792' AS DateTime2), 0, NULL, NULL, 2, 5, CAST(100900.00 AS Decimal(18, 2)), 1)
GO
SET IDENTITY_INSERT [dbo].[SaleDetails] OFF
GO
SET IDENTITY_INSERT [dbo].[Supplier] ON 
GO
INSERT [dbo].[Supplier] ([Id], [VersionNumber], [Status], [Rank], [CreateBy], [CreationDate], [ModifyBy], [ModificationDate], [BusinessId], [Name], [Address], [ContactNumber], [Email]) VALUES (1, 2, 1, 1, 1, CAST(N'2023-08-16T14:25:29.5976284' AS DateTime2), 1, CAST(N'2023-08-16T14:26:26.2331404' AS DateTime2), NULL, N'Supplier A', N'Village: X, District: Y', N'+8801755555555', N'supplierB@mail.com')
GO
SET IDENTITY_INSERT [dbo].[Supplier] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 16-Aug-23 2:37:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserId]    Script Date: 16-Aug-23 2:37:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserId]    Script Date: 16-Aug-23 2:37:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RoleId]    Script Date: 16-Aug-23 2:37:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserId]    Script Date: 16-Aug-23 2:37:28 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 16-Aug-23 2:37:28 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[PaymentDetails]  WITH CHECK ADD  CONSTRAINT [FK_1FE4255C] FOREIGN KEY([BankId])
REFERENCES [dbo].[Bank] ([Id])
GO
ALTER TABLE [dbo].[PaymentDetails] CHECK CONSTRAINT [FK_1FE4255C]
GO
ALTER TABLE [dbo].[PaymentDetails]  WITH CHECK ADD  CONSTRAINT [FK_773CA96C] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payment] ([Id])
GO
ALTER TABLE [dbo].[PaymentDetails] CHECK CONSTRAINT [FK_773CA96C]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_B397A332] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_B397A332]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_DE3A2F2B] FOREIGN KEY([BrandId])
REFERENCES [dbo].[Brand] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_DE3A2F2B]
GO
ALTER TABLE [dbo].[PurchaseDetails]  WITH CHECK ADD  CONSTRAINT [FK_7B82BA95] FOREIGN KEY([PurchaseId])
REFERENCES [dbo].[Purchase] ([Id])
GO
ALTER TABLE [dbo].[PurchaseDetails] CHECK CONSTRAINT [FK_7B82BA95]
GO
ALTER TABLE [dbo].[SaleDetails]  WITH CHECK ADD  CONSTRAINT [FK_ABE20313] FOREIGN KEY([SaleId])
REFERENCES [dbo].[Sale] ([Id])
GO
ALTER TABLE [dbo].[SaleDetails] CHECK CONSTRAINT [FK_ABE20313]
GO
USE [master]
GO
ALTER DATABASE [IMS] SET  READ_WRITE 
GO
