namespace Modules.Infrastructure.Definitions;

/// <summary>
/// Can bus IDs
///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
/// </summary>
public enum CanBusMessageIdentifier
{
  // Central Micro Controller: Read Values
  CentralMicroControllerPT1PT2PT3PT4Reading = 0 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),

  CentralMicroControllerPS1PS2Reading = 1 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerFM1Reading = 2 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerTS1Reading = 3 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerLC1Reading = 4 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),

  // Central Micro Controller: Register Values
  CentralMicroControllerFirmwareVersion = 8 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),

  CentralMicroControllerCPLDErrorRegister = 9 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerCPLDValveRegister = 10 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerCPLDSystemRegister = 11 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerSystemState = 12 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerAblationTime = 13 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerContinuousThawing = 14 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerTargetInjectionFlow = 15 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerPIDParameter = 16 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerPT1Threshold = 17 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerPT1Range = 18 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerPT2Threshold = 19 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerPT2Range = 20 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerPT3Threshold = 21 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerPT3Range = 22 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerPT4Threshold = 23 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerPT4Range = 24 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerTSThreshold = 25 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerTSRange = 26 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerFM1Threshold = 27 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerFM1Range = 28 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerPS1Threshold = 29 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerPS1Range = 30 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerPS2Threshold = 31 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerPS2Range = 32 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerLC1Threshold = 33 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerLC1Range = 34 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerSatutsAndErrorData = 35 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),


  //Boot register value CMCU
  CentralMicroControllerRTRBootData = ((ushort)BootLoaderID.CAN_ID_BOOT_XFR) | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerRTRBootDataStart = ((ushort)BootLoaderID.CAN_ID_BOOT_START) | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerRTRBootDataInit = ((ushort)BootLoaderID.CAN_ID_BOOT_INIT) | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),
  CentralMicroControllerRTRBootDataEnd = ((ushort)BootLoaderID.CAN_ID_BOOT_END) | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),

  //Boot register value PMCU
  PatientMicroControllerRTRBootData = ((ushort)BootLoaderID.CAN_ID_BOOT_XFR) | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),
  PatientMicroControllerRTRBootDataStart = ((ushort)BootLoaderID.CAN_ID_BOOT_START) | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),
  PatientMicroControllerRTRBootDataInit = ((ushort)BootLoaderID.CAN_ID_BOOT_INIT) | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),


  //Boot register value RMCU
  RepeaterMicroControllerRTRBootData = ((ushort)BootLoaderID.CAN_ID_BOOT_XFR) | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),
  RepeaterMicroControllerRTRBootDataStart = ((ushort)BootLoaderID.CAN_ID_BOOT_START) | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),
  RepeaterMicroControllerRTRBootDataInit = ((ushort)BootLoaderID.CAN_ID_BOOT_INIT) | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),


  CentralMicroControllerCPLDRegisterValvesState = 36 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),


  // Patient Micro Controller: Read Valuessi
  PatientMicroControllerTC1TC2Reading = 40 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),

  PatientMicroControllerCP1CP2TipReading = 41 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),
  PatientMicroControllerCIMP1CIMP2Reading = 42 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),

  // Patient Micro Controller: Register Values
  PatientMicroControllerFirmwareVersion = 48 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),

  PatientMicroControllerStatusAndErrorCode = 49 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),

  PatientMicroControllerCatheterExtenedCatheterSNAndLotNumber = 5 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),

  PatientMicroControllerCatheterIDAndSerialNumberAndFirmwareVersion = 50 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),

  PatientMicroControllerCatheterExpirationDateLastUseDateNumberOfInjections = 51 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),

  PatientMicroControllerTargetBalloonPressure = 52 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),
  PatientMicroControllerCP1CP2Threshold = 53 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),

  PatientMicroControllerTCT1CT2Threshold = 54 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),
  PatientMicroControllerPIDParameter = 55 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),
  CatheterFirmware = 56 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),
  PatientMicroControllerBallonSizeConfiguration = 57 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.patientMicrocontroller << 11),

  // Single Board Computer Register Values normal message
  SingleBoardComputerRegisterValues = ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.singleBoardComputer << 11) | ((ushort)Priority.normal << 14),

  CentralMicroControllerLoadCellCalibration = 62 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.mainMicrocontroller << 11),

  // Connection Box Micro Controller: Read Values

  BloodPressureSensorConnection = 1 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),

  ProbeFirstGroupsensors = 5 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),

  ProbeSecondGroupsensors = 6 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),

  BloodPressureSensorData = 7 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),



  //channel 0 is for 1-2,    channel 1 is for 3-4,     channel 2 is for 5-6,     channel 3  is for 7-8
  EcgChannel1And2Channel3And4Channel5And6Channel7And8 = 8 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),

  // channel 4 is for 9-10,     channel 5 is tip, channel 6 is Accelerometer
  EcgChannel9And10ChannelTipChannelAccelerometer = 9 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),

  RepeaterFirmwareAndICBFirmware = 11 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),


  RemoteControlFirmware = 24 | ((ushort)MessageType.registerRomValue << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),

  //Membrane Switch State
  MembraneSwitchState = 26 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),

  //Remote Control Heartbeat
  RemoteControlHeartbeat = 28 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),

  //Remote Control Heartbeat
  RemoteControlSystemState = 31 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11),

  //High resolution DMS message 0x20
  HighResolutionECGChannel = 32 | ((ushort)MessageType.readValues << 6) | ((ushort)MessageNodeId.canIdNodeConnBus2 << 11)
}