using System;
using DataAccessLayer;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class is for the Data Model, it allows access to the DataAccess
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class Data
    {
        private DataAccess dataAccess = null;

        /// <summary>
        /// Initializes a new instance of the Data Model class and its properties
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Data()
        {
            this.dataAccess = new DataAccess();
        }

        /// <summary>
        /// Gets or sets a DataAccess value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public DataAccess DataAccess
        {
            get
            {
                return dataAccess;
            }
            set
            {
                dataAccess = value;
            }
        }

        internal int CalculateRisk(double weight, double height, int gender, DateTime dateOfBirth)
        {
            throw new NotImplementedException();
        }
    }
}