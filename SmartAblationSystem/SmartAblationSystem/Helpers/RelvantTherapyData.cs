using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class handles the ablation information
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class AblationInformation 
    {
        /// <summary>
        /// Gets/sets bool value of is there ablation historical data
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool IsThereAbltionHistoricalData { get; set; } = false;
    }
}
