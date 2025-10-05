using System;
using System.Collections.Generic;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class handles the Register Comparator
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class RegistersComparator
    {
        /// <summary>
        /// Compares two set of values
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="listOfValuesToComapre">A List of Tuples to compare.</param>
        /// <param name="numberOfValues">An integer representing the number of values to compare.</param>
        /// <returns>A boolean whether the values are identical or not.</returns>
        public static bool CompareValues(List<Tuple<double, double>> listOfValuesToComapre, int numberOfValues)
        {
            bool result = false;

            switch (numberOfValues)
            {
                case 1:
                    result = listOfValuesToComapre[0].Item1.CompareTo(listOfValuesToComapre[0].Item2) == 0 ? true : false;
                    break;

                case 2:
                    result = (listOfValuesToComapre[0].Item1.CompareTo(listOfValuesToComapre[0].Item2) == 0 ? true : false &
                              listOfValuesToComapre[1].Item1.CompareTo(listOfValuesToComapre[1].Item2) == 0 ? true : false);
                    break;

                case 3:
                    result = (listOfValuesToComapre[0].Item1.CompareTo(listOfValuesToComapre[0].Item2) == 0 ? true : false &
                             listOfValuesToComapre[1].Item1.CompareTo(listOfValuesToComapre[1].Item2) == 0 ? true : false &
                             listOfValuesToComapre[2].Item1.CompareTo(listOfValuesToComapre[2].Item2) == 0 ? true : false);
                    break;

                case 4:
                    result = (listOfValuesToComapre[0].Item1.CompareTo(listOfValuesToComapre[0].Item2) == 0 ? true : false &
                            listOfValuesToComapre[1].Item1.CompareTo(listOfValuesToComapre[1].Item2) == 0 ? true : false &
                            listOfValuesToComapre[2].Item1.CompareTo(listOfValuesToComapre[2].Item2) == 0 ? true : false &
                            listOfValuesToComapre[3].Item1.CompareTo(listOfValuesToComapre[3].Item2) == 0 ? true : false);

                    break;
            }

            return result;
        }
    }
}