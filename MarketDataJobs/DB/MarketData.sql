USE [master]
GO
/****** Object:  Database [MarketData]    Script Date: 2018/4/8 0:46:41 ******/
CREATE DATABASE [MarketData]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MarketData', FILENAME = N'D:\Microsoft SQL Server\MSSQL13.SKYWOLFDB\MSSQL\DATA\MarketData.mdf' , SIZE = 16064512KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'MarketData_log', FILENAME = N'D:\Microsoft SQL Server\MSSQL13.SKYWOLFDB\MSSQL\DATA\MarketData_log.ldf' , SIZE = 69568KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [MarketData] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MarketData].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MarketData] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MarketData] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MarketData] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MarketData] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MarketData] SET ARITHABORT OFF 
GO
ALTER DATABASE [MarketData] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MarketData] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MarketData] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MarketData] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MarketData] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MarketData] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MarketData] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MarketData] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MarketData] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MarketData] SET  DISABLE_BROKER 
GO
ALTER DATABASE [MarketData] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MarketData] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MarketData] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MarketData] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MarketData] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MarketData] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MarketData] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MarketData] SET RECOVERY FULL 
GO
ALTER DATABASE [MarketData] SET  MULTI_USER 
GO
ALTER DATABASE [MarketData] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MarketData] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MarketData] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MarketData] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [MarketData] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'MarketData', N'ON'
GO
ALTER DATABASE [MarketData] SET QUERY_STORE = OFF
GO
USE [MarketData]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [MarketData]
GO
/****** Object:  User [liangshu]    Script Date: 2018/4/8 0:46:41 ******/
CREATE USER [liangshu] FOR LOGIN [liangshu] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [liangshu]
GO
ALTER ROLE [db_accessadmin] ADD MEMBER [liangshu]
GO
ALTER ROLE [db_securityadmin] ADD MEMBER [liangshu]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [liangshu]
GO
ALTER ROLE [db_backupoperator] ADD MEMBER [liangshu]
GO
ALTER ROLE [db_datareader] ADD MEMBER [liangshu]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [liangshu]
GO
ALTER ROLE [db_denydatareader] ADD MEMBER [liangshu]
GO
ALTER ROLE [db_denydatawriter] ADD MEMBER [liangshu]
GO
/****** Object:  Schema [av]    Script Date: 2018/4/8 0:46:41 ******/
CREATE SCHEMA [av]
GO
/****** Object:  Schema [fdd]    Script Date: 2018/4/8 0:46:41 ******/
CREATE SCHEMA [fdd]
GO
/****** Object:  Schema [price]    Script Date: 2018/4/8 0:46:41 ******/
CREATE SCHEMA [price]
GO
/****** Object:  Schema [sec]    Script Date: 2018/4/8 0:46:41 ******/
CREATE SCHEMA [sec]
GO
/****** Object:  Table [sec].[Stock]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [sec].[Stock](
	[SID] [bigint] IDENTITY(200000001,1) NOT NULL,
	[Name] [varchar](200) NULL,
	[FullName] [varchar](200) NULL,
	[SubType] [varchar](50) NULL,
	[Country] [varchar](200) NULL,
	[Currency] [varchar](10) NULL,
	[IssueDate] [date] NULL,
	[Usr] [varchar](200) NULL,
	[TS] [datetime] NULL,
	[AssetClass] [varchar](200) NULL,
	[Sector] [varchar](200) NULL,
	[Industry] [varchar](200) NULL,
 CONSTRAINT [PK_Stock] PRIMARY KEY CLUSTERED 
(
	[SID] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [sec].[vw_InstrumentName]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Script for SelectTopNRows command from SSMS  ******/



create view [sec].[vw_InstrumentName]
as
SELECT [SID]
      ,[Name]
  FROM [MarketData].[sec].[Stock]
GO
/****** Object:  Table [price].[AdjPrices]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [price].[AdjPrices](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [date] NOT NULL,
	[PriceTypeId] [int] NOT NULL,
	[Value] [decimal](18, 7) NULL,
	[Usr] [varchar](200) NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_AdjPrice] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC,
	[PriceTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [price].[vw_AdjPrices]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Script for SelectTopNRows tcommand from SSMS  ******/

CREATE view [price].[vw_AdjPrices]
as
select t.[SID], n.[Name], t.[AsOfDate], t.[Open],  t.[High], t.[Low], t.[Close], t.[Volume]
from (SELECT [SID], [AsOfDate], [1] as [Close], [2] as [Open], [3] as [High], [4] as [Low], [6] as [Volume]
  FROM (select [SID],[AsOfDate],[PriceTypeId],[Value] from [MarketData].[price].[AdjPrices] where PriceTypeId in (1,2,3,4,6)) as x
  pivot (sum([Value]) for PriceTypeId in ([1],[2],[3],[4],[6])) as pvt) as t
left join sec.vw_InstrumentName n on t.[SID] = n.[SID]

GO
/****** Object:  Table [price].[Prices]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [price].[Prices](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [date] NOT NULL,
	[PriceTypeId] [int] NOT NULL,
	[Value] [decimal](18, 7) NULL,
	[Usr] [varchar](200) NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_Price] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC,
	[PriceTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [price].[vw_Prices]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Script for SelectTopNRows tcommand from SSMS  ******/

CREATE view [price].[vw_Prices]
as
select t.[SID], n.[Name], t.[AsOfDate], t.[Open],  t.[High], t.[Low], t.[Close], t.[Volume]
from (SELECT [SID], [AsOfDate], [1] as [Close], [2] as [Open], [3] as [High], [4] as [Low], [6] as [Volume]
  FROM (select [SID],[AsOfDate],[PriceTypeId],[Value] from [MarketData].[price].[Prices] where PriceTypeId in (1,2,3,4,6)) as x
  pivot (sum([Value]) for PriceTypeId in ([1],[2],[3],[4],[6])) as pvt) as t
left join sec.vw_InstrumentName n on t.[SID] = n.[SID]

GO
/****** Object:  View [price].[vw_latestClosePrices]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [price].[vw_latestClosePrices] as
  select t.[SID], i.[Name], t.[AsOfDate], t.[Value] as [Close] from

  (select [SID]
                  ,[AsOfDate]
                  ,[Value] from
            (SELECT [SID]
                  ,[AsOfDate]
                  ,[Value]
	              ,ROW_NUMBER() over(PARTITION BY [SID] ORDER BY AsOfDate DESC) Num
              FROM [MarketData].[price].[Prices]
              where PriceTypeId = 1) as  t
              where Num = 1) as t
  left join sec.vw_InstrumentName i on t.SID = i.SID
GO
/****** Object:  View [price].[vw_latestCloseAdjPrices]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [price].[vw_latestCloseAdjPrices] as
  select t.[SID], i.[Name], t.[AsOfDate], t.[Value] as [Close] from

  (select [SID]
                  ,[AsOfDate]
                  ,[Value] from
            (SELECT [SID]
                  ,[AsOfDate]
                  ,[Value]
	              ,ROW_NUMBER() over(PARTITION BY [SID] ORDER BY AsOfDate DESC) Num
              FROM [MarketData].[price].[AdjPrices]
              where PriceTypeId = 1) as  t
              where Num = 1) as t
  left join sec.vw_InstrumentName i on t.SID = i.SID
GO
/****** Object:  View [price].[vw_VolatilityETFHistoricalCloseAdjPrices]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Script for SelectTopNRows command from SSMS  ******/
create view [price].[vw_VolatilityETFHistoricalCloseAdjPrices] as
select * from
(SELECT [Name]
	  ,[AsOfDate]
      ,[Close]
  FROM [MarketData].[price].[vw_AdjPrices]
  where [Name] in ('XIV','USO','DNO','XOP','XLE','SCO','UCO','VXX','SSO','UBT','BLV','IWN','SPY')) as t
pivot (sum([Close]) for [Name] in ([XIV],[USO],[DNO],[XOP],[XLE],[SCO],[UCO],[VXX],[SSO],[UBT],[BLV],[IWN],[SPY])) as pvt

GO
/****** Object:  Table [dbo].[DataSource]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataSource](
	[SourceID] [bigint] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[UrlRoot] [varchar](max) NULL,
	[Comments] [varchar](max) NULL,
	[Usr] [varchar](200) NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_DataSource] PRIMARY KEY CLUSTERED 
(
	[SourceID] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [fdd].[FundamentalDataType]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [fdd].[FundamentalDataType](
	[DataTypeID] [int] NOT NULL,
	[Description] [varchar](200) NOT NULL,
	[Comments] [varchar](max) NULL,
	[Usr] [varchar](200) NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_FundamentalDataType] PRIMARY KEY CLUSTERED 
(
	[DataTypeID] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [fdd].[FundamentalData]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [fdd].[FundamentalData](
	[SID] [bigint] NOT NULL,
	[DataSourceID] [int] NOT NULL,
	[DataTypeID] [int] NOT NULL,
	[FREQ] [nchar](2) NOT NULL,
	[SEQ] [int] NOT NULL,
	[PeriodEndDate] [date] NOT NULL,
	[ReportDate] [date] NOT NULL,
	[Value] [decimal](18, 0) NOT NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_FundamentalData] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[DataSourceID] ASC,
	[DataTypeID] ASC,
	[FREQ] ASC,
	[SEQ] ASC,
	[PeriodEndDate] ASC,
	[ReportDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [fdd].[vw_FundamentalData]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Script for SelectTopNRows command from SSMS  ******/

create view [fdd].[vw_FundamentalData] as
SELECT d.[SID]
	  ,e.[Name]
      ,d.[DataSourceID]
	  ,s.[Name] as DataSource
      ,d.[DataTypeID]
	  ,ft.[Description] as DataType
      ,d.[FREQ]
      ,d.[SEQ]
      ,d.[PeriodEndDate]
      ,d.[ReportDate]
      ,d.[Value]
      ,d.[TS]
  FROM [MarketData].[fdd].[FundamentalData] d
  left join [MarketData].[dbo].[DataSource] s on d.DataSourceID = s.SourceID
  left join [MarketData].[fdd].[FundamentalDataType] ft on d.DataTypeID = ft.DataTypeID
  left join [MarketData].[sec].[Stock] e on d.[SID] = e.[SID]
GO
/****** Object:  View [price].[vw_AdjPriceReturn20Day]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Script for SelectTopNRows command from SSMS  ******/

CREATE view [price].[vw_AdjPriceReturn20Day] as
with adjprices as (SELECT [SID]
      ,[AsOfDate]
      ,[Value]
	  ,ROW_NUMBER() over(PARTITION BY [SID] ORDER BY AsOfDate DESC) Num
  FROM [MarketData].[price].[AdjPrices]
  where PriceTypeId = 1)
  
  select p1.SID, i.Name, p1.AsOfDate, (p1.[Value] / p2.[Value] - 1) as Ret20D from
  adjprices p1
  left join adjprices p2 on p1.SID = p2.SID and (p1.Num + 20) = p2.Num
  left join [MarketData].[sec].[vw_InstrumentName] i on p1.SID = i.SID
  where p1.[Value] is not null and p2.[Value] is not null
 
   
GO
/****** Object:  View [price].[vw_LargeSmallStopLossIndicator]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Script for SelectTopNRows command from SSMS  ******/

create view [price].[vw_LargeSmallStopLossIndicator] as 
select big.AsOfDate, big.SID as LargeSID, big.[Name] as LargeName, big.Ret20D as Large20DayReturn, 
                     small.SID as SmallSID, small.[Name] as SmallName, small.Ret20D as Small20DayReturn from
(SELECT * FROM [MarketData].[price].[vw_AdjPriceReturn20Day] where SID = 200001109) as big
left join (select * from [MarketData].[price].[vw_AdjPriceReturn20Day] where SID = 200000914) as small on big.AsOfDate = small.AsOfDate


GO
/****** Object:  Table [sec].[InstrumentID]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [sec].[InstrumentID](
	[SID] [bigint] NOT NULL,
	[DataSource] [varchar](200) NOT NULL,
	[IDTypeId] [int] NOT NULL,
	[IDValue] [varchar](250) NOT NULL,
	[Usr] [varchar](200) NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_InstrumentID] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[DataSource] DESC,
	[IDTypeId] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [sec].[InstrumentIDType]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [sec].[InstrumentIDType](
	[IDTypeId] [int] NOT NULL,
	[IDType] [varchar](250) NOT NULL,
	[Comments] [varchar](500) NULL,
	[Usr] [varchar](200) NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_InstrumentIDType] PRIMARY KEY CLUSTERED 
(
	[IDTypeId] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [sec].[vw_InstrumentID]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Script for SelectTopNRows command from SSMS  ******/

create view [sec].[vw_InstrumentID]
as
SELECT id.[SID]
      ,id.[DataSource]
      ,id.[IDTypeId]
	  ,t.IDType
      ,id.[IDValue]
      ,id.[Usr]
      ,id.[TS]
  FROM [MarketData].[sec].[InstrumentID] id
  LEFT JOIN [MarketData].[sec].[InstrumentIDType] t on id.IDTypeId = t.IDTypeId
GO
/****** Object:  Table [price].[PricingRule]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [price].[PricingRule](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [date] NOT NULL,
	[DataSource] [varchar](200) NOT NULL,
	[TimeZone] [varchar](200) NULL,
	[Active] [bit] NOT NULL,
	[Usr] [varchar](200) NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_PriceRule1] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC,
	[DataSource] DESC,
	[Active] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [price].[vw_PricingRule]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Script for SelectTopNRows command from SSMS  ******/

create view [price].[vw_PricingRule] as
SELECT r.[SID]
      ,r.[AsOfDate]
      ,r.[DataSource]
	  ,id.IDValue as Ticker
      ,r.[TimeZone]
      ,r.[Active]
      ,r.[Usr]
      ,r.[TS]
  FROM [MarketData].[price].[PricingRule] r
  left join (select * from [MarketData].[sec].[InstrumentID] where IDTypeId = 1) id on r.DataSource = id.DataSource and r.SID = id.SID
GO
/****** Object:  Table [av].[AdjPrices_D1]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [av].[AdjPrices_D1](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
	[Open] [float] NULL,
	[High] [float] NULL,
	[Low] [float] NULL,
	[Close] [float] NULL,
	[Volume] [decimal](18, 7) NULL,
	[AdjClose] [float] NULL,
	[DividendAmount] [float] NULL,
	[SplitCoefficient] [float] NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_AV_AdjPrice_D1] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [av].[AdjPrices_MN]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [av].[AdjPrices_MN](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
	[Open] [float] NULL,
	[High] [float] NULL,
	[Low] [float] NULL,
	[Close] [float] NULL,
	[Volume] [decimal](18, 7) NULL,
	[AdjClose] [float] NULL,
	[DividendAmount] [float] NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_AV_AdjPrice_MN] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [av].[AdjPrices_W1]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [av].[AdjPrices_W1](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
	[Open] [float] NULL,
	[High] [float] NULL,
	[Low] [float] NULL,
	[Close] [float] NULL,
	[Volume] [decimal](18, 7) NULL,
	[AdjClose] [float] NULL,
	[DividendAmount] [float] NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_AV_AdjPrice_W1] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [av].[APIKeyList]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [av].[APIKeyList](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[EnterTime] [datetime] NOT NULL,
	[ExpireTime] [datetime] NULL,
	[Key] [varchar](200) NOT NULL,
	[Active] [bit] NOT NULL,
	[Usr] [varchar](200) NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_APIKeyList] PRIMARY KEY CLUSTERED 
(
	[ID] DESC,
	[Key] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [av].[Prices_D1]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [av].[Prices_D1](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
	[Open] [float] NULL,
	[High] [float] NULL,
	[Low] [float] NULL,
	[Close] [float] NULL,
	[Volume] [decimal](18, 7) NULL,
	[TS] [datetime] NULL,
	[MarketCap] [decimal](18, 7) NULL,
 CONSTRAINT [PK_AV_Price_D1] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [av].[Prices_H1]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [av].[Prices_H1](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
	[Open] [float] NULL,
	[High] [float] NULL,
	[Low] [float] NULL,
	[Close] [float] NULL,
	[Volume] [decimal](18, 7) NULL,
	[TS] [datetime] NULL,
	[MarketCap] [decimal](18, 7) NULL,
 CONSTRAINT [PK_AV_Price_H1] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [av].[Prices_M1]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [av].[Prices_M1](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
	[Open] [float] NULL,
	[High] [float] NULL,
	[Low] [float] NULL,
	[Close] [float] NULL,
	[Volume] [decimal](18, 7) NULL,
	[TS] [datetime] NULL,
	[MarketCap] [decimal](18, 7) NULL,
 CONSTRAINT [PK_AV_Price_M1] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [av].[Prices_M15]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [av].[Prices_M15](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
	[Open] [float] NULL,
	[High] [float] NULL,
	[Low] [float] NULL,
	[Close] [float] NULL,
	[Volume] [decimal](18, 7) NULL,
	[TS] [datetime] NULL,
	[MarketCap] [decimal](18, 7) NULL,
 CONSTRAINT [PK_AV_Price_M15] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [av].[Prices_M30]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [av].[Prices_M30](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
	[Open] [float] NULL,
	[High] [float] NULL,
	[Low] [float] NULL,
	[Close] [float] NULL,
	[Volume] [decimal](18, 7) NULL,
	[TS] [datetime] NULL,
	[MarketCap] [decimal](18, 7) NULL,
 CONSTRAINT [PK_AV_Price_M30] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [av].[Prices_M5]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [av].[Prices_M5](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
	[Open] [float] NULL,
	[High] [float] NULL,
	[Low] [float] NULL,
	[Close] [float] NULL,
	[Volume] [decimal](18, 7) NULL,
	[TS] [datetime] NULL,
	[MarketCap] [decimal](18, 7) NULL,
 CONSTRAINT [PK_AV_Price_M5] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [av].[Prices_MN]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [av].[Prices_MN](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
	[Open] [float] NULL,
	[High] [float] NULL,
	[Low] [float] NULL,
	[Close] [float] NULL,
	[Volume] [decimal](18, 7) NULL,
	[TS] [datetime] NULL,
	[MarketCap] [decimal](18, 7) NULL,
 CONSTRAINT [PK_AV_Price_MN] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [av].[Prices_W1]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [av].[Prices_W1](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
	[Open] [float] NULL,
	[High] [float] NULL,
	[Low] [float] NULL,
	[Close] [float] NULL,
	[Volume] [decimal](18, 7) NULL,
	[TS] [datetime] NULL,
	[MarketCap] [decimal](18, 7) NULL,
 CONSTRAINT [PK_AV_Price_W1] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Frequency]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Frequency](
	[ID] [int] NOT NULL,
	[CharID] [varchar](10) NOT NULL,
	[Value] [varchar](50) NOT NULL,
	[Usr] [varchar](200) NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_Frequency] PRIMARY KEY CLUSTERED 
(
	[ID] DESC,
	[CharID] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [price].[PriceType]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [price].[PriceType](
	[PriceTypeId] [int] NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Usr] [varchar](200) NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_PriceType] PRIMARY KEY CLUSTERED 
(
	[PriceTypeId] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [sec].[Stock_Dividend]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [sec].[Stock_Dividend](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [date] NOT NULL,
	[Amount] [float] NOT NULL,
	[Usr] [varchar](200) NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_Stock_Dividend] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [sec].[Stock_Split]    Script Date: 2018/4/8 0:46:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [sec].[Stock_Split](
	[SID] [bigint] NOT NULL,
	[AsOfDate] [date] NOT NULL,
	[Numerator] [float] NOT NULL,
	[Denominator] [float] NOT NULL,
	[SplitRatio] [varchar](200) NULL,
	[Usr] [varchar](200) NULL,
	[TS] [datetime] NULL,
 CONSTRAINT [PK_Stock_Split] PRIMARY KEY CLUSTERED 
(
	[SID] DESC,
	[AsOfDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [av].[AdjPrices_D1] ADD  CONSTRAINT [DF_AdjPrices_D1_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [av].[AdjPrices_MN] ADD  CONSTRAINT [DF_AdjPrices_MN_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [av].[AdjPrices_W1] ADD  CONSTRAINT [DF_AdjPrices_W1_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [av].[APIKeyList] ADD  CONSTRAINT [DF_APIKeyList_Usr]  DEFAULT (suser_name()) FOR [Usr]
GO
ALTER TABLE [av].[APIKeyList] ADD  CONSTRAINT [DF_APIKeyList_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [av].[Prices_D1] ADD  CONSTRAINT [DF_Prices_D1_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [av].[Prices_H1] ADD  CONSTRAINT [DF_Prices_H1_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [av].[Prices_M1] ADD  CONSTRAINT [DF_Prices_M1_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [av].[Prices_M15] ADD  CONSTRAINT [DF_Prices_M15_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [av].[Prices_M30] ADD  CONSTRAINT [DF_Prices_M30_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [av].[Prices_M5] ADD  CONSTRAINT [DF_Prices_M5_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [av].[Prices_MN] ADD  CONSTRAINT [DF_Prices_MN_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [av].[Prices_W1] ADD  CONSTRAINT [DF_Prices_W1_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [dbo].[DataSource] ADD  CONSTRAINT [DF_DataSource_Usr]  DEFAULT (suser_name()) FOR [Usr]
GO
ALTER TABLE [dbo].[DataSource] ADD  CONSTRAINT [DF_DataSource_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [dbo].[Frequency] ADD  CONSTRAINT [DF_Frequency_Usr]  DEFAULT (suser_name()) FOR [Usr]
GO
ALTER TABLE [dbo].[Frequency] ADD  CONSTRAINT [DF_Frequency_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [fdd].[FundamentalData] ADD  CONSTRAINT [DF_FundamentalData_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [fdd].[FundamentalDataType] ADD  CONSTRAINT [DF_FundamentalDataType_Usr]  DEFAULT (suser_name()) FOR [Usr]
GO
ALTER TABLE [fdd].[FundamentalDataType] ADD  CONSTRAINT [DF_FundamentalDataType_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [price].[AdjPrices] ADD  CONSTRAINT [DF_AdjPrices_Usr]  DEFAULT (suser_name()) FOR [Usr]
GO
ALTER TABLE [price].[AdjPrices] ADD  CONSTRAINT [DF_AdjPrices_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [price].[Prices] ADD  CONSTRAINT [DF_Prices_Usr]  DEFAULT (suser_name()) FOR [Usr]
GO
ALTER TABLE [price].[Prices] ADD  CONSTRAINT [DF_Prices_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [price].[PriceType] ADD  CONSTRAINT [DF_PriceType_Usr]  DEFAULT (right(suser_name(),charindex('\',reverse(suser_name()),(1))-(1))) FOR [Usr]
GO
ALTER TABLE [price].[PriceType] ADD  CONSTRAINT [DF_PriceType_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [price].[PricingRule] ADD  CONSTRAINT [DF_PricingRule1_Usr]  DEFAULT (suser_name()) FOR [Usr]
GO
ALTER TABLE [price].[PricingRule] ADD  CONSTRAINT [DF_PricingRule1_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [sec].[InstrumentID] ADD  CONSTRAINT [DF_InstrumentID_Usr]  DEFAULT (suser_name()) FOR [Usr]
GO
ALTER TABLE [sec].[InstrumentID] ADD  CONSTRAINT [DF_InstrumentID_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [sec].[InstrumentIDType] ADD  CONSTRAINT [DF_InstrumentIDType_Usr]  DEFAULT (suser_name()) FOR [Usr]
GO
ALTER TABLE [sec].[InstrumentIDType] ADD  CONSTRAINT [DF_InstrumentIDType_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [sec].[Stock] ADD  CONSTRAINT [DF_Stock_Usr]  DEFAULT (suser_name()) FOR [Usr]
GO
ALTER TABLE [sec].[Stock] ADD  CONSTRAINT [DF_Stock_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [sec].[Stock_Dividend] ADD  CONSTRAINT [DF_Stock_Dividend_Usr]  DEFAULT (suser_name()) FOR [Usr]
GO
ALTER TABLE [sec].[Stock_Dividend] ADD  CONSTRAINT [DF_Stock_Dividend_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
ALTER TABLE [sec].[Stock_Split] ADD  CONSTRAINT [DF_Stock_Split_Usr]  DEFAULT (suser_name()) FOR [Usr]
GO
ALTER TABLE [sec].[Stock_Split] ADD  CONSTRAINT [DF_Stock_Split_TS]  DEFAULT (getutcdate()) FOR [TS]
GO
USE [master]
GO
ALTER DATABASE [MarketData] SET  READ_WRITE 
GO
