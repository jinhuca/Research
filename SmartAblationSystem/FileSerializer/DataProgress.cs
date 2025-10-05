using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSerializer
{
    /// <summary>
    /// Gets/sets Data Progress value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class DataProgress
    {
        private static int maxValue = 250;
        private static int minValue = 0;

        private static int increment = 5;

        private static DataProgressStates currentDataProgressStates = DataProgressStates.Unknown;
        private static DataProgressStates previousDataProgressStates = DataProgressStates.Unknown;

        /// <summary>
        /// Gets/sets max value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static int MaxValue
        {
            get => maxValue;
            set => maxValue = value;
        }

        /// <summary>
        /// Gets/sets min value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static int MinValue
        {
            get => minValue;
            set => minValue = value;
        }
        /// <summary>
        /// Gets/sets current data progress states value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static DataProgressStates CurrentDataProgressStates
        {
            get => currentDataProgressStates;
            set => currentDataProgressStates = value;
        }

        /// <summary>
        /// Gets/sets previous data progress states value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static DataProgressStates PreviousDataProgressStates
        {
            get => previousDataProgressStates;
            set => previousDataProgressStates = value;
        }

        /// <summary>
        /// Gets increment value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static int Increment
        {
            get => increment;
        }

        /// <summary>
        /// Gets/sets convert value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static int ConvertValue(int value)
        {
            return ((value * 250) / 100);
        }
    }
    /// <summary>
    /// Data progress states value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public enum DataProgressStates
    {
        Unknown = 0,
        STARTING = 1,
        GENERATING_DATA = 2,
        CONVERTING_TOXLS = 3,
        CONVERTING_TO_PDF = 4,
        SAVING_TO_JSON = 5,
        ENDING = 6
    }
}
