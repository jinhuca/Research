USE Master
GO
IF EXISTS(SELECT name FROM sys.databases WHERE name = 'FieldService')
Begin
	 alter database [FieldService] set single_user with rollback immediate
	 DROP DATABASE [FieldService]
End
GO
CREATE Database [FieldService]
GO
Alter Database [FieldService] SET MULTI_USER;
Go

USE [FieldService]
GO
/****** Object:  Table [dbo].[ManualTestRecord]    Script Date: 5/31/2022 5:40:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ManualTestRecord](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TId] [int] NOT NULL,
	[TestPassed] [bit] NOT NULL,
	[TestType] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ManualTestRecord] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParameterCheckRecord]    Script Date: 5/31/2022 5:40:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ParameterCheckRecord](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TId] [int] NOT NULL,
	[StateType] [int] NOT NULL,
	[Temp] [float] NOT NULL,
	[FM1] [float] NOT NULL,
	[IBP] [float] NOT NULL,
	[OBP] [float] NOT NULL,
	[PT1] [float] NOT NULL,
	[PT2] [float] NOT NULL,
	[PT3] [float] NOT NULL,
	[PT5] [float] NOT NULL,
	[PWM1] [float] NOT NULL,
	[PWM2] [float] NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
	[Comments] [varchar](500) NULL,
 CONSTRAINT [PK_ParameterCheckRecord] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PerformanceTestRecord]    Script Date: 5/31/2022 5:40:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PerformanceTestRecord](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TId] [int] NOT NULL,
	[PId] [int] NOT NULL,
	[StateType] [int] NOT NULL,
	[Temp] [float] NOT NULL,
	[FM1] [float] NOT NULL,
	[IBP] [float] NOT NULL,
	[OBP] [float] NOT NULL,
	[PT1] [float] NOT NULL,
	[PT2] [float] NOT NULL,
	[PT3] [float] NOT NULL,
	[PT5] [float] NOT NULL,
	[PWM1] [float] NOT NULL,
	[PWM2] [float] NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
	[Comments] [varchar](500) NULL,
 CONSTRAINT [PK_PerformanceTestRecord] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TestRecord]    Script Date: 4/25/2022 1:51:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestRecord](
	[Id] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Notes] [varchar](2000) NULL,
 CONSTRAINT [PK_TestRecord] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ManualTestRecord] ON 
GO
INSERT [dbo].[ManualTestRecord] ([Id], [TId], [TestPassed], [TestType], [CreateDate]) VALUES (1, 1, 1, 1, CAST(N'2022-01-26T00:00:00.000' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[ManualTestRecord] OFF
GO
/****** Object:  StoredProcedure [dbo].[GetCatheterTypeInfo]    Script Date: 5/31/2022 5:40:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Emily
-- Create date: 2022-04-06
-- Description:	Get Catheter Type Info
-- =============================================
CREATE PROCEDURE [dbo].[GetCatheterTypeInfo]

AS
BEGIN
		SET NOCOUNT ON;
		SELECT [CatheterID],[Description],[EngineeringDescription] from [ConsoleDatabase].[dbo].[CatheterTypes] 

END
GO
/****** Object:  StoredProcedure [dbo].[GetCMCRegisterDefaultValues]    Script Date: 5/31/2022 5:40:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Emily
-- Create date: 2022-04-05
-- Description:	Get default values from CMCRegisterValues
-- =============================================
CREATE PROCEDURE [dbo].[GetCMCRegisterDefaultValues]
AS
BEGIN
		SET NOCOUNT ON;
		SELECT  CatheterTypeID, cr.StateID, cr.DGain,cr.IGain, cr.Offset, cr.PGain from [ConsoleDatabase].[dbo].[CMCRegisterValues] cr order by cr.CatheterTypeID,cr.StateID asc

END
GO
/****** Object:  StoredProcedure [dbo].[GetErrorMessagesByIdType]    Script Date: 5/31/2022 5:40:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Emily
-- Create date: 2022-01-26
-- Description:	Get Error Messages
-- =============================================
CREATE PROCEDURE [dbo].[GetErrorMessagesByIdType]
@errorId int,
@errorType int

AS
BEGIN
		SET NOCOUNT ON;
		SELECT ce.ErrorCode, ce.[Message], ce.CryterionMessage, ce.SolutionMessage from [ConsoleDatabase].[dbo].[ErrorMessages] ce where ce.[Type] = @errorType and ce.[Id]= @errorId and ce.LanguageId =1

END
GO
/****** Object:  StoredProcedure [dbo].[GetManualTestRecordData]    Script Date: 5/31/2022 5:40:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetManualTestRecordData]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * from [dbo].[ManualTestRecord]
END
GO
/****** Object:  StoredProcedure [dbo].[GetPMCRegisterDefaultValues]    Script Date: 5/31/2022 5:40:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Emily
-- Create date: 2022-04-05
-- Description:	Get Default values from PMCRegisterValues
-- =============================================
CREATE PROCEDURE [dbo].[GetPMCRegisterDefaultValues]

AS
BEGIN
		SET NOCOUNT ON;
		SELECT cp.CatheterTypeID,cp.StateID, cp.DGain,cp.IGain, cp.Offset, cp.PGain from [ConsoleDatabase].[dbo].[PMCRegisterValues] cp  order by  cp.CatheterTypeID, cp.StateID asc

END
GO
/****** Object:  StoredProcedure [dbo].[GetSMFConsoleInfo]    Script Date: 5/31/2022 5:40:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Emily
-- Create date: 2022-05-30
-- Description:	Get Console Info from ConsoleDatabase
-- =============================================
CREATE PROCEDURE [dbo].[GetSMFConsoleInfo]

AS
BEGIN
		SET NOCOUNT ON;
		SELECT TOP 1 c.[SerialNumber]
      ,c.[UtilisationDuration]
      ,c.[CurrentTank]
      ,c.[IsUsingPurge]
      ,c.[IsUsingCatheterDeflateSwitch]
      ,c.[IsBalloonRampDownActivated]
      ,c.[IsUsingDeflateAfterThaw]
      ,c.[HardDrive_Id]
      ,c.[IsUsingBloodPressureSensor]
      ,c.[ComPortName]
      ,c.[IsUsingLowFlow]
      ,c.[IsUsingDaylightSavingTime]
  FROM [ConsoleDatabase].[dbo].[Consoles] c

END
GO
/****** Object:  StoredProcedure [dbo].[GetSMFDBVersion]    Script Date: 5/31/2022 5:40:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Emily
-- Create date: 2022-03-04
-- Description:	Get DB Version
-- =============================================
CREATE PROCEDURE [dbo].[GetSMFDBVersion]

AS
BEGIN
		SET NOCOUNT ON;
		SELECT c.Id, c.[Version] as DBV from [ConsoleDatabase].[dbo].[DataBaseVersions] c

END
GO
