namespace FileSerializer
{
    /// <summary>
    /// This class contains properties for Ablation ECG Data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class AblationECGData : AblationData
    {
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

        /// <summary>
        /// Gets or sets the CP1 Reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CP1Reading { get; set; }

        /// <summary>
        /// Gets or sets the Esophagus Temperature Threshold Reached value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool EsophagusTemperatureThresholdReached { get; set; }

        /// <summary>
        /// Gets or sets the Esophagus Temperature value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int EsophagusTemperature { get; set; }

        /// <summary>
        /// Gets or sets the Is Diaphragm Movement Detected value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsDiaphragmMovementDetected { get; set; }

        /// <summary>
        /// Gets or sets the Diaphragm Amplitude value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int DiaphragmAmplitude { get; set; }

        /// <summary>
        /// Gets or sets the Diaphragm Amplitude Threshold Reached value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool DiaphragmAmplitudeThresholdReached { get; set; }

        /// <summary>
        /// Gets or sets the Ignore Minimum Diaphragm Movement value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IgnoreMinimumDiaphragmMovement { get; set; }

        /// <summary>
        /// Gets or sets the diaphragm sensor gain value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int DiaphragmSensorGain { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether system monitoring diaphram is alert or not
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSystemMonitoringDiaphragmAlert { get; set; }

        /// <summary>
        /// Gets or sets the value of Blood Detecor Im Value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int BloodDetecorImValue { get; set; }

    }
}