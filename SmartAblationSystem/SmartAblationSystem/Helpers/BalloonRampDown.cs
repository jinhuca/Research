using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class for balloon ramp down
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class BalloonRampDown
    {
        /// <summary>
        /// Gets/sets bool value for balloon ramp down activated
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool IsBalloonRampDownActivated { get; set; } = true;
    }
}
