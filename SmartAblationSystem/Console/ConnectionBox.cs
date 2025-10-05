using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public  class ConnectionBox
    {
        /// <summary>
        /// Creates the ConnectionBox class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ConnectionBox()
        {
        }

        /// <summary>
        /// Gets or sets the diaphragm minimum Value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double DiaphragmeMinimumValue { get; set; } = 0.04;

        /// <summary>
        /// Gets or sets the diaphragm maximum value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double DiaphragmeMaximumValue { get; set; } = 0.1;


    }
}
