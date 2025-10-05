using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class is intended to define the  outer balloon pressure threshold
    ///  . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
    /// </summary>
    class OuterBalloonPressureThreshold
    {
        /// <summary>
        /// Constructor that initialize outer balloon pressure threshold
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public OuterBalloonPressureThreshold()
        {

        }

        /// <summary>
        /// Calculate OBP thershold
        /// </summary>
        /// <param name="PT3Value">PT3 value</param>
        /// <param name="PSIGReference">PSIG reference</param>
        /// <returns>OBP thershold</returns>
        /// <id>SF-SDS-0103</id>
        public double GetThershold(double PT3Value, double PSIGReference = 4.7)
        {
            return(Math.Round(PSIGReference - PT3Value, 1));
        }
    }
}
