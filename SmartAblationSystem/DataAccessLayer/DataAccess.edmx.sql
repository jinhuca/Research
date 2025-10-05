
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/10/2020 09:17:54
-- Generated from EDMX file: C:\Users\smaila\Documents\Cryterion_Medical\branches\CryoTherapyV3\DataAccessLayer\DataAccess.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ConsoleDatabase];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_CatheterTypeCatheterInformation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CatheterInformations] DROP CONSTRAINT [FK_CatheterTypeCatheterInformation];
GO
IF OBJECT_ID(N'[dbo].[FK_CatheterTypeCMCRegisterValue]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CMCRegisterValues] DROP CONSTRAINT [FK_CatheterTypeCMCRegisterValue];
GO
IF OBJECT_ID(N'[dbo].[FK_CatheterTypePMCRegisterValue]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PMCRegisterValues] DROP CONSTRAINT [FK_CatheterTypePMCRegisterValue];
GO
IF OBJECT_ID(N'[dbo].[FK_ConsoleHardDrive]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Consoles] DROP CONSTRAINT [FK_ConsoleHardDrive];
GO
IF OBJECT_ID(N'[dbo].[FK_ConsoleTank]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tanks] DROP CONSTRAINT [FK_ConsoleTank];
GO
IF OBJECT_ID(N'[dbo].[FK_ErrorErrorMessage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ErrorMessages] DROP CONSTRAINT [FK_ErrorErrorMessage];
GO
IF OBJECT_ID(N'[dbo].[FK_GUIFieldTranslation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Translations] DROP CONSTRAINT [FK_GUIFieldTranslation];
GO
IF OBJECT_ID(N'[dbo].[FK_PatientProcedure]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Procedures] DROP CONSTRAINT [FK_PatientProcedure];
GO
IF OBJECT_ID(N'[dbo].[FK_PhysicianPatient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Patients] DROP CONSTRAINT [FK_PhysicianPatient];
GO
IF OBJECT_ID(N'[dbo].[FK_Physicianpreference]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Physicians] DROP CONSTRAINT [FK_Physicianpreference];
GO
IF OBJECT_ID(N'[dbo].[FK_ProcedureAblation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Ablations] DROP CONSTRAINT [FK_ProcedureAblation];
GO
IF OBJECT_ID(N'[dbo].[FK_ProcedureProcedureLog]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProcedureLogs] DROP CONSTRAINT [FK_ProcedureProcedureLog];
GO
IF OBJECT_ID(N'[dbo].[FK_UserType_Type]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserType] DROP CONSTRAINT [FK_UserType_Type];
GO
IF OBJECT_ID(N'[dbo].[FK_UserType_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserType] DROP CONSTRAINT [FK_UserType_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUserAction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserActions] DROP CONSTRAINT [FK_UserUserAction];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AblationDatas]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AblationDatas];
GO
IF OBJECT_ID(N'[dbo].[Ablations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Ablations];
GO
IF OBJECT_ID(N'[dbo].[Actions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Actions];
GO
IF OBJECT_ID(N'[dbo].[BalloonParameters]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BalloonParameters];
GO
IF OBJECT_ID(N'[dbo].[CatheterInformations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CatheterInformations];
GO
IF OBJECT_ID(N'[dbo].[CatheterTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CatheterTypes];
GO
IF OBJECT_ID(N'[dbo].[CMCRegisterValues]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CMCRegisterValues];
GO
IF OBJECT_ID(N'[dbo].[CMCUPIDLogs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CMCUPIDLogs];
GO
IF OBJECT_ID(N'[dbo].[Consoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Consoles];
GO
IF OBJECT_ID(N'[dbo].[DataBaseVersions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DataBaseVersions];
GO
IF OBJECT_ID(N'[dbo].[ErrorMessages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ErrorMessages];
GO
IF OBJECT_ID(N'[dbo].[Errors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Errors];
GO
IF OBJECT_ID(N'[dbo].[ErrorTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ErrorTypes];
GO
IF OBJECT_ID(N'[dbo].[FailuresLogs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FailuresLogs];
GO
IF OBJECT_ID(N'[dbo].[GUIFields]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GUIFields];
GO
IF OBJECT_ID(N'[dbo].[HardDrives]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HardDrives];
GO
IF OBJECT_ID(N'[dbo].[HospitalInformations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HospitalInformations];
GO
IF OBJECT_ID(N'[dbo].[Languages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Languages];
GO
IF OBJECT_ID(N'[dbo].[Patients]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Patients];
GO
IF OBJECT_ID(N'[dbo].[Physicians]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Physicians];
GO
IF OBJECT_ID(N'[dbo].[PMCRegisterValues]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PMCRegisterValues];
GO
IF OBJECT_ID(N'[dbo].[PMCUPIDLogs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PMCUPIDLogs];
GO
IF OBJECT_ID(N'[dbo].[preferences]', 'U') IS NOT NULL
    DROP TABLE [dbo].[preferences];
GO
IF OBJECT_ID(N'[dbo].[ProcedureLogs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProcedureLogs];
GO
IF OBJECT_ID(N'[dbo].[Procedures]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Procedures];
GO
IF OBJECT_ID(N'[dbo].[Settings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Settings];
GO
IF OBJECT_ID(N'[dbo].[SystemStates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SystemStates];
GO
IF OBJECT_ID(N'[dbo].[Tanks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tanks];
GO
IF OBJECT_ID(N'[dbo].[TankTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TankTypes];
GO
IF OBJECT_ID(N'[dbo].[Translations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Translations];
GO
IF OBJECT_ID(N'[dbo].[Types]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Types];
GO
IF OBJECT_ID(N'[dbo].[UserActions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserActions];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[UserType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserType];
GO
IF OBJECT_ID(N'[DataAccessStoreContainer].[ErrorLog]', 'U') IS NOT NULL
    DROP TABLE [DataAccessStoreContainer].[ErrorLog];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Patients'
CREATE TABLE [dbo].[Patients] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [LastName] nvarchar(max)  NOT NULL,
    [DateOfBirth] datetime  NOT NULL,
    [Gender] smallint  NOT NULL,
    [TreatmentDateTime] datetime  NOT NULL,
    [HospitalPatientId] nvarchar(150)  NOT NULL,
    [Weight] float  NULL,
    [Height] float  NULL,
    [PhysicianID] int  NOT NULL
);
GO

-- Creating table 'Physicians'
CREATE TABLE [dbo].[Physicians] (
    [ID] int  NOT NULL,
    [Name] nvarchar(150)  NOT NULL,
    [HospitalPhyscianID] nvarchar(150)  NOT NULL,
    [LastName] nvarchar(50)  NOT NULL,
    [FirstName] nvarchar(50)  NOT NULL,
    [preference_Id] int  NOT NULL
);
GO

-- Creating table 'Ablations'
CREATE TABLE [dbo].[Ablations] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [PatientID] int  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [AblationNumber] int  NOT NULL,
    [AblationDuration] int  NULL,
    [DataFile] nvarchar(max)  NOT NULL,
    [TreatmentNote] nvarchar(max)  NOT NULL,
    [ProcedureId] int  NOT NULL,
    [ErrorInformation] nvarchar(max)  NULL
);
GO

-- Creating table 'AblationDatas'
CREATE TABLE [dbo].[AblationDatas] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [TC1Reading] float  NOT NULL,
    [TimeInSecondIndex] int  NOT NULL,
    [AblationID] int  NOT NULL
);
GO

-- Creating table 'Consoles'
CREATE TABLE [dbo].[Consoles] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [SerialNumber] nvarchar(max)  NOT NULL,
    [ReleaseDate] datetime  NOT NULL,
    [IsReleased] bit  NOT NULL,
    [Efficiency] int  NOT NULL,
    [UtilisationDuration] bigint  NOT NULL,
    [CurrentTank] int  NULL,
    [IsUsingPurge] bit  NOT NULL,
    [IsUsingCatheterDeflateSwitch] bit  NOT NULL,
    [IsBalloonRampDownActivated] bit  NOT NULL,
    [IsUsingDeflateAfterThaw] bit  NOT NULL,
    [IsUsingBloodPressureSensor] bit  NOT NULL,
    [ComPortName] varchar(50)  NOT NULL,
    [IsUsingLowFlow] bit  NOT NULL,
    [HardDrive_Id] int  NOT NULL
);
GO

-- Creating table 'CatheterTypes'
CREATE TABLE [dbo].[CatheterTypes] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [CatheterID] int  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [EngineeringDescription] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'SystemStates'
CREATE TABLE [dbo].[SystemStates] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [StateID] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'CMCRegisterValues'
CREATE TABLE [dbo].[CMCRegisterValues] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [StateID] int  NOT NULL,
    [PT1TankPressureLow] float  NOT NULL,
    [PT1PressureThresholdHighLimit] float  NOT NULL,
    [PT1TankPressureTooHigh] float  NOT NULL,
    [PT1PressureLowRangeLimit] float  NOT NULL,
    [PT1PressureHighRangeLimit] float  NOT NULL,
    [PT2PressureThresholdHighLimit] float  NOT NULL,
    [PT2PressureLowRangeLimit] float  NOT NULL,
    [PT2PressureHighRangeLimit] float  NOT NULL,
    [PT3PressureThresholdHighLimit] float  NOT NULL,
    [PT3PressureLowRangeLimit] float  NOT NULL,
    [PT3PressureHighRangeLimit] float  NOT NULL,
    [PT4PressureThresholdHighLimit] float  NOT NULL,
    [PT4PressureLowRangeLimit] float  NOT NULL,
    [PT4PressureHighRangeLimit] float  NOT NULL,
    [TS1TemperatureThresholdHighLimit] float  NOT NULL,
    [TS1TemperatureLowRangeLimit] float  NOT NULL,
    [TS1TemperatureHighRangeLimit] float  NOT NULL,
    [FM1FlowMeterThresholLowlimit] float  NOT NULL,
    [FM1FlowMeterThresholHighlimit] float  NOT NULL,
    [FM1FlowMeterLowRangeLimit] float  NOT NULL,
    [FM1FlowMeterHighRangelimit] float  NOT NULL,
    [PS1PressureThresholdHighLimit] float  NOT NULL,
    [PS1PressureLowRangeLimit] float  NOT NULL,
    [PS1PressureHighRangeLimit] float  NOT NULL,
    [PS2PressureThresholdHighLimit] float  NOT NULL,
    [PS2PressureLowRangeLimit] float  NOT NULL,
    [PS2PressureHighRangeLimit] float  NOT NULL,
    [LC1LoadCellThresholdWarning] float  NOT NULL,
    [LC1LoadCellThresholdFail] float  NOT NULL,
    [LC1LoadCellLowRangeLimit] float  NOT NULL,
    [LC1LoadCellHighRangeLimit] float  NOT NULL,
    [PGain] float  NOT NULL,
    [IGain] float  NOT NULL,
    [DGain] float  NOT NULL,
    [Offset] float  NOT NULL,
    [CatheterTypeID] int  NOT NULL,
    [TargetInjectionFlow] float  NOT NULL,
    [TargetInjectionPressure] float  NOT NULL,
    [LowFlow] float  NOT NULL
);
GO

-- Creating table 'PMCRegisterValues'
CREATE TABLE [dbo].[PMCRegisterValues] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [StateID] int  NOT NULL,
    [CP1PressureThresholdHighLimit] float  NOT NULL,
    [CP1PressureLowRangeLimit] float  NOT NULL,
    [CP1PressureHighRangeLimit] float  NOT NULL,
    [CP2PressureThresholdHighLimit] float  NOT NULL,
    [CP2PressureLowRangeLimit] float  NOT NULL,
    [CP2PressureHighRangeLimit] float  NOT NULL,
    [TC1ThawingTemperature] float  NOT NULL,
    [Pgain] float  NOT NULL,
    [Igain] float  NOT NULL,
    [Dgain] float  NOT NULL,
    [Offset] float  NOT NULL,
    [TargetBalloonPressure] float  NOT NULL,
    [LowerBloodThreshold] smallint  NOT NULL,
    [UpperBloodThreshold] smallint  NOT NULL,
    [CatheterTypeID] int  NOT NULL,
    [ThawingTemperatureSetPoint] float  NOT NULL
);
GO

-- Creating table 'CatheterInformations'
CREATE TABLE [dbo].[CatheterInformations] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [SerialNumber] int  NOT NULL,
    [FirmwareVersion] int  NOT NULL,
    [CatheterExpirationDate] datetime  NOT NULL,
    [LastUseDate] datetime  NOT NULL,
    [NumberOfInjection] int  NOT NULL,
    [Lot] int  NOT NULL,
    [IsUsedForEngineering] bit  NOT NULL,
    [OverloadedCatheterID] int  NOT NULL,
    [CatheterTypeID] int  NOT NULL
);
GO

-- Creating table 'Procedures'
CREATE TABLE [dbo].[Procedures] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [ProcedureStartDateTime] datetime  NOT NULL,
    [PhysicianID] int  NOT NULL,
    [Diagnosis] nvarchar(max)  NOT NULL,
    [OutCome] nvarchar(max)  NOT NULL,
    [SkinToSkinDuration] smallint  NOT NULL,
    [IsDataEdited] bit  NOT NULL,
    [Archived] bit  NOT NULL,
    [ProcedureLogId] int  NOT NULL,
    [PatientID] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [LastName] nvarchar(max)  NOT NULL,
    [UserName] nvarchar(max)  NOT NULL,
    [EmailAdredress] nvarchar(max)  NOT NULL,
    [PhoneNumber] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [Status] bit  NOT NULL
);
GO

-- Creating table 'Types'
CREATE TABLE [dbo].[Types] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UserActions'
CREATE TABLE [dbo].[UserActions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Time] datetime  NOT NULL,
    [ActionId] int  NOT NULL,
    [UserId] int  NOT NULL
);
GO

-- Creating table 'Actions'
CREATE TABLE [dbo].[Actions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'CMCUPIDLogs'
CREATE TABLE [dbo].[CMCUPIDLogs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FM1] float  NOT NULL,
    [PT2] float  NOT NULL,
    [TargetFlow] float  NOT NULL,
    [TargetInjectionPressure] float  NOT NULL,
    [TargetFlowError] float  NOT NULL,
    [TargetInjectionPressureError] float  NOT NULL,
    [P] float  NOT NULL,
    [I] float  NOT NULL,
    [D] float  NOT NULL,
    [O] float  NOT NULL,
    [DateOfChange] datetime  NOT NULL
);
GO

-- Creating table 'PMCUPIDLogs'
CREATE TABLE [dbo].[PMCUPIDLogs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PT3] float  NOT NULL,
    [IBP] float  NOT NULL,
    [OBP] float  NOT NULL,
    [PWMINJ] float  NOT NULL,
    [PWMBAL] float  NOT NULL,
    [TargetBalloonPressure] float  NOT NULL,
    [TargetBallonPressureError] float  NOT NULL,
    [P] float  NOT NULL,
    [I] float  NOT NULL,
    [D] float  NOT NULL,
    [O] float  NOT NULL,
    [DateOfChange] datetime  NOT NULL
);
GO

-- Creating table 'HospitalInformations'
CREATE TABLE [dbo].[HospitalInformations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Rooms] int  NOT NULL,
    [beds] int  NOT NULL,
    [Scheduling] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NOT NULL,
    [City] nvarchar(max)  NOT NULL,
    [State] nvarchar(max)  NOT NULL,
    [Country] nvarchar(max)  NOT NULL,
    [PostalCode] nvarchar(max)  NOT NULL,
    [PhoneNumber] bigint  NOT NULL
);
GO

-- Creating table 'Tanks'
CREATE TABLE [dbo].[Tanks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] int  NOT NULL,
    [WeightAtReplacementDate] float  NOT NULL,
    [WeightAtEndOfUseDate] float  NOT NULL,
    [ReplacementDate] datetime  NOT NULL,
    [EndOfUseDate] datetime  NOT NULL,
    [TankTypesID] smallint  NOT NULL,
    [ConsoleID] int  NOT NULL
);
GO

-- Creating table 'preferences'
CREATE TABLE [dbo].[preferences] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CoolingRequiredTargetTemperature] float  NOT NULL,
    [ThawTimerToTemperature] float  NOT NULL,
    [LowAblationTemperatureAlarm] float  NOT NULL,
    [HighAblationTemperatureAlarm] float  NOT NULL,
    [EsophagusTemperature] float  NOT NULL,
    [DiaphragmAmplitude] float  NOT NULL,
    [DiaphragmAmplitudeType] int  NOT NULL,
    [BalloonPressureSelected] bit  NOT NULL,
    [TipPressureSelected] bit  NOT NULL,
    [AblationTimer] int  NOT NULL,
    [IsUsingAutoDeflation] bit  NOT NULL,
    [VolumeLevel] smallint  NOT NULL,
    [CurveStyle] smallint  NOT NULL,
    [CurveColor] smallint  NOT NULL,
    [Background] nvarchar(max)  NOT NULL,
    [IsUsingInflationFastSpeed] bit  NOT NULL,
    [RefrigerantLevelUnit] smallint  NOT NULL,
    [DiaphragmSensorGain] smallint  NOT NULL,
    [IgnoreDiaphragmMovement] bit  NOT NULL,
    [DMSDetectionThreshold] float  NOT NULL,
    [IgnoreEsophagusTemperatureMonitoring] bit  NOT NULL,
    [VeinIsolationDuration] int  NOT NULL,
    [RequestedAblationTime] int  NOT NULL,
    [NewVeinIsolationDuration] int  NOT NULL,
    [NewRequestedAblationTime] int  NOT NULL,
    [ExpectedVeinIsolationTime] int  NOT NULL,
    [AblationTimerTTIFixed] int  NOT NULL,
    [NewAblationTimerTTIFixed] int  NOT NULL,
    [DurationExpectedVeinIsolationTime] int  NOT NULL,
    [AblationTimerTTI] int  NOT NULL,
    [NewAblationTimerTTI] int  NOT NULL,
    [IsUsingAudioAlert] bit  NOT NULL,
    [AblationDurationType] smallint  NOT NULL,
    [EnabaleEnhancedAudio] bit  NOT NULL
);
GO

-- Creating table 'DataBaseVersions'
CREATE TABLE [dbo].[DataBaseVersions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Version] smallint  NOT NULL,
    [ChangeDescription] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'FailuresLogs'
CREATE TABLE [dbo].[FailuresLogs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FailureDateTime] datetime  NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Settings'
CREATE TABLE [dbo].[Settings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CurrentLanguage] int  NOT NULL,
    [AllowBluetooth] bit  NOT NULL,
    [AllowLocalisation] bit  NOT NULL,
    [AllowWifi] bit  NOT NULL,
    [WeightUnit] smallint  NOT NULL,
    [ToiseUnit] smallint  NOT NULL,
    [SelectedUserManualLanguage] int  NOT NULL,
    [LoadCellCalibrationFactor] float  NOT NULL
);
GO

-- Creating table 'Languages'
CREATE TABLE [dbo].[Languages] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [UserManualDocument] nvarchar(max)  NULL,
    [DisplayInGui] bit  NOT NULL,
    [DisplayInUserManual] bit  NOT NULL
);
GO

-- Creating table 'GUIFields'
CREATE TABLE [dbo].[GUIFields] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Text] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Translations'
CREATE TABLE [dbo].[Translations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FieldTranslation] nvarchar(max)  NOT NULL,
    [LanguageId] int  NOT NULL,
    [GUIFieldId] int  NOT NULL
);
GO

-- Creating table 'Errors'
CREATE TABLE [dbo].[Errors] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] bigint  NOT NULL
);
GO

-- Creating table 'ErrorTypes'
CREATE TABLE [dbo].[ErrorTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] smallint  NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ErrorMessages'
CREATE TABLE [dbo].[ErrorMessages] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LanguageId] int  NOT NULL,
    [Message] nvarchar(max)  NOT NULL,
    [SolutionMessage] nvarchar(max)  NOT NULL,
    [Type] smallint  NOT NULL,
    [ErrorCode] bigint  NOT NULL,
    [CryterionMessage] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'BalloonParameters'
CREATE TABLE [dbo].[BalloonParameters] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LowPressureSetpoint] float  NULL,
    [HighPressureSetPoint] float  NULL,
    [LowFlowSetPoint] float  NULL,
    [HighFlowSetPoint] float  NULL,
    [RampUpTimeByStep] float  NULL,
    [PressureRampUpValue] float  NULL,
    [RampDownTimeByStep] float  NULL,
    [PressureRampDownValue] float  NULL,
    [TotalRampUpTime] float  NULL,
    [TotalRampDowntime] float  NULL,
    [StateID] int  NOT NULL,
    [LowTargetInjectionPressure] float  NOT NULL,
    [HighTargetInjectionPressure] float  NOT NULL,
    [DASLowFlow] float  NOT NULL
);
GO

-- Creating table 'TankTypes'
CREATE TABLE [dbo].[TankTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CylinderSize] float  NOT NULL,
    [NetContent] float  NOT NULL,
    [Pressure] float  NOT NULL,
    [CGAConnection] smallint  NOT NULL,
    [Height] float  NOT NULL,
    [OutsideDiameter] float  NOT NULL,
    [Weight] float  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [MetalWeight] float  NOT NULL
);
GO

-- Creating table 'ProcedureLogs'
CREATE TABLE [dbo].[ProcedureLogs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [LogDate] datetime  NOT NULL,
    [PreviousInformation] nvarchar(max)  NOT NULL,
    [CommittedInformation] nvarchar(max)  NOT NULL,
    [ProcedureId] int  NOT NULL
);
GO

-- Creating table 'HardDrives'
CREATE TABLE [dbo].[HardDrives] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [WarningLimit] bigint  NOT NULL,
    [FailLimit] bigint  NOT NULL
);
GO

-- Creating table 'ErrorLog'
CREATE TABLE [dbo].[ErrorLog] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ErrorInformation] nvarchar(max)  NOT NULL,
    [ErrorDate] datetime  NOT NULL
);
GO

-- Creating table 'UserType'
CREATE TABLE [dbo].[UserType] (
    [Users_Id] int  NOT NULL,
    [Types_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'Patients'
ALTER TABLE [dbo].[Patients]
ADD CONSTRAINT [PK_Patients]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Physicians'
ALTER TABLE [dbo].[Physicians]
ADD CONSTRAINT [PK_Physicians]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Ablations'
ALTER TABLE [dbo].[Ablations]
ADD CONSTRAINT [PK_Ablations]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'AblationDatas'
ALTER TABLE [dbo].[AblationDatas]
ADD CONSTRAINT [PK_AblationDatas]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Consoles'
ALTER TABLE [dbo].[Consoles]
ADD CONSTRAINT [PK_Consoles]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'CatheterTypes'
ALTER TABLE [dbo].[CatheterTypes]
ADD CONSTRAINT [PK_CatheterTypes]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SystemStates'
ALTER TABLE [dbo].[SystemStates]
ADD CONSTRAINT [PK_SystemStates]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'CMCRegisterValues'
ALTER TABLE [dbo].[CMCRegisterValues]
ADD CONSTRAINT [PK_CMCRegisterValues]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PMCRegisterValues'
ALTER TABLE [dbo].[PMCRegisterValues]
ADD CONSTRAINT [PK_PMCRegisterValues]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'CatheterInformations'
ALTER TABLE [dbo].[CatheterInformations]
ADD CONSTRAINT [PK_CatheterInformations]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'Procedures'
ALTER TABLE [dbo].[Procedures]
ADD CONSTRAINT [PK_Procedures]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Types'
ALTER TABLE [dbo].[Types]
ADD CONSTRAINT [PK_Types]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserActions'
ALTER TABLE [dbo].[UserActions]
ADD CONSTRAINT [PK_UserActions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Actions'
ALTER TABLE [dbo].[Actions]
ADD CONSTRAINT [PK_Actions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CMCUPIDLogs'
ALTER TABLE [dbo].[CMCUPIDLogs]
ADD CONSTRAINT [PK_CMCUPIDLogs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PMCUPIDLogs'
ALTER TABLE [dbo].[PMCUPIDLogs]
ADD CONSTRAINT [PK_PMCUPIDLogs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'HospitalInformations'
ALTER TABLE [dbo].[HospitalInformations]
ADD CONSTRAINT [PK_HospitalInformations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tanks'
ALTER TABLE [dbo].[Tanks]
ADD CONSTRAINT [PK_Tanks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'preferences'
ALTER TABLE [dbo].[preferences]
ADD CONSTRAINT [PK_preferences]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DataBaseVersions'
ALTER TABLE [dbo].[DataBaseVersions]
ADD CONSTRAINT [PK_DataBaseVersions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FailuresLogs'
ALTER TABLE [dbo].[FailuresLogs]
ADD CONSTRAINT [PK_FailuresLogs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Settings'
ALTER TABLE [dbo].[Settings]
ADD CONSTRAINT [PK_Settings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Languages'
ALTER TABLE [dbo].[Languages]
ADD CONSTRAINT [PK_Languages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GUIFields'
ALTER TABLE [dbo].[GUIFields]
ADD CONSTRAINT [PK_GUIFields]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Translations'
ALTER TABLE [dbo].[Translations]
ADD CONSTRAINT [PK_Translations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Code] in table 'Errors'
ALTER TABLE [dbo].[Errors]
ADD CONSTRAINT [PK_Errors]
    PRIMARY KEY CLUSTERED ([Code] ASC);
GO

-- Creating primary key on [Id] in table 'ErrorTypes'
ALTER TABLE [dbo].[ErrorTypes]
ADD CONSTRAINT [PK_ErrorTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ErrorMessages'
ALTER TABLE [dbo].[ErrorMessages]
ADD CONSTRAINT [PK_ErrorMessages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BalloonParameters'
ALTER TABLE [dbo].[BalloonParameters]
ADD CONSTRAINT [PK_BalloonParameters]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TankTypes'
ALTER TABLE [dbo].[TankTypes]
ADD CONSTRAINT [PK_TankTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProcedureLogs'
ALTER TABLE [dbo].[ProcedureLogs]
ADD CONSTRAINT [PK_ProcedureLogs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'HardDrives'
ALTER TABLE [dbo].[HardDrives]
ADD CONSTRAINT [PK_HardDrives]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id], [ErrorInformation], [ErrorDate] in table 'ErrorLog'
ALTER TABLE [dbo].[ErrorLog]
ADD CONSTRAINT [PK_ErrorLog]
    PRIMARY KEY CLUSTERED ([Id], [ErrorInformation], [ErrorDate] ASC);
GO

-- Creating primary key on [Users_Id], [Types_Id] in table 'UserType'
ALTER TABLE [dbo].[UserType]
ADD CONSTRAINT [PK_UserType]
    PRIMARY KEY CLUSTERED ([Users_Id], [Types_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Users_Id] in table 'UserType'
ALTER TABLE [dbo].[UserType]
ADD CONSTRAINT [FK_UserType_User]
    FOREIGN KEY ([Users_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Types_Id] in table 'UserType'
ALTER TABLE [dbo].[UserType]
ADD CONSTRAINT [FK_UserType_Type]
    FOREIGN KEY ([Types_Id])
    REFERENCES [dbo].[Types]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserType_Type'
CREATE INDEX [IX_FK_UserType_Type]
ON [dbo].[UserType]
    ([Types_Id]);
GO

-- Creating foreign key on [UserId] in table 'UserActions'
ALTER TABLE [dbo].[UserActions]
ADD CONSTRAINT [FK_UserUserAction]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUserAction'
CREATE INDEX [IX_FK_UserUserAction]
ON [dbo].[UserActions]
    ([UserId]);
GO

-- Creating foreign key on [GUIFieldId] in table 'Translations'
ALTER TABLE [dbo].[Translations]
ADD CONSTRAINT [FK_GUIFieldTranslation]
    FOREIGN KEY ([GUIFieldId])
    REFERENCES [dbo].[GUIFields]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_GUIFieldTranslation'
CREATE INDEX [IX_FK_GUIFieldTranslation]
ON [dbo].[Translations]
    ([GUIFieldId]);
GO

-- Creating foreign key on [ErrorCode] in table 'ErrorMessages'
ALTER TABLE [dbo].[ErrorMessages]
ADD CONSTRAINT [FK_ErrorErrorMessage]
    FOREIGN KEY ([ErrorCode])
    REFERENCES [dbo].[Errors]
        ([Code])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ErrorErrorMessage'
CREATE INDEX [IX_FK_ErrorErrorMessage]
ON [dbo].[ErrorMessages]
    ([ErrorCode]);
GO

-- Creating foreign key on [PhysicianID] in table 'Patients'
ALTER TABLE [dbo].[Patients]
ADD CONSTRAINT [FK_PhysicianPatient]
    FOREIGN KEY ([PhysicianID])
    REFERENCES [dbo].[Physicians]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PhysicianPatient'
CREATE INDEX [IX_FK_PhysicianPatient]
ON [dbo].[Patients]
    ([PhysicianID]);
GO

-- Creating foreign key on [CatheterTypeID] in table 'CatheterInformations'
ALTER TABLE [dbo].[CatheterInformations]
ADD CONSTRAINT [FK_CatheterTypeCatheterInformation]
    FOREIGN KEY ([CatheterTypeID])
    REFERENCES [dbo].[CatheterTypes]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CatheterTypeCatheterInformation'
CREATE INDEX [IX_FK_CatheterTypeCatheterInformation]
ON [dbo].[CatheterInformations]
    ([CatheterTypeID]);
GO

-- Creating foreign key on [CatheterTypeID] in table 'CMCRegisterValues'
ALTER TABLE [dbo].[CMCRegisterValues]
ADD CONSTRAINT [FK_CatheterTypeCMCRegisterValue]
    FOREIGN KEY ([CatheterTypeID])
    REFERENCES [dbo].[CatheterTypes]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CatheterTypeCMCRegisterValue'
CREATE INDEX [IX_FK_CatheterTypeCMCRegisterValue]
ON [dbo].[CMCRegisterValues]
    ([CatheterTypeID]);
GO

-- Creating foreign key on [PatientID] in table 'Procedures'
ALTER TABLE [dbo].[Procedures]
ADD CONSTRAINT [FK_PatientProcedure]
    FOREIGN KEY ([PatientID])
    REFERENCES [dbo].[Patients]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PatientProcedure'
CREATE INDEX [IX_FK_PatientProcedure]
ON [dbo].[Procedures]
    ([PatientID]);
GO

-- Creating foreign key on [ProcedureId] in table 'Ablations'
ALTER TABLE [dbo].[Ablations]
ADD CONSTRAINT [FK_ProcedureAblation]
    FOREIGN KEY ([ProcedureId])
    REFERENCES [dbo].[Procedures]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProcedureAblation'
CREATE INDEX [IX_FK_ProcedureAblation]
ON [dbo].[Ablations]
    ([ProcedureId]);
GO

-- Creating foreign key on [ProcedureId] in table 'ProcedureLogs'
ALTER TABLE [dbo].[ProcedureLogs]
ADD CONSTRAINT [FK_ProcedureProcedureLog]
    FOREIGN KEY ([ProcedureId])
    REFERENCES [dbo].[Procedures]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProcedureProcedureLog'
CREATE INDEX [IX_FK_ProcedureProcedureLog]
ON [dbo].[ProcedureLogs]
    ([ProcedureId]);
GO

-- Creating foreign key on [preference_Id] in table 'Physicians'
ALTER TABLE [dbo].[Physicians]
ADD CONSTRAINT [FK_Physicianpreference]
    FOREIGN KEY ([preference_Id])
    REFERENCES [dbo].[preferences]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Physicianpreference'
CREATE INDEX [IX_FK_Physicianpreference]
ON [dbo].[Physicians]
    ([preference_Id]);
GO

-- Creating foreign key on [CatheterTypeID] in table 'PMCRegisterValues'
ALTER TABLE [dbo].[PMCRegisterValues]
ADD CONSTRAINT [FK_CatheterTypePMCRegisterValue]
    FOREIGN KEY ([CatheterTypeID])
    REFERENCES [dbo].[CatheterTypes]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CatheterTypePMCRegisterValue'
CREATE INDEX [IX_FK_CatheterTypePMCRegisterValue]
ON [dbo].[PMCRegisterValues]
    ([CatheterTypeID]);
GO

-- Creating foreign key on [ConsoleID] in table 'Tanks'
ALTER TABLE [dbo].[Tanks]
ADD CONSTRAINT [FK_ConsoleTank]
    FOREIGN KEY ([ConsoleID])
    REFERENCES [dbo].[Consoles]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ConsoleTank'
CREATE INDEX [IX_FK_ConsoleTank]
ON [dbo].[Tanks]
    ([ConsoleID]);
GO

-- Creating foreign key on [HardDrive_Id] in table 'Consoles'
ALTER TABLE [dbo].[Consoles]
ADD CONSTRAINT [FK_ConsoleHardDrive]
    FOREIGN KEY ([HardDrive_Id])
    REFERENCES [dbo].[HardDrives]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ConsoleHardDrive'
CREATE INDEX [IX_FK_ConsoleHardDrive]
ON [dbo].[Consoles]
    ([HardDrive_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------