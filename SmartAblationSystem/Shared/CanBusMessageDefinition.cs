// <copyright file="CanBusMessageDefinition.cs" company=" Cryterion Medical Inc.  ">
// Copyright (c) Cryterion Medical Inc. All rights reserved.
// </copyright>
// <author>Alex Smail</author>
// <date>01-18-2017</date>
// <summary> Define the  Can Bus Message </summary>

using System;

namespace Communication
{
    /// <summary>
    /// Defines the CAN bus message
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class CanBusMessageDefinition
    {
        /// <summary>
        /// Frame type data or RTR
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum FrameType
        {
            Data = 4,
            Remote = 5 //RTR
        }

        /// <summary>
        /// Boot loader ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum BootLoaderID
        {
            CAN_ID_BOOT_XFR = 58,
            CAN_ID_BOOT_START = 59,
            CAN_ID_BOOT_INIT = 60,
            CAN_ID_BOOT_END = 61
        }

        /// <summary>
        /// Module keys
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum ModuleKeys : Int64
        {
            CMCUKey = 49374, //0xC0DE
            CPLDKey = 50398, //0xC4DE
            PMCUKey = 50910, //0xC6DE 
            RMCUKey = 51422, //0xC8DE
            RCMCUKey = 51934, // 0xCADE
            BMCUKey = 52446, // 0xCCDE     
            CMCUREBOOT = 45057, //B001
            PMCUREBOOT = 45058, //B002
            RMCUREBOOT = 45059, //B003
            BMCUREBOOT = 45060 // B004
        }

        /// <summary>
        /// CPLD and CMCU status key
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum CPLDStatusKey : Int64
        {
            CMCUPASS = 44033,  //AC01 the data are inverted 
            CMCUPASSINTERMEDAIREITERMEDIARYPASS = 44034, //AC02 the data are inverted 
            CMCUANDCPLDPASS = 44035, //AC03 the data are inverted 
            CMCUFAIL = 48385, //0xBD01
            INTERMEDAIREITERMEDIARYFAIL = 48386, //0xBD02
            CMCUANDCPLDFAIL = 48387, //0xBD03

        }

        /// <summary>
        /// Switch State enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum SwitchState
        {
            Unknown = 0,
            SwitchStateDeactivated = 255,
            AblationTimerDecrement = 254,
            AblationTimerIncrement = 253,
            AblationSiteLeft = 251,
            StartButton = 247,
            StopButton = 239,
            AblationSiteRight = 223,
            BalloonDiameterIncrease = 191,
            BalloonDiameterDecrease = 127

        }

        /// <summary>
        /// Console state
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum MessageStateId
        {
            CAN_ID_STATE_UNKNOWN = 0,
            CAN_ID_STATE_IDLE = 256,
            CAN_ID_STATE_READY = 512,
            CAN_ID_STATE_INFLATION = 768,
            CAN_ID_STATE_TRANSITION = 1024,
            CAN_ID_STATE_ABLATION = 1280,
            CAN_ID_STATE_THAWING = 1536,
            CAN_ID_STATE_EXCEPTION = 1792
        }

        /// <summary>
        /// Message mask
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum Mask
        {
            CAN_ID_ELEMENT_MASK = 63, // 0x003f
            CAN_ID_NODE_MASK = 14336, // 0x3800
            CAN_ID_TYPE_MASK = 192,   // 0x00c0
            CAN_ID_STATE_MASK = 1792 //0x0700
        }

        /// <summary>
        /// Message node ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum MessageNodeId
        {
            // patient MCU
            patientMicrocontroller = 0,

            // central MCU
            mainMicrocontroller = 1, //0x0800

            // Single Board Computer
            singleBoardComputer = 2, // 0x1000

            //From Connection Box CAN node
            canIdNodeConnBus2 = 3,

            //From Single Board Computer CAN2 node
            canIdNodeSbcBus2 = 4
        }

        /// <summary>
        /// Message type register or value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum MessageType
        {
            readValues = 0,
            registerRomValue = 1, // 0x00c0
        }

        /// <summary>
        /// Message priority
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum Priority
        {
            error = 0,
            warning = 1,
            attention = 2,
            normal = 3
        }

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

        /// <summary>
        /// Central microcontroller errors
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        [Flags]
        public enum CMCUStatusError : Int64
        {
            ExceptionType1 = 536870912, //0x20000000

            ExceptionType2 = 1073741824, // 0x40000000

            ExceptionType3 = 1610612736, // 0x60000000

            ExceptionType4 = 2147483648,  // 0x80000000

            ExceptionType5 = 268435456,  // 0x10000000

            CPLDWatchDogTimerError = 1, // 0x00000001

            TwoMultiplexReadingDoesNotMatch = 2, // 0x00000002

            FlowTooHigh = 4, // 0x00000004

            FlowTooLow = 8, // 0x00000008

            FlowReadingOutOfRange = 16, // 0x00000010

            LoadCellWeightWarning = 32, // 0x00000020

            LoadCellWeightFail = 64, // 0x00000040

            LoadCellReadingOutOfRange = 128, // 0x00000080

            PressureInTankIsHighFanToBeOn = 256, // 0x00000100

            PressurePT1InTankIsLow = 512, // 0x00000200

            PressurePT1InTankIsTooHigh = 1024, // 0x00000400

            PressurePT1InTankReadingOutOfRange = 2048, // 0x00000800

            PressurePT2AfterCatheterButBeforeReturnLineTooHigh = 4096, // 0x00001000

            PT2ReadingOutOfRange = 8192, //0x00002000

            ReturnPressurePT3TooHigh = 16384, //0x00004000

            ReturnPressurePT3OutOfRange = 32768, //0x00008000

            VacuumPressurePT4TooHigh = 65536, //0x00010000

            VacuumPressurePT4OutOfRange = 131072, //0x00020000

            SubCoolerTemperatureIsHigh = 262144, //0x00040000

            SubCoolerTemperatureOutOfRange = 524288, // 0x00080000

            InjectionVentPressureIsHigh = 1048576, // 0x00100000

            InjectionVentPressureOutOfRange = 2097152, // 0x00200000

            ScavengingPressureIsHigh = 4194304, // 0x00400000

            CatheterTubeConnected = 33554432, // 0x02000000

            SelfTestFail = 67108864,  // 0x04000000

            FootSwitchLock = 8388608, // 0x00800000

            VeinIsolated = 16777216,               //0x01000000

            CMCUReady = 134217728, // 0x08000000
        }

        /// <summary>
        /// Patient microcontroller errors
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        [Flags]
        public enum PMCUStatusError : Int64
        {
            ExceptionType1 = 536870912, //0x20000000

            ExceptionType2 = 1073741824, // 0x40000000

            ExceptionType3 = 1610612736, // 0x60000000

            ExceptionType4 = 2147483648,  // 0x80000000

            ExceptionType5 = 268435456,  // 0x10000000

            CPLDWatchDogTimerError = 1, // 0x00000001

            SelfTestFail = 2,  //0x00000002

            InnerBalloonPressureTooHigh = 4, // 0x00000004

            InnerBalloonPressureTooLow = 8, //0x00000008 

            //InnerBalloonPressureReadingOutOfRange = 8, //0x00000008

            BalloonTemperatureLowWarning = 16, //0x00000010

            OuterBalloonPressureTooHigh = 32, //0x00000020

            OuterBalloonPressureReadingOutOrRange = 64, //0x00000040

            BalloonTipPressureTooHigh = 128, //0x00000080

            BalloonTipPressureTooLow = 256, //0x00000100

            BalloonTipPressurePeadingOutOfRange = 512, //0x00000200

            ThawingTemperatureTooHigh = 1024, //0x00000400

            ThawingTemperatureTooLow = 2048, //0x00000800

            BalloonTemperatureTooHigh = 4096, // 0x0001000

            BloodDetectedInCatheter = 16384, // 0x0004000

            BloodDetectorOpenWires = 32768, // 0x00008000

            CatheterCableConnected = 16777216, //0x01000000

            //SelfTestFail = 67108864,  // 0x04000000

            PMCUReady = 134217728, // 0x08000000
        }

        /// <summary>
        /// Central microcontroller valves status
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum CMCUValvesStatus : Int64
        {
            SolenoidValve1ON = 1,

            SolenoidValve2ON = 2,

            SolenoidValve3ON = 4,

            SolenoidValve4ON = 8,

            SolenoidValve5ON = 16,

            SolenoidValve6ON = 32,

            SolenoidValve7ON = 64,

            SolenoidValve8ON = 128,

            SolenoidValve9ON = 256,

        }

        /// <summary>
        /// Central microcontroller valves status
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        [Flags]
        public enum CPLDFPINStatus : byte
        {
            NStopFootSwitch = 1, //0x01

            StopFootSwitch = 2, //0x02

            NStartFootSwitch = 4, //0x04

            StartFootSwitch = 8, //0x08

            NStopButton = 16,   //0x10 

            StopButton = 32,    //0x20

            NStartButton = 64,  //0x40

            StartButton = 128,  //0x80
        }

        public enum SensorConnectionStatus : uint
        {
            DMS = 1,

            Pressure = 2,

            ETSSingle = 4,

            ETSMulti = 8,

        }
    }
}