using DataAccessLayer;
using FileSerializer;
using PDFReportsGenerator;
using Shared;
using SmartAblationSystem.Converters;
using SmartAblationSystem.Models;
using SmartAblationSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Communication;
using UniversalLoginManager;

namespace SmartAblationSystem.Helpers
{
	/// <summary>
	/// This class manages the data and format for PDF reports generator
	/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
	/// </summary>
	public class PDFDataManager
	{
		private Series _seriesTemperature;
		private Chart _chartTemperature;

		private readonly string ProcedureDateFormat = "MMMM dd, yyyy";
		private readonly System.Drawing.Color GRID_LINES_COLOR = System.Drawing.Color.Gray;
		private readonly System.Drawing.Color ECG_GRID_LINES_COLOR = System.Drawing.Color.DimGray;
		private readonly System.Drawing.Color SERIES_COLOR = System.Drawing.ColorTranslator.FromHtml("#00AFEF");
		private readonly System.Drawing.Color SERIES_COLOR_ISOLATED_VEIN = System.Drawing.ColorTranslator.FromHtml("#01DF01");
		private readonly System.Drawing.Color SERIES_COLOR_ABLATION_FAIL = System.Drawing.ColorTranslator.FromHtml("#FF0000");
		private readonly System.Drawing.Color SERIES_COLOR_THRESHOLD_EXCEEDED = System.Drawing.Color.Red;
		private readonly ChartDashStyle TEMPERATURE_GRID_DASH_STYLE = ChartDashStyle.Dot;

		private const double TEMPERATURE_MIN_VALUE = -80;
		private const double TEMPERATURE_MAX_VALUE = 60;
		private const string WEIGHT = "WEIGHT";
		private const string HEIGHT = "HEIGHT";
		private const string BMI = "BMI";
		private const string PdfFolder = "PDFFiles";
		private const string Underscore = "_";
		private const string Colon = ":";
		private const string ColonAndWhiteSpace = ": ";
		private const string WhiteSpace = " ";
		private readonly PDFTemplate _pdfTemplate;

		#region Fields

		string ProcedureDateField = "Procedure Date";
		string AblationSummaryReportField = "Ablation Summary Report";
		string CoverType = "Cover";
		string PhysicianField = "Physician";
		string CatheterUsedfield = "Catheter Used";
		string PROCEDUREINFOfield = "PROCEDURE INFO";
		string DateofBirthfield = "Date of Birth";
		string Weightfield = "Weight";
		string Genderfield = "Gender";
		string Heightfield = "Height";
		string PatientField = "Patient";
		string NumberField = "NumberField";
		string DIAGNOSISField = "DIAGNOSIS";
		string PATIENTINFOField = "PATIENT INFO";
		string OUTCOMEField = "OUTCOME";
		string TreatmentField = "Treatment";
		string TreatmentNoteField = "Treatment Note";
		string AblationSiteField = "Ablation Site";
    string BalloonSizeField = "Balloon Size";
		string AblationsField = "Ablation";
		string ABLATIONSUMMARYField = "ABLATION SUMMARY";
		string DurationField = "Duration (sec)";
		string MinESOTempField = "Min ESO Temp";
		string MinTempField = "Min Temp";
		string TimetoTargetField = "Time to Target";
		string TimetoVeinIsolationField = "Time to Vein Isolation";
		string TimetoThawField = "Time to Thaw";
		string CoolingTimerSetPointField = "Time to Target SP";
		string ThawTimerSetPointField = "Time to Thaw SP";
		string MinDMSField = "Min DMS";
		string TreatmentStartTimeField = "Treat- ment Start Time";
    string TotalThawingTimeField = "Total Thawing Time";
		string TreatmentInfoField = "TREATMENT INFO";
		string TreatmentNote = "TREATMENT NOTE:";
		string InBodyTimeField = "In Body Time";
		string minField = "min";
		string RSPVField = "RSPV";
		string LSPVField = "LSPV";
		string RIPVField = "RIPV";
		string LIPVField = "LIPV";
		string OtherField = "Other";
    string LCPVField = "LCPV";
    string RMPVField = "RMPV";
    string TotalField = "Total";
		string TemperatureField = "Temperature";
		string pageField = "Page";
		string PROCEDUREAUDITTRAILField = "PROCEDURE AUDIT TRAIL";
		string BMIField = "BMI";

		#endregion Fields

		/// <summary>
		/// This function manages the data and format for PDF reports generator
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public PDFDataManager()
		{
			//  PDFConversion = new PDFConversion();
			_pdfTemplate = new PDFTemplate();
			_seriesTemperature = new Series();
		}

		private UserType GetUserType(LoginManager.AccessControlType userType)
		{
			var user_ = UserType.Unknown;

			switch(userType)
			{
				case LoginManager.AccessControlType.CRYTERION:
					user_ = UserType.Bsc;
					break;
				case LoginManager.AccessControlType.BSCADMIN:
					user_ = UserType.BostonBsc;
					break;
				case LoginManager.AccessControlType.DOCTOR:
					user_ = UserType.Doctor;
					break;
				case LoginManager.AccessControlType.ADMIN:
					user_ = UserType.Admin;
					break;
				case LoginManager.AccessControlType.USER:
					user_ = UserType.User;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(userType), userType, null);
			}

			return user_;
		}

		private string GenerateFileName(UserType user, ProcedureRecords procedure)
		{
			var folder_ = Path.Combine(GetBasePath(), PdfFolder);
			var fileType_ = new FileType(FileTypeEnum.PDF);
			var fileName_ = user + Underscore + procedure.Procedure.Description + fileType_.Extension;
			return Path.Combine(folder_, fileName_);
		}

		/// <summary>
		/// This function manages PDF elements and calling PDFTemplate to generate PDF file
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public string GeneratePDFFile(
			List<List<AblationDataDetails>> allAblationDataList,
			ProcedureRecords ProcedureRecords,
			LoginManager.AccessControlType accessControlType,
			string hospitalName,
			List<AblationReport> ablationReportList,
			AblationSummary AblationSummary,
			Enumeration.WeightUnit WeightUnit,
			List<string> procedureLogstring,
			string PhysicianName,
			bool isPatientInfoAnonymous,
			List<Enumeration.CatheterType> CatheterTypeList)
		{
			InitializeFields(accessControlType);

			var pdfImagePath_ = ConfigurationManager.AppSettings["PDFImagePath"];
			var ablationInfoCollection_ = new List<PDFElementsTable>();
			var ablationSite_ = new AblationSiteToStringConverter();
			var userType_ = GetUserType(accessControlType);
			var fullPdfFileName_ = GenerateFileName(userType_, ProcedureRecords);

			#region Cover Page

			var coverInfoTable_ = CreateCoverInfo_();
			ablationInfoCollection_.Add(coverInfoTable_);

			PDFElementsTable CreateCoverInfo_()
			{
				var ElementValues_ = new string[1][];
				ElementValues_[0] = new[] { hospitalName };

				var coverPageElementsTable_ = new PDFElementsTable
				{
					ElementType = CoverType,
					ElementDispalyName = AblationSummaryReportField,
					ElementValue = ElementValues_
				};
				return coverPageElementsTable_;
			}

			#endregion Cover Page

			#region Procedure Info

			var procedureInfoTable_ = CreateProcedureInfoTable_();
			ablationInfoCollection_.Add(procedureInfoTable_);

			PDFElementsTable CreateProcedureInfoTable_()
			{
				var ElementValues_ = new string[2][];
				if(accessControlType == LoginManager.AccessControlType.DOCTOR ||
           accessControlType == LoginManager.AccessControlType.ADMIN ||
           accessControlType == LoginManager.AccessControlType.USER)
				{
					ElementValues_[0] = new[] { PhysicianField + ": " + PhysicianName, CatheterUsedfield + ": " + CatheterUsed(CatheterTypeList, "CatheterDescriptionSplit") };
				}
				else
				{
					ElementValues_[0] = new[] { "Physician: --", "Catheter Used: " + CatheterUsed(CatheterTypeList, "CatheterDescriptionSplit") };
				}

				ElementValues_[1] = new[] { ProcedureDateField + ": " + ProcedureRecords.ProcedureDate.ToString(ProcedureDateFormat), "" };

				var procedureElementsTable_ = new PDFElementsTable
				{
					ElementType = "table",
					ElementDispalyName = PROCEDUREINFOfield + ": ",
					ElementValue = ElementValues_
				};
				return procedureElementsTable_;
			}

			#endregion Procedure Info

			#region System Info

			if(accessControlType == LoginManager.AccessControlType.BSCADMIN || accessControlType == LoginManager.AccessControlType.CRYTERION)
			{
				var systemInfoTable_ = CreateSystemInfoTable_();
				ablationInfoCollection_.Add(systemInfoTable_);
			}

			PDFElementsTable CreateSystemInfoTable_()
			{
				string[][] SystemValues = new string[5][];
				SystemValues[0] = new[] { "Catheter Firmware: " + allAblationDataList[0][0].CatheterFirmware, "CPLD Firmware: " + allAblationDataList[0][0].CPLDFirmware };
				SystemValues[1] = new[] { "PMCU Firmware: " + allAblationDataList[0][0].PMCUFirmware, "Repeater Firmware: " + allAblationDataList[0][0].RepeaterFirmware };
				SystemValues[2] = new[] { "ICB Firmware: " + allAblationDataList[0][0].ICBFirmware, "CMCU Firmware: " + allAblationDataList[0][0].CMCUFirmware };
				SystemValues[3] = new[] { "Console Serial Number: " + allAblationDataList[0][0].ConsoleSerialNumber, "Database Version: " + allAblationDataList[0][0].DatabaseVersion };

				string remotefirmwaretemp = "--";
				if(allAblationDataList[0][0].RemoteFirmware != "0" && allAblationDataList[0][0].RemoteFirmware != null)
					remotefirmwaretemp = allAblationDataList[0][0].RemoteFirmware;
				SystemValues[4] = new string[] { "GUIVersion: " + allAblationDataList[0][0].GUIVersion, "Remote Firmware: " + remotefirmwaretemp };

				var systemInfoElementsTable_ = new PDFElementsTable
				{
					ElementType = "table",
					ElementDispalyName = "SYSTEM INFO: ",
					ElementValue = SystemValues
				};
				return systemInfoElementsTable_;
			}

			#endregion System Info

			#region Patient Info

			if(accessControlType == LoginManager.AccessControlType.DOCTOR 
			   || accessControlType == LoginManager.AccessControlType.ADMIN || accessControlType == LoginManager.AccessControlType.USER)
			{
				var patientValues_ = CreatePatientInfoTable_();
				ablationInfoCollection_.Add(patientValues_);

				if(!string.IsNullOrEmpty(ProcedureRecords.Procedure.Diagnosis))
				{
					var table_ = CreateDiagnosisInfoTable_();
					ablationInfoCollection_.Add(table_);
				}

				if(!string.IsNullOrEmpty(ProcedureRecords.Procedure.OutCome))
				{
					var table_ = CreateOutComeInfoTable_();
					ablationInfoCollection_.Add(table_);
				}

				const string DoubleDash = "--";
				PDFElementsTable CreatePatientInfoTable_()
				{
					string[][] patientInfoValues_ = new string[4][];

					var patientFirstName_ = isPatientInfoAnonymous ? DoubleDash : ProcedureRecords.Procedure.Patient.FirstName;
					var patientLastName_ = isPatientInfoAnonymous ? DoubleDash : ProcedureRecords.Procedure.Patient.LastName;
					var hospitalPatientId_ = isPatientInfoAnonymous ? DoubleDash : ProcedureRecords.Procedure.Patient.HospitalPatientId;
					patientInfoValues_[0] = new[]
					{
						PatientField + ": " + patientFirstName_ + " " + patientLastName_,
						"ID " + NumberField + ": " + hospitalPatientId_
					};

					var patientDob_ = isPatientInfoAnonymous
						? DoubleDash
						: DateBirthConvert(ProcedureRecords.Procedure.Patient.DateOfBirth.ToString(ProcedureDateFormat));
					var patientGender_ =
						isPatientInfoAnonymous ? DoubleDash : GetGender(ProcedureRecords.Procedure.Patient.Gender);

					patientInfoValues_[1] = new[]
					{
						DateofBirthfield + ": " + patientDob_,
						Genderfield + " : " + patientGender_
					};

					var patientWeight_ = isPatientInfoAnonymous
						? DoubleDash
						: ProcedureConverter(ProcedureRecords.Procedure.Patient, WEIGHT) + ScaleUnit(WeightUnit, WEIGHT);
					var patientHeight_ = isPatientInfoAnonymous
						? DoubleDash
						: ProcedureConverter(ProcedureRecords.Procedure.Patient, HEIGHT) + ToiseUnit(WeightUnit, HEIGHT);
					patientInfoValues_[2] = new[]
					{
						Weightfield + ": " + patientWeight_,
						Heightfield + ": " + patientHeight_
					};

					var patientBmi_ = isPatientInfoAnonymous
						? DoubleDash
						: ProcedureConverter(ProcedureRecords.Procedure.Patient, BMI);
					patientInfoValues_[3] = new[]
					{
						BMIField + ": " + patientBmi_,
						string.Empty
					};

					var patientInfoElementsTable_ = new PDFElementsTable { ElementType = "table", ElementDispalyName = PATIENTINFOField + ": ", ElementValue = patientInfoValues_ };
					return patientInfoElementsTable_;
				}

				PDFElementsTable CreateDiagnosisInfoTable_()
				{
					string[][] diagnosisInfoElementValues_ = new string[2][];
					diagnosisInfoElementValues_[0] = new[] { WhiteSpace };
					diagnosisInfoElementValues_[1] = new[] { ProcedureRecords.Procedure.Diagnosis };

					var diagnosisInfoElementsTable_ = new PDFElementsTable
					{
						ElementType = "tableSmall",
						ElementDispalyName = DIAGNOSISField + ColonAndWhiteSpace,
						ElementValue = diagnosisInfoElementValues_
					};
					return diagnosisInfoElementsTable_;
				}

				PDFElementsTable CreateOutComeInfoTable_()
				{
					string[][] OutcomeValues = new string[2][];
					OutcomeValues[0] = new[] { WhiteSpace };
					OutcomeValues[1] = new[] { ProcedureRecords.Procedure.OutCome };
					var outcomeElementTable_ = new PDFElementsTable
					{
						ElementType = "tableSmall",
						ElementDispalyName = OUTCOMEField + ColonAndWhiteSpace,
						ElementValue = OutcomeValues
					};
					return outcomeElementTable_;
				}
			}

			#endregion Patient Info

			#region Treatment Info

			var treatmentCount_ = allAblationDataList.Count;
			var TreatmentElementValues_ = new string[treatmentCount_ + 1][];

			var treatmentInfoTable_ = CreateTreatmentInfoTable_();
			ablationInfoCollection_.Add(treatmentInfoTable_);

			PDFElementsTable CreateTreatmentInfoTable_()
			{
				TreatmentElementValues_[0] = new[]
				{
					null,
					AblationSiteField,
          BalloonSizeField+" (mm)",
          DurationField,
					MinESOTempField + "(°C)",
					MinTempField + " (°C)",
					TimetoTargetField + " (sec)",
					CoolingTimerSetPointField + " (°C)",
					TimetoVeinIsolationField + " (sec)",
					TimetoThawField + " (sec)",
					ThawTimerSetPointField + " (°C)",
          TotalThawingTimeField + " (sec) ",
          MinDMSField + " (%)",
          TreatmentStartTimeField
				};
				var table_ = new PDFElementsTable
				{
					ElementType = "tableBig",
					ElementDispalyName = TreatmentInfoField + ": ",
					ElementValue = TreatmentElementValues_
				};
				return table_;
			}

			#endregion Treatment Info

			#region InBodyTime

			var inBodyTable_ = CreateInBodyTimeTable_();
			ablationInfoCollection_.Add(inBodyTable_);

			PDFElementsTable CreateInBodyTimeTable_()
			{
				var InBodyTime = (ProcedureRecords.Procedure.SkinToSkinDuration / 60).ToString();
				var inBodyTimeElementValues_ = new string[2][];
				inBodyTimeElementValues_[0] = new[] { WhiteSpace };
				inBodyTimeElementValues_[1] = new[] { InBodyTimeField + ColonAndWhiteSpace + InBodyTime + WhiteSpace + minField };
				var table_ = new PDFElementsTable
				{
					ElementType = "tableSmall",
					ElementDispalyName = WhiteSpace,
					ElementValue = inBodyTimeElementValues_
				};
				return table_;
			}

			#endregion InBodyTime

			#region Ablation Summary

			var ablationSummary_ = CreateAblationSummary_();
			ablationInfoCollection_.Add(ablationSummary_);

			PDFElementsTable CreateAblationSummary_()
			{
				var ablationSummaryElementValues_ = new string[9][];
				ablationSummaryElementValues_[0] = new[]
				{
					AblationSiteField,
					AblationsField,
					DurationField
				};
				ablationSummaryElementValues_[1] = new[]
				{
					LSPVField,
					AblationSummary.TotalLSPV.ToString(),
					AblationSummary.TotalLSPVDuration.ToString(CultureInfo.InvariantCulture)
				};
				ablationSummaryElementValues_[2] = new[]
				{
					LIPVField,
					AblationSummary.TotalLIPV.ToString(),
					AblationSummary.TotalLIPVDuration.ToString(CultureInfo.InvariantCulture)
				};
				ablationSummaryElementValues_[3] = new[]
				{
					RIPVField,
					AblationSummary.TotalRIPV.ToString(),
					AblationSummary.TotalRIPVDuration.ToString(CultureInfo.InvariantCulture)
				};
				ablationSummaryElementValues_[4] = new[]
				{
					RSPVField,
					AblationSummary.TotalRSPV.ToString(),
					AblationSummary.TotalRSPVDuration.ToString(CultureInfo.InvariantCulture)
				};
        ablationSummaryElementValues_[5] = new[]
        {
          LCPVField,
          AblationSummary.TotalLCPV.ToString(),
          AblationSummary.TotalLCPVDuration.ToString(CultureInfo.InvariantCulture)
        };
        ablationSummaryElementValues_[6] = new[]
        {
          RMPVField,
          AblationSummary.TotalRMPV.ToString(),
          AblationSummary.TotalRMPVDuration.ToString(CultureInfo.InvariantCulture)
        };
        ablationSummaryElementValues_[7] = new[]
        {
          OtherField,
          AblationSummary.TotalOther.ToString(),
          AblationSummary.TotalOtherDuration.ToString(CultureInfo.InvariantCulture)
        };
        ablationSummaryElementValues_[8] = new[]
				{
					TotalField,
					AblationSummary.TotalAblation.ToString(),
					AblationSummary.TotalAblationDuration.ToString(CultureInfo.InvariantCulture)
				};

				var table_ = new PDFElementsTable
				{
					ElementType = "tableBig",
					ElementDispalyName = ABLATIONSUMMARYField,
					ElementValue = ablationSummaryElementValues_
				};
				return table_;
			}

			#endregion Ablation Summary

			#region Treatment Notes

			var treatmentDictionary_ = new Dictionary<int, string>();
			
			if (checkGenerateTreatmentNotes())
			{
				var treatmentNotes_ = CreateTreatmentNotes_();
				ablationInfoCollection_.Add(new PDFElementsTable
				{
					ElementType = "newpage",
					ElementDispalyName = string.Empty,
					ElementValue = null
				});
				ablationInfoCollection_.Add(treatmentNotes_);
			}

			bool checkGenerateTreatmentNotes()
			{
				if(accessControlType == LoginManager.AccessControlType.BSCADMIN || 
           accessControlType == LoginManager.AccessControlType.CRYTERION)
					return false;

				var result_ = false;
				
				foreach (var ablation in ProcedureRecords.Procedure.Ablations)
				{
					if(ablation.TreatmentNote.Trim() != string.Empty && ablation.TreatmentNote.Trim() != "N-A")
						treatmentDictionary_.Add(ablation.AblationNumber, ablation.TreatmentNote);
				}
				if(treatmentDictionary_.Count>0)
					result_ = true;

				return result_;
			}

			PDFElementsTable CreateTreatmentNotes_()
      {
				var treatmentNotesElementValues_ = new string[treatmentDictionary_.Count + 1][];

				treatmentNotesElementValues_[0] = new[] { "Treatment", "Note" };

				for(int index = 0; index < treatmentDictionary_.Count; index++)
				{
					treatmentNotesElementValues_[index + 1] = new[]
					{
						"Treatment " + treatmentDictionary_.ElementAt(index).Key,
						treatmentDictionary_.ElementAt(index).Value
					};
				}
        var table_ = new PDFElementsTable
				{
					ElementType = "tableTreatmentNote",
					ElementDispalyName = TreatmentNote,
					ElementValue = treatmentNotesElementValues_
				};
				return table_;
			}

			#endregion Treatment Notes

			#region Images

			int ListNum = allAblationDataList.Count;
			string[][] ImageElementValues = new string[ListNum][];

			string title = "";

			var atemp_ = allAblationDataList.Count;
			var rtemp_ = ablationReportList.Count;

			for(int i = 0; i < ListNum; i++)
			{
				int AblationItemnum = allAblationDataList[i].Count - 1;
				string lastAblationSiteValue = allAblationDataList[i][AblationItemnum].AblationSite.ToString();
				title = ablationSite_.Convert(lastAblationSiteValue, null, null, null).ToString();
				SaveAblationTemperatureChartAsImage(
					allAblationDataList[i],
					ProcedureRecords.Procedure.Description + "_" + (i + 1) + ".png",
					TreatmentField + (i + 1) + " " + TemperatureField + " ( " + title + " ) ",
					pdfImagePath_);
			}
			int treatmentNumber = 1;
			string imagepath = "";
			foreach(Ablation ablation in ProcedureRecords.Procedure.Ablations)
			{
				if(treatmentNumber <= ListNum)
				{
					List<string> ablationImage = new List<string>();

					imagepath = GetBasePath() + pdfImagePath_ + ProcedureRecords.Procedure.Description + "_" + treatmentNumber + ".png";

					ImageElementValues[treatmentNumber - 1] = new string[] { imagepath, string.Empty };
					treatmentNumber++;
				}
			}

			ablationInfoCollection_.Add(new PDFElementsTable { ElementType = "images", ElementDispalyName = " ", ElementValue = ImageElementValues });

			#endregion Images

			#region Change Log

			int logcount = procedureLogstring.Count;
			string[][] procedureLogsElementValues;


			if(logcount > 0)
			{
				procedureLogsElementValues = new string[logcount + 1][];
				procedureLogsElementValues[0] = new string[] { " " };
				for(int i = 0; i < logcount; i++)
				{
					procedureLogsElementValues[i + 1] = new string[] { procedureLogstring[i] };
				}
			}
			else
			{
				procedureLogsElementValues = Array.Empty<string[]>();
			}

			if(procedureLogsElementValues.Length > 0)
			{
				ablationInfoCollection_.Add(
					new PDFElementsTable
					{
						ElementType = "tablenoheaderNewPage",
						ElementDispalyName = PROCEDUREAUDITTRAILField + " : ",
						ElementValue = procedureLogsElementValues
					});
			}

			#endregion Change Log

			#region Treatment Error Message

			var errorList_ = new Dictionary<string, string>();
			for(int i = 0; i < ListNum; i++)
			{
				var row = i + 1;
				var startTime = ablationReportList[i].LocalTime.Split(' ');
				var ablationCount_ = allAblationDataList[i].Count - 1;

				try
				{
					var lastAblationSiteValue = allAblationDataList[i]?[ablationCount_]?.AblationSite ?? (int)AblationSiteEnum.UNKNOWN;
          var totalThawingTime_ = allAblationDataList[i]
            ?.Where(x => x.SystemState == (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING).Count();

          TreatmentElementValues_[row] = new[]
					{
						ablationReportList[i].Treatment,
						ablationSite_.Convert(lastAblationSiteValue, null, null, null).ToString(),
            ablationReportList[i].BalloonSize,
            ablationReportList[i].Duration.ToString(CultureInfo.InvariantCulture),
						MinsConvert(ablationReportList[i].MinimumEsophagusTemperatureValue, "DiaphragmMovement"),
						ablationReportList[i].MaxTemperatureRate.ToString(CultureInfo.InvariantCulture),
						MinsConvert(ablationReportList[i].TimeToTarget, "TimeToTarget"),
						ablationReportList[i].RequiredTargetTemperature.ToString(CultureInfo.InvariantCulture),
						MinsConvert(ablationReportList[i].TimeToVeinIsolation, "TimeToVeinIsolation"),
						MinsConvert(ablationReportList[i].TimeToThaw, "TimeToThaw"),
						ablationReportList[i].ThawTimeToTemperature.ToString(CultureInfo.InvariantCulture),
            (totalThawingTime_ ?? 0).ToString(),
            MinsConvert(ablationReportList[i].MinimumDiaphragmMovementValue, "DiaphragmMovement"),
						startTime[1].Substring(0, startTime[1].Length - 4),
          };
				}
				catch(Exception e)
				{
					LogSystem.LogService.LogException(e);
				}

				if((CommonViewModel.Current.IsBSCADMINUser || CommonViewModel.Current.IsCryterionUser)
					 && !string.IsNullOrEmpty(ablationReportList[i]?.Errors)
					 && ablationReportList[i]?.Errors != @"N/A")
				{
					var treatment_ = ablationReportList[i]?.Treatment;
					if(treatment_ != null && !errorList_.ContainsKey(treatment_))
					{
						errorList_.Add(ablationReportList[i].Treatment, ablationReportList[i].Errors);
					}
				}
			}

			var errorCount_ = errorList_.Count;
			var count = 1;
			string[][] ErrorMessageElementValues;
			if(errorCount_ > 0)
			{
				ErrorMessageElementValues = new string[errorCount_ + 1][];
				ErrorMessageElementValues[0] = new[] { "TREATMENT", "MESSAGE" };
				foreach(var definition in errorList_)
				{
					ErrorMessageElementValues[count] = new[] { definition.Key, definition.Value };
					count++;
				}
			}
			else
			{
				ErrorMessageElementValues = Array.Empty<string[]>();
			}

			// if (accessControlType == LoginManager.AccessControlType.CRYTERION && ErrorMessageElementValues.Length  > 0)
			if((CommonViewModel.Current.IsBSCADMINUser || CommonViewModel.Current.IsCryterionUser) && ErrorMessageElementValues.Length > 0)
			{
				ablationInfoCollection_.Add(new PDFElementsTable { ElementType = "tableBigNewPage", ElementDispalyName = "TREATMENT ERROR MESSAGE : ", ElementValue = ErrorMessageElementValues });
			}

			#endregion Treatment Error Message

			var procedureDate_ = ProcedureDateField + ": " + ProcedureRecords.ProcedureDate.ToString(ProcedureDateFormat);
			var procedureId_ = "Procedure ID: " + ProcedureRecords.Procedure.Id;

			_pdfTemplate.SaveToPDFTemplate(fullPdfFileName_, ablationInfoCollection_, procedureId_, procedureDate_, pageField, pdfImagePath_);
			return fullPdfFileName_;
		}

		private void InitializeFields(LoginManager.AccessControlType accessControlType)
		{
			if(accessControlType == LoginManager.AccessControlType.DOCTOR || accessControlType == LoginManager.AccessControlType.ADMIN
			   || accessControlType == LoginManager.AccessControlType.USER)
			{
				//PatientIDField = FieldToTextConverterobject("PatientIdLabel", null, null);
				ProcedureDateField = FieldToTextConverterobject("ProcedureDateLabel", null, null);
				AblationSummaryReportField = FieldToTextConverterobject("AblationSummaryReportUID", null, "TITLECASE");
				PhysicianField = FieldToTextConverterobject("PhysicianLabel", null, null);
				PROCEDUREINFOfield = FieldToTextConverterobject("ProcedureInfoLabel", null, null);
				DateofBirthfield = FieldToTextConverterobject("BirthDateLabel", null, null);
				Weightfield = FieldToTextConverterobject("WeightLabel", null, null);
				Genderfield = FieldToTextConverterobject("GenderLabel", null, null);
				Heightfield = FieldToTextConverterobject("HeightLabel", null, null);
				CatheterUsedfield = FieldToTextConverterobject("CatheterUsedLabel", null, null);
				NumberField = FieldToTextConverterobject("NumberUID", null, null);
				DIAGNOSISField = FieldToTextConverterobject("DiagnosisLabel", null, "CAPS");
				PATIENTINFOField = FieldToTextConverterobject("PatientInfoLabel", null, null);
				OUTCOMEField = FieldToTextConverterobject("OutcomeLabel", null, "CAPS");
				TreatmentField = FieldToTextConverterobject("TreatmentLabel", null, null);
				AblationSiteField = FieldToTextConverterobject("AblationSiteLabel", null, null);
				DurationField = FieldToTextConverterobject("DurationInSecLabel", null, null);
				MinESOTempField = FieldToTextConverterobject("minESOtempUID", null, null);
				MinTempField = FieldToTextConverterobject("minTempUID", null, null);
				TimetoTargetField = FieldToTextConverterobject("timetotargetUID", null, null);
				
        TimetoVeinIsolationField = FieldToTextConverterobject("timetoveinisolationUID", null, null);
				//MinDMSField = FieldToTextConverterobject("timetothawUID", null, null);
        // TreatmentStartTimeField = FieldToTextConverterobject("treatmentstarttimeUID", null, null);
				TreatmentInfoField = FieldToTextConverterobject("TreatmentInfoLabel", null, "CAPS");
				InBodyTimeField = FieldToTextConverterobject("inbodytimeUID", null, null);
				minField = FieldToTextConverterobject("minLabel", null, null);
				AblationsField = FieldToTextConverterobject("AblationsLabel", null, null);

				RSPVField = FieldToTextConverterobject("RSPVLabel", null, null);
				LSPVField = FieldToTextConverterobject("LSPVLabel", null, null);
				RIPVField = FieldToTextConverterobject("RIPVLabel", null, null);
				LIPVField = FieldToTextConverterobject("LIPVLabel", null, null);
        LCPVField = "LCPV"; //FieldToTextConverterobject("LCPVLabel", null, null);
        RMPVField = "RMPV"; // FieldToTextConverterobject("RMPVLabel", null, null);
        OtherField = FieldToTextConverterobject("OTHERLabel", null, null);
				TotalField = FieldToTextConverterobject("TotalLabel", null, "TITLECASE");
				ABLATIONSUMMARYField = FieldToTextConverterobject("ABLATIONSUMMARYLabel", null, "CAPS");
				TreatmentNoteField = FieldToTextConverterobject("treatmentnotesUID", null, null);
				TemperatureField = FieldToTextConverterobject("TEMPERATURELabel", null, "TITLECASE");
				pageField = FieldToTextConverterobject("PageUID", null, null);
				PROCEDUREAUDITTRAILField = FieldToTextConverterobject("PROCEDUREAUDITTRAILUID", null, null);
				BMIField = FieldToTextConverterobject("BMILabel", null, null);
			}
		}

		/// <summary>
		/// Saves ablation temperature chart as an image
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="ablationDatasListItems">ablation datas list items</param>
		/// <param name="imageName">image name</param>
		/// <param name="imageTitle">image title</param>
		/// <param name="PDFImagePath"></param>
		private void SaveAblationTemperatureChartAsImage(
			List<AblationDataDetails> ablationDatasListItems,
			string imageName,
			string imageTitle,
			string PDFImagePath)
		{
			double previousYValue = double.MinValue;
			bool isContinuousLine = false;
			List<Chart> chartList = new List<Chart>();

			var chart = new Chart
			{
				BackColor = System.Drawing.Color.Transparent,
				Name = "ChartTemperature",
				Dock = DockStyle.Fill,
				Enabled = true
			};

			InitializeTemperatureGraphic(chart);

			_chartTemperature = chart;

			chartList.Add(chart);

			_seriesTemperature = chartList[0].Series[0];
			_seriesTemperature.Points.Add(0, 0);


			for(var i = 0; i < ablationDatasListItems.Count; i++)
			{
				//For performance consideration, do not add Point when the Y value remains the same.
				//This will avoid the chart to be redraw each time a point is added which can cause performance issue
				//when the list have several thousands of points.
				if(Math.Abs(ablationDatasListItems[i].TC1Reading - previousYValue) > 0.01)
				{
					if(isContinuousLine)
					{
						_seriesTemperature.Points.AddXY(ablationDatasListItems[i - 1].ID, ablationDatasListItems[i - 1].TC1Reading);
						isContinuousLine = false;
					}

					_seriesTemperature.Points.AddXY(ablationDatasListItems[i].ID, ablationDatasListItems[i].TC1Reading);
				}
				else
				{
					if(i == ablationDatasListItems.Count - 1)
					{
						_seriesTemperature.Points.AddXY(ablationDatasListItems[i].ID, ablationDatasListItems[i].TC1Reading);
					}

					isContinuousLine = true;
				}
				previousYValue = ablationDatasListItems[i].TC1Reading;

			}

			//Select the first X-axis value and update related values
			_chartTemperature.Annotations.Clear();
			if(_seriesTemperature.Points.Count > 1)
			{
				var firstTimeFrame = 0;
				var verticalLine = GetVerticalAnnotationLine(firstTimeFrame, _chartTemperature.ChartAreas[0]);
				if(verticalLine != null)
				{
					_chartTemperature.Annotations.Clear();
					_chartTemperature.Annotations.Add(verticalLine);
				}
			}

			_chartTemperature.ChartAreas[0].AxisX.Maximum = ablationDatasListItems.Count + 10; //give some place for user's finger
			SetTemperatureChartInterval(ablationDatasListItems.Count);

			var directoryToCreate_ = Path.Combine(GetBasePath(), PDFImagePath);
			if(!Directory.Exists(directoryToCreate_))
			{
				Directory.CreateDirectory(Path.Combine(GetBasePath(), PDFImagePath));
			}
			SaveToImage(GetBasePath() + PDFImagePath + imageName, imageTitle);
		}

		/// <summary>
		/// This function gets vertical annotation line for the chart
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private VerticalLineAnnotation GetVerticalAnnotationLine(int xValue, ChartArea chartArea)
		{
			var verticalLine = new VerticalLineAnnotation();

			verticalLine.AxisX = chartArea.AxisX;
			verticalLine.AllowMoving = true;
			verticalLine.IsInfinitive = true;
			verticalLine.ClipToChartArea = verticalLine.Name;
			verticalLine.Name = "myVerticalLine";
			verticalLine.LineColor = SERIES_COLOR;
			verticalLine.LineWidth = 2;
			verticalLine.X = xValue;
			verticalLine.ClipToChartArea = chartArea.Name;
			return verticalLine;
		}

		/// <summary>
		/// This function sets temperature interval for the chart
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void SetTemperatureChartInterval(int xAxisTimeValue)
		{
			if(_chartTemperature != null && _chartTemperature.ChartAreas != null &&
					_chartTemperature.ChartAreas.Count > 0 && _chartTemperature.ChartAreas[0].AxisX != null)
			{
				//Preset values set by pressing the timer increase/decrease arrows.
				if(xAxisTimeValue <= 30)
				{
					_chartTemperature.ChartAreas[0].AxisX.Interval = 5;
				}
				else if(xAxisTimeValue <= 60)
				{
					_chartTemperature.ChartAreas[0].AxisX.Interval = 10;
				}
				else if(xAxisTimeValue <= 150)
				{
					_chartTemperature.ChartAreas[0].AxisX.Interval = 20;
				}
				else if(xAxisTimeValue <= 240)
				{
					_chartTemperature.ChartAreas[0].AxisX.Interval = 30;
				}
				else if(xAxisTimeValue <= 480)
				{
					_chartTemperature.ChartAreas[0].AxisX.Interval = 50;
				}
				else
				{
					_chartTemperature.ChartAreas[0].AxisX.Interval = Math.Ceiling(_chartTemperature.ChartAreas[0].AxisX.Maximum / 10);
				}
			}
			else
			{
				_chartTemperature.ChartAreas[0].AxisX.Interval = 5;
			}
		}

		/// <summary>
		/// This function initialize temperature value for the chart
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void InitializeTemperatureGraphic(Chart ChartTemperature)
		{
			string cryoballoonTemperature = string.Empty;
			if(Models.Languages.GuiFieldTranslation.ContainsKey("CryoBalloonTemperatureLabel"))
			{
				cryoballoonTemperature = Models.Languages.GuiFieldTranslation["CryoBalloonTemperatureLabel"];
			}

			ChartTemperature.Titles.Add(cryoballoonTemperature);
			ChartTemperature.Titles[0].Font = new System.Drawing.Font("Courrier New", 16.0f, System.Drawing.FontStyle.Bold);
			ChartTemperature.Titles[0].ForeColor = System.Drawing.Color.White;
			ChartTemperature.Titles[0].Alignment = System.Drawing.ContentAlignment.MiddleCenter;
			ChartTemperature.ChartAreas.Add("TemperatureArea");
			ChartTemperature.ChartAreas[0].BackColor = System.Drawing.Color.Transparent;
			ChartTemperature.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
			ChartTemperature.ChartAreas[0].AxisX.Minimum = 0;
			ChartTemperature.ChartAreas[0].AxisX.Maximum = 240;
			ChartTemperature.ChartAreas[0].AxisX.Interval = 1;  //30;
			ChartTemperature.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
			ChartTemperature.ChartAreas[0].AxisX.IsStartedFromZero = true;
			ChartTemperature.ChartAreas[0].AxisX.MajorGrid.LineColor = GRID_LINES_COLOR;
			ChartTemperature.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = TEMPERATURE_GRID_DASH_STYLE;
			ChartTemperature.ChartAreas[0].AxisX.LineColor = GRID_LINES_COLOR;
			ChartTemperature.ChartAreas[0].AxisX.LineDashStyle = TEMPERATURE_GRID_DASH_STYLE;
			ChartTemperature.ChartAreas[0].AxisX.LabelStyle.ForeColor = GRID_LINES_COLOR;
			ChartTemperature.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Courrier New", 10.0f, System.Drawing.FontStyle.Bold);

			ChartTemperature.ChartAreas[0].AxisY.MajorGrid.LineColor = GRID_LINES_COLOR;
			ChartTemperature.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = TEMPERATURE_GRID_DASH_STYLE;
			ChartTemperature.ChartAreas[0].AxisY.LineColor = GRID_LINES_COLOR;
			ChartTemperature.ChartAreas[0].AxisY.LabelStyle.ForeColor = GRID_LINES_COLOR;
			ChartTemperature.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("Courrier New", 10.0f, System.Drawing.FontStyle.Bold);
			ChartTemperature.ChartAreas[0].AxisY.Minimum = TEMPERATURE_MIN_VALUE;
			ChartTemperature.ChartAreas[0].AxisY.Maximum = TEMPERATURE_MAX_VALUE;
			ChartTemperature.ChartAreas[0].AxisY.Interval = 20;

			//To make the X-axis appear on Y axis 0.
			ChartTemperature.ChartAreas[0].AxisX.Crossing = 0;
			ChartTemperature.ChartAreas[0].AxisY.Crossing = 0;

			//// Set Antialiasing mode
			////this can be set lower if there are any performance issues!
			ChartTemperature.AntiAliasing = AntiAliasingStyles.None;
			ChartTemperature.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;

			ChartTemperature.Series.Clear();
			ChartTemperature.Series.Add("Temperature");
			ChartTemperature.Series[0].ChartType = SeriesChartType.FastLine;
			ChartTemperature.Series[0].BorderWidth = 2;
			ChartTemperature.Series[0].IsVisibleInLegend = false;
			ChartTemperature.Series[0].Color = SERIES_COLOR;

			//Add isolation vein duration serie
			ChartTemperature.Series.Add("VeinIsolationDuration");
			ChartTemperature.Series[1].ChartType = SeriesChartType.Bubble;
			ChartTemperature.Series[1].BorderWidth = 1;
			ChartTemperature.Series[1].IsVisibleInLegend = false;
			ChartTemperature.Series[1].Color = SERIES_COLOR_ISOLATED_VEIN;
			ChartTemperature.Series[1].MarkerStyle = MarkerStyle.Circle;
			ChartTemperature.Series[1]["BubbleMinSize"] = "5";
			ChartTemperature.Series[1]["BubbleMaxSize"] = "5";

			ChartTemperature.Series.Add("AblationFail");
			ChartTemperature.Series[2].ChartType = SeriesChartType.Bubble;
			ChartTemperature.Series[2].BorderWidth = 1;
			ChartTemperature.Series[2].IsVisibleInLegend = false;
			ChartTemperature.Series[2].Color = SERIES_COLOR_ABLATION_FAIL;
			ChartTemperature.Series[2].MarkerStyle = MarkerStyle.Triangle;
			ChartTemperature.Series[2]["BubbleMinSize"] = "5";
			ChartTemperature.Series[2]["BubbleMaxSize"] = "5";

		}

		/// <summary>
		/// This function converts chart to image
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>

		private void SaveToImage(string imageName, string imageTitle)
		{

			_chartTemperature.Titles[0].ForeColor = System.Drawing.Color.Black;
			_chartTemperature.Titles[0].Text = imageTitle;
			_chartTemperature.Width = 750;
			_chartTemperature.Height = 250;
			_chartTemperature.SaveImage(imageName, ChartImageFormat.Png);
		}

		/// <summary>
		/// This function returns catheter type string
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private string CatheterUsed(List<Enumeration.CatheterType> catheterType, string parameter)
		{
			var info = new CatheterIDToNameMult();
			return info.Convert(catheterType, null, parameter, null).ToString();
		}

		/// <summary>
		/// This function converts a string to double value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private string MinsConvert(object value, string parameter)
		{
			var minsConvert = new StringTodoubleConverter();
			return minsConvert.Convert(value, null, parameter, null).ToString();
		}

		/// <summary>
		/// This function returns scale unit value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private string ScaleUnit(Enumeration.WeightUnit value, string parameter)
		{
			var scaleUnit = new ScaleUnitToTextConverter();
			return scaleUnit.Convert(value, null, parameter, null).ToString();
		}
		/// <summary>
		/// This function returns toise unit
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>

		private string ToiseUnit(Enumeration.WeightUnit value, string parameter)
		{
			var toiseUnit = new ToiseUnitToTextConverter();
			return toiseUnit.Convert(value, null, parameter, null).ToString();
		}
		/// <summary>
		/// This function coverts procedure type to string
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>

		private string ProcedureConverter(object value, string parameter)
		{
			string returnresult = " ";
			ProcedureToStringConverter procedurevalue = new ProcedureToStringConverter();
			returnresult = procedurevalue.Convert(value, null, parameter, null).ToString();
			if(returnresult == "00")
				returnresult = "--";
			return returnresult;
		}

		/// <summary>
		/// This function converts field value to translate text string.
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		private string FieldToTextConverterobject(object value, System.Type targetType, object parameter)
		{
			var fieldtotextValue = new FieldToTextConverter();
			return fieldtotextValue.Convert(value, targetType, parameter, null).ToString();
		}

		private string GetBasePath()
		{
			string thePath = "";

			String path = AppDomain.CurrentDomain.BaseDirectory;
			String[] extract = Regex.Split(path, "bin");  //split it in bin
			thePath = extract[0];
			return thePath;
		}
		/// <summary>
		/// This function coverts gender int to translated string.
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private string GetGender(int value)
		{
			if(value == 1)
			{
				return FieldToTextConverterobject("MaleLabel", null, null);
			}
			else if(value == 0)
			{
				return FieldToTextConverterobject("FemaleLabel", null, null);
			}
			else
				return "--";

		}

		/// <summary>
		/// This function converts date birth  formate
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private string DateBirthConvert(string value)
		{
			string DateBirth = "--";
			if(value.IndexOf("1800") > -1 || value == "")
				DateBirth = "--";
			else
				DateBirth = value;
			return DateBirth;
		}
	}
}