namespace Console
{
    /// <summary>
    /// Represents  the calibration components and  CPLD Registers
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class CalibrationComponentANDCPLDRegister
    {
        /// <summary>
        /// Calibration component ID enumeration
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum CalibrationComponentId
        {
            CMCU_Thermal_Couple = 1,
            CMCU_Load_Cell = 2,
            PMCU_Thermal_Couple_1 = 4,
            PMCU_Thermal_Couple_2 = 8,
        }

        /// <summary>
        /// Solenoid valve fan enumeration
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum CPLDRegisterId
        {
            SV1 = 1,
            SV2 = 2,
            SV3 = 4,
            SV4 = 8,
            SV5 = 16,
            SV6 = 32,
            Sv7 = 64,
            SV8 = 128,
            SV9 = 256,
            FAN = 512,
            SV10 = 1024,
            SV11 = 2048,
        }

        /// <summary>
        /// Heart beat status enumeration
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum HeartbeatStatus
        {
            GUI_is_Ready = 1,
            GUI_in_Maintenance_Mode = 2,
            No_Error_Report_Mode = 4,
            GUI_In_Test_Mode = 8,
            Enhanced_Audio = 16,
            NOT_USED_32 = 32,
            NOT_USED_64 = 64,
            NOT_USED_128 = 128,
            PID_MANUAL_MODE = 256,
            PRESSURE_FLOW_MODE = 512,
            AUTO_DEFLATION = 1024,
            SLOW_FAST_INFLATION = 2048,
            DIAPHRAGM_ESOPHAGUS_AUDIO_ALERTS = 4096, // 0x1000
            SYSTEM_PURGE = 8192, // 0x2000
            FOOT_SWITCH = 16384 , // 0x4000
            DEACTIVATE_FEATURES = 32768 // 0x8000
        }

        /// <summary>
        /// CAN 2 heart beat status
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum CanTwoHeartbeatStatus
        {
            CONSOLE_IS_IN_ABLATION_STATE= 1,
            VITAL_PARAMETERS = 2

        }
    }
}