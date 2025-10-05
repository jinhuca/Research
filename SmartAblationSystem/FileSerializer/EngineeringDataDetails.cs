namespace FileSerializer
{
    /// <summary>
    /// This class contains properties for Ablation Data details.
    /// IEC 62304 Class A.
    /// </summary>
    public class EngineeringDataDetails
    {
        /// <summary>
        /// Gets or sets the engineering data details time stamp
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the system state
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int SystemState { get; set; }

        /// <summary>
        /// Gets or sets the time to target temperature
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TimeToTargetTemperature { get; set; }

        /// <summary>
        /// Gets or sets the required target temperature
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RequiredTargetTemperature { get; set; }

        /// <summary>
        /// Gets or sets the time To thaw
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TimeToThaw { get; set; }

        /// <summary>
        /// Gets or sets the Thaw Timer To Temperature
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ThawTimerToTemperature { get; set; }

        /// <summary>
        /// Gets or sets the Catheter ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterId { get; set; }

        /// <summary>
        /// Gets or sets the Catheter Lot
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterLot { get; set; }

        /// <summary>
        /// Gets or sets the TC1 reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TC1Reading { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TimeInSecondIndex { get; set; }

        /// <summary>
        /// Gets or sets patient cold-junction reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PMCUCJReading { get; set; }

        /// <summary>
        /// Gets or sets the PT1 reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT1Reading { get; set; }

        /// <summary>
        /// Gets or sets the PT2 reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT2Reading { get; set; }

        /// <summary>
        /// Gets or sets the PT3 reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT3Reading { get; set; }

        /// <summary>
        /// Gets or sets the PT4 reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT4Reading { get; set; }

        /// <summary>
        /// Gets or sets the PT5 reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT5Reading { get; set; }

        /// <summary>
        /// Gets or sets the PS1 reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PS1Reading { get; set; }

        /// <summary>
        /// Gets or sets the FM1 reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double FM1Reading { get; set; }

        /// <summary>
        /// Gets or sets the TS1 reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TS1Reading { get; set; }

        /// <summary>
        /// Gets or sets the TN2O reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TN2OReading { get; set; }

        /// <summary>
        /// Gets or sets the LC1 reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double LC1Reading { get; set; }

        /// <summary>
        /// Gets or sets the TIP reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TIPReading { get; set; }

        /// <summary>
        /// Gets or sets the inner balloon pressure reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CP1Reading { get; set; }//IBP

        /// <summary>
        /// Gets or sets the outer balloon pressure reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CP2Reading { get; set; }//OBP

        /// <summary>
        /// Gets or sets the CIMP1 reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CIMP1Reading { get; set; }

        /// <summary>
        /// Gets or sets the pulse Width Modulation for the injection
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PWMINJ { get; set; }

        /// <summary>
        /// Gets or sets the Pulse Width Modulation for the ballon
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PWMBAL { get; set; }

        /// <summary>
        /// Gets or sets ecg channel 1 and 2 reading
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double EcgChannel1And2Reading { get; set; }

        /// <summary>
        /// Gets or sets ecg channel 3 and 4 reading
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double EcgChannel3And4Reading { get; set; }

        /// <summary>
        /// Gets or sets ecg channel 5 and 6 reading
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double EcgChannel5And6Reading { get; set; }

        /// <summary>
        /// Gets or sets ecg channel 7 and 8 reading
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double EcgChannel7And8Reading { get; set; }
    }
}