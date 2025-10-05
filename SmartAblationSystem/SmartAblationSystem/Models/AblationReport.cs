using Shared;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class is the Ablation Report Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class AblationReport
    {
        private string treatment;
        private AblationSiteEnum ablationSite;
        private double duration;
        private double minTemperature;
        private double maxTemperatureRate;
        private double timeToTarget;
        private double timeToVeinIsolation;
        private double requiredTargetTemperature;
        private double timeToThaw;
        private double thawTimeToTemperature;
        private int catheterId;
        private int catheterLot;
        private bool isUsedForEngineering;
        private string notes;

        private int procedureId;

        private int minimumDiaphragmMovementValue;
        private int minimumEsophagusTemperatureValue;

        private string errors;
        private string localTime;
        /// <summary>
        /// Initializes a new instance of the AblationReport class
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public AblationReport()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AblationReport class
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="treatment">A string representing a treatment.</param>
        /// <param name="ablationSite">A string representing an ablation site.</param>
        /// <param name="duration">A double representing an ablation duration.</param>
        /// <param name="minTemperature">A double representing the minimum temperature.</param>
        /// <param name="maxTemperatureRate">A double representing the maximum temperature.</param>
        /// <param name="timeToTarget">A double representing the time to target.</param>
        /// <param name="timetoVeinIsolation">A double representing the time to vein isolation.</param>
        /// <param name="requiredTargetTemperature">A double representing the required target temperature.</param>
        /// <param name="timeToThaw">A double representing the time to thaw.</param>
        /// <param name="thawTimerToTemperature">A double representing the thaw timer to temperature.</param>
        /// <param name="catheterId">An integer representing the catheter id.</param>
        /// <param name="catheterLot">An integer representing the catheter lot.</param>
        /// <param name="notes">A string representing an ablation note.</param>
        public AblationReport(string treatment, AblationSiteEnum ablationSite, double duration, double minTemperature, double maxTemperatureRate, 
                              double timeToTarget, double timetoVeinIsolation, double requiredTargetTemperature, double timeToThaw,
                              double thawTimeToTemperature, int catheterId, int catheterLot, string notes, int procedureId, int minimumDiaphragmMovementValue, 
                              int minimumEsophagusTemperatureValue, string error, string localTime,bool IsUsedForEngineering, string balloonSize, int totalThawingTime,
                              int timeSinceVeinIsolation, int temperatureAtIsolation)
        {
            this.Treatment = treatment;
            this.AblationSite = ablationSite;
            this.Duration = duration;
            this.MinTemperature = minTemperature;
            this.MaxTemperatureRate = maxTemperatureRate;
            this.RequiredTargetTemperature = requiredTargetTemperature;
            this.TimeToTarget = timeToTarget;
            this.TimeToVeinIsolation = timetoVeinIsolation;
            this.TimeToThaw = timeToThaw;
            this.ThawTimeToTemperature = thawTimeToTemperature;
            this.CatheterId = catheterId;
            this.CatheterLot = catheterLot;
            this.Notes = notes;
            this.ProcedureId = procedureId;
            this.MinimumDiaphragmMovementValue = minimumDiaphragmMovementValue;
            this.MinimumEsophagusTemperatureValue = minimumEsophagusTemperatureValue;
            this.Errors =error;
            this.LocalTime = localTime;
            this.IsUsedForEngineering = IsUsedForEngineering;
            this.BalloonSize = balloonSize;
            TotalThawingTime = totalThawingTime;
            TimeSinceIsolation = timeSinceVeinIsolation;
            TemperatureAtIsolation = temperatureAtIsolation;
        }

        /// <summary>
        /// Gets or sets the treatment value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Treatment
        {
            get { return treatment; }
            set { treatment = value; }
        }

        /// <summary>
        /// Gets or sets the ablation site value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public AblationSiteEnum AblationSite
        {
            get { return ablationSite; }
            set { ablationSite = value; }
        }

        /// <summary>
        /// Gets or sets the duration value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        /// <summary>
        /// Gets or sets the minimum temperature value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double MinTemperature
        {
            get { return minTemperature; }
            set { minTemperature = value; }
        }

        /// <summary>
        /// Gets or sets the maximum temperature rate value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double MaxTemperatureRate
        {
            get { return maxTemperatureRate; }
            set { maxTemperatureRate = value; }
        }

        /// <summary>
        /// Gets or sets the required target temperature value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double RequiredTargetTemperature
        {
            get { return requiredTargetTemperature; }
            set { requiredTargetTemperature = value; }
        }

        /// <summary>
        /// Gets or sets the time to target value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TimeToTarget
        {
            get { return timeToTarget; }
            set { timeToTarget = value; }
        }

        /// <summary>
        /// Gets or sets the time to vein isolation value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TimeToVeinIsolation
        {
            get { return timeToVeinIsolation; }
            set { timeToVeinIsolation = value; }
        }

        /// <summary>
        /// Gets or sets the time to thaw value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TimeToThaw
        {
            get { return timeToThaw; }
            set { timeToThaw = value; }
        }

        public int TotalThawingTime { get; set; }

        public int TimeSinceIsolation { get; set; }
        public int TemperatureAtIsolation { get; set; }

        /// <summary>
        /// Gets or sets the thaw time to temperature
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThawTimeToTemperature
        {
            get { return thawTimeToTemperature; }
            set { thawTimeToTemperature = value; }
        }

        /// <summary>
        /// Gets or sets the catheter ID
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterId
        {
            get { return catheterId; }
            set { catheterId = value; }
        }

        /// <summary>
        /// Gets or sets the catheter lot
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterLot
        {
            get { return catheterLot; }
            set { catheterLot = value; }
        }


        /// <summary>
        /// Gets or sets the catheter lot
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsUsedForEngineering
        {
            get { return isUsedForEngineering; }
            set { isUsedForEngineering = value; }
        }

        /// <summary>
        /// Gets or sets the treatment notes value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string NotesImage
        {
            get
            {
                return "/Images/Notes.png";
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Gets or sets the treatment value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Notes
        {
            get
            {
                return notes;
            }
            set
            {
                notes = value;
            }
        }
        /// <summary>
        /// Gets or sets procedure ID value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ProcedureId
        {
            get => procedureId;
            set => procedureId = value;
        }
        /// <summary>
        /// Gets or sets minimum diaphragm movement value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int MinimumDiaphragmMovementValue
        {
            get => minimumDiaphragmMovementValue;
            set => minimumDiaphragmMovementValue = value;
        }
        /// <summary>
        /// Gets or sets minimum esophagus temperature value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int MinimumEsophagusTemperatureValue
        {
            get => minimumEsophagusTemperatureValue;
            set => minimumEsophagusTemperatureValue = value;
        }
        /// <summary>
        /// Gets or sets errors value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Errors
        {
            get
            {
                return errors;
            }
            set
            {
                errors = value;
            }
        }
        /// <summary>
        /// Gets or sets the local time value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string LocalTime
        {
            get
            {
                return localTime;
            }
            set
            {
                localTime = value;
            }
        }
 
        public string BalloonSize { get; set; }
    }
}