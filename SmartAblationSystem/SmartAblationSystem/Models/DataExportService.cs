using Communication;
using DataAccessLayer;
using FileSerializer;
using Ionic.Zip;
using Newtonsoft.Json;
using PDFReportsGenerator;
using Shared;
using SmartAblationSystem.Converters;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UniversalLoginManager;
using static LogSystem.AppLogConstants;
using static LogSystem.LogService;
using static SmartAblationSystem.UIConstants;

namespace SmartAblationSystem.Models
{
	public class DataExportService : IDataExportService
	{
		public DataExportService(
			UserType selectedUserType,
			DirectoryInfo destinationDirectoryInfo,
			List<ProcedureRecords> procedureRecords,
			string consoleSn = "")
		{
			SelectedUserType = selectedUserType;
			DestinationDirectoryInfo = destinationDirectoryInfo;
			SelectedProcedureRecordsList = procedureRecords;
			_consoleSn = consoleSn;
			_dataAccess = CommonViewModel.Current.Data.DataAccess;
		}

		public DataExportService(
			UserType selectedUserType,
			ProcedureRecords selectedProcedureRecords,
			DirectoryInfo destinationDirectoryInfo,
			bool patientInfoAnonymized,
			string password = EmptyString)
		{
			SelectedUserType = selectedUserType;
			SelectedProcedureRecords = selectedProcedureRecords;
			SelectedProcedureRecordsList = new List<ProcedureRecords>();
			DestinationDirectoryInfo = destinationDirectoryInfo;
			_patientInfoAnonymized = patientInfoAnonymized;
			_password = password;
			_dataAccess = CommonViewModel.Current.Data.DataAccess;
			_ablationDataFile = SelectedProcedureRecords?.Procedure?.Ablations?.FirstOrDefault()?.DataFile;
			_selectedAblationDataDetails = ExtractAblationDetails(SelectedProcedureRecords);
		}

		public DataExportService(
			UserType selectedUserType,
			List<ProcedureRecords> procedureRecordsList)
		{
			SelectedUserType = selectedUserType;
			SelectedProcedureRecordsList = procedureRecordsList;
			_dataAccess = CommonViewModel.Current.Data.DataAccess;
		}

		#region Argument Check

		private void CheckExportArguments()
		{
			if(SelectedUserType == UserType.Unknown)
			{
				throw new ArgumentException(nameof(SelectedUserType) + UnknownUserTypeExceptionMessage);
			}
			if(!DestinationDirectoryInfo.Exists)
			{
				try
				{
					Directory.CreateDirectory(DestinationDirectoryInfo.FullName);
				}
				catch(IOException ioe)
				{
					LogException(ioe);
					throw;
				}
			}
			//if(string.IsNullOrEmpty(_ablationDataFile))
			//{
			//	throw new ArgumentNullException(DataFileEmptyMessage);
			//}
			//if(!File.Exists(_ablationDataFile))
			//{
			//	throw new FileNotFoundException(DataFileNotExistMessage);
			//}

			if(_selectedAblationDataDetails == null)
			{
				throw new ArgumentNullException(DataFileNullExceptionMessage);
			}
		}

		#endregion Argument Check

		#region Exception Message

		private const string UnknownUserTypeExceptionMessage = " - Unknow user type.";
		private const string NullSelectedProcedureRecordsMessage = " - Null Procedure Records selected.";
		private const string ExportPathNotExistMessage = " - Export path does not exist.";
		private const string DataFileEmptyMessage = " - ProcedureRecords data file name is empty.";
		private const string DataFileNotExistMessage = " - Procedure data file not exist.";
		private const string DataFileNullExceptionMessage = " - Procedure data file null exception.";

		#endregion Exception Message

		#region Constants

		private const string EmptyString = "";
		private const string JsonExtension = @".json";
		private const string PDFTempFolder = @"PDFFiles\";
		private const string ExcelTempFolder = @"Files\";
		private const string JsonTempFolder = @"Files\";
		private const string CaseReportTempFolder = @"Files\";
		private const string ActionLogFile = "ActionLog_";
		private const string ErrorLogFile = "ErrorLog_";
		private const string LogFolder = "";

		private const string LogFilePrefix = "Log_";
		private const string ZipFileExtension = ".zip";
		private const string ZipFolder = @"ZipFile\";
		private const string DoctorTitle = "Dr. ";
		private const string Whitespace = " ";
		private const string DoubleDash = "--";
		private const string DateFormatString = "MM/dd/yyyy";
		private const string Gender = "GENDER";
		private const string Weight = "WEIGHT";
		private const string Height = "HEIGHT";
		private const string BMI = "BMI";
		private const string LogFileDateFormat = "00";

		#endregion Constants

		#region Interface Method Implementation

		public DirectoryInfo DestinationDirectoryInfo { get; }
		public UserType SelectedUserType { get; set; }
		public ProcedureRecords SelectedProcedureRecords { get; set; }
		public List<ProcedureRecords> SelectedProcedureRecordsList { get; }

		public FileInfo ExportJsonFile()
		{
			CheckExportArguments();

			var procedureInfo_ = GenerateProcedureInfo_();
			var treatmentNotes_ = GenerateTreatmentNotes_();
			var jsonFileType_ = new FileType(FileTypeEnum.Json);

			var jsonExportPath_ = GetSourcePath_(jsonFileType_);
			var jsonFileName_ = GenerateSourceFileName(jsonFileType_);
			var jsonTempFullFileName_ = Path.Combine(jsonExportPath_, jsonFileName_);

			var generateExportJsonFile_ = GenerateExportJsonFile_(new FileInfo(jsonTempFullFileName_));
			var zipFileName_ = GenerateFilePrefix_(SelectedUserType)
												 + SelectedProcedureRecords.Procedure.Description
												 + ZipFileExtension;

			var zippedJsonFile_ = ZipJsonFile_(generateExportJsonFile_);

			var moveZippedFile2_ = MoveZippedJsonFile_(zippedJsonFile_);

			return moveZippedFile2_;

			FileInfo GenerateExportJsonFile_(FileInfo jsonFileInfo)
			{
				if(!Directory.Exists(jsonExportPath_))
				{
					try
					{
						Directory.CreateDirectory(jsonExportPath_);
					}
					catch(Exception e)
					{
						LogException(e);
						throw new Exception(e.Message);
					}
				}
				try
				{
					_jsonManager = new JsonManager();
					var generatedJsonFileName_ = _jsonManager.SerializeAnalysisFile(
						new AnalysisData(_selectedAblationDataDetails, treatmentNotes_, procedureInfo_),
						jsonFileInfo.FullName);
					var result_ = new FileInfo(generatedJsonFileName_);
					return result_;
				}
				catch(Exception e)
				{
					LogException(e);
				}

				return null;
			}

			FileInfo ZipJsonFile_(FileInfo jsonFile)
			{
				var zipDirectoryInfo_ = new DirectoryInfo(Path.Combine(GetBasePath(), ZipFolder));
				if(!Directory.Exists(zipDirectoryInfo_.FullName))
				{
					try
					{
						Directory.CreateDirectory(zipDirectoryInfo_.FullName);
					}
					catch(Exception e)
					{
						LogException(e);
					}
				}

        var password_ = SelectedUserType == UserType.Bsc || SelectedUserType == UserType.BostonBsc
          ? GeneratePassword(string.Empty)
          : GeneratePassword(_password);

        var zippedJsonFileInfo_ = new FileInfo(Path.Combine(zipDirectoryInfo_.FullName, zipFileName_));
				using(var zip = new ZipFile())
				{
					zip.Password = password_;
					try
					{
						zip.AddFile(jsonFile.FullName, string.Empty);
						zip.Save(zippedJsonFileInfo_.FullName);
					}
					catch(Exception e)
					{
						LogException(e);
					}

					try
					{
						File.Delete(jsonFile.FullName);
					}
					catch(Exception e)
					{
						LogException(e);
					}

					return zippedJsonFileInfo_;
				}
			}

			FileInfo MoveZippedJsonFile_(FileInfo zippedJsonFileInfo)
			{
				FileInfo movedFileInfo_ = new FileInfo(Path.Combine(DestinationDirectoryInfo.FullName, zipFileName_));
				if(File.Exists(movedFileInfo_.FullName))
				{
					try
					{
						File.Delete(movedFileInfo_.FullName);
					}
					catch(Exception e)
					{
						LogException(e);
					}
				}

				try
				{
					zippedJsonFileInfo.CopyTo(movedFileInfo_.FullName);
				}
				catch(Exception e)
				{
					LogException(e);
				}

				if(File.Exists(zippedJsonFileInfo.FullName))
				{
					try
					{
						File.Delete(zippedJsonFileInfo.FullName);
					}
					catch(Exception e)
					{
						LogException(e);
					}
				}

				return movedFileInfo_;
			}
		}

		private string GeneratePassword(string password)
		{
			string pw_ = new string(PasswordUtils.DecryptPasscode(_credentialCode));
			if(CommonViewModel.Current.IsDoctor || CommonViewModel.Current.IsAdminUser || CommonViewModel.Current.IsUser)
			{
				pw_ += password;
			}
			return pw_;
		}

		private string HospitalName => _dataAccess.GetHospitalName() ?? string.Empty;
		internal MaliciousDataChangeModel MaliciousDataChangeModelInstance { get; set; } = MaliciousDataChangeModel.Instance;

		public FileInfo ExportExcelFile()
		{
			CheckExportArguments();

			var patientInfoList_ = GeneratePatientInfo_();
			var cn_ = _selectedAblationDataDetails[0]?[0] != null
				? _selectedAblationDataDetails[0][0].ConsoleSerialNumber
				: string.Empty;

			var generalInfo_ = GetGeneralInfoCSVHeader_(patientInfoList_, cn_) ??
												 throw new ArgumentNullException(nameof(GetGeneralInfoCSVHeader_));

			var consoleInfoCSVHeader_ = GetConsoleInfoCSVHeaders_()
																	?? throw new ArgumentNullException(nameof(GetConsoleInfoCSVHeaders_));

			var patientInfoCSVHeader_ = GetPatientInfoCSVHeader_(patientInfoList_) ??
																	throw new ArgumentNullException(nameof(GetPatientInfoCSVHeader_));

			var legendCSVHeader_ = GetLegendCSVHeaders();

			var headers_ = new List<List<(string, string)>>
			{
				generalInfo_,
				consoleInfoCSVHeader_,
				patientInfoCSVHeader_,
				legendCSVHeader_
			};

			var fileType_ = new FileType(FileTypeEnum.Excel);
			var ablationDetailsCSVHeader_ = GetAblationDetailsCSVHeaders_();
			var treatmentInfoCSVHeader_ = GetTreatmentCSVHeader_();
			var procedureData_ = new ProcedureData(_selectedAblationDataDetails);
			var file_ = DestinationDirectoryInfo.FullName + GenerateSourceFileNameWithoutExtension(fileType_);

			_csvManager = new CSVManager
			{
				User = SelectedUserType,
				DestinationPath = DestinationDirectoryInfo.FullName,
				Password = _password
			};

			_csvManager.GenerateAndWriteToFile(
				procedureData_,
				file_,
				ablationDetailsCSVHeader_,
				treatmentInfoCSVHeader_,
				headers_
				);

			return new FileInfo(file_ + fileType_.Extension);

			List<string> GeneratePatientInfo_()
			{
				if(SelectedProcedureRecords.Procedure?.Patient == null)
				{
					return null;
				}

				var patientInfo_ = new List<string>
				{
					HospitalName,
					SelectedProcedureRecords.Procedure.Patient.HospitalPatientId
				};

				if(SelectedUserType == UserType.Doctor || SelectedUserType == UserType.Admin || SelectedUserType == UserType.User)
				{
					try
          {
						var physician_ = _dataAccess.GetphysicianByID(SelectedProcedureRecords.Procedure.PhysicianID);
						patientInfo_.Add(DoctorTitle + physician_?.FirstName + Whitespace + physician_?.LastName);
					}
					catch(Exception e)
					{
						LogException(e);
						patientInfo_.Add(DoubleDash);
					}
				}
				else
				{
					patientInfo_.Add(DoubleDash);
				}
				patientInfo_.Add(SelectedProcedureRecords.Procedure.Patient.FirstName);
				patientInfo_.Add(SelectedProcedureRecords.Procedure.Patient.LastName);
				patientInfo_.Add(SelectedProcedureRecords.Procedure.Patient.DateOfBirth.ToString(DateFormatString));
				patientInfo_.Add(ProcedureConverter(SelectedProcedureRecords.Procedure.Patient, Gender));
				patientInfo_.Add(ProcedureToStringConverterobject(SelectedProcedureRecords.Procedure.Patient, null, Weight));
				patientInfo_.Add(ProcedureToStringConverterobject(SelectedProcedureRecords.Procedure.Patient, null, Height));
				patientInfo_.Add(ProcedureConverter(SelectedProcedureRecords.Procedure.Patient, BMI));

				patientInfo_.Add(SelectedProcedureRecords.Procedure.Diagnosis);
				patientInfo_.Add(SelectedProcedureRecords.Procedure.OutCome);
				patientInfo_.Add(SecondsToMinutesIntConvert(SelectedProcedureRecords.Procedure.SkinToSkinDuration));
				patientInfo_.Add(SelectedProcedureRecords.Procedure.PhysicianID.ToString());
				patientInfo_.Add(SelectedProcedureRecords.Procedure.Id.ToString());
				return patientInfo_;
			}

			List<(string, string)> GetGeneralInfoCSVHeader_(List<string> patientInfo, string consoleSN)
			{
				if(patientInfo == null)
				{
					throw new ArgumentNullException(nameof(patientInfo));
				}
				if(consoleSN == null)
				{
					throw new ArgumentNullException(nameof(consoleSN));
				}
				var generalInfoHeaderList = new List<(string, string)>();
				try
				{
					switch(SelectedUserType)
					{
						case UserType.Doctor:
						case UserType.Admin:
						case UserType.User:
							generalInfoHeaderList.Add((FieldToTextConverterobject("HospitalNameUID", null, null), patientInfo[0]));
							generalInfoHeaderList.Add((FieldToTextConverterobject("PhysicianNameUID", null, null), patientInfo[2]));
							generalInfoHeaderList.Add((FieldToTextConverterobject("PatientIdLabel", null, null), _patientInfoAnonymized ? DoubleDash : patientInfo[1]));
							generalInfoHeaderList.Add(("Console S/N", consoleSN));
							generalInfoHeaderList.Add(("In Body Time (min)", patientInfo[12]));
							generalInfoHeaderList.Add((FieldToTextConverterobject("Procedure ID", null, null), patientInfo[14]));
							break;
						case UserType.Bsc:
						case UserType.BostonBsc:
							generalInfoHeaderList.Add((FieldToTextConverterobject("HospitalNameUID", null, null), patientInfo[0]));
							generalInfoHeaderList.Add(("Console S/N", consoleSN));
							generalInfoHeaderList.Add(("In Body Time (min)", patientInfo[12]));
							generalInfoHeaderList.Add((FieldToTextConverterobject("Procedure ID", null, null), patientInfo[14]));
							break;
						case UserType.Unknown:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				catch(ArgumentOutOfRangeException e)
				{
					LogException(e);
				}

				return generalInfoHeaderList;
			}

			List<(string, string)> GetConsoleInfoCSVHeaders_()
			{
				var consoleVersion_ = CommonViewModel.Current.CreateAConsoleVersion()
															?? throw new ArgumentNullException("Exception to get Console Version.");

				var consoleInfoList_ = new List<(string, string)>
				{
					("GUI Version", CommonViewModel.Current.GuiVersion),
					("Database Version", CommonViewModel.Current.DatabaseVersion.ToString()),
					("CMCU Firmware", consoleVersion_.ControlFirmware),
					("CMCU Bootloader Firmware", consoleVersion_.ControlFirmwareBootLoader),
					("CPLD Firmware", consoleVersion_.CPLDFirmware),
					("PMCU Firmware", consoleVersion_.PatientFirmware),
					("PMCU Bootloader Firmware", consoleVersion_.PatientFirmwareBootLoader),
					("Repeater Firmware", consoleVersion_.RepeaterFirmware),
					("Repeater Bootloader Firmware", consoleVersion_.RepeaterFirmwareBootLoader),
					("ICB Firmware", consoleVersion_.ICBFirmware),
					("ICB Bootloader Firmware", consoleVersion_.ICBFirmwareBootLoader),
					("Remote Firmware", consoleVersion_.RemoteFirmware),
					("Remote Bootloader Firmware", consoleVersion_.RemoteFirmwareBootLoader),
					("Catheter Firmware", consoleVersion_.CatheterFirmware)
				};

				return consoleInfoList_;
			}

			List<(string, string)> GetPatientInfoCSVHeader_(List<string> patientInfo)
			{
				var patientInfoHeaderList = new List<(string, string)>();
				try
				{
					patientInfoHeaderList.Add((PatientFirstName, _patientInfoAnonymized ? DoubleDash : patientInfo[3]));
					patientInfoHeaderList.Add((PatientLastName, _patientInfoAnonymized ? DoubleDash : patientInfo[4]));
					patientInfoHeaderList.Add((PatientGender, _patientInfoAnonymized ? DoubleDash : patientInfo[6]));
					patientInfoHeaderList.Add((PatientBirthDate, _patientInfoAnonymized ? DoubleDash : patientInfo[5]));
					patientInfoHeaderList.Add((PatientHeight + ToiseUnitToTextConverterobject(null, null, null),
						_patientInfoAnonymized ? DoubleDash : patientInfo[8]));
					patientInfoHeaderList.Add((PatientWeight + ScaleUnitToTextConverterobject(null, null, null),
						_patientInfoAnonymized ? DoubleDash : patientInfo[7]));
					patientInfoHeaderList.Add((PatientBMI, _patientInfoAnonymized ? DoubleDash : patientInfo[9]));
					patientInfoHeaderList.Add((FieldToTextConverterobject("ProcedureDiagnosisUID", null, null), _patientInfoAnonymized ? DoubleDash : patientInfo[10]));
					patientInfoHeaderList.Add((FieldToTextConverterobject("ProcedureOutcomeUID", null, null), _patientInfoAnonymized ? DoubleDash : patientInfo[11]));
				}
				catch(ArgumentNullException aex)
				{
					LogException(aex);
					throw;
				}
				catch(ArgumentOutOfRangeException ex)
				{
					LogException(ex);
					throw;
				}
				return patientInfoHeaderList;
			}

			List<(string, string)> GetLegendCSVHeaders()
			{
				var legendTable_ = new List<(string, string)>();

				switch(SelectedUserType)
				{
					case UserType.Bsc:
					case UserType.BostonBsc:
						legendTable_.Add(("Timestamp", "Timestamp"));
						legendTable_.Add(("Time", FieldToTextConverterobject("TimeUID", null, null) + " (" + FieldToTextConverterobject("SecLabel", null, null) + ")"));
						legendTable_.Add(("ID", "Ablation ID"));
						legendTable_.Add(("State", "System State"));
						legendTable_.Add(("TR", FieldToTextConverterobject("TemperatureRateUID", null, null) + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + "/" + FieldToTextConverterobject("SecLabel", null, null) + ")"));
						legendTable_.Add(("TC1", "Balloon Temperature (TC1)" + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + ")"));
						legendTable_.Add(("TC1CJ", "TC1 Cold Junction" + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + ")"));
						legendTable_.Add(("PT1", "Tank Pressure (psig)"));
						legendTable_.Add(("PT2", "Injection Pressure (psig)"));
						legendTable_.Add(("PT3", "Return Line Pressure (psia)"));
						legendTable_.Add(("PT4", "Vacuum Line Pressure (psia)"));
						legendTable_.Add(("PT5", "Scavenging Line Pressure (psia)"));
						legendTable_.Add(("PS1", "Vent Line Switch (0/1)"));
						legendTable_.Add(("FM1", "Flow (sccm)"));
						legendTable_.Add(("TS1", "Sub-Cooler Temperature" + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + ")"));
						legendTable_.Add(("TN2O", "Not Used"));
						legendTable_.Add(("LC1", "Tank Weight (lbs)"));
						legendTable_.Add(("IBP", "Inner Balloon Pressure (psig)"));
						legendTable_.Add(("OBP", "Outer Balloon Pressure (psig)"));
						legendTable_.Add(("TS1CJ", "TS1 Cold Junction" + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + ")"));
						legendTable_.Add(("IPWM", "Injection PWM (%)"));
						legendTable_.Add(("BPWM", "Balloon PWM (%)"));
						legendTable_.Add(("DMS (G)", "DMS Value (G)"));
						legendTable_.Add(("DMS %", "DMS Value (%)"));
						legendTable_.Add(("BDI", "Blood Detection Index"));
						legendTable_.Add(("ESO Temp", "Displayed Esophagus Temperature"));
						legendTable_.Add(("ESO CH1", "Circa Channel 1 Temperature"));
						legendTable_.Add(("ESO CH2", "Circa Channel 2 Temperature"));
						legendTable_.Add(("ESO CH3", "Circa Channel 3 Temperature"));
						legendTable_.Add(("ESO CH4", "Circa Channel 4 Temperature"));
						legendTable_.Add(("ESO CH5", "Circa Channel 5 Temperature"));
						legendTable_.Add(("ESO CH6", "Circa Channel 6 Temperature"));
						legendTable_.Add(("ESO CH7", "Circa Channel 7 Temperature"));
						legendTable_.Add(("ESO CH8", "Circa Channel 8 Temperature"));
						legendTable_.Add(("ESO CH9", "Circa Channel 9 Temperature"));
						legendTable_.Add(("ESO CH10", "Circa Channel 10 Temperature"));
						legendTable_.Add(("ESO CH11", "Circa Channel 11 Temperature"));
						legendTable_.Add(("ESO CH12", "Circa Channel 12 Temperature"));
						legendTable_.Add(("ESO CH13", "Series 400 Temperature"));
						break;
					case UserType.Doctor:
					case UserType.Admin:
					case UserType.User:
						legendTable_.Add(("ESO CH1", "Circa Channel 1 Temperature"));
						legendTable_.Add(("ESO CH2", "Circa Channel 2 Temperature"));
						legendTable_.Add(("ESO CH3", "Circa Channel 3 Temperature"));
						legendTable_.Add(("ESO CH4", "Circa Channel 4 Temperature"));
						legendTable_.Add(("ESO CH5", "Circa Channel 5 Temperature"));
						legendTable_.Add(("ESO CH6", "Circa Channel 6 Temperature"));
						legendTable_.Add(("ESO CH7", "Circa Channel 7 Temperature"));
						legendTable_.Add(("ESO CH8", "Circa Channel 8 Temperature"));
						legendTable_.Add(("ESO CH9", "Circa Channel 9 Temperature"));
						legendTable_.Add(("ESO CH10", "Circa Channel 10 Temperature"));
						legendTable_.Add(("ESO CH11", "Circa Channel 11 Temperature"));
						legendTable_.Add(("ESO CH12", "Circa Channel 12 Temperature"));
						legendTable_.Add(("ESO CH13", "Series 400 Temperature"));
						break;
					case UserType.Unknown:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				return legendTable_;
			}

			List<string> GetAblationDetailsCSVHeaders_()
			{
				List<string> ablationdetailslist = new List<string>();

				if(SelectedUserType == UserType.Doctor || SelectedUserType == UserType.Admin || SelectedUserType == UserType.User)
				{
					ablationdetailslist.Add(FieldToTextConverterobject("TimeStampUID", null, null));
					ablationdetailslist.Add(FieldToTextConverterobject("TimeUID", null, null) + " (" + FieldToTextConverterobject("SecLabel", null, null) + ")");
					ablationdetailslist.Add(FieldToTextConverterobject("AblationIDUID", null, null));
					ablationdetailslist.Add(FieldToTextConverterobject("SystemStateUID", null, null));
					ablationdetailslist.Add(FieldToTextConverterobject("TemperatureRateUID", null, null) + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + "/" + FieldToTextConverterobject("SecLabel", null, null) + ")");
					ablationdetailslist.Add("Balloon Temperature" + " (TC1)" + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + ")");
					ablationdetailslist.Add($"DMS{Environment.NewLine}(G)");
					ablationdetailslist.Add($"DMS{Environment.NewLine}(%)");
					ablationdetailslist.Add($"ESO");
					ablationdetailslist.Add($"ESO{Environment.NewLine}CH1");
					ablationdetailslist.Add($"ESO{Environment.NewLine}CH2");
					ablationdetailslist.Add($"ESO{Environment.NewLine}CH3");
					ablationdetailslist.Add($"ESO{Environment.NewLine}CH4");
					ablationdetailslist.Add($"ESO{Environment.NewLine}CH5");
					ablationdetailslist.Add($"ESO{Environment.NewLine}CH6");
					ablationdetailslist.Add($"ESO{Environment.NewLine}CH7");
					ablationdetailslist.Add($"ESO{Environment.NewLine}CH8");
					ablationdetailslist.Add($"ESO{Environment.NewLine}CH9");
					ablationdetailslist.Add($"ESO{Environment.NewLine}CH10");
					ablationdetailslist.Add($"ESO{Environment.NewLine}CH11");
					ablationdetailslist.Add($"ESO{Environment.NewLine}CH12");
					ablationdetailslist.Add($"ESO{Environment.NewLine}CH13");
				}
				else if(SelectedUserType == UserType.BostonBsc || SelectedUserType == UserType.Bsc)
				{
					ablationdetailslist.Add("Timestamp");
					ablationdetailslist.Add("Time");
					ablationdetailslist.Add("ID");
					ablationdetailslist.Add("State");
					ablationdetailslist.Add("TR");
					ablationdetailslist.Add("TC1"); // 
					ablationdetailslist.Add("TC1CJ"); // PMCU CJ READING
					ablationdetailslist.Add("PT1"); // 
					ablationdetailslist.Add("PT2");
					ablationdetailslist.Add("PT3");
					ablationdetailslist.Add("PT4");
					ablationdetailslist.Add("PT5");
					ablationdetailslist.Add("PS1");
					ablationdetailslist.Add("FM1");
					ablationdetailslist.Add("TS1");
					ablationdetailslist.Add("TN2O");
					ablationdetailslist.Add("LC1");
					ablationdetailslist.Add("IBP");
					ablationdetailslist.Add("OBP");
					ablationdetailslist.Add("TS1CJ"); // CMCU CJ READING
					ablationdetailslist.Add("IPWM"); // PWMINJ
					ablationdetailslist.Add("BPWM");
                    ablationdetailslist.Add($"DMS{Environment.NewLine}(G)");
                    ablationdetailslist.Add($"DMS{Environment.NewLine}(%)");
                    ablationdetailslist.Add("BDI");
                    ablationdetailslist.Add($"ESO");
                    ablationdetailslist.Add($"ESO{Environment.NewLine}CH1");
                    ablationdetailslist.Add($"ESO{Environment.NewLine}CH2");
                    ablationdetailslist.Add($"ESO{Environment.NewLine}CH3");
                    ablationdetailslist.Add($"ESO{Environment.NewLine}CH4");
                    ablationdetailslist.Add($"ESO{Environment.NewLine}CH5");
                    ablationdetailslist.Add($"ESO{Environment.NewLine}CH6");
                    ablationdetailslist.Add($"ESO{Environment.NewLine}CH7");
                    ablationdetailslist.Add($"ESO{Environment.NewLine}CH8");
                    ablationdetailslist.Add($"ESO{Environment.NewLine}CH9");
                    ablationdetailslist.Add($"ESO{Environment.NewLine}CH10");
                    ablationdetailslist.Add($"ESO{Environment.NewLine}CH11");
                    ablationdetailslist.Add($"ESO{Environment.NewLine}CH12");
                    ablationdetailslist.Add($"ESO{Environment.NewLine}CH13");
                }

				return ablationdetailslist;
			}

			List<string> GetTreatmentCSVHeader_()
			{
				List<string> doctortreatmentList = new List<string>();

				doctortreatmentList.Add(FieldToTextConverterobject("AblationSiteLabel", null, null));
				doctortreatmentList.Add("Balloon Size" + " (mm)");
				doctortreatmentList.Add("Minimum Temperature" + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + ")");
				doctortreatmentList.Add(FieldToTextConverterobject("CoolingTimerSetpointUID", null, null) + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + ")");
				doctortreatmentList.Add(FieldToTextConverterobject("CoolingTimeUID", null, null) + " (" + FieldToTextConverterobject("SecLabel", null, null) + ")");
				doctortreatmentList.Add(FieldToTextConverterobject("AblationDurationSetpointUID", null, null) + " (" + FieldToTextConverterobject("SecLabel", null, null) + ")");
				doctortreatmentList.Add(FieldToTextConverterobject("timetoveinisolationUID", null, null) + " (" + FieldToTextConverterobject("SecLabel", null, null) + ")");
				doctortreatmentList.Add("Temperature at Isolation" + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + ")");
				doctortreatmentList.Add("Time Since Isolation" + " (" + FieldToTextConverterobject("SecLabel", null, null) + ")");
				doctortreatmentList.Add(FieldToTextConverterobject("ThawTimeUID", null, null) + " (" + FieldToTextConverterobject("SecLabel", null, null) + ")");
				doctortreatmentList.Add(FieldToTextConverterobject("ThawTimerSetpointUID", null, null) + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + ")");
        doctortreatmentList.Add("Total Thawing Time (sec)");
        doctortreatmentList.Add(FieldToTextConverterobject("CatheterIDUID", null, null));
				doctortreatmentList.Add(FieldToTextConverterobject("CatheterLotNumUID", null, null));
				doctortreatmentList.Add(FieldToTextConverterobject("CatheterSerialUID", null, null));
        doctortreatmentList.Add("Catheter Container");
				doctortreatmentList.Add(FieldToTextConverterobject("MinimumDMSValueUID", null, null) + " (%)");
				doctortreatmentList.Add("Minimum Esophagus Temperature");
				
				return doctortreatmentList;
			}

			string ProcedureConverter(object value, string parameter)
			{
				var procedureToStringConverter_ = new ProcedureToStringConverter();
				return procedureToStringConverter_.Convert(value, null, parameter, null).ToString();
			}

			string ProcedureToStringConverterobject(object value, System.Type targetType, object parameter)
			{
				string returnresult = Whitespace;
				var procedureToStringConverter_ = new ProcedureToStringConverter();
				returnresult = procedureToStringConverter_.Convert(value, targetType, parameter, null).ToString();

				if(returnresult == "00")
					returnresult = DoubleDash;

				return returnresult;
			}

			string SecondsToMinutesIntConvert(int sec)
			{
				return (sec / 60).ToString();
			}

			string FieldToTextConverterobject(object value, System.Type targetType, object parameter)
			{
				var fieldtotextValue = new FieldToTextConverter();
				return fieldtotextValue.Convert(value, targetType, parameter, null).ToString();
			}

			string ToiseUnitToTextConverterobject(object value, System.Type targetType, object parameter)
			{
				var fieldtotextValue = new ToiseUnitToTextConverter();
				return fieldtotextValue.Convert(value, targetType, parameter, null).ToString();
			}

			string ScaleUnitToTextConverterobject(object value, System.Type targetType, object parameter)
			{
				var fieldtotextValue = new ScaleUnitToTextConverter();
				return fieldtotextValue.Convert(value, targetType, parameter, null).ToString();
			}
		}

		public FileInfo ExportPdfFile()
		{
			CheckExportArguments();

			var generatedPdfFileName_ = GeneratePdfFile().FullName;
			var destinationFile_ = Path.Combine(DestinationDirectoryInfo.FullName, GenerateSourceFileName(new FileType(FileTypeEnum.PDF)));

			_pdfConversion = new PDFConversion();
			var tempResult_ = _pdfConversion.Encrypt(generatedPdfFileName_, destinationFile_, _password);

			if(File.Exists(generatedPdfFileName_))
			{
				File.Delete(generatedPdfFileName_);
			}

			return new FileInfo(tempResult_);
		}

		public FileInfo GeneratePdfFile()
		{
			return new FileInfo(GeneratePdfReport_());

			string GeneratePdfReport_()
			{
				var ablationSummary_ = GetTheAblationSummary(_selectedAblationDataDetails);
				ProcedureRecords procedure_;

				if(MaliciousDataChangeModelInstance.IsDataEdited)
				{
					DeleteCurrentPDFs(SelectedProcedureRecords.Procedure.Description);
					procedure_ = GetLatestTreatmentNote(SelectedProcedureRecords);
				}
				else
				{
					procedure_ = SelectedProcedureRecords;
				}
				
				var ablationReportList = GetAblationReportListByProcedureRecord(_selectedAblationDataDetails);
				CatheterTypeList = GetCatheterTypeList(ablationReportList);
				var logList = GetProcedureLogsList(procedure_.Procedure.Id);

				var generatedPdfFile_ = GetBasePath() + PDFTempFolder + GenerateSourceFileName(new FileType(FileTypeEnum.PDF));

				if(File.Exists(generatedPdfFile_) && !procedure_.Procedure.IsDataEdited)
				{
					return generatedPdfFile_;
				}

				_pdfDataManager = new PDFDataManager();
				try
				{
					_pdfDataManager.GeneratePDFFile(
						_selectedAblationDataDetails,
						procedure_,
						UserTypeToAccessTypeConverter(SelectedUserType),
						HospitalName,
						ablationReportList,
						ablationSummary_,
						WeightUnit,
						logList,
						CurrentPhysician,
						_patientInfoAnonymized,
						CatheterTypeList
					);
				}
				catch(Exception e)
				{
					LogException(e);
					throw;
				}

				return File.Exists(generatedPdfFile_) ? generatedPdfFile_ : null;
			}

			AblationSummary GetTheAblationSummary(List<List<AblationDataDetails>> ListAblationDetails)
			{
				int duration = 0;

				//Clears all existing data
				AblationSummary AblationSummary = new AblationSummary();
				int ablationSite = (int)AblationSiteEnum.OTHER;

				if(AblationSummary != null && ListAblationDetails?.Count > 0)
				{
					// Only compute the duration in Ablation (not thawing)
					// Generate/compute the Ablation duration (depending of the site) for each Ablations in the procedure.
					foreach(List<AblationDataDetails> listAblationDetails in ListAblationDetails)
					{
						if(listAblationDetails != null)
						{
							//Compute the Ablation duration (stop the increment when in Thawing)
							foreach(AblationDataDetails ablationDetails in listAblationDetails)
							{
								//Keep duration for ablation only
								if(ablationDetails.SystemState != (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING &&
										ablationDetails.SystemState != (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE &&
										ablationDetails.SystemState != (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY &&
										ablationDetails.SystemState != (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION &&
										ablationDetails.SystemState != (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION &&
										ablationDetails.SystemState != (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN)
								{
									duration = ablationDetails.ID;
								}

								ablationSite = ablationDetails.AblationSite;
							}
							switch(ablationSite)
							{
								case (int)AblationSiteEnum.RSPV:
									AblationSummary.TotalRSPV++;
									AblationSummary.TotalRSPVDuration += duration;
									break;

								case (int)AblationSiteEnum.RIPV:
									AblationSummary.TotalRIPV++;
									AblationSummary.TotalRIPVDuration += duration;
									break;

								case (int)AblationSiteEnum.LSPV:
									AblationSummary.TotalLSPV++;
									AblationSummary.TotalLSPVDuration += duration;
									break;

								case (int)AblationSiteEnum.LIPV:
									AblationSummary.TotalLIPV++;
									AblationSummary.TotalLIPVDuration += duration;
									break;

								case (int)AblationSiteEnum.LCPV:
                  AblationSummary.TotalLCPV++;
                  AblationSummary.TotalLCPVDuration += duration;
                  break;

                case (int)AblationSiteEnum.RMPV:
                  AblationSummary.TotalRMPV++;
                  AblationSummary.TotalRMPVDuration += duration;
                  break;

                case (int)AblationSiteEnum.OTHER:
									AblationSummary.TotalOther++;
									AblationSummary.TotalOtherDuration += duration;
									break;
							}
						}
					}
				}
				return AblationSummary;
			}

			ProcedureRecords GetLatestTreatmentNote(ProcedureRecords procRec)
			{
				List<AblationReportChanges> ablationReportChanges_ = MaliciousDataChangeModelInstance.AblationReportChanges;
				foreach(var ablation in procRec.Procedure.Ablations)
				{
					ablation.TreatmentNote = _dataAccess.GetAblationNote(ablation.AblationNumber, procRec.Procedure.Id);
				}
				return procRec;
			}

			List<AblationReport> GetAblationReportListByProcedureRecord(List<List<AblationDataDetails>> lst)
			{
				var result_ = new List<AblationReport>();
				foreach(var ablationDataDetail in lst)
				{
					var duration = ablationDataDetail.Count(
						a => a.SystemState == (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION || a.SystemState == (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);

					var MinDMSValue = ablationDataDetail[ablationDataDetail.Count - 1].MinimumDiaphragmMovementValue;

					var lastTimeToThaw = 0;
					if(ablationDataDetail[ablationDataDetail.Count - 1].TC1Reading >= ablationDataDetail[ablationDataDetail.Count - 1].ThawTimerToTemperature)
					{
						lastTimeToThaw = ablationDataDetail[ablationDataDetail.Count - 1].TimeToThaw;
					}

          int totalThawingTime_ = ablationDataDetail.Count(x =>
            x.SystemState == (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);

          var ablationSiteValue = ablationDataDetail[ablationDataDetail.Count - 1].AblationSite;
					var report_ = new AblationReport(
						ablationDataDetail[ablationDataDetail.Count - 1].AblationID.ToString(),
						Enum.IsDefined(typeof(AblationSiteEnum), ablationSiteValue) ? (AblationSiteEnum)ablationSiteValue : AblationSiteEnum.UNKNOWN,
						duration,
						ablationDataDetail[ablationDataDetail.Count - 1].TemperatureRate,
						ablationDataDetail[ablationDataDetail.Count - 1].MaxTemperatureRate,
						ablationDataDetail[ablationDataDetail.Count - 1].TimeToTargetTemperature,
						ablationDataDetail[ablationDataDetail.Count - 1].TimeToVeinIsolation,
						ablationDataDetail[ablationDataDetail.Count - 1].RequiredTargetTemperature,
						lastTimeToThaw,
						ablationDataDetail[ablationDataDetail.Count - 1].ThawTimerToTemperature,
						ablationDataDetail[ablationDataDetail.Count - 1].CatheterId,
						ablationDataDetail[ablationDataDetail.Count - 1].CatheterLot,
						_dataAccess.GetAblationNote(ablationDataDetail[ablationDataDetail.Count - 1].AblationID, ablationDataDetail[ablationDataDetail.Count - 1].ProcedureId),
						ablationDataDetail[ablationDataDetail.Count - 1].ProcedureId,
						MinDMSValue,
						ablationDataDetail[ablationDataDetail.Count - 1].MinimumEsophagusTemperatureValue,
						ablationDataDetail[ablationDataDetail.Count - 1].Error,
						ablationDataDetail[0].TimeStamp,
						ablationDataDetail[ablationDataDetail.Count - 1].IsUsedForEngineering,
						ablationDataDetail[ablationDataDetail.Count - 1].BalloonSize,
						totalThawingTime_,
						ablationDataDetail[ablationDataDetail.Count - 1].TimeSinceVeinIsolation,
						ablationDataDetail[ablationDataDetail.Count - 1].TemperatureAtIsolation
					);
					result_.Add(report_);
				}

				return result_;
			}

			List<string> GetProcedureLogsList(int PId)
			{
				ObservableCollection<ProcedureLog> PLog = CommonViewModel.Current.Data.DataAccess.GetAllProcedureLogsAccordingToProcedureID(PId);
				int procdureLogCount_ = PLog.Count;
				var procdureLogstring_ = new List<string>();
				for(int i = 0; i < procdureLogCount_; i++)
				{
					procdureLogstring_.Add(PLog[i].LogDate.ToString() + " : " + PLog[i].Description + "  from " + PLog[i].PreviousInformation + " to " + PLog[i].CommittedInformation);
				}
				return procdureLogstring_;
			}
		}

		public FileInfo ExportCaseReportFile()
		{
			var fileInfo_ = new FileInfo("");

			return fileInfo_;
		}

		public FileInfo ExportLogFile(IDataExportable context, CancellationToken cancellationToken)
		{
			if (CheckCancellation(cancellationToken))
			{
				context.LogMessage = "Exporting log files canceled.";
				return null;
			}

			var logDirectoryInfo_ = new DirectoryInfo(Path.Combine(GetBasePath(), ZipFolder));
			if(Directory.Exists(logDirectoryInfo_.FullName))
			{
				DeleteFilesInFolder(logDirectoryInfo_);
			}
			else
			{
				CreateFolder(logDirectoryInfo_);
			}

			context.LogProgressBarValue = 0;
			var timeStamp_ = CreateLogFileTimeStamp();

			context.LogMessage = "Exporting Action Log...";
			var actionLogFileInfo_ = GenerateActionLogFile(timeStamp_);
			if(File.Exists(actionLogFileInfo_.FullName))
			{
				context.ActionLogExported = true;
				context.LogMessage = "Action Log Exported.";
				context.LogProgressBarValue++;
			}

			if(CheckCancellation(cancellationToken))
			{
				context.LogMessage = "Exporting log files canceled.";
				return null;
			}

			context.LogMessage = "Exporting Error Log...";
			var errorLogFileInfo_ = GenerateErrorLogFile(timeStamp_);
			{
				context.ErrorLogExported = true;
				context.LogMessage = "Error Log Exported.";
				context.LogProgressBarValue++;
			}

			if(CheckCancellation(cancellationToken))
			{
				context.LogMessage = "Exporting log files canceled.";
				return null;
			}

			context.LogMessage = "Exporting SmartFreeze Log...";
			var smartFreezeLogInfo_ = GenerateSmartFreezeLogFile(timeStamp_);
			if(File.Exists(smartFreezeLogInfo_.FullName))
			{
				context.SmartFreezeLogExported = true;
				context.LogMessage = "SmartFreeze Log Exported.";
				context.LogProgressBarValue++;
			}

			if(CheckCancellation(cancellationToken))
			{
				context.LogMessage = "Exporting log files canceled.";
				return null;
			}

			context.LogMessage = "Exporting Windows Event Log...";
			var winLogFileInfo_ = GenerateWinLogFile(timeStamp_);
			if(File.Exists(winLogFileInfo_.FullName))
			{
				context.WinEventLogExported = true;
				context.LogMessage = "Windows Event Log Exported.";
				context.LogProgressBarValue++;
			}

			if(CheckCancellation(cancellationToken))
			{
				context.LogMessage = "Exporting log files canceled.";
				return null;
			}

      context.LogMessage = "Exporting log procedure data...";

      // add excel files for selected procedures.
			CreateExcelForLog(context, cancellationToken);

			// add json files for selected procedures.
			CreateJsonForLog(context, cancellationToken);

			if(CheckCancellation(cancellationToken))
			{
				context.LogMessage = "Exporting log files canceled.";
				return null;
			}

			context.LogMessage = "Log procedure data exported.";
			var zippedLogFileName_ = LogFilePrefix + _consoleSn + Underscore + timeStamp_ + ZipFileExtension;

			if(CheckCancellation(cancellationToken))
			{
				context.LogMessage = "Exporting log files canceled.";
				return null;
			}

			var password_ = new string(PasswordUtils.DecryptPasscode(_credentialCode));
			var dZipFile_ = Path.Combine(DestinationDirectoryInfo.FullName, zippedLogFileName_);

			context.LogMessage = "Zipping Log Files...";
			var fileAction_ = new FileAction();
			fileAction_.ZipFilesWithPassword(logDirectoryInfo_.FullName, password_, dZipFile_);
			context.LogProgressBarValue++;
			context.LogMessage = "Log Files Exported.";

			if(CheckCancellation(cancellationToken))
			{
				context.LogMessage = "Exporting log files canceled.";
				return null;
			}

			DeleteFilesInFolder(logDirectoryInfo_);
			context.SaveLogSelected = false;
			var destinationFile_ = new FileInfo(dZipFile_);
			return destinationFile_;
		}

		public async Task PrintPdfReport()
		{
			if(SelectedProcedureRecordsList?.Count == 0) return;
			var fileInfoList_ = new List<FileInfo>();
			if (fileInfoList_ == null) throw new ArgumentNullException(nameof(fileInfoList_));

			var generatePdfFilesTask_ = Task.Run(() =>
			{
				foreach(var procedureRecord_ in SelectedProcedureRecordsList)
				{
					SelectedProcedureRecords = procedureRecord_;
					_selectedAblationDataDetails = ExtractAblationDetails(procedureRecord_);
					var reportFileToPrint_ = GeneratePdfFile();
					fileInfoList_.Add(reportFileToPrint_);
				}
			});
			await generatePdfFilesTask_;

			var printerManager_ = new PrinterManager();
      var printPdfFilesTask_ = Task.Run(() => 
			{
        foreach (var fileInfo_ in fileInfoList_)
        {
          if(!File.Exists(fileInfo_.FullName)) continue;
          printerManager_.PrinterStatus(fileInfo_.FullName);
        }
      });
			await printPdfFilesTask_;
    }

		private bool CheckCancellation(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				CleanUponCancellation();
				return true;
			}

			return false;
		}

		private void CleanUponCancellation()
		{
		}

		private string CreateLogFileTimeStamp()
		{
			return DateTime.Now.Year
						 + DateTime.Now.Month.ToString(LogFileDateFormat)
						 + DateTime.Now.Day.ToString(LogFileDateFormat)
						 + DateTime.Now.Hour.ToString(LogFileDateFormat)
						 + DateTime.Now.Minute.ToString(LogFileDateFormat)
						 + DateTime.Now.Second.ToString(LogFileDateFormat);
		}

		private void CreateExcelForLog(IDataExportable context, CancellationToken cancellationToken)
		{
			var logDirectoryInfo_ = new DirectoryInfo(Path.Combine(GetBasePath(), ZipFolder));

			try
			{
				foreach(var pr in SelectedProcedureRecordsList)
				{
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}
					SelectedUserType = UserType.Bsc;
					_password = new string(PasswordUtils.DecryptPasscode(_credentialCode));
					SelectedProcedureRecords = pr;
					_selectedAblationDataDetails = ExtractAblationDetails(pr);
					context.LogMessage = $"Exporting Procedure Excel Data ... {SelectedProcedureRecordsList.IndexOf(SelectedProcedureRecords)} of {SelectedProcedureRecordsList.Count}";
					var file_ = ExportExcelFile();
					if(File.Exists(Path.Combine(logDirectoryInfo_.FullName, file_.Name)))
					{
						File.Delete(Path.Combine(logDirectoryInfo_.FullName, file_.Name));
					}
					File.Copy(file_.FullName, Path.Combine(logDirectoryInfo_.FullName, file_.Name), true);
					File.Delete(file_.FullName);
				}
			}
			catch (Exception e)
			{
				LogException(e);
			}
		}

    private void CreateJsonForLog(IDataExportable context, CancellationToken cancellationToken)
    {
      var logDirectoryInfo_ = new DirectoryInfo(Path.Combine(GetBasePath(), ZipFolder));

      try
      {
        foreach(var pr in SelectedProcedureRecordsList)
        {
          if(cancellationToken.IsCancellationRequested)
          {
            return;
          }
          SelectedUserType = UserType.Bsc;
          SelectedProcedureRecords = pr;
          context.LogMessage = $"Exporting Procedure Json Data ... {SelectedProcedureRecordsList.IndexOf(SelectedProcedureRecords)} of {SelectedProcedureRecordsList.Count}";
          var file_ = ExportJsonFile();
          if(File.Exists(Path.Combine(logDirectoryInfo_.FullName, file_.Name)))
          {
            File.Delete(Path.Combine(logDirectoryInfo_.FullName, file_.Name));
          }
          File.Copy(file_.FullName, Path.Combine(logDirectoryInfo_.FullName, file_.Name), true);
          File.Delete(file_.FullName);
        }
      }
      catch (Exception e)
      {
				LogException(e);
      }
    }

    private bool CreateFolder(DirectoryInfo dirInfo)
		{
			bool createdFolder_;
			try
			{
				Directory.CreateDirectory(dirInfo.FullName);
				createdFolder_ = true;
			}
			catch(Exception ex_)
			{
				LogException(ex_);
				createdFolder_ = false;
			}
			return createdFolder_;
		}

		private void DeleteFilesInFolder(DirectoryInfo dirInfo)
		{
			try
			{
				foreach(var fileInfo_ in dirInfo.EnumerateFiles())
				{
					fileInfo_.Delete();
				}

				foreach(var dirInfo_ in dirInfo.EnumerateDirectories())
				{
					dirInfo_.Delete(true);
				}
			}
			catch(IOException ioe_)
			{
				LogException(ioe_);
			}
		}

		private FileInfo GenerateActionLogFile(string timeStampString)
		{
			var actionLog_ = CommonViewModel.Current.ActionLog.ToList();
			var logs_ = new List<UserLog>();
			foreach(var record_ in actionLog_)
			{
				logs_.Add(new UserLog
				{
					TimeStamp = record_.Time,
					User = record_.User.UserName,
					Action = UserLog.ConvertActionIdToString((Enumeration.Actions)record_.ActionId),
					Message = record_.Message
				});
			}
			var actionLogJson_ = JsonConvert.SerializeObject(logs_, Formatting.Indented);
			var actionLogDirectoryInfo_ = new DirectoryInfo(Path.Combine(GetBasePath(), ZipFolder));

			var file_ = ActionLogFile + timeStampString + JsonExtension;
			var actionLogFileInfo_ = new FileInfo(Path.Combine(actionLogDirectoryInfo_.FullName, file_));
			File.WriteAllText(actionLogFileInfo_.FullName, actionLogJson_);
			return actionLogFileInfo_;
		}

		private FileInfo GenerateErrorLogFile(string timeStampString)
		{
			var errorLog_ = CommonViewModel.Current.Data.DataAccess.GetErrorLog();
			var errors_ = new List<ErrorLog>();
			foreach(var record_ in errorLog_)
			{
				errors_.Add(new ErrorLog
				{
					Entry = record_.Id,
					ErrorNumber = record_.ErrorInformation,
					ConsoleState = ErrorLog.ConvertStateToString((CanBusMessageDefinition.MessageStateId)record_.SystemStatesID),
					Timestamp = record_.ErrorDate.ToLongDateString()
				});
			}

			var errorLogJson_ = JsonConvert.SerializeObject(errors_, Formatting.Indented);
			var errorLogDirectoryInfo_ = new DirectoryInfo(Path.Combine(GetBasePath(), ZipFolder));

			var errorLogFile_ = ErrorLogFile + timeStampString + JsonExtension;
			var errorLogFileInfo_ = new FileInfo(Path.Combine(errorLogDirectoryInfo_.FullName, errorLogFile_));
			File.WriteAllText(errorLogFileInfo_.FullName, errorLogJson_);
			return errorLogFileInfo_;
		}

		private FileInfo GenerateSmartFreezeLogFile(string timeStampString)
		{
			var logSourcePath_ = LogPath;
			if(logSourcePath_ == null || !Directory.Exists(logSourcePath_))
			{
				return null;
			}
			var logZipFileName_ = LogTitle + timeStampString + ZipExtension;
			var smartFreezeLogZipFileInfo_ = new FileInfo(Path.Combine(logSourcePath_, logZipFileName_));
			var password_ = GeneratePassword(string.Empty);
			var fileAction_ = new FileAction();
			fileAction_.ZipFilesWithPassword(logSourcePath_, password_, smartFreezeLogZipFileInfo_.FullName);
			var smartFreezeLogDestinationFile_ = Path.Combine(Path.Combine(GetBasePath(), ZipFolder), logZipFileName_);

			try
			{
				if(File.Exists(smartFreezeLogZipFileInfo_.FullName))
				{
					File.Copy(smartFreezeLogZipFileInfo_.FullName, smartFreezeLogDestinationFile_, true);
					File.Delete(smartFreezeLogZipFileInfo_.FullName);
				}
			}
			catch(IOException ioe_)
			{
				LogException(ioe_);
			}

			return new FileInfo(smartFreezeLogDestinationFile_);
		}

		private FileInfo GenerateWinLogFile(string timeStampString)
		{
			var directoryInfo_ = new DirectoryInfo(Path.Combine(GetBasePath(), ZipFolder));
			var evtxFileName_ = WinEvtLogTitle + timeStampString + WinEvtLogExtension;
			var evtxExtractedFullName_ = Path.Combine(directoryInfo_.FullName, evtxFileName_);
			var fileInfo_ = new FileInfo(evtxExtractedFullName_);
			ExtractWinEventLogAndMsg(evtxExtractedFullName_);

			return fileInfo_;
		}

		#endregion Interface Method Implementation

		public List<Enumeration.CatheterType> CatheterTypeList { get; set; }
		public Enumeration.WeightUnit WeightUnit { get; set; }

		#region Private Fields

		//private DirectoryInfo _tempFolderInfo;
		private string _consoleSn;
		private List<List<AblationDataDetails>> _selectedAblationDataDetails;
		private string _ablationDataFile;
		private string _password;
		private DataAccess _dataAccess;
		private bool _patientInfoAnonymized;
		private PDFConversion _pdfConversion;
		private PDFDataManager _pdfDataManager;
		private JsonManager _jsonManager;
		private CSVManager _csvManager;
		private readonly byte[] _credentialCode = new byte[] { 0xdc, 0xf0, 0xe0, 0xd6, 0x6a, 0x70, 0xc4, 0xdc, 0x7f, 0xce, 0x5a };

		#endregion Private Fields

		#region Private Methods

		List<Enumeration.CatheterType> GetCatheterTypeList(List<AblationReport> lst)
		{
			int CatheterTypeID = 0;
			List<Enumeration.CatheterType> TypeList = new List<Enumeration.CatheterType>();
			for(int i = 0; i < lst.Count; i++)
			{
				if(lst[i].IsUsedForEngineering == true || lst[i].CatheterId == 0)
					CatheterTypeID = 0;
				else
					CatheterTypeID = _dataAccess.GetCatheterTypeById(lst[i].CatheterId);

				if(!IsInAready(TypeList, CatheterTypeID))
					TypeList.Add((Enumeration.CatheterType)CatheterTypeID);
			}
			return TypeList;
		}

		private bool IsInAready(List<Enumeration.CatheterType> list, int ID)
		{
			bool returnvalue = false;
			foreach(Enumeration.CatheterType item in list)
			{
				if(item == (Enumeration.CatheterType)ID)
				{
					returnvalue = true;
					break;
				}
			}
			return returnvalue;
		}

		private string currentPhysicianName = DoubleDash;
		public string CurrentPhysician
		{
			get
			{
				if(CommonViewModel.Current.IsDoctor)
				{
					var physician_ = _dataAccess.GetphysicianByID(CommonViewModel.Current.CurrentUser.Id);
					currentPhysicianName = DoctorTitle + physician_.FirstName + Whitespace + physician_.LastName;
				}
				else if (CommonViewModel.Current.IsAdminUser || CommonViewModel.Current.IsUser)
        {
          var physician_ = _dataAccess.GetphysicianByID(SelectedProcedureRecords.Procedure.PhysicianID);
          currentPhysicianName = DoctorTitle + physician_.FirstName + Whitespace + physician_.LastName;
        }
				else
				{
					currentPhysicianName = DoubleDash;
				}
				return currentPhysicianName;
			}
		}

		private LoginManager.AccessControlType UserTypeToAccessTypeConverter(UserType userType)
		{
			switch(userType)
			{
				case UserType.Admin:
					return LoginManager.AccessControlType.ADMIN;
				case UserType.BostonBsc:
					return LoginManager.AccessControlType.BSCADMIN;
				case UserType.Doctor:
					return LoginManager.AccessControlType.DOCTOR;
				case UserType.Bsc:
					return LoginManager.AccessControlType.CRYTERION;
				case UserType.User:
					return LoginManager.AccessControlType.USER;
				case UserType.Unknown:
				default:
					throw new ArgumentOutOfRangeException(nameof(userType), userType, null);
			}
		}

		private List<List<AblationDataDetails>> ExtractAblationDetails(ProcedureRecords procRec)
		{
			var allAblationDataDetails_ = new List<List<AblationDataDetails>>();
			var jsonManager_ = new JsonManager();

			try
			{
				foreach(var ablation in procRec.Procedure.Ablations)
				{
					if(!string.IsNullOrEmpty(ablation?.DataFile) && File.Exists(ablation.DataFile))
					{
						var ablationData_ = LoadAblationFromFile(jsonManager_, ablation.DataFile);
						if(ablationData_ != null)
						{
							allAblationDataDetails_.Add(ablationData_);
						}
					}
				}
				return allAblationDataDetails_;
			}
			catch(Exception e)
			{
				LogException(e);
				return null;
			}
		}

		private List<AblationDataDetails> LoadAblationFromFile(JsonManager jsonManager, string fileName)
		{
			try
			{
				var ablationData = jsonManager.DeserializeAblationData<AblationFileDataStruct>(fileName);

				return ablationData != null
					? ablationData.ConvertToAblationDataDetails()
					: jsonManager.DeserializeAblationData<List<AblationDataDetails>>(fileName)
						.Select(ab => ab.UpdateBalloonSizeIfEmpty())
						.ToList();
			}
			catch(Exception e)
			{
				LogException(e);
				return null;
			}
		}

		private string GetBasePath()
		{
			var thePath_ = string.Empty;
			var path_ = AppDomain.CurrentDomain.BaseDirectory;
			var extractedStrings_ = Regex.Split(path_, "bin");
			thePath_ = extractedStrings_[0];
			return thePath_;
		}

		public void DeleteCurrentPDFs(string filePathName)
		{
			string currentDoctorPDFName = GetBasePath() + "PDFFiles\\Doctor_" + filePathName + ".pdf";

			if(File.Exists(currentDoctorPDFName))
			{
				File.Delete(currentDoctorPDFName);
			}

			string currentBostonPDFName = GetBasePath() + "PDFFiles\\Boston_" + filePathName + ".pdf";

			if(File.Exists(currentBostonPDFName))
			{
				File.Delete(currentBostonPDFName);
			}

			string currentBostonBSCPDFName = GetBasePath() + "PDFFiles\\BostonBSC_" + filePathName + ".pdf";

			if(File.Exists(currentBostonBSCPDFName))
			{
				File.Delete(currentBostonBSCPDFName);
			}
		}

		private string GetSourcePath_(FileType fileType)
		{
			var BasePath = string.Empty;

			switch(fileType.Type)
			{
				case FileTypeEnum.PDF:
					BasePath = GetBasePath() + PDFTempFolder;
					break;
				case FileTypeEnum.Excel:
					BasePath = GetBasePath() + ExcelTempFolder;
					break;
				case FileTypeEnum.Json:
					BasePath = GetBasePath() + JsonTempFolder;
					break;
				case FileTypeEnum.CaseReport:
					BasePath = GetBasePath() + CaseReportTempFolder;
					break;
				case FileTypeEnum.Log:
					BasePath = GetBasePath() + LogFolder;
					break;
				case FileTypeEnum.Unknown:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return BasePath;
		}

		private string GenerateSourceFileName(FileType fileType)
		{
			var prefix_ = GenerateFilePrefix_(SelectedUserType);
			var desc_ = SelectedProcedureRecords.Procedure.Description;
			var ext_ = fileType.Extension;
			var fileName_ = prefix_ + desc_ + ext_;
			return fileName_;
		}

		private string GenerateSourceFileNameWithoutExtension(FileType fileType)
		{
			var prefix_ = GenerateFilePrefix_(SelectedUserType);
			var desc_ = SelectedProcedureRecords.Procedure.Description;
			var fileName_ = prefix_ + desc_;
			return fileName_;
		}

		private string GenerateFilePrefix_(UserType userType)
		{
			string prefix_;
			switch(userType)
			{
				case UserType.Admin:
					prefix_ = "Admin_";
					break;
				case UserType.Bsc:
					prefix_ = "Bsc_";
					break;
				case UserType.BostonBsc:
					prefix_ = "BostonBsc_";
					break;
				case UserType.Doctor:
					prefix_ = "Doctor_";
					break;
				case UserType.Unknown:
					prefix_ = string.Empty;
					break;
				case UserType.User:
					prefix_ = "User_";
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(userType), userType, null);
			}

			return prefix_;
		}

		ProcedureInfo GenerateProcedureInfo_()
		{
			var ablationData_ = _selectedAblationDataDetails[0];
			var procedureInfo_ = new ProcedureInfo
			{
				CatheterFirmware = ablationData_[0]?.CatheterFirmware ?? string.Empty,
				CMCUFirmware = ablationData_[0].CMCUFirmware ?? string.Empty,
				ConsoleSerialNumber = ablationData_[0].ConsoleSerialNumber ?? string.Empty,
				CPLDFirmware = ablationData_[0].CPLDFirmware ?? string.Empty,
				PMCUFirmware = ablationData_[0].PMCUFirmware ?? string.Empty,
				ICBFirmware = ablationData_[0].ICBFirmware ?? string.Empty,
				DatabaseVersion = ablationData_[0].DatabaseVersion,
				GUIVersion = ablationData_[0].GUIVersion ?? string.Empty
			};

			procedureInfo_.PMCUFirmware = ablationData_[0].PMCUFirmware ?? string.Empty;
			procedureInfo_.ProcedureStartDateTime = SelectedProcedureRecords.Procedure.ProcedureStartDateTime;
			procedureInfo_.OutComeNote = SelectedProcedureRecords.Procedure.OutCome ?? string.Empty;
			procedureInfo_.DiagnosisNote = SelectedProcedureRecords.Procedure.Diagnosis ?? string.Empty;
			procedureInfo_.SkinToSkinDuration = SelectedProcedureRecords.Procedure.SkinToSkinDuration;
			procedureInfo_.TreatmentDateTime = SelectedProcedureRecords.Procedure.Patient.TreatmentDateTime;
			procedureInfo_.HospitalName = _dataAccess.GetHospitalName() ?? string.Empty;

			if(SelectedProcedureRecords.Procedure == CommonViewModel.Current.CurrentProcedure)
			{
				procedureInfo_.HospitalPhyscianID = "";
			}
			else
			{
				procedureInfo_.HospitalPhyscianID =
					SelectedProcedureRecords?.Procedure?.Patient?.Physician?.HospitalPhyscianID ?? string.Empty;
			}

			if(SelectedUserType == UserType.Doctor || SelectedUserType == UserType.Admin || SelectedUserType== UserType.User)
			{
				var physician_ = _dataAccess.GetphysicianByID(CommonViewModel.Current.CurrentProcedure.PhysicianID)
												 ?? throw new ArgumentNullException($"Exception to access physician information.");
        procedureInfo_.DoctorFirstName = $"{{ {string.Join(", ", PasswordUtils.GenerateEncryptionCode(physician_.FirstName).Select(b => $"0x{b:x}"))} }}";
        procedureInfo_.DoctorLastName =$"{{ {string.Join(", ", PasswordUtils.GenerateEncryptionCode(physician_.LastName).Select(b => $"0x{b:x}"))} }}";

				if (SelectedProcedureRecords.Procedure.Patient != null)
				{
					procedureInfo_.PatientGender = SelectedProcedureRecords.Procedure.Patient.Gender;
					procedureInfo_.PatientHeight = SelectedProcedureRecords.Procedure.Patient.Height;
					procedureInfo_.PatientWeight = SelectedProcedureRecords.Procedure.Patient.Weight;
				}

			}
			else
			{
				procedureInfo_.DoctorFirstName = Encoding.Default.GetString(_dataAccess.EncryptedValue("-"));
				procedureInfo_.DoctorLastName = Encoding.Default.GetString(_dataAccess.EncryptedValue("-"));
				procedureInfo_.PatientGender = -1;
				procedureInfo_.PatientHeight = 0;
				procedureInfo_.PatientWeight = 0;
			}

			procedureInfo_.HospitalPatientId = SelectedProcedureRecords.Procedure.Patient.HospitalPatientId ?? string.Empty;

			if(SelectedUserType == UserType.Doctor || SelectedUserType == UserType.Admin || SelectedUserType== UserType.User)
			{
        procedureInfo_.PatientFirstName =  $"{{ {string.Join(", ", PasswordUtils.GenerateEncryptionCode(SelectedProcedureRecords.Procedure.Patient.FirstName).Select(b => $"0x{b:x}"))} }}";
        procedureInfo_.PatientLastName = $"{{ {string.Join(", ", PasswordUtils.GenerateEncryptionCode(SelectedProcedureRecords.Procedure.Patient.LastName).Select(b => $"0x{b:x}"))} }}";
        procedureInfo_.DateOfBirthEncry = $"{{ {string.Join(", ", PasswordUtils.GenerateEncryptionCode(SelectedProcedureRecords.Procedure.Patient.DateOfBirth.ToShortDateString()).Select(b => $"0x{b:x}"))} }}";
				
			}
			else
			{
				procedureInfo_.PatientFirstName = Encoding.Default.GetString(_dataAccess.EncryptedValue("-"));
				procedureInfo_.PatientLastName = Encoding.Default.GetString(_dataAccess.EncryptedValue("-"));
				procedureInfo_.DateOfBirthEncry = Encoding.Default.GetString(_dataAccess.EncryptedValue("1800-01-01"));
				procedureInfo_.DateOfBirth = Encoding.Default.GetString(_dataAccess.EncryptedValue("1800-01-01"));
			}

			procedureInfo_.Age = SelectedProcedureRecords.Procedure.Patient.DateOfBirth.Year != 1800 &&
													 (SelectedUserType == UserType.Doctor || SelectedUserType == UserType.Admin)
					? SelectedProcedureRecords.Procedure.ProcedureStartDateTime.Year -
						SelectedProcedureRecords.Procedure.Patient.DateOfBirth.Year
					: -1;

			return procedureInfo_;
		}

		List<TreatmentNotes> GenerateTreatmentNotes_()
		{
			var treatmentNotesList_ = new List<TreatmentNotes>();
			foreach(var ablation_ in SelectedProcedureRecords.Procedure.Ablations)
			{
				var note_ = new TreatmentNotes
				{
					ProcedureId = ablation_.ProcedureId,
					TreatmentId = ablation_.AblationNumber,
					TreatmentNote = ablation_.TreatmentNote
				};
				treatmentNotesList_.Add(note_);
			}
			return treatmentNotesList_;
		}

		#endregion Private Methods

		#region IDisposable Implementation

		private void ReleaseUnmanagedResources()
		{
		}

		public void Dispose()
		{
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		~DataExportService()
		{
			ReleaseUnmanagedResources();
		}

		#endregion IDisposable Implementation
	}

	internal struct UserLog
	{
		public DateTime TimeStamp { get; set; }
		public string User { get; set; }
		public string Action { get; set; }
		public string Message { get; set; }

		public static string ConvertActionIdToString(Enumeration.Actions action)
		{
			switch(action)
			{
				case Enumeration.Actions.Login:
					return "Login";
				case Enumeration.Actions.Logout:
					return "Logout";
				case Enumeration.Actions.StartCommand:
					return "Start Command";
				case Enumeration.Actions.StopCommand:
					return "Stop Command";
				case Enumeration.Actions.CreateProcedure:
					return "Create Procedure";
				case Enumeration.Actions.CreateUser:
					return "Create User";
				case Enumeration.Actions.EditUser:
					return "Edit User";
				case Enumeration.Actions.DeleteUser:
					return "Delete User";
				case Enumeration.Actions.ResetPassword:
					return "Reset Password";
				case Enumeration.Actions.AccessRecord:
					return "Access Record";
				case Enumeration.Actions.AccessChangeTank:
					return "Access Change Tank";
				case Enumeration.Actions.AccessSettings:
					return "Access Settings";
				case Enumeration.Actions.AccessManageUsers:
					return "Access Manage Users";
				case Enumeration.Actions.AccessDateAndTime:
					return "Access Date And Time";
				case Enumeration.Actions.AccessMaintenance:
					return "Access Maintenance";
				case Enumeration.Actions.AccessSiteSetup:
					return "Access Site Setup";
				case Enumeration.Actions.AccessPIDControl:
					return "Access PID Control";
				case Enumeration.Actions.AccessElectricalMonitoring:
					return "Access Electrical Monitoring";
				case Enumeration.Actions.AccessLoadCellCalibration:
					return "Access Load Cell Calibration";
				case Enumeration.Actions.AccessSystemFiles:
					return "Access System Files";
				case Enumeration.Actions.AccessCatheterDatabase:
					return "Access Catheter Database";
				case Enumeration.Actions.AccessMechanicalMonitoring:
					return "Access Mechanical Monitoring";
				case Enumeration.Actions.AccessFlowCurveProgramming:
					return "Access Flow Curve Programming";
				case Enumeration.Actions.LoadFirmwareVersionCommand:
					return "Load Firmware Version Command";
				case Enumeration.Actions.AppModeCommand:
					return "App Mode Command";
				case Enumeration.Actions.DiaphragmReset:
					return "Diaphragm Reset";
				case Enumeration.Actions.DeleteProcedure:
					return "Delete Procedure";
				default:
					throw new ArgumentOutOfRangeException(nameof(action), action, null);
			}
		}
	}

	internal struct ErrorLog
	{
		public int Entry { get; set; }
		public string ErrorNumber { get; set; }
		public string ConsoleState { get; set; }
		public string Timestamp { get; set; }

		public static string ConvertStateToString(CanBusMessageDefinition.MessageStateId stateId)
		{
			switch(stateId)
			{
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN:
					return "UNKNOWN";
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE:
					return "IDLE";
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY:
					return "READY";
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION:
					return "INFLATION";
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION:
					return "TRANSITION";
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION:
					return "ABLATION";
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING:
					return "THAWING";
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION:
					return "EXCEPTION";
				default:
					throw new ArgumentOutOfRangeException(nameof(stateId), stateId, null);
			}
		}
	}
}