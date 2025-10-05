using Shared;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class is intended to ablation report changes
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class AblationReportChanges
    {

        private int procedureId;
        private int treatmentNumber;
        private AblationSiteEnum ablationSite;
        private string notes;
        private string diagnosis;
        private string outcome;
        /// <summary>
        /// Gets/sets the value of procedure id
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ProcedureId
        {
            get => procedureId;
            set => procedureId = value;
        }
        /// <summary>
        /// Gets/sets the value of treatment number
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TreatmentNumber
        {
            get => treatmentNumber;
            set => treatmentNumber = value;
        }
        /// <summary>
        /// Gets/sets the value of ablation site
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public AblationSiteEnum AblationSite
        {
            get => ablationSite;
            set => ablationSite = value;
        }
        /// <summary>
        /// Gets/sets the value of notes
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Notes
        {
            get => notes;
            set => notes = value;
        }
        /// <summary>
        /// Gets/sets the value of diagnosis
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Diagnosis
        {
            get => diagnosis;
            set => diagnosis = value;
        }
        /// <summary>
        /// Gets/sets the value of outcome
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Outcome
        {
            get => outcome;
            set => outcome = value;
        }

        /// <summary>
        /// Initializes a new instance of the AblationReport class
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="procedureId">procedure ID</param>
        /// <param name="treatmentNumber">treatment number</param>
        /// <param name="ablationSite">ablation site</param>
        /// <param name="notes">doctor notes</param>
        /// <param name="diagnosis">doctor diagnosis</param>
        /// <param name="outcome">doctor outcome</param>
        public AblationReportChanges(int procedureId, int treatmentNumber, AblationSiteEnum ablationSite, string notes, string diagnosis, string outcome)
        {
            this.ProcedureId = procedureId;
            this.TreatmentNumber = treatmentNumber;
            this.AblationSite = ablationSite;
            this.Notes = notes;
            this.Diagnosis = diagnosis;
            this.Outcome = Outcome;
        }


        /// <summary>
        /// Initializes a new instance of the AblationReport class
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public AblationReportChanges()
        {

        }

    }
}
