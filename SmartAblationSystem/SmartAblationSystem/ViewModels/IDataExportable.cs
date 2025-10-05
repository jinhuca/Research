using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SmartAblationSystem.ViewModels
{
	public interface IDataExportable : INotifyPropertyChanged, INotifyDataErrorInfo
  {
		bool ActionLogExported { get; set; }
		bool ErrorLogExported { get; set; }
		bool SmartFreezeLogExported { get; set; }
		bool WinEventLogExported { get; set; }
		string LogMessage { get; set; }
		int LogFileCount { get; }
		int LogProgressBarValue { get; set; }
		int ProcedureRecordsCount { get; }
		int ProgressBarValue { get; set; }
		bool IsExportingFiles { get; set; }
		bool IsCanceled { get; set; }
		bool SaveLogSelected { get; set; }
		bool SaveToCSVSelected { get; set; }
		bool SaveToJSONSelected { get; set; }
		bool SaveToPDFSelected { get; set; }
		bool SaveToReportSelected { get; set; }
		bool DeletionSelected { get; set; }
		string ProcedureStartTime { get; set; }
		string ProcedureEndTime { get; set; }
		bool IsPasswordValid { get; set; }
		bool IsPasswordConfirmed { get; set; }
		string FilePassword { get; set; }
		string ConfirmPassword { get; set; }
		bool IsPatientInfoAnonymized { get; set; }
		bool IsCryterionUser { get; }
		bool IsBSCADMINUser { get; }
		bool IsAdminUser { get; }
		bool IsDoctor { get; }
		bool IsExportingCurrentProcedure { get; set; }
		string ExceptionMessage { get; set; }
		event EventHandler USBExportProgressEvent;
		Task OnDeleteDataFiles(bool? delete);
	}
}
