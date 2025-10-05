using Module.Console.Interfaces;
using Module.Infrastructure;
using Module.Infrastructure.AppLog;
using Module.Infrastructure.Constants;
using Module.Infrastructure.Controls;
using Module.Infrastructure.Helpers;
using Module.Infrastructure.PubSubEvents;
using Module.Infrastructure.TestInterfaces;
using Module.Infrastructure.TestResults.Implementation;
using Module.Infrastructure.TestResults.Interfaces;
using Module.Report.Interfaces;
using Module.Report.Results;
using Module.Report.Results.Tests;
using Module.SystemParameters.Extensions;
using Module.TestProcess.Models.Tests;
using Module.TestProcess.Services;
using OfficeOpenXml;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using static Module.Infrastructure.Constants.Strings;
using static Module.Infrastructure.SessionStatus;
using static System.DateTime;

namespace Module.Report
{
	public class ReportAggregation : BindableBase
	{
		private readonly IUnityContainer _container;
		private readonly IMachineModel _machineModel;
		private readonly IEventAggregator _eventAggregator;
		private readonly IDialogService _dialogService;
		private readonly IGeneralInformation _generalInformation;
		private readonly ISessionModel _sessionModel;
		private readonly IXlsxService _xlsxService;
		private string _consoleSN = string.Empty;
		private string _firstName = string.Empty;
		private string _lastName = string.Empty;
		private string _testerInitial = string.Empty;
		private string _hospitalName = string.Empty;
		private string _fstVersion = string.Empty;

		public ITestReport TestReport { get; set; }

		public ReportAggregation(
			IUnityContainer container,
			IEventAggregator eventAggregator,
			IGeneralInformation generalInformation,
			ITestReport report,
			IDialogService dialogService,
			IMachineModel machineModel,
			ISessionModel sessionModel,
			IXlsxService xlsxService)
		{
			_container = container;
			_eventAggregator = eventAggregator;
			_generalInformation = generalInformation;
			_dialogService = dialogService;
			_machineModel = machineModel;
			_sessionModel = sessionModel;
			_xlsxService = xlsxService;
			TestReport = report;
		}

		public void Start()
		{
			TestReport = _container.Resolve<ITestReport>();
			_eventAggregator.GetEvent<USBConnectionEvent>().Subscribe(OnUSBConnectionEvent);
			_eventAggregator.GetEvent<TesterInfoEvent>().Subscribe(OnReceiveTesterInfoEvent);
			_eventAggregator.GetEvent<ConsoleSerialNumberEvent>().Subscribe(OnReceiveConsoleSerialNumberEvent);
			_eventAggregator.GetEvent<HospitalNameEvent>().Subscribe(OnReceiveHospitalName);
			_eventAggregator.GetEvent<FstVersionEvent>().Subscribe(OnReceiveFstVersionEvent);
			_eventAggregator.GetEvent<SessionStatusEvent>().Subscribe(OnReceiveSessionStatusEvent);
			_eventAggregator.GetEvent<StepEvent>().Subscribe(OnReceiveStepEvent);
			_eventAggregator.GetEvent<TestEvent>().Subscribe(OnReceiveTestEvent);
			_eventAggregator.GetEvent<SummaryEvent>().Subscribe(OnReceiveSummaryEvent);
			_eventAggregator.GetEvent<RetryRationaleEvent>().Subscribe(OnReceiveRetryRationaleEvent);
			_eventAggregator.GetEvent<ErrorListUpdateEvent>().Subscribe(OnConsoleErrorEvent);
			_eventAggregator.GetEvent<TimeOutEvent>().Subscribe(OnTimeOutEvent);
		}

		private void OnTimeOutEvent(string msg)
		{
			TestReport.ErrorReport.Result.ErrorList.Add((TimeOutTitle, msg));
		}

		private void OnConsoleErrorEvent(IList<ErrorMessageExtender> errorList)
		{
			foreach(var error_ in errorList)
			{
				if(TestReport.ErrorReport.Result.ErrorList.All(e => e.Item1 != error_.Item2))
					TestReport.ErrorReport.Result.ErrorList.Add((error_.Item2, error_.Item4));
			}
		}

		private void OnReceiveRetryRationaleEvent((string id, string msg) obj)
		{
			TestReport.RationaleReport.Result.RationaleList.Add((obj.id, obj.msg));
		}

		private void OnUSBConnectionEvent(string usbDriveName) => USBDriveName = usbDriveName;

		private void OnReceiveTestEvent(ITestModel testModel)
		{
			switch(testModel.Info.Status)
			{
				case TestStatus.Unknown:
				case TestStatus.Retry:
				case TestStatus.NotStarted:
				case TestStatus.Inprogress:
				case TestStatus.Paused:
				default:
					return;
				case TestStatus.Passed:
				case TestStatus.Failed:
				case TestStatus.Finished:
				case TestStatus.Stopped:
				case TestStatus.Aborted:
					switch(testModel.Info.Entity.Id)
					{
						case TestId.VersionVerification:
							CreateVersionVerificationReport(testModel as Step1VersionVerificationModel);
							break;
						case TestId.InputTest:
							CreateInputTestReport(testModel as Step1InputTestModel);
							break;
						case TestId.VisualTest:
							CreateVisualTestReport(testModel as Step1VisualTestModel);
							break;
						case TestId.AudibleTest:
							CreateAudibleTestReport(testModel as Step1AudibleTestModel);
							break;
						case TestId.IdleStateCheck:
							CreateIdleStateCheckReport(testModel as Step2IdleCheckModel);
							break;
						case TestId.ReadyStateCheck:
							CreateReadyStateCheckReport(testModel as Step2ReadyCheckModel);
							break;
						case TestId.AblationTests:
							CreateAblationTestReport(testModel as Step3AblationTestModel);
							break;
						case TestId.DMSTests:
							//	CreateDmsTestReport(testModel as Step3DmsTestModel);
							break;
						case TestId.ETSTests:
							//  CreateEtsTestReport(testModel as Step3EtsTestModel);
							break;
						case TestId.OPSTests:
							//	CreateOpsTestReport(testModel as Step3OpsTestModel);
							break;
						case TestId.Unknown:
						default:
							break;
					}
					break;
			}
		}

		private void CreateVersionVerificationReport(Step1VersionVerificationModel testModel)
		{
			var versionReport = new VersionVerificationReport
			{
				Information = testModel.Info,
				Result = new VersionTestResult()
			};

			versionReport.Result.Passed = testModel.Result.Passed;
			versionReport.Result.CMCUVersion = testModel.Result.CMCUVersion;
			versionReport.Result.CPLDVersion = testModel.Result.CPLDVersion;
			versionReport.Result.PMCUVersion = testModel.Result.PMCUVersion;
			versionReport.Result.RMCUVersion = testModel.Result.RMCUVersion;
			versionReport.Result.ICBVersion = testModel.Result.ICBVersion;
			versionReport.Result.RCMCUVersion = testModel.Result.RCMCUVersion;
			versionReport.Result.GUIVersion = testModel.Result.GUIVersion;
			versionReport.Result.DBVersion = testModel.Result.DBVersion;
			versionReport.Result.CMCUBootVersion = testModel.Result.CMCUBootVersion;
			versionReport.Result.RCMCUBootVersion = testModel.Result.RCMCUBootVersion;
			versionReport.Result.PMCUBootVersion = testModel.Result.PMCUBootVersion;
			versionReport.Result.RMCUBootVersion = testModel.Result.RMCUBootVersion;
			versionReport.Result.ICBBootVersion = testModel.Result.ICBBootVersion;

			TestReport.VersionReport = versionReport;

			_generalInformation.CmcuVersion = testModel.Result.CMCUVersion;
			_generalInformation.CpldVersion = testModel.Result.CPLDVersion;
			_generalInformation.PmcuVersion = testModel.Result.PMCUVersion;
			_generalInformation.RepeaterVersion = testModel.Result.RMCUVersion;
			_generalInformation.IcbVersion = testModel.Result.ICBVersion;
			_generalInformation.RemoteVersion = testModel.Result.RCMCUVersion;
			_generalInformation.GuiVersion = testModel.Result.GUIVersion;
			_generalInformation.DatabaseVersion = testModel.Result.DBVersion;
			_generalInformation.CmcuBootVersion = testModel.Result.CMCUBootVersion;
			_generalInformation.RcmcuBootVersion = testModel.Result.RCMCUBootVersion;
			_generalInformation.PmcuBootVersion = testModel.Result.PMCUBootVersion;
			_generalInformation.RmcuBootVersion = testModel.Result.RMCUBootVersion;
			_generalInformation.IcbBootVersion = testModel.Result.ICBBootVersion;
			_generalInformation.CatheterVersion = _machineModel.CatheterFirmwareVersion.ToVersionString();
		}

		private void CreateInputTestReport(Step1InputTestModel testModel)
		{
			TestReport.InputReport.Information = testModel.Info;
			TestReport.InputReport.Result = testModel.Result;
		}

		private void CreateVisualTestReport(Step1VisualTestModel testModel)
		{
			TestReport.VisualReport.Information = testModel.Info;
			TestReport.VisualReport.Result = testModel.Result;
		}

		private void CreateAudibleTestReport(Step1AudibleTestModel testModel)
		{
			TestReport.AudibleReport.Information = testModel.Info;
			TestReport.AudibleReport.Result = testModel.Result;
		}

		private void CreateIdleStateCheckReport(Step2IdleCheckModel testModel)
		{
			TestReport.IdleStateReport.Information = testModel.Info;
			TestReport.IdleStateReport.Result = testModel.Result;
		}

		private void CreateReadyStateCheckReport(Step2ReadyCheckModel testModel)
		{
			TestReport.ReadyStateReport.Information = testModel.Info;
			TestReport.ReadyStateReport.Result = testModel.Result;
		}

		private void CreateAblationTestReport(Step3AblationTestModel testModel)
		{
			TestReport.AblationReport.Information = testModel.Info;
			TestReport.AblationReport.Result = testModel.Result;
		}

		//private void CreateDmsTestReport(Step3DmsTestModel testModel)
		//{
		//	TestReport.DmsReport.Information = testModel.Info;
		//	TestReport.DmsReport.Result = testModel.Result;
		//}

		//private void CreateEtsTestReport(Step3EtsTestModel testModel)
		//{
		//	TestReport.EtsReport.Information = testModel.Info;
		//	TestReport.EtsReport.Result = testModel.Result;
		//}

		//private void CreateOpsTestReport(Step3OpsTestModel testModel)
		//{
		//	TestReport.OpsReport.Information = testModel.Info;
		//	TestReport.OpsReport.Result = testModel.Result;
		//}

		private void OnReceiveStepEvent(IStepModel obj)
		{
		}

		private async void OnReceiveSessionStatusEvent((SessionStatus status, DateTime dateTime) sessionData)
		{
			switch(sessionData.status)
			{
				case Unknown:
					break;
				case Ready:
					break;
				case Started:
					TestReport = _container.Resolve<ITestReport>();
					TestReport.StartDateTime = sessionData.dateTime;
					TestReport.RationaleReport = _container.Resolve<RetryRationaleReport>();
					TestReport.ErrorReport = _container.Resolve<ConsoleErrorReport>();
					break;
				case Paused:
					break;
				case Resumed:
					break;
				case Stopped:
					break;
				case Stopping:
					break;
				case Finished:
					EvaluateTestPassed(sessionData);
					await CreateDataFile();
					await CreateTestReport();
					break;
				case Starting:
					break;
				case Pausing:
					break;
				case Resuming:
					break;
				case Finishing:
					break;
				case SessionStatus.Exception:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnReceiveSummaryEvent(string summary)
			=> TestReport.SessionSummaryReport.Result = summary;

		private void OnReceiveTesterInfoEvent((string first, string last, DateTime dt) TesterInfo)
		{
			(_firstName, _lastName, _) = TesterInfo;
			_testerInitial = Dash + (_firstName.Substring(0, 1) + _lastName.Substring(0, 1)).ToUpper() + Dash;
		}

		private void OnReceiveConsoleSerialNumberEvent(string consoleSerialNumber)
		{
			_consoleSN = consoleSerialNumber;
			TestReport.ConsoleSerialNumber = consoleSerialNumber;
			_generalInformation.ConsoleSN = consoleSerialNumber;
		}

		private void OnReceiveFstVersionEvent(string versionMsg)
		{
			_fstVersion = versionMsg;
			TestReport.FstVersion = versionMsg;
			_generalInformation.ServiceToolVersion = versionMsg;
		}

		private void OnReceiveHospitalName(string hospitalMsg)
		{
			_hospitalName = hospitalMsg;
			TestReport.HospitalName = hospitalMsg;
			_generalInformation.HospitalName = hospitalMsg;
		}

		private void EvaluateTestPassed((SessionStatus status, DateTime dateTime) sessionData)
		{
			var (status, dateTime) = sessionData;
			TestReport.FinishDateTime = dateTime;
			switch(status)
			{
				case Unknown:
					break;
				case Ready:
					break;
				case Starting:
					break;
				case Started:
					break;
				case Pausing:
					break;
				case Paused:
					break;
				case Resuming:
					break;
				case Resumed:
					break;
				case Stopping:
					break;
				case Finishing:
					break;
				case Stopped:
					break;
				case Finished:
					TestReport.Passed = new List<bool?>
					{
						TestReport.VersionReport?.Result?.Passed,
						TestReport.InputReport?.Result?.Passed,
						TestReport.VisualReport?.Result?.Passed,
						TestReport.AudibleReport?.Result?.Passed,
						TestReport.IdleStateReport?.Result?.Passed,
						TestReport.ReadyStateReport?.Result?.Passed,
						TestReport.AblationReport?.Result?.Passed,
					}.All(passed_ => passed_.HasValue && passed_.Value);
					break;
				case SessionStatus.Exception:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private async Task CreateTestReport()
		{
			try
			{
				var appLocation_ = ConfigurationManager.AppSettings[ConfigurationConstants.ReportPath];
				var reportPdfFileName_ = CreateReportPdfFileName();
				TestReport.TesterReport.FirstName = _firstName;
				TestReport.TesterReport.LastName = _lastName;
				TestReport.ConsoleSerialNumber = _consoleSN;
				TestReport.HospitalName = _hospitalName;
				TestReport.FstVersion = _fstVersion;

				await Task.Run(() => ReportPDF.GeneratePdfReport(TestReport, Path.Combine(appLocation_, reportPdfFileName_)));
				_eventAggregator.GetEvent<GenerateReportEvent>().Publish(false);
				InvokeDialog();
			}
			catch(Exception ex)
			{
				FieldServiceTrace.LogException(ex);
			}

			void InvokeDialog()
			{
				var parameters_ = new DialogParameters
				{
					{ DialogTitleKey, ReportFileTitle },
					{ DialogMessageKey, ReportFileMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(MessageDialog), parameters_, null);
				});
			}
		}

		private async Task CreateDataFile()
		{
			_eventAggregator.GetEvent<GenerateReportEvent>().Publish(true);
			if(TestReport.IdleStateReport.Result == null)
			{
				return;
			}

			if(string.IsNullOrEmpty(_xlsxService.FileName))
			{
				_xlsxService.FileName = _xlsxService.GenerateXlsxFileName(_sessionModel.StartTime?.ToString(ReportDateTimeFormat));
			}

			await _xlsxService.CreateIdleStateParameterSheet(TestReport.IdleStateReport.Result);

			if(TestReport.ReadyStateReport.Result == null)
			{
				return;
			}

			await _xlsxService.CreateReadyStateParameterSheet(TestReport.ReadyStateReport.Result);

			if(TestReport.AblationReport.Result != null)
			{
				var file_ = new FileInfo(_xlsxService.FileName);
				if(file_ != null)
				{
					using (var package_ = new ExcelPackage(file_))
					{
						if(package_.Workbook.Worksheets.Any(x => x.Name == GeneralInfoSheetTitle))
						{
							package_.Workbook.Worksheets.MoveAfter(IdleStateCheckDetailsText, GeneralInfoSheetTitle);
							package_.Workbook.Worksheets.MoveAfter(ReadyStateCheckDetailsText, IdleStateCheckDetailsText);
						}
						await package_.SaveAsAsync(_xlsxService.FileName);
					}
				}
			}

			_xlsxService.FileName = null;
		}

		private void RejectTestReport(DateTime cancelTime)
		{
			TestReport = _container.Resolve<ITestReport>();
		}

		private string _usbDriveName;
		public string USBDriveName
		{
			get => _usbDriveName;
			set => SetProperty(ref _usbDriveName, value);
		}

		private string GetUSBDriveName()
		{
			try
			{
				using(var _usbManager = new USBManager((s, e) => { }))
				{
					var _drives = _usbManager.GetUSBDriveList();
					return _drives != null && _drives.Count > 0 && File.Exists(_drives[0].Name + FSTZipName)
						? _drives[0].Name
						: string.Empty;
				}
			}
			catch(Exception ex)
			{
				FieldServiceTrace.LogException(ex);
			}
			return string.Empty;
		}

		private string CreateReportFile()
		{
			var _driveName = GetUSBDriveName();
			var _csn = TestReport.ConsoleSerialNumber + Dash;
			var _initials = (TestReport.TesterReport.FirstName.Substring(0, 1) + TestReport.TesterReport.LastName.Substring(0, 1)).ToUpper() + Dash;
			var _timeStamp = Now.ToString(ReportDateTimeFormat);
			return _driveName + ReportHeader + _csn + _initials + TestReportPrefix + _timeStamp + txtExtension;
		}

		private string CreateReportPdfFileName()
		{
			var _driveName = GetUSBDriveName();
			var _timeStamp = TestReport.StartDateTime.Value.ToString(ReportDateTimeFormat);
			return _driveName + ReportHeader + _consoleSN + Dash + _timeStamp + pdfExtension;
		}
	}
}
