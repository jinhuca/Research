using Shared;
using SmartAblationSystem.ViewModels;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAblationSystem.Models
{
	public interface IDataExportService : IDisposable
	{
		[ReadOnly(true)]
		UserType SelectedUserType { get; }

		[ReadOnly(true)]
		ProcedureRecords SelectedProcedureRecords { get; }

		[ReadOnly(true)]
		DirectoryInfo DestinationDirectoryInfo { get; }

		FileInfo ExportJsonFile();

		FileInfo ExportPdfFile();

		FileInfo ExportExcelFile();

		FileInfo GeneratePdfFile();

		FileInfo ExportCaseReportFile();

		FileInfo ExportLogFile(IDataExportable context, CancellationToken cancellationToken);

		Task PrintPdfReport();
	}
}