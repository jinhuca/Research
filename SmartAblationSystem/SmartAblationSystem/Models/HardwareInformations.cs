using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class is for hardware information Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class HardwareInformations
    {
        private string cMCUFirmware = string.Empty;
        private string pMCUFirmware = string.Empty;
        private string repeaterFirmware = string.Empty;
        private string iCBFirmware = string.Empty;
        private string catheterFirmware = string.Empty;
        private string cPLDFirmware = string.Empty;
        private string consoleSerialNumber = string.Empty;
        private string remoteFirmware = string.Empty;
        /// <summary>
        /// Initializes a new instance hardware informations
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public HardwareInformations()
        {

        }
        /// <summary>
        /// Gets/sets CMCU Firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string CMCUFirmware
        {
            get => cMCUFirmware;
            set => cMCUFirmware = value;
        }
        /// <summary>
        /// Gets/sets PMCU Firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string PMCUFirmware
        {
            get => pMCUFirmware;
            set => pMCUFirmware = value;
        }

        /// <summary>
        /// Gets/sets Repeater Firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string RepeaterFirmware
        {
            get => repeaterFirmware;
            set => repeaterFirmware = value;
        }

        /// <summary>
        /// Gets/sets ICB Firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ICBFirmware
        {
            get => iCBFirmware;
            set => iCBFirmware = value;
        }

        /// <summary>
        /// Gets/sets Catheter Firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string CatheterFirmware
        {
            get => catheterFirmware;
            set => catheterFirmware = value;
        }

        /// <summary>
        /// Gets/sets CPLD Firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string CPLDFirmware
        {
            get => cPLDFirmware;
            set => cPLDFirmware = value;
        }

        /// <summary>
        /// Gets/sets console serial number
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ConsoleSerialNumber
        {
            get => consoleSerialNumber;
            set => consoleSerialNumber = value;
        }


        /// <summary>
        /// Gets/sets Remote Firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string RemoteFirmware
        {
            get => remoteFirmware;
            set => remoteFirmware = value;
        }
    }
}
