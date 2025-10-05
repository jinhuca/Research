using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class scale
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class Scale
    {

        // Pounds to Kilograms ratio
        private static double massRatio = 0.45;

        /// <summary>
        /// Gets/set the current weight unit
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>  
        public static Enumeration.WeightUnit CurrentWeightUnit
        {
            get;
            set;
        }

        /// <summary>
        /// Convert pounds to Kilograms
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0032</id>
        /// <param name="weight">weight in lbs</param>
        /// <returns>weight in Kg</returns>
        public static double  ConvertLbToKg(double weight)
        {
            return (weight * massRatio);
        }

        /// <summary>
        /// convert Kilograms to pounds
        /// afety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0033</id>
        /// <param name="weight">weight in kg</param>
        /// <returns>weight in lb </returns>
        public static double  ConvertKgToLb(double weight)
        {
            return (weight / massRatio);
        }
    }
}
