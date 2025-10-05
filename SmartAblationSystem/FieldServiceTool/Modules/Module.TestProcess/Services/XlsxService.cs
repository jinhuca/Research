using Module.Console.Interfaces;
using Module.Infrastructure.AppLog;
using Module.Infrastructure.Helpers;
using Module.Infrastructure.PubSubEvents;
using Module.Infrastructure.TestResults.Implementation;
using Module.Infrastructure.TestResults.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Module.SystemParameters.Extensions;
using static Module.Infrastructure.Constants.Strings;
using static System.Drawing.Color;
using LicenseContext = OfficeOpenXml.LicenseContext;
using Module.FlowMeterComm.Models;
using Module.Infrastructure.TestInterfaces;

namespace Module.TestProcess.Services
{
	public class XlsxService : IXlsxService
	{
		private static readonly Color ColumnHeaderBackgroundColor = FromArgb(1, 49, 140, 231);
		private static readonly Color AlternativeTextBackgroundColor = FromArgb(0x1, 0xF5, 0xF5, 0xF5);

		private readonly IContainerProvider _containerProvider;
		private readonly IMachineModel _machineModel;
		private string _FstVersion = string.Empty;
		private string _HospitalName = string.Empty;
		private string _ConsoleSN = string.Empty;
		private string _TesterInitials = string.Empty;

		public XlsxService(
			IContainerProvider containerProvider,
			IMachineModel machineModel,
			IEventAggregator eventAggregator)
		{
			_containerProvider = containerProvider;
			_machineModel = machineModel;
			eventAggregator.GetEvent<FstVersionEvent>().Subscribe(OnReceiveFstVersionEvent);
			eventAggregator.GetEvent<HospitalNameEvent>().Subscribe(OnReceiveHospitalNameEvent);
			eventAggregator.GetEvent<ConsoleSerialNumberEvent>().Subscribe(OnReceiveConsoleSNEvent);
			eventAggregator.GetEvent<TesterInfoEvent>().Subscribe(OnReceiveTesterInfoEvent);
		}

		private void OnReceiveTesterInfoEvent((string fstName, string lastName, DateTime) msgObj)
			=> _TesterInitials = (msgObj.fstName.Substring(0, 1) + msgObj.lastName.Substring(0, 1)).ToUpper() + Dash;

		private void OnReceiveConsoleSNEvent(string consoleSerialNumber)
			=> _ConsoleSN = consoleSerialNumber;

		private void OnReceiveHospitalNameEvent(string hospitalMsg)
			=> _HospitalName = hospitalMsg;

		private void OnReceiveFstVersionEvent(string versionMsg)
			=> _FstVersion = versionMsg;
		public string FileName { get; set; } = string.Empty;

		private IGeneralInformation GetGeneralInformation(IAblationTestResult ablationResult)
		{
			var generalInformation_ = _containerProvider.Resolve<IGeneralInformation>();
			var versions_ = _containerProvider.Resolve<IVersionTestResult>();

			generalInformation_.CmcuVersion = versions_.CMCUVersion;
			generalInformation_.CpldVersion = versions_.CPLDVersion;
			generalInformation_.PmcuVersion = versions_.PMCUVersion;
			generalInformation_.RepeaterVersion = versions_.RMCUVersion;
			generalInformation_.IcbVersion = versions_.ICBVersion;
			generalInformation_.RemoteVersion = versions_.RCMCUVersion;
			generalInformation_.GuiVersion = versions_.GUIVersion;
			generalInformation_.DatabaseVersion = versions_.DBVersion;
			generalInformation_.CmcuBootVersion = versions_.CMCUBootVersion;
			generalInformation_.RcmcuBootVersion = versions_.RCMCUBootVersion;
			generalInformation_.PmcuBootVersion = versions_.PMCUBootVersion;
			generalInformation_.RmcuBootVersion = versions_.RMCUBootVersion;
			generalInformation_.IcbBootVersion = versions_.ICBBootVersion;
			generalInformation_.CatheterVersion = _machineModel.CatheterFirmwareVersion.ToVersionString();
			generalInformation_.Treatments = ablationResult.AblationSummaryList;
			generalInformation_.ConsoleSN = _ConsoleSN;
			generalInformation_.HospitalName = _HospitalName;
			generalInformation_.ServiceToolVersion = _FstVersion;

			return generalInformation_;
		}

		private Dictionary<string, (string, string)> GetGeneralInformationDictionary(IGeneralInformation info)
		{
			var NameDescValueDictionary_ = new Dictionary<string, (string, string)>();
			foreach(var propertyInfo in info.GetType().GetProperties())
			{
				var description_ = Attribute.IsDefined(propertyInfo, typeof(DescriptionAttribute))
					? (Attribute.GetCustomAttribute(propertyInfo, typeof(DescriptionAttribute)) as DescriptionAttribute).Description
					: null;
				NameDescValueDictionary_[propertyInfo.Name] = (description_, propertyInfo.GetValue(info, null)?.ToString());
			}
			return NameDescValueDictionary_;
		}

		private string GetUSBDrive()
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

		public string GenerateXlsxFileName(string timestamp_)
		{
			var driveName_ = GetUSBDrive();
			var consoleSN_ = _ConsoleSN + Dash;
			var file_ = driveName_ + ReportHeader + consoleSN_ + timestamp_ + XlsxExtension;
			return file_;
		}

		public async Task<bool> CreateExcelFile(string excelFileName)
		{
			bool savedSuccessfully_ = false;
			ExcelPackage.LicenseContext = LicenseContext.Commercial;
			using(var package = new ExcelPackage(new FileInfo(excelFileName)))
			{
				try
				{
					var ablationSheet_ = package.Workbook.Worksheets.Add(GeneralInfoSheetTitle);
					FileName = excelFileName;
					await package.SaveAsAsync(excelFileName);
					savedSuccessfully_ = true;
				}
				catch(Exception e)
				{
					FieldServiceTrace.LogException(e);
					savedSuccessfully_ = false;
				}
			}

			return savedSuccessfully_;
		}

		public async Task<bool> AddSummaryToWorksheet(IAblationTestResult ablationTestResult, string excelFileName)
		{
			var created_ = false;
			var generalInformation_ = GetGeneralInformation(ablationTestResult);
			using(var package = new ExcelPackage(excelFileName))
			{
				try
				{
					// Section I
					var generalInformationDictionary_ = GetGeneralInformationDictionary(generalInformation_);
					var generalInfoWorksheet_ = package.Workbook.Worksheets.FirstOrDefault();

					generalInfoWorksheet_.Column(1).Width = 40;
					for(int colIndex_ = 2; colIndex_ < 12; colIndex_++)
					{
						generalInfoWorksheet_.Column(colIndex_).Width = 24;
					}

					var row_ = 1;
					var col_ = 1;
					const int colspan_ = 6;
					generalInfoWorksheet_.Row(row_).Height = 38;
					generalInfoWorksheet_.Cells[row_, col_, row_, colspan_].Style.Fill.SetBackground(Black);
					generalInfoWorksheet_.Cells[row_, col_, row_, colspan_].Style.Font.Color.SetColor(White);
					generalInfoWorksheet_.Cells[row_, col_, row_, colspan_].Style.Font.Bold = true;
					generalInfoWorksheet_.Cells[row_, col_, row_, colspan_].Style.Font.Size = 24;
					generalInfoWorksheet_.Cells[row_, col_ + 2].Value = generalInformationDictionary_[nameof(IGeneralInformation.Title)].Item1;

					row_ = 2;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_, row_, colspan_].Style.Fill.SetBackground(ColumnHeaderBackgroundColor);
					generalInfoWorksheet_.Cells[row_, col_, row_, colspan_].Style.Font.Bold = true;
					generalInfoWorksheet_.Cells[row_, col_, row_, colspan_].Style.Font.Size = 16;
					generalInfoWorksheet_.Cells[row_ + 1, col_, row_ + 1, colspan_].Style.Fill.SetBackground(LightGreen);
					generalInfoWorksheet_.Cells[row_ + 1, col_, row_ + 1, colspan_].Style.Font.Size = 15;
					generalInfoWorksheet_.Cells[row_ + 1, col_, row_ + 1, colspan_].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.HospitalName)].Item1;
					generalInfoWorksheet_.Cells[row_ + 1, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.HospitalName)].Item2;

					col_ = 3;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.ConsoleSN)].Item1;
					generalInfoWorksheet_.Cells[row_ + 1, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.ConsoleSN)].Item2;

					// Section II
					row_ = 5;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_, row_ + 2, col_].Style.Fill.SetBackground(ColumnHeaderBackgroundColor);
					generalInfoWorksheet_.Cells[row_, col_, row_ + 2, col_].Style.Font.Bold = true;
					generalInfoWorksheet_.Cells[row_, col_, row_ + 2, col_].Style.Font.Size = 14;

					generalInfoWorksheet_.Cells[row_, col_ + 1, row_ + 2, col_ + 1].Style.Fill.SetBackground(LightGreen);
					generalInfoWorksheet_.Cells[row_, col_ + 1, row_ + 2, col_ + 1].Style.Font.Size = 14;
					generalInfoWorksheet_.Cells[row_, col_ + 1, row_ + 2, col_ + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					generalInfoWorksheet_.Cells[14, 1, 14, 5].Style.Fill.SetBackground(White);

					for(int i = 5; i < 8; i++)
					{
						generalInfoWorksheet_.Cells[i, 1, i + 9, 2].Style.Border.BorderAround(ExcelBorderStyle.Hair);
					}

					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.GuiVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.GuiVersion)].Item2;

					row_++;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.DatabaseVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.DatabaseVersion)].Item2;

					row_++;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.ServiceToolVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.ServiceToolVersion)].Item2;

					// Section III
					row_ = 9;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_, row_ + 11, col_].Style.Fill.SetBackground(ColumnHeaderBackgroundColor);
					generalInfoWorksheet_.Cells[row_, col_, row_ + 11, col_].Style.Font.Bold = true;
					generalInfoWorksheet_.Cells[row_, col_, row_ + 11, col_].Style.Font.Size = 14;

					for(int i = 9; i < 21; i++)
					{
						generalInfoWorksheet_.Cells[i, 1, i + 9, 2].Style.Border.BorderAround(ExcelBorderStyle.Hair);
					}

					row_ = 9;
					col_ = 2;
					generalInfoWorksheet_.Cells[row_, col_, row_ + 11, col_].Style.Fill.SetBackground(LightGreen);
					generalInfoWorksheet_.Cells[row_, col_, row_ + 11, col_].Style.Font.Bold = false;
					generalInfoWorksheet_.Cells[row_, col_, row_ + 11, col_].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					generalInfoWorksheet_.Cells[row_, col_, row_ + 11, col_].Style.Font.Size = 14;

					row_ = 9;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.CmcuBootVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.CmcuBootVersion)].Item2;

					row_++;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.CmcuVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.CmcuVersion)].Item2;

					row_++;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.CpldVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.CpldVersion)].Item2;

					row_++;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.PmcuBootVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.PmcuBootVersion)].Item2;

					row_++;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.PmcuVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.PmcuVersion)].Item2;

					row_++;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.RmcuBootVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.RmcuBootVersion)].Item2;

					row_++;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.RepeaterVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.RepeaterVersion)].Item2;

					row_++;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.IcbBootVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.IcbBootVersion)].Item2;

					row_++;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.IcbVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.IcbVersion)].Item2;

					row_++;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.CatheterVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.CatheterVersion)].Item2;

					row_++;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.RcmcuBootVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.RcmcuBootVersion)].Item2;

					row_++;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = generalInformationDictionary_[nameof(IGeneralInformation.RemoteVersion)].Item1;
					generalInfoWorksheet_.Cells[row_, col_ + 1].Value = generalInformationDictionary_[nameof(IGeneralInformation.RemoteVersion)].Item2;

					// Section IV
					row_ = 22;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_, row_ + 4, col_].Style.Fill.SetBackground(ColumnHeaderBackgroundColor);
					generalInfoWorksheet_.Cells[row_, col_, row_ + 4, col_].Style.Font.Bold = true;
					generalInfoWorksheet_.Cells[row_, col_, row_ + 4, col_].Style.Font.Size = 15;
					generalInfoWorksheet_.Cells[row_, col_, row_ + 4, col_].Style.Border.BorderAround(ExcelBorderStyle.Hair);

					row_ = 23;
					col_ = 1;
					generalInfoWorksheet_.Cells[row_, col_].Value = CatheterIDDescription;
					generalInfoWorksheet_.Cells[row_ + 1, col_].Value = CatheterLotNumDescription;
					generalInfoWorksheet_.Cells[row_ + 2, col_].Value = CatheterSNDescription;
					generalInfoWorksheet_.Cells[row_ + 3, col_].Value = InflationSpeedDescription;

					row_ = 22;
					col_ = 1;
					for(int i = 1; i <= ablationTestResult.AblationSummaryList.Count; i++)
					{
						generalInfoWorksheet_.Cells[row_, col_ + i].Style.Fill.SetBackground(ColumnHeaderBackgroundColor);
						generalInfoWorksheet_.Cells[row_, col_ + i].Style.Font.Bold = true;
						generalInfoWorksheet_.Cells[row_, col_ + i].Style.Font.Size = 15;
						generalInfoWorksheet_.Cells[row_, col_ + i].Style.Border.BorderAround(ExcelBorderStyle.Hair);
						generalInfoWorksheet_.Cells[row_, col_ + i].Value = TreatmentText + i;
					}

					row_ = 23;
					col_ = 2;
					for(int i = 0; i < ablationTestResult.AblationSummaryList.Count; i++)
					{
						var ablationSummary_ = ablationTestResult.AblationSummaryList[i];
						generalInfoWorksheet_.Cells[row_, col_ + i].Value = ablationSummary_.CatheterID;
						generalInfoWorksheet_.Cells[row_ + 1, col_ + i].Value = ablationSummary_.CatheterLotNum;
						generalInfoWorksheet_.Cells[row_ + 2, col_ + i].Value = ablationSummary_.CatheterSN;
						generalInfoWorksheet_.Cells[row_ + 3, col_ + i].Value = ablationSummary_.InflationSpeed;

						for(int j = 0; j < 4; j++)
						{
							generalInfoWorksheet_.Cells[row_ + j, col_ + i].Style.Font.Size = 14;
							generalInfoWorksheet_.Cells[row_ + j, col_ + i].Style.Fill.SetBackground(LightGreen);
							generalInfoWorksheet_.Cells[row_ + j, col_ + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						}
					}

					created_ = true;
				}
				catch(Exception e)
				{
					created_ = false;
					FieldServiceTrace.LogException(e);
				}

				await package.SaveAsync();
			}

			return created_;
		}

		public async Task<bool> AddAblationDetailToWorksheet(int ablationId, IAblationTestResult ablationTestResult, string excelFileName)
		{
			var created_ = false;

			var ablationDictionary_ = ablationTestResult.AblationDetailsList
				.GroupBy(ablationDetails_ => ablationDetails_.ID)
				.ToDictionary(group_ => group_.Key, group_ => group_.ToList());

			var detailsList_ = ablationDictionary_[ablationId];

			using(var package = new ExcelPackage(excelFileName))
			{
				var detailWorksheet_ = package.Workbook.Worksheets.Add(AblationDetailsText + ablationId);
				detailWorksheet_.Row(1).Height = 32;
				detailWorksheet_.Cells[1, 1, 1, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
				detailWorksheet_.Cells[1, 1, 1, 17].Style.Fill.BackgroundColor.SetColor(Black);
				detailWorksheet_.Cells[1, 3].Value = AblationDetailsTitle;
				detailWorksheet_.Cells[1, 3].Style.Font.Color.SetColor(White);
				detailWorksheet_.Cells[1, 3].Style.Font.Bold = true;
				detailWorksheet_.Cells[1, 3].Style.Font.Size = 22;

				detailWorksheet_.Row(2).Style.Fill.PatternType = ExcelFillStyle.Solid;
				detailWorksheet_.Row(2).Style.Fill.BackgroundColor.SetColor(White);
				detailWorksheet_.Cells[2, 1, 2, 17].Style.Fill.BackgroundColor.SetColor(ColumnHeaderBackgroundColor);

				detailWorksheet_.Column(1).Width = 33;
				detailWorksheet_.Column(4).Width = 16;
				detailWorksheet_.View.FreezePanes(3, 1);

				var row_ = 3;
				foreach(var propInfo_ in typeof(AblationDetails).GetProperties())
				{
					detailWorksheet_.Cells[row_, 19].Value = propInfo_.Name;
					detailWorksheet_.Cells[row_, 19].Style.Fill.PatternType = ExcelFillStyle.Solid;
					detailWorksheet_.Cells[row_, 19].Style.Fill.BackgroundColor.SetColor(ColumnHeaderBackgroundColor);
					detailWorksheet_.Cells[row_, 19].Style.Font.Bold = true;
					detailWorksheet_.Cells[row_, 19].Style.Font.Size = 15;
					detailWorksheet_.Cells[row_, 19].Style.Border.BorderAround(ExcelBorderStyle.Hair);
					detailWorksheet_.Column(19).Width = 16;

					detailWorksheet_.Cells[row_, 20].Value = (propInfo_.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description ?? NullDescriptionString;
					detailWorksheet_.Cells[row_, 20].Style.Fill.PatternType = ExcelFillStyle.Solid;
					detailWorksheet_.Cells[row_, 20].Style.Fill.BackgroundColor.SetColor(AlternativeTextBackgroundColor);
					detailWorksheet_.Cells[row_, 20].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
					detailWorksheet_.Cells[row_, 20].Style.Font.Size = 14;
					detailWorksheet_.Column(20).Width = 38;
					row_++;
				}

				var col_ = 1;
				foreach(var propInfo_ in typeof(AblationDetails).GetProperties())
				{
					detailWorksheet_.Cells[2, col_].Value = propInfo_.Name;
					detailWorksheet_.Cells[2, col_].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					detailWorksheet_.Cells[2, col_].Style.Font.Size = 16;
					detailWorksheet_.Cells[2, col_].Style.Font.Bold = true;
					col_++;
				}

				int k = 1;
				foreach(var item_ in detailsList_)
				{
					int j = 1;
					foreach(var propertyInfo_ in item_.GetType().GetProperties())
					{
						if(propertyInfo_.Name == nameof(IAblationDetails.Timestamp))
						{
							var t_ = ((DateTime)propertyInfo_.GetValue(item_)).ToString(TimestampFormatString);
							detailWorksheet_.Cells[k + 2, j].Value = t_;
						}
						else
						{
							detailWorksheet_.Cells[k + 2, j].Value = propertyInfo_.GetValue(item_);
						}

						detailWorksheet_.Cells[k + 2, j].Style.Font.Size = 14;
						detailWorksheet_.Cells[k + 2, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						if(k % 2 == 0)
						{
							detailWorksheet_.Cells[k + 2, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
							detailWorksheet_.Cells[k + 2, j].Style.Fill.BackgroundColor.SetColor(AlternativeTextBackgroundColor);
						}
						detailWorksheet_.Cells[k + 2, j].Style.Border.BorderAround(ExcelBorderStyle.Hair);
						j++;
					}
					k++;
				}

				try
				{
					await package.SaveAsAsync(excelFileName);
					created_ = true;
				}
				catch(Exception e)
				{
					FieldServiceTrace.LogException(e);
					created_ = false;
				}
			}

			return created_;
		}

		public async Task<bool> AddFlowMeterDetailToWorksheet(int ablationId, IList<FlowRateData> flowMeterTestData, string excelFileName)
		{
			if(flowMeterTestData == null || flowMeterTestData.Count == 0)
			{
				return false;
			}

			var created = false;
			var flowMeterCheckSheetName = FlowMeterCheckWorksheet;
			using(var package = new ExcelPackage(excelFileName))
			{
				var flowMeterWorksheet = package.Workbook.Worksheets.Add(flowMeterCheckSheetName);

				var row = 1;
				var columnCounts = FlowMeterCheckColumns.Length;
				// Worksheet Header "Flow Meter Check Data" 
				flowMeterWorksheet.Row(row).Height = 32;
				flowMeterWorksheet.Cells[row, 1, row, columnCounts].Style.Fill.PatternType = ExcelFillStyle.Solid;
				flowMeterWorksheet.Cells[row, 1, row, columnCounts].Style.Fill.BackgroundColor.SetColor(Black);
				var headerCell = flowMeterWorksheet.Cells[row, 2];
				headerCell.Value = FlowMeterCheckTitle;
				headerCell.Style.Font.Color.SetColor(White);
				headerCell.Style.Font.Bold = true;
				headerCell.Style.Font.Size = 22;

				// Column header {"Index", "Timestamp", "Int. FM1", "Ext. FM1" }
				row++;
				flowMeterWorksheet.Row(row).Style.Fill.PatternType = ExcelFillStyle.Solid;
				flowMeterWorksheet.Row(row).Style.Fill.BackgroundColor.SetColor(White);
				flowMeterWorksheet.Cells[row, 1, row, columnCounts].Style.Fill.BackgroundColor.SetColor(ColumnHeaderBackgroundColor);

				for(int col = 0; col < columnCounts; ++col)
				{
					var cell = flowMeterWorksheet.Cells[row, col + 1];
					cell.Value = FlowMeterCheckColumns[col];
					cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					cell.Style.Font.Size = 16;
					cell.Style.Font.Bold = true;
				}

				// Data List
				flowMeterWorksheet.Column(1).Width = 10;
				flowMeterWorksheet.Column(2).Width = 33;
				flowMeterWorksheet.Column(3).Width = 15;
				flowMeterWorksheet.Column(4).Width = 15;
				flowMeterWorksheet.View.FreezePanes(3, 1);

				var index = 0;
				foreach(var flowRateData in flowMeterTestData)
				{
					++row;

					flowMeterWorksheet.Cells[row, 1].Value = ++index;
					flowMeterWorksheet.Cells[row, 2].Value = flowRateData.Timestamp.ToString(TimestampFormatString);
					flowMeterWorksheet.Cells[row, 3].Value = flowRateData.FM1;
					flowMeterWorksheet.Cells[row, 4].Value = flowRateData.FMExt;

					flowMeterWorksheet.Cells[row, 1, row, 4].Style.Font.Size = 14;
					flowMeterWorksheet.Cells[row, 1, row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					if(row % 2 == 0)
					{
						flowMeterWorksheet.Cells[row, 1, row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
						flowMeterWorksheet.Cells[row, 1, row, 4].Style.Fill.BackgroundColor.SetColor(AlternativeTextBackgroundColor);
					}

					flowMeterWorksheet.Cells[row, 1, row, 4].Style.Border.Right.Style = ExcelBorderStyle.Hair;
					flowMeterWorksheet.Cells[row, 1, row, 4].Style.Border.BorderAround(ExcelBorderStyle.Hair);
				}

				var ablationDetailSheetName = AblationDetailsText + ablationId;
				// We may want to move the Flow Meter Check worksheet before the ablation details worksheet 
				if(package.Workbook.Worksheets[ablationDetailSheetName] != null)
				{
					try
					{
						package.Workbook.Worksheets.MoveBefore(flowMeterCheckSheetName, ablationDetailSheetName);
					}
					catch(Exception)
					{
						FieldServiceTrace.Log($"{ablationDetailSheetName} worksheet does not exist, no need to move.");
					}
				}

				try
				{
					await package.SaveAsAsync(excelFileName);
					created = true;
				}
				catch(Exception e)
				{
					FieldServiceTrace.LogException(e);
					created = false;
				}
			}

			return created;
		}

		public async Task<bool> CreateIdleStateParameterSheet(IIdleStateCheckResult idleCheckResult)
		{
			var detailsList_ = idleCheckResult.Details;
			if(detailsList_ == null || detailsList_.Count <= 0)
			{
				return false;
			}
			ExcelPackage.LicenseContext = LicenseContext.Commercial;
			using(var package = new ExcelPackage(FileName))
			{
				var detailWorksheet_ = package.Workbook.Worksheets.Add(IdleStateCheckDetailsText);
				detailWorksheet_.Row(1).Height = 32;
				detailWorksheet_.Cells[1, 1, 1, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
				detailWorksheet_.Cells[1, 1, 1, 8].Style.Fill.BackgroundColor.SetColor(Black);
				detailWorksheet_.Cells[1, 2].Value = IdleStateDetailsTitle;
				detailWorksheet_.Cells[1, 2].Style.Font.Color.SetColor(White);
				detailWorksheet_.Cells[1, 2].Style.Font.Bold = true;
				detailWorksheet_.Cells[1, 2].Style.Font.Size = 22;

				detailWorksheet_.Row(2).Style.Fill.PatternType = ExcelFillStyle.Solid;
				detailWorksheet_.Row(2).Style.Fill.BackgroundColor.SetColor(White);
				detailWorksheet_.Cells[2, 1, 2, 8].Style.Fill.BackgroundColor.SetColor(ColumnHeaderBackgroundColor);

				detailWorksheet_.Column(1).Width = 33;
				detailWorksheet_.Column(4).Width = 16;
				detailWorksheet_.View.FreezePanes(3, 1);

				var row_ = 3;
				foreach(var propInfo_ in typeof(IdleStateCheckDetails).GetProperties())
				{
					detailWorksheet_.Cells[row_, 10].Value = propInfo_.Name;
					detailWorksheet_.Cells[row_, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
					detailWorksheet_.Cells[row_, 10].Style.Fill.BackgroundColor.SetColor(ColumnHeaderBackgroundColor);
					detailWorksheet_.Cells[row_, 10].Style.Font.Bold = true;
					detailWorksheet_.Cells[row_, 10].Style.Font.Size = 15;
					detailWorksheet_.Cells[row_, 10].Style.Border.BorderAround(ExcelBorderStyle.Hair);
					detailWorksheet_.Column(10).Width = 16;

					detailWorksheet_.Cells[row_, 11].Value = (propInfo_.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description ?? NullDescriptionString;
					detailWorksheet_.Cells[row_, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
					detailWorksheet_.Cells[row_, 11].Style.Fill.BackgroundColor.SetColor(AlternativeTextBackgroundColor);
					detailWorksheet_.Cells[row_, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
					detailWorksheet_.Cells[row_, 11].Style.Font.Size = 14;
					detailWorksheet_.Column(11).Width = 38;
					row_++;
				}

				int col_ = 1;
				foreach(var propInfo_ in typeof(IdleStateCheckDetails).GetProperties())
				{
					detailWorksheet_.Cells[2, col_].Value = propInfo_.Name;
					detailWorksheet_.Cells[2, col_].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					detailWorksheet_.Cells[2, col_].Style.Font.Size = 16;
					detailWorksheet_.Cells[2, col_].Style.Font.Bold = true;
					col_++;
				}

				int k = 1;
				foreach(var item_ in detailsList_)
				{
					int j = 1;
					foreach(var propertyInfo_ in item_.GetType().GetProperties())
					{
						if(propertyInfo_.Name == nameof(IAblationDetails.Timestamp))
						{
							var t_ = ((DateTime)propertyInfo_.GetValue(item_)).ToString(TimestampFormatString);
							detailWorksheet_.Cells[k + 2, j].Value = t_;
						}
						else
						{
							detailWorksheet_.Cells[k + 2, j].Value = propertyInfo_.GetValue(item_);
						}
						detailWorksheet_.Cells[k + 2, j].Style.Font.Size = 14;
						detailWorksheet_.Cells[k + 2, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						if(k % 2 == 0)
						{
							detailWorksheet_.Cells[k + 2, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
							detailWorksheet_.Cells[k + 2, j].Style.Fill.BackgroundColor.SetColor(AlternativeTextBackgroundColor);
						}
						detailWorksheet_.Cells[k + 2, j].Style.Border.BorderAround(ExcelBorderStyle.Hair);
						j++;
					}
					k++;
				}

				var created_ = false;
				try
				{
					await package.SaveAsAsync(FileName);
					created_ = true;
				}
				catch(Exception e)
				{
					created_ = false;
					FieldServiceTrace.LogException(e);
				}

				return created_;
			}
		}

		public async Task<bool> CreateReadyStateParameterSheet(IReadyStateCheckResult readyCheckResult)
		{
			var detailsList_ = readyCheckResult.Details;
			if(detailsList_ == null || detailsList_.Count <= 0)
			{
				return false;
			}

			using(var package = new ExcelPackage(FileName))
			{
				var detailWorksheet_ = package.Workbook.Worksheets.Add(ReadyStateCheckDetailsText);
				detailWorksheet_.Row(1).Height = 32;
				detailWorksheet_.Cells[1, 1, 1, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
				detailWorksheet_.Cells[1, 1, 1, 8].Style.Fill.BackgroundColor.SetColor(Black);
				detailWorksheet_.Cells[1, 2].Value = ReadyStateDetailsTitle;
				detailWorksheet_.Cells[1, 2].Style.Font.Color.SetColor(White);
				detailWorksheet_.Cells[1, 2].Style.Font.Bold = true;
				detailWorksheet_.Cells[1, 2].Style.Font.Size = 22;

				detailWorksheet_.Row(2).Style.Fill.PatternType = ExcelFillStyle.Solid;
				detailWorksheet_.Row(2).Style.Fill.BackgroundColor.SetColor(White);
				detailWorksheet_.Cells[2, 1, 2, 8].Style.Fill.BackgroundColor.SetColor(ColumnHeaderBackgroundColor);

				detailWorksheet_.Column(1).Width = 33;
				detailWorksheet_.Column(4).Width = 16;
				detailWorksheet_.View.FreezePanes(3, 1);

				var row_ = 3;
				foreach(var propInfo_ in typeof(ReadyStateCheckDetails).GetProperties())
				{
					detailWorksheet_.Cells[row_, 10].Value = propInfo_.Name;
					detailWorksheet_.Cells[row_, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
					detailWorksheet_.Cells[row_, 10].Style.Fill.BackgroundColor.SetColor(ColumnHeaderBackgroundColor);
					detailWorksheet_.Cells[row_, 10].Style.Font.Bold = true;
					detailWorksheet_.Cells[row_, 10].Style.Font.Size = 15;
					detailWorksheet_.Cells[row_, 10].Style.Border.BorderAround(ExcelBorderStyle.Hair);
					detailWorksheet_.Column(10).Width = 16;

					detailWorksheet_.Cells[row_, 11].Value = (propInfo_.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description ?? NullDescriptionString;
					detailWorksheet_.Cells[row_, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
					detailWorksheet_.Cells[row_, 11].Style.Fill.BackgroundColor.SetColor(AlternativeTextBackgroundColor);
					detailWorksheet_.Cells[row_, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
					detailWorksheet_.Cells[row_, 11].Style.Font.Size = 14;
					detailWorksheet_.Column(11).Width = 38;
					row_++;
				}

				int col_ = 1;
				foreach(var propInfo_ in typeof(ReadyStateCheckDetails).GetProperties())
				{
					detailWorksheet_.Cells[2, col_].Value = propInfo_.Name;
					detailWorksheet_.Cells[2, col_].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					detailWorksheet_.Cells[2, col_].Style.Font.Size = 16;
					detailWorksheet_.Cells[2, col_].Style.Font.Bold = true;
					col_++;
				}

				int k = 1;
				foreach(var item_ in detailsList_)
				{
					int j = 1;
					foreach(var propertyInfo_ in item_.GetType().GetProperties())
					{
						if(propertyInfo_.Name == nameof(IAblationDetails.Timestamp))
						{
							var t_ = ((DateTime)propertyInfo_.GetValue(item_)).ToString(TimestampFormatString);
							detailWorksheet_.Cells[k + 2, j].Value = t_;
						}
						else
						{
							detailWorksheet_.Cells[k + 2, j].Value = propertyInfo_.GetValue(item_);
						}
						detailWorksheet_.Cells[k + 2, j].Style.Font.Size = 14;
						detailWorksheet_.Cells[k + 2, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						if(k % 2 == 0)
						{
							detailWorksheet_.Cells[k + 2, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
							detailWorksheet_.Cells[k + 2, j].Style.Fill.BackgroundColor.SetColor(AlternativeTextBackgroundColor);
						}
						detailWorksheet_.Cells[k + 2, j].Style.Border.BorderAround(ExcelBorderStyle.Hair);
						j++;
					}
					k++;
				}

				var created_ = false;
				try
				{
					await package.SaveAsAsync(FileName);
					created_ = true;
				}
				catch(Exception e)
				{
					created_ = false;
					FieldServiceTrace.LogException(e);
				}
				return created_;
			}
		}
	}
}
