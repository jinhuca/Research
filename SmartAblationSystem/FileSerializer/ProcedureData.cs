using System.Collections.Generic;

namespace FileSerializer
{
    /// <summary>
    /// This class contains a procedure data : ablation details and ablation ECG details list
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ProcedureData
    {
        /// <summary>
        /// This property gets/sets AblationDetails list
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<List<AblationDataDetails>> AblationDetails { get; set; }

        /// <summary>
        /// Default constructor that receives a list of ablation details and a list of ablation ECG details
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="ablationDetails">A list of ablation details list.</param>
        /// <param name="ablationECGDetails">A list of ablation ECG details list.</param>
        public ProcedureData(List<List<AblationDataDetails>> ablationDetails)
        {
            this.AblationDetails = ablationDetails;
        }
    }
}