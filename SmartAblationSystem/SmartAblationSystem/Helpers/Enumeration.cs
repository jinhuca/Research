using System.ComponentModel;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class contains Enumerations
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class Enumeration
    {
        /// <summary>
        /// Constructor that initialize enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Enumeration()
        {

        }
        /// <summary>
        /// Tank weight enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum TankWeight
        {
            THE_TANK_WEIGHT_IS_LOW = 0,
            THE_TANK_WEIGHT_IS_TOO_LOW = 1,
            THE_TANK_WEIGHT_IS_OF_BOUNDS = 2,
            THE_TANK_WEIGHT_IS_IN_BOUNDS = 3,
        }

        /// <summary>
        /// Screen id enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum ScreenID
        {
            CHANGE_TANK = 0,
            SHUT_DOWN = 1,
            CRYO_THERAPY = 2,
            RECORDS = 3,
            MAINTENANCE = 4,
        }

        /// <summary>
        /// Catheter type enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum CatheterType
        {
            ID_UNKNOWN_mm = 0,
            ID28mm = 1,
            Plus = 2,
        }

        /// <summary>
        /// Actions enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum Actions
        {
            Login = 1,
            Logout = 2,
            StartCommand = 3,
            StopCommand = 4,
            CreateProcedure = 5,
            CreateUser = 6,
            EditUser = 7,
            DeleteUser = 8,
            ResetPassword = 9,
            AccessRecord = 10,
            AccessChangeTank = 11,
            AccessSettings = 12,
            AccessManageUsers = 13,
            AccessDateAndTime = 14,
            AccessMaintenance = 15,
            AccessSiteSetup = 16,
            AccessPIDControl = 17,
            AccessElectricalMonitoring = 18,
            AccessLoadCellCalibration = 19,
            AccessSystemFiles = 20,
            AccessCatheterDatabase = 21,
            AccessMechanicalMonitoring = 22,
            AccessFlowCurveProgramming = 23,
            LoadFirmwareVersionCommand = 24,
            AppModeCommand = 25,
            DiaphragmReset = 26,
						DeleteProcedure = 27
        }

        /// <summary>
        /// Tank states enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum TankStates
        {
            Tank_Opened = 0,
            Tank_Closing = 1,
            Tank_Closed = 2,
            Tank_Purging = 3,
            Tank_Purged = 4,
            TanK_Removing = 5,
            Tank_Removed = 6,
            Tank_Placing = 7,
            Tank_Replacing = 8,
            Tank_Replaced = 9,
            Tank_Openning = 10
        }

        /// <summary>
        /// Tank type enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum TankType
        {
            Unknown = 0,
            Tank_10pounds = 1,
            Tank_15pounds = 2
        }

        /// <summary>
        /// Cmcu Sorting enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum CmcuSorting
        {
            Unknown = 0,
            TargetFlowError = 1,
            TargetInjectionPressureError = 2
        }

        /// <summary>
        /// Pmcu Sorting enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum PmcuSorting
        {
            Unknown = 0,
            TargetBallonPressureError = 1,
        }


        /// <summary>
        /// Serie color enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum SerieColor
        {
            Unknown = 0,
            Blue = 1,
            Yellow = 2,
            Green = 3,
            White = 4,
        }

        /// <summary>
        /// Curve style enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum CurveStyle
        {
            Unknown = 0,
            Line = 2,
            Area = 3,
        }

        /// <summary>
        /// Regrigerant Level enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum RefrigerantUnit
        {
            Lbs = 0,
            Minute = 1,
            Ablations = 2
        }

        /// <summary>
        /// Weight unit enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum WeightUnit
        {
            Unknown = 0,
            Lbs = 1,
            Kg = 2
        }

        /// <summary>
        /// Length unit enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum LengthUnit
        {
            Unknown = 0,
            Inches = 1,
            Centimeters = 2

        }

        /// <summary>
        /// Firmware enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum Firmware
        {
            Unknown = 0,
            CMCU = 1,
            PMCU = 2,
            RMCU = 3,
            CPLD = 4,
            BMCU = 5,
            RCMCU = 6
            // Catheter = 6,

        }

        /// <summary>
        /// GUI messages enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum GUIMessages
        {
            [Description("Load cell error")]
            ID1 = 26081,
            [Description("Do you want to reset")]
            ID2 = 26082,
            [Description("System Error")]
            ID3 = 26083,
            [Description("Ablation Writing Error")]
            ID4 = 26084,
            [Description("Ablation ECG Writing Error")]
            ID5 = 26085,
            [Description("Stop wrinting to JSON file")]
            ID6 = 26086,
            [Description("Treatment Loading Error")]
            ID7 = 26087,
            [Description("Please Select A Registetr")]
            ID8 = 26088,
            [Description("Are you sure that you want to close the software?")]
            ID9 = 26089,
            [Description("Are you sure that you want to shutdown your computer now?")]
            ID10 = 260810,
            [Description("Are you sure you want to end the procedure ?")]
            ID11 = 260811,
            [Description("You don't have sufficient privileges to access the Settings.")]
            ID12 = 260812,
            [Description("Are you sure you want to end the procedure without adding any out come notes?")]
            ID13 = 260813,
            [Description("Are you sure you want to quit the procedure?")]
            ID14 = 260814,
            [Description("Are you sure you want to logout from the system?")]
            ID15 = 260815,
            [Description("The username :")]
            ID16 = 260816,
            [Description("already exists!")]
            ID17 = 260817,
            [Description("User Name Exists")]
            ID18 = 260818,
            [Description("already exists but the user is inactive.\n\n Do you want to reactivate it ?")]
            ID19 = 260819,
            [Description("Re-Activate User ?")]
            ID20 = 260820,
            [Description("The physician's name :")]
            ID21 = 260821,
            [Description("Physician Exists")]
            ID22 = 260822,
            [Description("Do you really want to delete the user :")]
            ID23 = 260823,
            [Description("Delete User")]
            ID24 = 260824,
            [Description("The current password for :")]
            ID25 = 260825,
            [Description("is not valid!")]
            ID26 = 260826,
            [Description("Password Invalid")]
            ID27 = 260827,
            [Description("Could not retrieve the selected Physician!")]
            ID28 = 260828,
            [Description("Physician not found")]
            ID29 = 260829,
            [Description("A physician must be selected!")]
            ID30 = 260830,
            [Description("Physician Missing")]
            ID31 = 260831,
            [Description("The patient's birth date is not valid!")]
            ID32 = 260832,
            [Description("Invalid Date")]
            ID33 = 260833,
            [Description("This patient ID already exists in the database!")]
            ID34 = 260834,
            [Description("Patient Already Exist")]
            ID35 = 260835,
            [Description("An error occurred while inserting a new Patient in the database!")]
            ID36 = 260836,
            [Description("Patient Insertion Error")]
            ID37 = 260837,
            [Description("The Physician could not be retrieved!")]
            ID38 = 260838,
            [Description("An error occurred while creating the ablation procedure!")]
            ID39 = 260839,
            [Description("Procedure Creation Error")]
            ID40 = 260840,
            [Description("An error occurred while generating the USB drive list!")]
            ID41 = 260841,
            [Description("USB Drive List error")]
            ID42 = 260842,
            [Description("The specified path is invalid or cannot be found!")]
            ID43 = 260843,
            [Description("Engineering Data Not Saved!")]
            ID44 = 260844,
            [Description("Access denied.  You don't have access to the specified path!")]
            ID45 = 260845,
            [Description("The specified path is invalid!")]
            ID46 = 260846,
            [Description("The specified path is invalid!  An unsupported character has been detected.")]
            ID47 = 260847,
            [Description("Target file or directory does not exist anymore!")]
            ID48 = 260848,
            [Description("An error occurred while saving the engineering data files to the USB drive!")]
            ID49 = 260849,
            [Description("An error occurred while saving the engineering data files to the USB drive!")]
            ID50 = 260850,
            [Description("The engineering data files have been saved to USB drive successfully!")]
            ID51 = 260851,
            [Description("Engineering Data Saved Successfully!")]
            ID52 = 260852,
            [Description("An error occurred while generating the USB drive list!")]
            ID53 = 260853,
            [Description("USB Drive List error")]
            ID54 = 260854,
            [Description("An error occurred while saving the procedure's outcome to the database!")]
            ID55 = 260855,
            [Description("Error Saving Outcome")]
            ID56 = 260856,
            [Description("An error occurred while saving the procedure's diagnosis to the database!")]
            ID57 = 260857,
            [Description("Error Saving Diagnosis")]
            ID58 = 260858,
            [Description("The hospital informations are not valid")]
            ID59 = 260859,
            [Description("An error occurred while generating the USB drive list!")]
            ID60 = 260860,
            [Description("USB Drive List error")]
            ID61 = 260861,
            [Description("An error occurred while generating the procedure records list!")]
            ID62 = 260862,
            [Description("Procedure Records Error")]
            ID63 = 260863,
            [Description("The procedure has been saved to USB drive successfully!")]
            ID64 = 260864,
            [Description("Procedure Saved Successfully!")]
            ID65 = 260865,
            [Description("The specified path is invalid or cannot be found!")]
            ID66 = 260866,
            [Description("Procedure Not Saved!")]
            ID67 = 260867,
            [Description("Access denied.  You don't have access to the specified path!")]
            ID68 = 260868,
            [Description("The specified path is invalid!")]
            ID69 = 260869,
            [Description("The specified path is invalid!  An unsupported character has been detected.")]
            ID70 = 260870,
            [Description("Target file or directory does not exist anymore!")]
            ID71 = 260871,
            [Description("An error occurred while saving the procedure to the USB drive!")]
            ID72 = 260872,
            [Description("Do you really want to clear the warning messages list ?")]
            ID73 = 260873,
            [Description("Clear System Notification List")]
            ID74 = 260874,
            [Description("An error occurred while updating tip/balloon pressure charts!")]
            ID75 = 260875,
            [Description("Tip/Balloon Pressure Chart Error")]
            ID76 = 260876,
            [Description("An error occurred while Loading on charts!")]
            ID77 = 260877,
            [Description("Temperature/Diaphragm Movement Chart Error")]
            ID78 = 260878,
            [Description("An error occured while trying to display the treatment notes.")]
            ID79 = 260879,
            [Description("Treatment Notes Error")]
            ID80 = 260880,
            [Description("An error occurred during ablation")]
            ID81 = 260881,
            [Description("CAN1 Communication")]
            ID82 = 260882,
            [Description("CAN2 Communication")]
            ID83 = 260883,
            [Description("This language is not supported in this version yet.")]
            ID84 = 260884,
            [Description("Please restart the system to apply new language settings.")]
            ID85 = 260885,
            [Description("Please restart the system to apply new language settings.")]
            ID86 = 260886,
            [Description("Please wait two seconds before stating an ablation.")]
            ID87 = 260887,
            [Description("DAS Balloon Error")]
            ID88 = 260888,
            [Description("You are running out of disk space. Contact customer service")]
            ID89 = 260889,
            [Description("Disk Error")]
            ID90 = 260890,
            [Description("Disk space warning")]
            ID91 = 260891,
            [Description("Disk warning")]
            ID92 = 260892,
            [Description("Please Wait Until The Temperature Reach 20 °C, To Change Balloon Seize")]
            ID93 = 260893,
            [Description("System Is Using DAS Balloon")]
            ID94 = 260894,
            [Description("Select a firmware to Load")]
            ID95 = 260895,
            [Description("Selection Error")]
            ID96 = 260896,
            [Description("Hex File Not Found!")]
            ID97 = 260897,
            [Description("File Error")]
            ID98 = 260898,
            [Description("Source File or Directory Not Found!")]
            ID99 = 260899,
            [Description("USB Path or File Error!")]
            ID100 = 260900,
            [Description("You Cannot Select more than 20 records.")]
            ID101 = 260901,
            [Description("Record ERROR")]
            ID102 = 260902,
            [Description("There is not enough space to archive data")]
            ID103 = 260903,
            [Description("Are you sure that you want to archive? After archive you will not be able to access the procedure data !")]
            ID104 = 260904,
            [Description("No data to archive")]
            ID105 = 260905,
            [Description("Archive failed")]
            ID106 = 260906,
            [Description("Archive successful")]
            ID107 = 260907,
            [Description("You have to choose at least one from procedure records list please")]
            ID108 = 260908,
            [Description("An error occurred while printing. It might be no print diver was setup on this machine.  Contact Boston Scientific technical support please")]
            ID109 = 260909,
            [Description("System Exception: Cryo-Cable")]
            ID110 = 268435456,
            [Description("An error occurred while loading the treatment file in memory. Please exit to the Home screen and then return to the Therapy screen.")]
            ID111 = 260910,
            [Description("An error occurred while retrieving the procedure record!")]
            ID112 = 260911,
            [Description("The GUI Is Freezing")]
            ID113 = 260912,
            [Description("password has been reset")]
            ID114 = 260913,
            //[Description("The system has detected multiple remote control buttons pressed. This may be due to a stuck button. The remote control commands will not be functional until all buttons have been successfully released.")]
            //ID115 =260914,

        }

        /// <summary>
        /// Error types enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum ErrorTypes
        {
            Unknown = 0,
            CMCU = 1,
            PMCU = 2,
            GUI = 3

        }
        /// <summary>
        /// Data progress states value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum DataProgressStates
        {
            Unknown = 0,
            STARTING = 1,
            GENERATING_DATA = 2,
            CONVERTING_TOXLS = 3,
            ENDING = 4
        }
        /// <summary>
        /// Catheter blood errors types
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>  
        public enum CatheterBloodErrorsTypes //To do change the naming of the type
        {
            Unknown = 0,
            BloodDetected = 1,
            BrokenWire = 2,
        }

        /// <summary>
        /// Ablation duration type
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum AblationDurationType
        {
            Unknown = 0,
            FixedTimer = 1,
            TTIFixedTimer = 2,
            TTIDurationTimer = 3,

        }
        /// <summary>
        /// Recycle flags
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum RecycleFlags : uint
        {
            SHERB_NOCONFIRMATION = 0x00000001,
            SHERB_NOPROGRESSUI = 0x00000002,
            SHERB_NOSOUND = 0x00000004
        }

        /// <summary>
        /// LS Pro Console Status
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum LSPROConsoleStatus
        {
            stop = 0,
            start = 1,
        }

        public enum InflationSpeedMode
        {
          Fast = 0,
          Slow = 1
        }

        public enum GenderType : short
        {
          Female = 0,
          Male = 1,
          None = -1
        }
    }
}