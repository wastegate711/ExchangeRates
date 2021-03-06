USE [master]
GO
/****** Object:  Database [ExchangeRates]    Script Date: 04.04.2021 14:59:09 ******/
CREATE DATABASE [ExchangeRates]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ExchangeRates', FILENAME = N'D:\DataBase\ExchangeRates.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'ExchangeRates_log', FILENAME = N'D:\DataBase\ExchangeRates_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [ExchangeRates] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ExchangeRates].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ExchangeRates] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ExchangeRates] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ExchangeRates] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ExchangeRates] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ExchangeRates] SET ARITHABORT OFF 
GO
ALTER DATABASE [ExchangeRates] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ExchangeRates] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ExchangeRates] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ExchangeRates] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ExchangeRates] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ExchangeRates] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ExchangeRates] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ExchangeRates] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ExchangeRates] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ExchangeRates] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ExchangeRates] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ExchangeRates] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ExchangeRates] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ExchangeRates] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ExchangeRates] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ExchangeRates] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ExchangeRates] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ExchangeRates] SET RECOVERY FULL 
GO
ALTER DATABASE [ExchangeRates] SET  MULTI_USER 
GO
ALTER DATABASE [ExchangeRates] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ExchangeRates] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ExchangeRates] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ExchangeRates] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [ExchangeRates] SET DELAYED_DURABILITY = DISABLED 
GO
USE [ExchangeRates]
GO
/****** Object:  UserDefinedFunction [dbo].[GetRate]    Script Date: 04.04.2021 14:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetRate]
(
	@id_char varchar(50),
	@date date
)
RETURNS @returntable TABLE
(
	Rate numeric(10,4)
)
AS
BEGIN
	INSERT @returntable
	SELECT Rate
	FROM DailyRate left outer join Currency on (DailyRate.IdCurrency=Currency.Id)
	WHERE Currency.Id=@id_char
    AND DailyRate.Dt=@date;
	RETURN
END
GO
/****** Object:  Table [dbo].[Currency]    Script Date: 04.04.2021 14:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Currency](
	[Id] [varchar](50) NOT NULL,
	[NumCode] [varchar](50) NOT NULL,
	[CharCode] [varchar](50) NOT NULL,
	[Nominal] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DailyRate]    Script Date: 04.04.2021 14:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DailyRate](
	[IdCurrency] [varchar](50) NOT NULL,
	[Dt] [date] NOT NULL,
	[Rate] [numeric](10, 4) NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DailyRate]  WITH CHECK ADD  CONSTRAINT [FK_DailyRate_Currency] FOREIGN KEY([IdCurrency])
REFERENCES [dbo].[Currency] ([Id])
GO
ALTER TABLE [dbo].[DailyRate] CHECK CONSTRAINT [FK_DailyRate_Currency]
GO
USE [master]
GO
ALTER DATABASE [ExchangeRates] SET  READ_WRITE 
GO
