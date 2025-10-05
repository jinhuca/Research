// <copyright file="BloodDetector.cs" company=" Cryterion Medical Inc.  ">
// Copyright (c) Cryterion Medical Inc. All rights reserved.
// </copyright>
// <author>Alex Smail</author>
// <date>07-18-2017</date>
// <summary> Represents the blood detector</summary>

namespace Console
{
    /// <summary>
    /// Represents the blood detector
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class BloodDetector : IBloodDetector
    {
        private int iD;

        private short lowerBloodThreshold = 17;
        private short upperBloodThreshold = 75;

        /// <summary>
        /// Gets or sets the blood detector
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ID
        {
            get
            {
                return iD;
            }

            set
            {
                iD = value;
            }
        }

        /// <summary>
        /// Gets or sets the lower blood threshold
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public short LowerBloodThreshold
        {
            get
            {
                return lowerBloodThreshold;
            }
            set
            {
                lowerBloodThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the upper blood threshold
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public short UpperBloodThreshold
        {
            get
            {
                return upperBloodThreshold;
            }
            set
            {
                upperBloodThreshold = value;
            }
        }
    }
}