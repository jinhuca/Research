using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class handles the toise.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class Toise
    {
        /// <summary>
        /// Converts between Inch and Cm ratio
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private static double lenghtRatio = 2.54;
        /// <summary>
        /// Gets/sets length unit
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static Enumeration.LengthUnit CurrentToiseUnit
        {
            get;
            set;
        }

        /// <summary>
        /// Converts Cm to Inch
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="lenght">the lenght in cm</param>
        /// <returns>the lenght in inch</returns>
        public static double ConvertCmToInch(double lenght)
        {
            
            return (lenght / lenghtRatio);
        }

        /// <summary>
        /// Converts Inch to Cm 
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="lenght">the lenght in inch</param>
        /// <returns>the lenght in cm</returns>
        public static double ConvertInchToCm(double lenght)
        {
            return (lenght * lenghtRatio);
        }
    }
}
