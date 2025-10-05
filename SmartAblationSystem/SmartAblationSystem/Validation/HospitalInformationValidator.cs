using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Validation
{
    /// <summary>
    /// This class validates hospital information
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class HospitalInformationValidator
    {
        public const int nameMinLenght = 8;
        public const int adressMinLenght = 8;
        public const int cityMinLenght = 2;
        public const int stateMinLenght = 2;
        public const int postalCodeMinLenght = 4;
        public const int coutryMinLenght = 4;
        public const int phoneMinLength = 1;

        /// <summary>
        /// Initializes a new instance of the hospital information validator
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public HospitalInformationValidator()
        {

        }

        /// <summary>
        /// Checks if hospital information is valid
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>   
        /// <param name="name">hospital name</param>
        /// <param name="adress">hospital adress</param>
        /// <param name="city">hospital city</param>
        /// <param name="state">hospital state</param>
        /// <param name="postalCode">hospital postal code</param>
        /// <param name="coutry">hospital coutry</param>
        /// <param name="phoneNumber">hospital phone number</param>
        /// <returns>A boolean value indicating hospital information validation result</returns>
        public bool IsHospitalInformationValid(string name, string adress, string city, string state, string postalCode, string coutry, long phoneNumber)
        {
            if (name?.Length < nameMinLenght || adress?.Length < adressMinLenght || city?.Length < cityMinLenght || state?.Length < stateMinLenght
            || postalCode?.Length < postalCodeMinLenght || coutry?.Length < coutryMinLenght)
                return false;


                return true;
        }
    }
}
