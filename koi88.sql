USE [master]
GO
/****** Object:  Database [Koi88]    Script Date: 9/29/2024 8:48:41 PM ******/
CREATE DATABASE [Koi88]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Koi88', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\Koi88.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Koi88_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\Koi88_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [Koi88] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Koi88].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Koi88] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Koi88] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Koi88] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Koi88] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Koi88] SET ARITHABORT OFF 
GO
ALTER DATABASE [Koi88] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [Koi88] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Koi88] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Koi88] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Koi88] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Koi88] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Koi88] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Koi88] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Koi88] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Koi88] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Koi88] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Koi88] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Koi88] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Koi88] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Koi88] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Koi88] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Koi88] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Koi88] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Koi88] SET  MULTI_USER 
GO
ALTER DATABASE [Koi88] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Koi88] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Koi88] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Koi88] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Koi88] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Koi88] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [Koi88] SET QUERY_STORE = ON
GO
ALTER DATABASE [Koi88] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [Koi88]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[account_id] [int] IDENTITY(1,1) NOT NULL,
	[role_id] [int] NULL,
	[username] [nvarchar](50) NULL,
	[password] [nvarchar](100) NULL,
	[lastname] [nvarchar](100) NULL,
	[firstname] [nvarchar](100) NULL,
	[gender] [nvarchar](50) NULL,
	[phone] [nvarchar](100) NULL,
	[email] [nvarchar](100) NULL,
	[imageUrl] [nvarchar](100) NULL,
	[status] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[account_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Booking]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Booking](
	[booking_id] [int] IDENTITY(1,1) NOT NULL,
	[trip_id] [int] NULL,
	[po_id] [int] NULL,
	[booking_payment_id] [int] NULL,
	[form_booking_id] [int] NULL,
	[feedback_id] [int] NULL,
	[quoted_amount] [decimal](10, 2) NULL,
	[quote_sent_date] [date] NULL,
	[quote_approved_date] [date] NULL,
	[manager_approval] [bit] NULL,
	[status] [nvarchar](200) NULL,
	[start_date] [date] NULL,
	[end_date] [date] NULL,
	[koi_delivery_date] [date] NULL,
	[koi_delivery_time] [time](7) NULL,
	[total_amount] [decimal](10, 2) NULL,
	[bookingDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[booking_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingPayment]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingPayment](
	[booking_payment_id] [int] IDENTITY(1,1) NOT NULL,
	[status] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[booking_payment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[customer_id] [int] IDENTITY(1,1) NOT NULL,
	[account_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[customer_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[feedback_id] [int] IDENTITY(1,1) NOT NULL,
	[customer_id] [int] NULL,
	[rating] [int] NULL,
	[comments] [nvarchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[feedback_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormBooking]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormBooking](
	[form_booking_id] [int] IDENTITY(1,1) NOT NULL,
	[customer_id] [int] NULL,
	[fullname] [nvarchar](100) NULL,
	[phone] [nvarchar](100) NULL,
	[email] [nvarchar](100) NULL,
	[favoritefarm] [nvarchar](100) NULL,
	[estimatedCost] [decimal](10, 2) NULL,
	[favoriteKoi] [nvarchar](200) NULL,
	[hotel_accommodation] [nvarchar](100) NULL,
	[estimatedDepartureDate] [date] NULL,
	[returnDate] [date] NULL,
	[gender] [nvarchar](50) NULL,
	[note] [nvarchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[form_booking_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KoiFarm]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KoiFarm](
	[farm_id] [int] IDENTITY(1,1) NOT NULL,
	[trip_detail_id] [int] NULL,
	[koi_id] [int] NULL,
	[farm_name] [nvarchar](100) NULL,
	[location] [nvarchar](200) NULL,
	[contact_info] [nvarchar](200) NULL,
	[imageUrl] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[farm_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KoiFish]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KoiFish](
	[koi_id] [int] IDENTITY(1,1) NOT NULL,
	[variety_id] [int] NULL,
	[type] [nvarchar](50) NULL,
	[price] [decimal](10, 2) NULL,
	[size] [nvarchar](50) NULL,
	[imageUrl] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[koi_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KoiPackage]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KoiPackage](
	[package_id] [int] IDENTITY(1,1) NOT NULL,
	[farm_id] [int] NULL,
	[po_detail_id] [int] NULL,
	[package_name] [nvarchar](100) NULL,
	[description] [nvarchar](200) NULL,
	[price] [decimal](10, 2) NULL,
	[imageUrl] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[package_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentMethod]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentMethod](
	[payment_method_id] [int] IDENTITY(1,1) NOT NULL,
	[booking_payment_id] [int] NULL,
	[po_payment_id] [int] NULL,
	[method_name] [nvarchar](50) NULL,
	[description] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[payment_method_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PO]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PO](
	[po_id] [int] IDENTITY(1,1) NOT NULL,
	[farm_id] [int] NULL,
	[total_amount] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[po_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PODetail]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PODetail](
	[po_detail_id] [int] IDENTITY(1,1) NOT NULL,
	[po_id] [int] NULL,
	[koi_id] [int] NULL,
	[farm_id] [int] NULL,
	[deposit] [decimal](10, 2) NULL,
	[total_koi_price] [decimal](10, 2) NULL,
	[quantity] [int] NULL,
	[imageUrl] [nvarchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[po_detail_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[POPayment]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[POPayment](
	[po_payment_id] [int] IDENTITY(1,1) NOT NULL,
	[po_id] [int] NULL,
	[amount] [decimal](10, 2) NULL,
	[payment_date] [date] NULL,
	[status] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[po_payment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[role_id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SpecialVariety]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SpecialVariety](
	[special_variety_id] [int] IDENTITY(1,1) NOT NULL,
	[farm_id] [int] NULL,
	[variety_id] [int] NULL,
	[description] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[special_variety_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Trip]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Trip](
	[trip_id] [int] IDENTITY(1,1) NOT NULL,
	[trip_detail_id] [int] NULL,
	[trip_name] [nvarchar](100) NULL,
	[status] [nvarchar](100) NULL,
	[price] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[trip_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TripDetail]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TripDetail](
	[trip_detail_id] [int] IDENTITY(1,1) NOT NULL,
	[main_topic] [nvarchar](200) NULL,
	[sub_topic] [nvarchar](1000) NULL,
	[note_price] [nvarchar](1000) NULL,
	[Day] [date] NULL,
	[status] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[trip_detail_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Variety]    Script Date: 9/29/2024 8:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Variety](
	[variety_id] [int] IDENTITY(1,1) NOT NULL,
	[package_id] [int] NULL,
	[variety_name] [nvarchar](100) NULL,
	[description] [nvarchar](200) NULL,
	[imageUrl] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[variety_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Account] ON 

INSERT [dbo].[Account] ([account_id], [role_id], [username], [password], [lastname], [firstname], [gender], [phone], [email], [imageUrl], [status]) VALUES (1, 1, N'trung123', N'12345678', N'Trung', N'doan', NULL, N'0931209123', N'trung@gmail.com', NULL, 1)
INSERT [dbo].[Account] ([account_id], [role_id], [username], [password], [lastname], [firstname], [gender], [phone], [email], [imageUrl], [status]) VALUES (2, 1, N'noshibi123@gmail.com', NULL, N'03', N'-Trần Quốc Bang', NULL, NULL, N'noshibi123@gmail.com', NULL, 1)
INSERT [dbo].[Account] ([account_id], [role_id], [username], [password], [lastname], [firstname], [gender], [phone], [email], [imageUrl], [status]) VALUES (4, 1, N'trungdnse183494@fpt.edu.vn', NULL, N'(K18 HCM)', N'Doan Ngoc Trung', NULL, NULL, N'trungdnse183494@fpt.edu.vn', NULL, 1)
SET IDENTITY_INSERT [dbo].[Account] OFF
GO
SET IDENTITY_INSERT [dbo].[Booking] ON 

INSERT [dbo].[Booking] ([booking_id], [trip_id], [po_id], [booking_payment_id], [form_booking_id], [feedback_id], [quoted_amount], [quote_sent_date], [quote_approved_date], [manager_approval], [status], [start_date], [end_date], [koi_delivery_date], [koi_delivery_time], [total_amount], [bookingDate]) VALUES (1, NULL, NULL, NULL, 6, NULL, NULL, NULL, NULL, NULL, N'Pending', CAST(N'2024-09-28' AS Date), CAST(N'2024-10-01' AS Date), NULL, NULL, NULL, NULL)
INSERT [dbo].[Booking] ([booking_id], [trip_id], [po_id], [booking_payment_id], [form_booking_id], [feedback_id], [quoted_amount], [quote_sent_date], [quote_approved_date], [manager_approval], [status], [start_date], [end_date], [koi_delivery_date], [koi_delivery_time], [total_amount], [bookingDate]) VALUES (2, NULL, NULL, NULL, 7, NULL, NULL, NULL, NULL, NULL, N'Pending', CAST(N'2024-09-28' AS Date), CAST(N'2024-10-01' AS Date), NULL, NULL, NULL, NULL)
INSERT [dbo].[Booking] ([booking_id], [trip_id], [po_id], [booking_payment_id], [form_booking_id], [feedback_id], [quoted_amount], [quote_sent_date], [quote_approved_date], [manager_approval], [status], [start_date], [end_date], [koi_delivery_date], [koi_delivery_time], [total_amount], [bookingDate]) VALUES (3, NULL, NULL, NULL, 8, NULL, NULL, NULL, NULL, NULL, N'Pending', CAST(N'2024-09-28' AS Date), CAST(N'2024-09-29' AS Date), NULL, NULL, NULL, NULL)
INSERT [dbo].[Booking] ([booking_id], [trip_id], [po_id], [booking_payment_id], [form_booking_id], [feedback_id], [quoted_amount], [quote_sent_date], [quote_approved_date], [manager_approval], [status], [start_date], [end_date], [koi_delivery_date], [koi_delivery_time], [total_amount], [bookingDate]) VALUES (4, NULL, NULL, NULL, 9, NULL, NULL, NULL, NULL, NULL, N'Pending', CAST(N'2024-09-28' AS Date), CAST(N'2024-10-04' AS Date), NULL, NULL, NULL, NULL)
INSERT [dbo].[Booking] ([booking_id], [trip_id], [po_id], [booking_payment_id], [form_booking_id], [feedback_id], [quoted_amount], [quote_sent_date], [quote_approved_date], [manager_approval], [status], [start_date], [end_date], [koi_delivery_date], [koi_delivery_time], [total_amount], [bookingDate]) VALUES (5, NULL, NULL, NULL, 10, NULL, NULL, NULL, NULL, NULL, N'Pending', CAST(N'2024-10-11' AS Date), CAST(N'2024-10-17' AS Date), NULL, NULL, NULL, NULL)
INSERT [dbo].[Booking] ([booking_id], [trip_id], [po_id], [booking_payment_id], [form_booking_id], [feedback_id], [quoted_amount], [quote_sent_date], [quote_approved_date], [manager_approval], [status], [start_date], [end_date], [koi_delivery_date], [koi_delivery_time], [total_amount], [bookingDate]) VALUES (6, NULL, NULL, NULL, 14, NULL, NULL, NULL, NULL, NULL, N'Pending', CAST(N'2024-09-28' AS Date), CAST(N'2024-10-04' AS Date), NULL, NULL, NULL, CAST(N'2024-09-28T15:38:27.897' AS DateTime))
INSERT [dbo].[Booking] ([booking_id], [trip_id], [po_id], [booking_payment_id], [form_booking_id], [feedback_id], [quoted_amount], [quote_sent_date], [quote_approved_date], [manager_approval], [status], [start_date], [end_date], [koi_delivery_date], [koi_delivery_time], [total_amount], [bookingDate]) VALUES (7, NULL, NULL, NULL, 15, NULL, NULL, NULL, NULL, NULL, N'Pending', CAST(N'2024-09-28' AS Date), CAST(N'2024-10-04' AS Date), NULL, NULL, NULL, CAST(N'2024-09-28T15:52:30.387' AS DateTime))
INSERT [dbo].[Booking] ([booking_id], [trip_id], [po_id], [booking_payment_id], [form_booking_id], [feedback_id], [quoted_amount], [quote_sent_date], [quote_approved_date], [manager_approval], [status], [start_date], [end_date], [koi_delivery_date], [koi_delivery_time], [total_amount], [bookingDate]) VALUES (8, NULL, NULL, NULL, 16, NULL, NULL, NULL, NULL, NULL, N'Pending', CAST(N'2024-09-29' AS Date), CAST(N'2024-10-04' AS Date), NULL, NULL, NULL, CAST(N'2024-09-28T23:59:36.477' AS DateTime))
INSERT [dbo].[Booking] ([booking_id], [trip_id], [po_id], [booking_payment_id], [form_booking_id], [feedback_id], [quoted_amount], [quote_sent_date], [quote_approved_date], [manager_approval], [status], [start_date], [end_date], [koi_delivery_date], [koi_delivery_time], [total_amount], [bookingDate]) VALUES (9, NULL, NULL, NULL, 17, NULL, NULL, NULL, NULL, NULL, N'Pending', CAST(N'2024-09-29' AS Date), CAST(N'2024-10-04' AS Date), NULL, NULL, NULL, CAST(N'2024-09-29T00:36:39.710' AS DateTime))
INSERT [dbo].[Booking] ([booking_id], [trip_id], [po_id], [booking_payment_id], [form_booking_id], [feedback_id], [quoted_amount], [quote_sent_date], [quote_approved_date], [manager_approval], [status], [start_date], [end_date], [koi_delivery_date], [koi_delivery_time], [total_amount], [bookingDate]) VALUES (10, NULL, NULL, NULL, 18, NULL, NULL, NULL, NULL, NULL, N'Pending', CAST(N'2024-10-04' AS Date), CAST(N'2024-10-12' AS Date), NULL, NULL, NULL, CAST(N'2024-09-29T01:06:58.937' AS DateTime))
SET IDENTITY_INSERT [dbo].[Booking] OFF
GO
SET IDENTITY_INSERT [dbo].[Customer] ON 

INSERT [dbo].[Customer] ([customer_id], [account_id]) VALUES (1, 1)
INSERT [dbo].[Customer] ([customer_id], [account_id]) VALUES (2, 4)
SET IDENTITY_INSERT [dbo].[Customer] OFF
GO
SET IDENTITY_INSERT [dbo].[FormBooking] ON 

INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (1, NULL, N'Trung Doan', N'0920392039', N'trung@gmail.com', N'Asagi farm', CAST(3000.00 AS Decimal(10, 2)), N'Asagi koi', N'muong thanh', CAST(N'2024-09-28' AS Date), CAST(N'2024-09-30' AS Date), N'Male', N'di 2 ngay 1 dem')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (2, NULL, N'dsa', N'23213233213', N'trung@gmail.com', N'è', CAST(213233.00 AS Decimal(10, 2)), N'das', N'2323', CAST(N'2024-09-13' AS Date), CAST(N'2024-09-25' AS Date), N'Male', N'233')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (3, NULL, N'das', N'2010390900', N'trung@gmail.com', N'23', CAST(3000.00 AS Decimal(10, 2)), N'32', N'4000', CAST(N'2024-08-31' AS Date), CAST(N'2024-09-05' AS Date), N'Male', N'3 ngay 2 dem')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (4, NULL, N'Trung Doan', N'0787171600', N'trung@gmail.com', N'Asagi farm', CAST(3000.00 AS Decimal(10, 2)), N'Asagi koi', N'2000', CAST(N'2024-09-28' AS Date), CAST(N'2024-10-01' AS Date), N'Male', N'3 ngay 2 dem')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (5, NULL, N'Trung Doan', N'0787171600', N'trung332@gmail.com', N'Asagi farm', CAST(300.00 AS Decimal(10, 2)), N'Asagi koi', N'2000', CAST(N'2024-09-28' AS Date), CAST(N'2024-09-30' AS Date), N'Male', N'more to more')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (6, NULL, N'Trung Doan', N'0787171600', N'trung@gmail.com', N'Asagi farm', CAST(3000.00 AS Decimal(10, 2)), N'Asagi koi', N'muong thanh', CAST(N'2024-09-28' AS Date), CAST(N'2024-10-01' AS Date), N'Female', N'3 ngay 2 dem')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (7, 1, N'Trung Doan', N'0787171600', N'trung@gmail.com', N'Asagi farm', CAST(3000.00 AS Decimal(10, 2)), N'Asagi koi', N'kk', CAST(N'2024-09-28' AS Date), CAST(N'2024-10-01' AS Date), N'Male', N'kk')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (8, 1, N'Trung Doan', N'0787171600', N'trung@gmail.com', N'Asagi farm', CAST(3000.00 AS Decimal(10, 2)), N'Asagi koi', N'muong thnah', CAST(N'2024-09-28' AS Date), CAST(N'2024-09-29' AS Date), N'Male', N'kk')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (9, 1, N'Trung Doan', N'0787171600', N'trung@gmail.com', N'Asagi farm', CAST(3000.00 AS Decimal(10, 2)), N'Asagi koi', N'haha', CAST(N'2024-09-28' AS Date), CAST(N'2024-10-04' AS Date), N'Male', N'haha')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (10, 1, N'Trung Doan', N'0787171600', N'trung@gmail.com', N'Asagi farm', CAST(30000.00 AS Decimal(10, 2)), N'Asagi koi', N'qao', CAST(N'2024-10-11' AS Date), CAST(N'2024-10-17' AS Date), N'Male', N'qao')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (11, 1, N'Trung Doan', N'0787171600', N'trung@gmail.com', N'Asagi farm', CAST(3000.00 AS Decimal(10, 2)), N'Asagi koi', N'muong thanh', CAST(N'2024-09-28' AS Date), CAST(N'2024-10-01' AS Date), N'Male', N'qoa qao')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (12, 1, N'Trung Doan', N'0787171600', N'trung@gmail.com', N'Asagi farm', CAST(3000.00 AS Decimal(10, 2)), N'Asagi koi', N'lala', CAST(N'2024-09-29' AS Date), CAST(N'2024-10-05' AS Date), N'Female', N'laala')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (13, 1, N'Trung Doan', N'0787171600', N'trung@gmail.com', N'Asagi farm', CAST(3000.00 AS Decimal(10, 2)), N'Asagi koi', N'5 sao', CAST(N'2024-09-14' AS Date), CAST(N'2024-10-04' AS Date), N'Male', N' haha')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (14, 1, N'Trung Doan', N'0787171600', N'trung@gmail.com', N'Asagi farm', CAST(300.00 AS Decimal(10, 2)), N'Asagi koi', N'3 sao', CAST(N'2024-09-28' AS Date), CAST(N'2024-10-04' AS Date), N'Male', N'3000')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (15, 1, N'Trung Doan', N'0787171600', N'trung332@gmail.com', N'Asagi farm', CAST(30000.00 AS Decimal(10, 2)), N'Asagi koi', N'10 sao', CAST(N'2024-09-28' AS Date), CAST(N'2024-10-04' AS Date), N'Male', N'kaka')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (16, 1, N'd', N'0930219393', N'trung@gmail.com', N'asagi farm', CAST(3000.00 AS Decimal(10, 2)), N'asagi koi', N'muong thanh tang 4', CAST(N'2024-09-29' AS Date), CAST(N'2024-10-04' AS Date), N'Male', N'qua da 2 lon pepsi qua da')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (17, 1, N'trung doan', N'0903210932', N'trung@gmail.com', N'koi asagi', CAST(300.00 AS Decimal(10, 2)), N'koi asagi', N'asagi', CAST(N'2024-09-29' AS Date), CAST(N'2024-10-04' AS Date), N'Female', N'asagi')
INSERT [dbo].[FormBooking] ([form_booking_id], [customer_id], [fullname], [phone], [email], [favoritefarm], [estimatedCost], [favoriteKoi], [hotel_accommodation], [estimatedDepartureDate], [returnDate], [gender], [note]) VALUES (18, 1, N'koi', N'0931029393', N'trung1123@gmail.com', N'koi', CAST(3000.00 AS Decimal(10, 2)), N'koi', N'5 sao', CAST(N'2024-10-04' AS Date), CAST(N'2024-10-12' AS Date), N'Male', N'koi')
SET IDENTITY_INSERT [dbo].[FormBooking] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([role_id], [name]) VALUES (1, N'customer')
INSERT [dbo].[Role] ([role_id], [name]) VALUES (2, N'manager')
INSERT [dbo].[Role] ([role_id], [name]) VALUES (3, N'sale staff')
INSERT [dbo].[Role] ([role_id], [name]) VALUES (4, N'culsulting staff')
INSERT [dbo].[Role] ([role_id], [name]) VALUES (5, N'delivering staff')
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_Account_Role] FOREIGN KEY([role_id])
REFERENCES [dbo].[Role] ([role_id])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_Account_Role]
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_BookingPayment] FOREIGN KEY([booking_payment_id])
REFERENCES [dbo].[BookingPayment] ([booking_payment_id])
GO
ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_BookingPayment]
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Feedback] FOREIGN KEY([feedback_id])
REFERENCES [dbo].[Feedback] ([feedback_id])
GO
ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_Feedback]
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_FormBooking] FOREIGN KEY([form_booking_id])
REFERENCES [dbo].[FormBooking] ([form_booking_id])
GO
ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_FormBooking]
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_PO] FOREIGN KEY([po_id])
REFERENCES [dbo].[PO] ([po_id])
GO
ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_PO]
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Trip] FOREIGN KEY([trip_id])
REFERENCES [dbo].[Trip] ([trip_id])
GO
ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_Trip]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_Account] FOREIGN KEY([account_id])
REFERENCES [dbo].[Account] ([account_id])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_Account]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Customer] FOREIGN KEY([customer_id])
REFERENCES [dbo].[Customer] ([customer_id])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK_Feedback_Customer]
GO
ALTER TABLE [dbo].[FormBooking]  WITH CHECK ADD  CONSTRAINT [FK_FormBooking_Customer] FOREIGN KEY([customer_id])
REFERENCES [dbo].[Customer] ([customer_id])
GO
ALTER TABLE [dbo].[FormBooking] CHECK CONSTRAINT [FK_FormBooking_Customer]
GO
ALTER TABLE [dbo].[KoiFarm]  WITH CHECK ADD  CONSTRAINT [FK_KoiFarm_KoiFish] FOREIGN KEY([koi_id])
REFERENCES [dbo].[KoiFish] ([koi_id])
GO
ALTER TABLE [dbo].[KoiFarm] CHECK CONSTRAINT [FK_KoiFarm_KoiFish]
GO
ALTER TABLE [dbo].[KoiFarm]  WITH CHECK ADD  CONSTRAINT [FK_KoiFarm_TripDetail] FOREIGN KEY([trip_detail_id])
REFERENCES [dbo].[TripDetail] ([trip_detail_id])
GO
ALTER TABLE [dbo].[KoiFarm] CHECK CONSTRAINT [FK_KoiFarm_TripDetail]
GO
ALTER TABLE [dbo].[KoiFish]  WITH CHECK ADD  CONSTRAINT [FK_KoiFish_Variety] FOREIGN KEY([variety_id])
REFERENCES [dbo].[Variety] ([variety_id])
GO
ALTER TABLE [dbo].[KoiFish] CHECK CONSTRAINT [FK_KoiFish_Variety]
GO
ALTER TABLE [dbo].[KoiPackage]  WITH CHECK ADD  CONSTRAINT [FK_KoiPackage_Farm] FOREIGN KEY([farm_id])
REFERENCES [dbo].[KoiFarm] ([farm_id])
GO
ALTER TABLE [dbo].[KoiPackage] CHECK CONSTRAINT [FK_KoiPackage_Farm]
GO
ALTER TABLE [dbo].[KoiPackage]  WITH CHECK ADD  CONSTRAINT [FK_KoiPackage_PODetail] FOREIGN KEY([po_detail_id])
REFERENCES [dbo].[PODetail] ([po_detail_id])
GO
ALTER TABLE [dbo].[KoiPackage] CHECK CONSTRAINT [FK_KoiPackage_PODetail]
GO
ALTER TABLE [dbo].[PaymentMethod]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethod_BookingPayment] FOREIGN KEY([booking_payment_id])
REFERENCES [dbo].[BookingPayment] ([booking_payment_id])
GO
ALTER TABLE [dbo].[PaymentMethod] CHECK CONSTRAINT [FK_PaymentMethod_BookingPayment]
GO
ALTER TABLE [dbo].[PaymentMethod]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethod_POPayment] FOREIGN KEY([po_payment_id])
REFERENCES [dbo].[POPayment] ([po_payment_id])
GO
ALTER TABLE [dbo].[PaymentMethod] CHECK CONSTRAINT [FK_PaymentMethod_POPayment]
GO
ALTER TABLE [dbo].[PO]  WITH CHECK ADD  CONSTRAINT [FK_PO_KoiFarm] FOREIGN KEY([farm_id])
REFERENCES [dbo].[KoiFarm] ([farm_id])
GO
ALTER TABLE [dbo].[PO] CHECK CONSTRAINT [FK_PO_KoiFarm]
GO
ALTER TABLE [dbo].[PODetail]  WITH CHECK ADD  CONSTRAINT [FK_PODetail_KoiFarm] FOREIGN KEY([farm_id])
REFERENCES [dbo].[KoiFarm] ([farm_id])
GO
ALTER TABLE [dbo].[PODetail] CHECK CONSTRAINT [FK_PODetail_KoiFarm]
GO
ALTER TABLE [dbo].[PODetail]  WITH CHECK ADD  CONSTRAINT [FK_PODetail_KoiFish] FOREIGN KEY([koi_id])
REFERENCES [dbo].[KoiFish] ([koi_id])
GO
ALTER TABLE [dbo].[PODetail] CHECK CONSTRAINT [FK_PODetail_KoiFish]
GO
ALTER TABLE [dbo].[PODetail]  WITH CHECK ADD  CONSTRAINT [FK_PODetail_PO] FOREIGN KEY([po_id])
REFERENCES [dbo].[PO] ([po_id])
GO
ALTER TABLE [dbo].[PODetail] CHECK CONSTRAINT [FK_PODetail_PO]
GO
ALTER TABLE [dbo].[POPayment]  WITH CHECK ADD  CONSTRAINT [FK_POPayment_PO] FOREIGN KEY([po_id])
REFERENCES [dbo].[PO] ([po_id])
GO
ALTER TABLE [dbo].[POPayment] CHECK CONSTRAINT [FK_POPayment_PO]
GO
ALTER TABLE [dbo].[SpecialVariety]  WITH CHECK ADD  CONSTRAINT [FK_SpecialVariety_Farm] FOREIGN KEY([farm_id])
REFERENCES [dbo].[KoiFarm] ([farm_id])
GO
ALTER TABLE [dbo].[SpecialVariety] CHECK CONSTRAINT [FK_SpecialVariety_Farm]
GO
ALTER TABLE [dbo].[SpecialVariety]  WITH CHECK ADD  CONSTRAINT [FK_SpecialVariety_Variety] FOREIGN KEY([variety_id])
REFERENCES [dbo].[Variety] ([variety_id])
GO
ALTER TABLE [dbo].[SpecialVariety] CHECK CONSTRAINT [FK_SpecialVariety_Variety]
GO
ALTER TABLE [dbo].[Trip]  WITH CHECK ADD  CONSTRAINT [FK_Trip_TripDetail] FOREIGN KEY([trip_detail_id])
REFERENCES [dbo].[TripDetail] ([trip_detail_id])
GO
ALTER TABLE [dbo].[Trip] CHECK CONSTRAINT [FK_Trip_TripDetail]
GO
ALTER TABLE [dbo].[Variety]  WITH CHECK ADD  CONSTRAINT [FK_Variety_KoiPackage] FOREIGN KEY([package_id])
REFERENCES [dbo].[KoiPackage] ([package_id])
GO
ALTER TABLE [dbo].[Variety] CHECK CONSTRAINT [FK_Variety_KoiPackage]
GO
USE [master]
GO
ALTER DATABASE [Koi88] SET  READ_WRITE 
GO
