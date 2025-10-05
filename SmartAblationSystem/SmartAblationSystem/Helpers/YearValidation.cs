using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class handles year validation.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class YearValidation
    {
        /// <summary>
        /// Gets/set the value indicating whether is user allowed to change year
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool IsUserAllowedTochangeYear { get; set; } = false;
        /// <summary>
        /// Gets/set the value of hour
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static short Hour { get; set; } = 0;
        /// <summary>
        /// Gets/set the value of minute
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static short Minute { get; set; } = 0;
        /// <summary>
        /// Gets/set the value of time format
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static string TimeFormat { get; set; } = string.Empty;
    }   
}
