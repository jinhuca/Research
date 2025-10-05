using SmartAblationSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class extract ablation data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class AblationDataExtractor : IAblationDataExtractor
    {

        private Dictionary<ProcedureRecords, List<AblationReport>> ablationReportAccordingToProcedure;

        /// <summary>
        /// Constructor that initialize the ablation data extractor
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="procedureRecord">procedure record</param>
        /// <param name="ablationList">ablation list</param>
        public AblationDataExtractor(ProcedureRecords procedureRecord, List<AblationReport> ablationList)
        {
            if (AblationReportAccordingToProcedure == null)
            {
                AblationReportAccordingToProcedure = new Dictionary<ProcedureRecords, List<AblationReport>>();
            }

            if (procedureRecord != null && ablationList != null)
            {
                AblationReportAccordingToProcedure.Add(procedureRecord, ablationList);
            }
        }
        /// <summary>
        /// Gets/sets vaule of ablation report according the procedure
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Dictionary<ProcedureRecords, List<AblationReport>> AblationReportAccordingToProcedure
        {
            get => ablationReportAccordingToProcedure;
            set => ablationReportAccordingToProcedure = value;
        }
    }
}
