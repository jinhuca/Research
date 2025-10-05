using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSerializer
{
    /// <summary>
    /// This class contains treatment notes for desktop application Json file
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class TreatmentNotes
    {
        /// <summary>
        /// Gets/sets procedure ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ProcedureId { get; set; }

        /// <summary>
        /// Gets/sets treatment ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TreatmentId { get; set; }

        /// <summary>
        /// Gets/sets treatment note
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string TreatmentNote { get; set; }

    }
}
