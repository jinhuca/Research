using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class is for tank creation
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class TankBuilder
    {
        private double metalWeight = 18;


        /// <summary>
        /// Initializes a new instance of the tank builder
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="tank">tank to build</param>
        /// <param name="data">tank data</param>
        public TankBuilder(Tank tank, Data data)
        {
            MetalWeight =  data.DataAccess.GetTankTypes(tank.Type).MetalWeight; 
        }

        /// <summary>
        /// Gets or sets the tank metal weight
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double MetalWeight
        {
            get
            {
                return metalWeight;
            }

            set
            {
                metalWeight = value;
            }
        }
    }
}
