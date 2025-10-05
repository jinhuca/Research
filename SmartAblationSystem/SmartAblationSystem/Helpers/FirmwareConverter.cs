using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class convert firmware value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal static class FirmwareConverter
    {

        /// <summary>
        /// Convert firmware value to string
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static string ConvertToMicrosfotversioning(Int64 firmware)
        {
            string valueToConvert = string.Empty;

                valueToConvert = System.Convert.ToInt64(firmware).ToString("X");
                if (valueToConvert.Length >= 4)
                    valueToConvert = valueToConvert.Insert(3, ".").Insert(2, ".").Insert(1, ".");
            

            return valueToConvert;


        }
            
    }
}
