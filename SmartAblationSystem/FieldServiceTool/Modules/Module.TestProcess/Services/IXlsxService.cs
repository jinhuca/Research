using System.Collections.Generic;
using Module.Infrastructure.TestResults.Interfaces;
using System.Threading.Tasks;
using Module.FlowMeterComm.Models;
using OfficeOpenXml;

namespace Module.TestProcess.Services
{
	public interface IXlsxService
	{
		Task<bool> CreateExcelFile(string excelFileName);
		string GenerateXlsxFileName(string timestamp_);
		Task<bool> AddSummaryToWorksheet(IAblationTestResult ablationTestResult, string excelFileName);
		Task<bool> AddAblationDetailToWorksheet(int ablationId, IAblationTestResult ablationTestResult, string excelFileName);
		Task<bool> AddFlowMeterDetailToWorksheet(int ablationId, IList<FlowRateData> flowMeterTestData, string excelFileName);
		Task<bool> CreateIdleStateParameterSheet(IIdleStateCheckResult idleCheckResult);
		Task<bool> CreateReadyStateParameterSheet(IReadyStateCheckResult readyCheckResult);
		string FileName { get; set; }
	}
}
