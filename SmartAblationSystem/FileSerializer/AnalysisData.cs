using System.Collections.Generic;
using System.Linq;

namespace FileSerializer
{
  /// <summary>
  /// This property gets/sets Analysis Data for desktop application
  ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class AnalysisData
  {
    /// <summary>
    /// This property gets/sets AblationDetails list
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public List<AblationFileDataStruct> AblationDetails { get; set; }

    /// <summary>
    /// This property gets/sets TreatmentNoteList list
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public List<TreatmentNotes> TreatmentNoteList { get; set; }

    /// <summary>
    /// This property gets/sets ProcedureInfo list
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public ProcedureInfo ProcedureInfo { get; set; }

    /// <summary>
    /// AnalysisData for desktop application
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public AnalysisData(List<List<AblationDataDetails>> ablationDetails, List<TreatmentNotes> treatmentNotesList, ProcedureInfo procedureInfo)
    {
      this.AblationDetails = ablationDetails
          .Select(AblationFileDataStruct.ConvertAblationDataDetailsToFileStruct)
          .ToList();

      this.TreatmentNoteList = treatmentNotesList;
      this.ProcedureInfo = procedureInfo;
    }

  }
}
