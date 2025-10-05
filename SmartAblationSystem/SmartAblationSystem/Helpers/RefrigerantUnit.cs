using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class handles get and set refrigerant level unit value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class RefrigerantUnit
    {
        private static short refrigerantLevelUnit = 0;

        /// <summary>
        /// Gets/sets refrigerant level unit value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static short RefrigerantLevelUnit
        {
            get
            {
                return refrigerantLevelUnit;
            }

            set
            {
                refrigerantLevelUnit = value;
            }
        }
    }
}
