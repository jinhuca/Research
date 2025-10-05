namespace FileSerializer
{
    /// <summary>
    /// This class contains properties for Ablation Data.
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class AblationData
    {
        /// <summary>
        /// Gets or sets the ablation time stamp
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the ablation data ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ID { get; set; } // we are using the ID as a time

        /// <summary>
        /// Gets or sets the ablation data ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int AblationID { get; set; } //Treatment Number

        /// <summary>
        /// Gets or sets the ablation data system state
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int SystemState { get; set; }

        /// <summary>
        /// Gets or sets the hospital name
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Hospital { get; set; }

        /// <summary>
        /// Gets or sets the error
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Error { get; set; }


        /// <summary>
        /// Gets or sets the Minimum Diaphragm Movement Value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int MinimumDiaphragmMovementValue { get; set; } = 100;

        /// <summary>
        /// Gets or sets Minimum Esophagus Temperature Value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int MinimumEsophagusTemperatureValue { get; set; } = 100;

        /// <summary>
        /// Gets or sets Database Version
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int DatabaseVersion { get; set; }

        /// <summary>
        /// Gets or sets GUI Version
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string GUIVersion { get; set; }
    }
}