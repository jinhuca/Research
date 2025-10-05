using SmartAblationSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class handles procedure space
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ProcedureSpaceModel
    {
        private const double averageProcedureMemory = 10;

        /// <summary>
        /// Initializes a new instance of procedure space model
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ProcedureSpaceModel()
        {

        }
        /// <summary>
        /// Gets the remaining procedure value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RemainingProcedure
        {
            get
            {
                return (int)(DrivesInformation.FreeSapceInMB / averageProcedureMemory);
            }
        }
    }
}
