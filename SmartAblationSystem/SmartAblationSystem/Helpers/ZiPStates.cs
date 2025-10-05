using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class handles zip states.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class ZiPStates
    {
        static private bool isZipingFiles = false;
        /// <summary>
        /// Gets/sets bool value of is ziping files.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool IsZipingFiles
        {
            get => isZipingFiles;
            set => isZipingFiles = value;
        }
    }
}
