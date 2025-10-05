// <copyright file="Balloon.cs" company=" Cryterion Medical Inc.  ">
// Copyright (c) Cryterion Medical Inc. All rights reserved.
// </copyright>
// <author>Alex Smail</author>
// <date>03-23-2017</date>
// <summary> Represents the balloon catheter</summary>

namespace Console
{
    /// <summary>
    /// Represents the balloon catheter
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class Balloon
    {
        private int balloonId;
        private double targetBalloonPressure;
        private double targetBalloonFlow;

        /// <summary>
        /// Gets or sets the balloon ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int BalloonId
        {
            get
            {
                return balloonId;
            }

            set
            {
                balloonId = value;
            }
        }

        /// <summary>
        /// Gets or sets the balloon pressure
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TargetBalloonPressure
        {
            get
            {
                return targetBalloonPressure;
            }

            set
            {
                targetBalloonPressure = value;
            }
        }

        /// <summary>
        /// Gets or sets the target balloon flow
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TargetBalloonFlow
        {
            get
            {
                return targetBalloonFlow;
            }

            set
            {
                targetBalloonFlow = value;
            }
        }
    }
}