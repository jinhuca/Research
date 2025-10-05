using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSerializer
{
    /// <summary>
    /// This class contains procedure general info
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ProcedureInfo
    {
        /// <summary>
        /// Gets/sets hospital name
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string HospitalName { get; set; }

        /// <summary>
        /// Gets/sets doctor name
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string DoctorName { get; set; }


        public string DoctorFirstName { get; set; }


        public string DoctorLastName { get; set; }

        /// <summary>
        /// Gets/sets hospital physcian ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string HospitalPhyscianID { get; set; }

        /// <summary>
        /// Gets/sets patient first name
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string PatientFirstName { get; set; }

        /// <summary>
        /// Gets/sets patient last name
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string PatientLastName { get; set; }

        /// <summary>
        /// Gets/sets hospital patient ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string HospitalPatientId { get; set; }

        /// <summary>
        /// Gets/sets entry value of date of birth
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string DateOfBirthEncry { get; set; }

        /// <summary>
        /// Gets/sets date of birth
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string DateOfBirth { get; set; }

        /// <summary>
        /// Gets/sets diagnosis note
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string DiagnosisNote { get; set; }

        /// <summary>
        /// Gets/sets outcome note
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string OutComeNote { get; set; }

        /// <summary>
        /// Gets/sets procedure start date time
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public DateTime ProcedureStartDateTime { get; set; }

        /// <summary>
        /// Gets/sets CMCU Firmware
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string CMCUFirmware { get; set; }

        /// <summary>
        /// Gets/sets CPLD Firmware
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string CPLDFirmware { get; set; }

        /// <summary>
        /// Gets/sets PMCU Firmware
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string PMCUFirmware { get; set; }

        /// <summary>
        /// Gets/sets Repeater Firmware
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string RepeaterFirmware { get; set; }

        /// <summary>
        /// Gets/sets ICB Firmware
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ICBFirmware { get; set; }

        /// <summary>
        /// Gets/sets Catheter Firmware
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string CatheterFirmware { get; set; }

        /// <summary>
        /// Gets/sets Console Serial Number
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ConsoleSerialNumber { get; set; }

        /// <summary>
        /// Gets/sets Patient height
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double? PatientHeight { get; set; }

        /// <summary>
        /// Gets/sets Patient Weight
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double? PatientWeight { get; set; }


        /// <summary>
        /// Gets/sets Patient Gender
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public short PatientGender { get; set; }

        /// <summary>
        /// Gets/sets Skin To Skin Duration
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public short SkinToSkinDuration { get; set; }

        /// <summary>
        /// Gets/sets Treatment Date Time
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public DateTime TreatmentDateTime { get; set; }

        /// <summary>
        /// Gets/sets age
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Gets/sets Database Version
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int DatabaseVersion { get; set; }

        /// <summary>
        /// Gets/sets GUI Version
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string GUIVersion { get; set; }

    }
}
