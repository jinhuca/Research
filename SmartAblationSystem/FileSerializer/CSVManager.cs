using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OfficeOpenXml;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using System.Drawing;
using OfficeOpenXml.Style;
using Shared;
using System.Linq;

namespace FileSerializer
{
  /// <summary>
  /// This class provides functions to generate a CSV files from an object
  ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class CSVManager
  {
    private static readonly byte[] bscDataChangePasscode = { 0xdb, 0xcb, 0x89, 0xa0, 0x18, 0x10, 0xb0, 0xa9, 0x6, 0xae, 0x3d, 0x3e, 0x35 };
    private string header;
    private readonly string GeneralInformationTitle = "General Information";
    private readonly string PatientInformationTitle = "Patient Information";
    private readonly string AblationDetailsInformationTitle = "Ablation Details Information";
    private readonly string GeneralInfoWorksheetName = "GeneralInfo";
    private readonly string AblationDetailsWorksheetName = "AblationDetails";
    private readonly int nbRowsTableDelimiter = 2;
    private string USBPath;
    private string BridgeFolderPath;
    private string Filename;

    List<(string, string)> PatientInfo;
    List<(string, string)> GeneralInfo;
    List<(string, string)> ConsoleInfo;
    List<(string, string)> LegendInfo;
    int inBodtyTime = 0;

    public string Password { get; set; }
    public UserType User { get; set; } = UserType.Unknown;
    public string DestinationPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value for skin to skin duration
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int InBodtyTime
    {
      get => inBodtyTime;
      set => inBodtyTime = value;
    }

    /// <summary>
    /// Default constructor
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public CSVManager()
    {
    }

    /// <summary>
    /// This function calls functions that generate CSV header / data and writes it to a file
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="engineeringData">The EngineeringData object to write to a CSV file.</param>
    /// <param name="filename">The path and filename where to save the CSV file.</param>
    public void GenerateAndWriteToFile(EngineeringData engineeringData, string filename)
    {
      try
      {
        GenerateEngineeringDataHeader();
        GenerateAndWriteEngineeringData(engineeringData, filename);
      }
      catch(Exception exception)
      {
        //To Do
        exception.ToString();
        throw;
      }
    }

    public void GenerateAndWriteToFile(
      ProcedureData data,
      string filename,
      List<string> ablationDetailsCSVHeader,
      List<string> treatmentInfoCSVHeader,
      List<List<(string, string)>> headers)
    {
      try
      {
        Filename = filename;
        GenerateAndWriteProcedureData(data);
        ConvertToXLS(data.AblationDetails, ablationDetailsCSVHeader, treatmentInfoCSVHeader, headers);
      }
      catch(Exception ex)
      {
        LogSystem.LogService.LogException(ex);
        throw;
      }
    }

    /// <summary>
    /// This function generates a CSV file content (not header) from a ProcedureData and writes it to a file
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="data">The ProcedureData object to write to a CSV file.</param>
    public void GenerateAndWriteProcedureData(ProcedureData data)
    {


      BridgeFolderPath = Path.Combine(GetBasePath(), "AblationData");

      if(!Directory.Exists(BridgeFolderPath))
      {
        Directory.CreateDirectory(BridgeFolderPath);
      }

      string dataLine = string.Empty;

      // int ablationDetailsCounter = 0;
      // int ablationECGDetailsCounter = 0;
      // DateTime ablationDetailsTimestamp = new DateTime();
      // DateTime ablationECGDetailsTimestamp = new DateTime();

      if(data != null)
      {
        try
        {
          if(!File.Exists((BridgeFolderPath + "\\AblationDetails.csv")))
          {
            File.Create((BridgeFolderPath + "\\AblationDetails.csv"));
          }

          //File.Create((BridgeFolderPath + "\\AblationDetails.csv"));



          using(StreamWriter file = new StreamWriter(BridgeFolderPath + "AblationDetails.csv", false, Encoding.UTF8))
          {

            //For each ablation treaments


            foreach(List<AblationDataDetails> _AblationDetails in data.AblationDetails)
            {

              int ListLen = _AblationDetails.Count - 1;
              int lastSite = _AblationDetails[ListLen].AblationSite;
              for(int i = 0; i < _AblationDetails.Count; i++)
              {

                //Generate line and Write data to file
                dataLine = GenerateDataLine(_AblationDetails[i], lastSite);
                file.WriteLine(dataLine);
              }
            }
          }
        }
        catch(Exception ex)
        {
          ex.ToString();
          throw;
        }
      }
    }

    /// <summary>
    /// This function generates a CSV file content (not header) from Engineering Data and writes it to a file
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="engineeringData">The EngineeringData object to write to a CSV file.</param>
    /// <param name="filename">The path and filename where to save the CSV file.</param>
    public void GenerateAndWriteEngineeringData(EngineeringData engineeringData, string filename)
    {
      string dataLine = string.Empty;

      if(engineeringData != null)
      {
        try
        {
          //Creates the directory, if it already exists this function call does nothing
          Directory.CreateDirectory(Path.GetDirectoryName(filename));

          using(StreamWriter file = new StreamWriter(filename + ".csv"))
          {
            file.WriteLine(header);

            //For each engineering data details
            for(int i = 0; i < engineeringData.EngineeringDataDetails.Count; i++)
            {
              //Generate line and Write data to file
              dataLine = GenerateDataLine(engineeringData.EngineeringDataDetails[i]);
              file.WriteLine(dataLine);
            }
          }
        }
        catch(Exception ex)
        {
          // TODO
          ex.ToString();
          throw;
        }
      }
    }

    /// <summary>
    /// This function generates a CSV file content (not header) from a test report list and writes it to a file
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="testReport">The string list to write to a CSV file.</param>
    /// <param name="filename">The path and filename where to save the CSV file.</param>
    public void GenerateAndWriteTestReport(List<String> testReport, string filename)
    {
      string dataLine = string.Empty;

      if(testReport != null)
      {
        try
        {
          using(StreamWriter file = new StreamWriter(filename + ".csv"))
          {
            header = "TestNumber,Name,ExpectedValue,ActualValue,Result";
            file.WriteLine(header);

            //For each test
            for(int i = 0; i < testReport.Count; i++)
            {
              //Write data to file
              file.WriteLine(testReport[i]);
            }
          }
        }
        catch(Exception ex)
        {
          // TODO
          ex.ToString();
          throw;
        }
      }
    }

    /// <summary>
    /// This function generates a CSV line from properties contained in AblationDataDetails for a given timestamp.
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="ablationDataDetails">Ablation Data detail object</param>
    /// <returns></returns>
    public string GenerateDataLine(AblationDataDetails ablationDataDetails, int lastSite)
    {
      string dataLine = string.Empty;

      if(ablationDataDetails != null)
      {
        string CatheterSerialNumber = "--";
        if(ablationDataDetails.CatheterSerialNumber > 0)
          CatheterSerialNumber = ablationDataDetails.CatheterSerialNumber.ToString();


        if(User == UserType.Doctor || User == UserType.Admin || User == UserType.User)
        {
          dataLine = ablationDataDetails.TimeStamp + "," +
                  ablationDataDetails.ID + "," +
                  ablationDataDetails.AblationID + "," +
                  Enum.GetName(typeof(MessageStateId), ablationDataDetails.SystemState).Replace("CAN_ID_STATE_", "") + "," +
                  ablationDataDetails.TemperatureRate + "," +
                  ablationDataDetails.TC1Reading + "," +
                  ablationDataDetails.EcgChannel3And4Reading + "," +
                  MinsConvert((int)ablationDataDetails.EcgChannel7And8Reading) + "," +
                  GetESOTemp(ablationDataDetails.EcgChannel5And6Reading) + "," +
                  ablationDataDetails.EtsSensor1 + "," +
                  ablationDataDetails.EtsSensor2 + "," +
                  ablationDataDetails.EtsSensor3 + "," +
                  ablationDataDetails.EtsSensor4 + "," +
                  ablationDataDetails.EtsSensor5 + "," +
                  ablationDataDetails.EtsSensor6 + "," +
                  ablationDataDetails.EtsSensor7 + "," +
                  ablationDataDetails.EtsSensor8 + "," +
                  ablationDataDetails.EtsSensor9 + "," +
                  ablationDataDetails.EtsSensor10 + "," +
                  ablationDataDetails.EtsSensor11 + "," +
                  ablationDataDetails.EtsSensor12 + "," +
                  ablationDataDetails.EtsSensor13;
        }
        else
        {
          dataLine = ablationDataDetails.TimeStamp + "," +
                  ablationDataDetails.ID + "," +
                  ablationDataDetails.AblationID + "," +
                  Enum.GetName(typeof(MessageStateId), ablationDataDetails.SystemState).Replace("CAN_ID_STATE_", "") + "," +
                  //  Enum.GetName(typeof(AblationSiteEnum), ablationDataDetails.AblationSite) + "," + //ablationDataDetails.AblationSite + "," +
                  // ablationDataDetails.TimeInAblation + "," +
                  ablationDataDetails.TemperatureRate + "," +
                  ablationDataDetails.TC1Reading + "," +
                  ablationDataDetails.PMCUCJReading + "," +
                  ablationDataDetails.PT1Reading + "," +
                  ablationDataDetails.PT2Reading + "," +
                  ablationDataDetails.PT3Reading + "," +
                  ablationDataDetails.PT4Reading + "," +
                  ablationDataDetails.PT5Reading + "," +
                  ablationDataDetails.PS1Reading + "," +
                  ablationDataDetails.FM1Reading + "," +
                  ablationDataDetails.TS1Reading + "," +
                  ablationDataDetails.TN2OReading + "," +
                  ablationDataDetails.LC1Reading + "," +
                  ablationDataDetails.CP1Reading + "," +
                  ablationDataDetails.CP2Reading + "," +
                  ablationDataDetails.CMCUCJReading + "," +
                  ablationDataDetails.PWMINJ + "," +
                  ablationDataDetails.PWMBAL + "," +
                  ablationDataDetails.EcgChannel3And4Reading + "," +
                  MinsConvert((int)ablationDataDetails.EcgChannel7And8Reading) + "," +
                  ablationDataDetails.BloodDetecorImValue + "," +
                  GetESOTemp(ablationDataDetails.EcgChannel5And6Reading) + "," +
                  ablationDataDetails.EtsSensor1 + "," +
                  ablationDataDetails.EtsSensor2 + "," +
                  ablationDataDetails.EtsSensor3 + "," +
                  ablationDataDetails.EtsSensor4 + "," +
                  ablationDataDetails.EtsSensor5 + "," +
                  ablationDataDetails.EtsSensor6 + "," +
                  ablationDataDetails.EtsSensor7 + "," +
                  ablationDataDetails.EtsSensor8 + "," +
                  ablationDataDetails.EtsSensor9 + "," +
                  ablationDataDetails.EtsSensor10 + "," +
                  ablationDataDetails.EtsSensor11 + "," +
                  ablationDataDetails.EtsSensor12 + "," +
                  ablationDataDetails.EtsSensor13;
        }
      }

      return dataLine;
    }

    /// <summary>
    /// This function generates a CSV line from properties contained in EngineeringDataDetails for
    /// a given timestamp
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="engineeringDataDetails">Engineering Data detail object</param>
    /// <returns>data line</returns>
    public string GenerateDataLine(EngineeringDataDetails engineeringDataDetails)
    {
      string dataLine = string.Empty;

      if(engineeringDataDetails != null)
      {
        dataLine = "\"" + engineeringDataDetails.TimeStamp + "\"," +
                    engineeringDataDetails.SystemState + "," +
                    engineeringDataDetails.RequiredTargetTemperature + ", " +
                    engineeringDataDetails.TimeToTargetTemperature + "," +
                    engineeringDataDetails.TimeToThaw + "," +
                    engineeringDataDetails.ThawTimerToTemperature + "," +
                    engineeringDataDetails.CatheterId + "," +
                    engineeringDataDetails.CatheterLot + "," +
                    engineeringDataDetails.TC1Reading + "," +
                    engineeringDataDetails.TimeInSecondIndex + "," +
                    engineeringDataDetails.PMCUCJReading + "," +
                    engineeringDataDetails.PT1Reading + "," +
                    engineeringDataDetails.PT2Reading + "," +
                    engineeringDataDetails.PT3Reading + "," +
                    engineeringDataDetails.PT4Reading + "," +
                    engineeringDataDetails.PT5Reading + "," +
                    engineeringDataDetails.PS1Reading + "," +
                    engineeringDataDetails.FM1Reading + "," +
                    engineeringDataDetails.TS1Reading + "," +
                    engineeringDataDetails.TN2OReading + "," +
                    engineeringDataDetails.LC1Reading + "," +
                    engineeringDataDetails.TIPReading + "," +
                    engineeringDataDetails.CP1Reading + "," +
                    engineeringDataDetails.CP2Reading + "," +
                    engineeringDataDetails.CIMP1Reading + "," +
                    engineeringDataDetails.PWMINJ + "," +
                    engineeringDataDetails.PWMBAL + "," +
                    engineeringDataDetails.EcgChannel1And2Reading + "," +
                    engineeringDataDetails.EcgChannel3And4Reading + "," +
                    engineeringDataDetails.EcgChannel5And6Reading + "," +
                    engineeringDataDetails.EcgChannel7And8Reading;
      }

      return dataLine;
    }

    /// <summary>
    /// This function generates the CSV file header (first line)
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void GenerateEngineeringDataHeader()
    {
      //WARNING : ANY CHANGE TO THE HEADER ORDER WILL HAVE AN IMPACT ON THE DATA'S ORDER
      header = "Timestamp," +
                  "SystemState," +
                  "RequiredTargetTemperature," +
                  "TimeToTargetTemperature," +
                  "TimeToThaw," +
                  "ThawTimerToTemperature," +
                  "CatheterId," +
                  "CatheterLot," +
                  "TC1Reading," +
                  "TimeInSecondIndex," +
                  "PMCUCJReading," +
                  "PT1Reading," +
                  "PT2Reading," +
                  "PT3Reading," +
                  "PT4Reading," +
                  "PT5Reading," +
                  "PS1Reading," +
                  "FM1Reading," +
                  "TS1Reading," +
                  "TN2OReading," +
                  "LC1Reading," +
                  "TIPReading," +
                  "CP1Reading," +
                  "CP2Reading," +
                  "CIMP1Reading," +
                  "PWMINJ," +
                  "PWMBAL," +
                  "EcgChannel1And2Reading," +
                  "EcgChannel3And4Reading," +
                  "EcgChannel5And6Reading," +
                  "EcgChannel7And8Reading";
    }
    //public void GenerateEngineeringDataHeader()
    //{
    //    //WARNING : ANY CHANGE TO THE HEADER ORDER WILL HAVE AN IMPACT ON THE DATA'S ORDER
    //    header = "Timestamp," +
    //                "SystemState," +
    //                "Cooling Timer Setpoint (°C)," +//"RequiredTargetTemperature," +
    //                "Cooling Time (sec)," + //"TimeToTargetTemperature," +
    //                "Thaw Time (sec)," + //"TimeToThaw," +
    //                "Thaw Timer Setpoint (°C)," + //"ThawTimerToTemperature," +
    //                "Catheter ID," +
    //                "Catheter Lot#," +
    //                "Ballon Temperature (TC1) (°C)," + //"TC1Reading," +
    //                "TimeInSecondIndex," +
    //                "TC1 CJ (°C)," +//"PMCUCJReading," +
    //                "Tank Pressure (PT1) (psig)," + //"PT1Reading," +
    //                "Injection Pressure (PT2) (psig)," + //"PT2Reading," +
    //                "Return Line Pressure (PT3) (psia)," + //"PT3Reading," +
    //                "Vacuum Line Pressure (PT4) (psia)," + //"PT4Reading," +
    //                "Scavenging Line Pressure(PT5) (psia)," + //"PT5Reading," +
    //                "Vent Line Switch(PS1)  (0 / 1)," + //"PS1Reading," +
    //                "Flow(FM1)(sccm)," +//"FM1Reading," +
    //                "Sub-Cooler Temperature (TS1) (°C)," +//"TS1Reading," +
    //                "TN2O (°C)," +//"TN2OReading," +
    //                "Tank Weight(LC1)(lbs)," + //"LC1Reading," +
    //                "TIPReading," +
    //                "Inner Balloon Pressure (IBP) (psig)," +// "CP1Reading," +
    //                "Outer Balloon Pressure(OBP) (psig)," +//"CP2Reading," +
    //                "TS1 CJ (°C)," + //"CIMP1Reading," +
    //                "Injection PWM (%)," +//"PWMINJ," +
    //                "Balloon PWM (%)," +//"PWMBAL," +
    //                "EcgChannel1And2Reading," +
    //                "DMS Value (G)," +//"EcgChannel3And4Reading," +
    //                "EcgChannel5And6Reading," +
    //                "DMS Value(%),";//"EcgChannel7And8Reading";
    //}

    /// <summary>
    /// This function generates a DateTime object from a string received in parameter
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="timeStamp">The timestamp to parse in DateTime.</param>
    /// <returns>DateTime parsed from string received in parameter.</returns>
    public DateTime ParseTimeStamp(string timeStamp)
    {
      DateTime datetime = new DateTime();

      if(!string.IsNullOrWhiteSpace(timeStamp))
      {
        if(!DateTime.TryParse(timeStamp, out datetime))
        {
          throw new NotImplementedException();
        }
      }

      return datetime;
    }

    /// <summary>
    /// This function reads data from xls file
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void ReadXls(string excelFileName)
    {
      Excel.Application app = new Excel.Application();

      Excel.Workbook workBook = app.Workbooks.Open(excelFileName, Type.Missing, true, Type.Missing, Password, Type.Missing, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);


      foreach(Excel.Worksheet sheet in workBook.Worksheets)
      {
        sheet.Select();
        if(sheet.Name == AblationDetailsWorksheetName)
        {
          int lastUsedRow = sheet.Cells.Find("*", System.Reflection.Missing.Value,
                         System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                         Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlPrevious,
                         false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;

          int lastUsedColumn = sheet.Cells.Find("*", System.Reflection.Missing.Value,
                         System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                         Excel.XlSearchOrder.xlByColumns, Excel.XlSearchDirection.xlPrevious,
                         false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Column;

          string columnName = GetExcelColumnName(lastUsedColumn);

          for(int i = 1; i <= lastUsedRow; i++)
          {
            Excel.Range range = sheet.get_Range("A" + i.ToString(), columnName + i.ToString());
            System.Array myvalues = (System.Array)range.Cells.Value;
            string[] strArray = ConvertToStringArray(myvalues);
            if(i > 2)
            {
              DateTime origin = new DateTime(1899, 12, 30, 00, 00, 00, 00);
              string t = origin.AddMilliseconds(Convert.ToDouble(strArray[0]) * 24 * 3600 * 1000).ToString("yyyy/MM/dd HH:mm:ss.fff");

            }
          }
        }
        else
        {

        }
      }
      workBook.Close(false);
    }

    /// <summary>
    /// This function gets Excel column name, not being used
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private string GetExcelColumnName(int columnNumber)
    {
      int dividend = columnNumber;
      string columnName = String.Empty;
      int modulo;

      while(dividend > 0)
      {
        modulo = (dividend - 1) % 26;
        columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
        dividend = (int)((dividend - modulo) / 26);
      }

      return columnName;
    }

    /// <summary>
    /// This function saves the data to array
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    string[] ConvertToStringArray(System.Array values)
    {

      // create a new string array
      string[] theArray = new string[values.Length];

      // loop through the 2-D System.Array and populate the 1-D String Array
      for(int i = 1; i <= values.Length; i++)
      {
        if(values.GetValue(1, i) == null)
          theArray[i - 1] = "";
        else
          theArray[i - 1] = (string)values.GetValue(1, i).ToString();
      }

      return theArray;
    }

    /// <summary>
    /// This function converts CSV to XLS
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>

    public void ConvertToXLS(List<List<AblationDataDetails>> ablationDataDetails, List<string> ablationDetailsCSVHeader,
      List<string> treatmentInfoCSVHeader, List<List<(string, string)>> dataList)
    {
      if(User != UserType.Doctor && User != UserType.Bsc && User != UserType.BostonBsc && User != UserType.Admin && User != UserType.User)
      {
        return;
      }

      if(ablationDataDetails == null)
      {
        throw new ArgumentException(nameof(ablationDataDetails));
      }

      if(ablationDetailsCSVHeader == null)
      {
        throw new ArgumentException(nameof(ablationDetailsCSVHeader));
      }

      if(treatmentInfoCSVHeader == null)
      {
        throw new ArgumentException(nameof(treatmentInfoCSVHeader));
      }

      if(dataList == null)
      {
        throw new ArgumentException(nameof(dataList));
      }

      int numberOfTreatments_ = ablationDataDetails.Count();

      GeneralInfo = dataList[0];
      ConsoleInfo = dataList[1];
      PatientInfo = dataList[2];
      LegendInfo = dataList[3];

      string worksheetsName = AblationDetailsWorksheetName;

      bool firstRowIsHeader = false;

      var format = new ExcelTextFormat();
      format.Delimiter = ',';
      format.EOL = "\r";              // DEFAULT IS "\r\n";
                                      // format.TextQualifier = '"';  

      string csvFileName = BridgeFolderPath + "AblationDetails.csv";
      string excelFileName = Filename + ".xlsx";  //USBPath;   

      if(File.Exists(excelFileName)) File.Delete(excelFileName);

      ExcelPackage.LicenseContext = LicenseContext.Commercial;
      using(ExcelPackage package = new ExcelPackage(new FileInfo(excelFileName)))
      {
        ExcelWorksheet worksheet2 = package.Workbook.Worksheets.Add(GeneralInfoWorksheetName);

        worksheet2.TabColor = Color.Yellow;

        /*** General Information ***/
        int nbRowsGeneralInfo = WriteGeneralInformation(worksheet2);

        if(User == UserType.Doctor || User == UserType.Admin || User == UserType.User)
        {
          int generalInfoHeaderCount = GeneralInfo.Count;
          int patientInfoHeaderCount = PatientInfo.Count;

          /*** Patient Information ***/
          XLSHeader1(nbRowsGeneralInfo + nbRowsTableDelimiter, 1, generalInfoHeaderCount, PatientInformationTitle, worksheet2);
          for(int idx = 0; idx < patientInfoHeaderCount; idx++)
          {
            XLSLabel2(idx + nbRowsGeneralInfo + nbRowsTableDelimiter + 1, 1, worksheet2, PatientInfo[idx].Item1);
            if(PatientInfo[idx].Item1 == "Procedure Diagnosis" || PatientInfo[idx].Item1 == "Procedure Outcome")
            {
              XLSLargeMultiCellValue(idx + nbRowsGeneralInfo + nbRowsTableDelimiter + 1, 2, generalInfoHeaderCount, worksheet2, PatientInfo[idx].Item2);
            }
            else if (PatientInfo[idx].Item1 == "Date of Birth")
            {
              var dob_ = PatientInfo[idx].Item2 == "01/01/1800" ? "--" : PatientInfo[idx].Item2;
              XLSMultiCellValue(idx + nbRowsGeneralInfo + nbRowsTableDelimiter + 1, 2, generalInfoHeaderCount, worksheet2, dob_);
            }
            else
            {
              XLSMultiCellValue(idx + nbRowsGeneralInfo + nbRowsTableDelimiter + 1, 2, generalInfoHeaderCount, worksheet2, PatientInfo[idx].Item2);
            }
          }

          /*** Treatment Information ***/
          WriteTreatmentInformation(worksheet2, nbRowsGeneralInfo + nbRowsTableDelimiter + patientInfoHeaderCount + nbRowsTableDelimiter, 1);
        }
        else if(User == UserType.BostonBsc || User == UserType.Bsc)
        {
          /*** Firmware versions ***/
          int firmwareInfoHeaderCount = ConsoleInfo.Count;
          for(int i = 0; i < firmwareInfoHeaderCount; i++)
          {
            XLSLabel2(nbRowsGeneralInfo + nbRowsTableDelimiter + i, 1, worksheet2, ConsoleInfo[i].Item1); ;
            XLSValue2(nbRowsGeneralInfo + nbRowsTableDelimiter + i, 2, worksheet2, ConsoleInfo[i].Item2);
          }

          /*** Treatment Information ***/
          WriteTreatmentInformation(worksheet2, nbRowsGeneralInfo + nbRowsTableDelimiter + firmwareInfoHeaderCount + 1, 1);
        }

        worksheet2.Protection.IsProtected = true;
        worksheet2.Protection.SetPassword(new string(PasswordUtils.DecryptPasscode(bscDataChangePasscode)));
        worksheet2.Protection.AllowFormatColumns = true;
        worksheet2.Cells.AutoFitColumns();

        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetsName);

        /*** Ablation Detail Information Header ***/
        XLSHeader1(1, 1, ablationDetailsCSVHeader.Count, AblationDetailsInformationTitle, worksheet);
        for(int idx = 0; idx < ablationDetailsCSVHeader.Count; idx++)
        {
          XLSLabel2(2, idx + 1, worksheet, ablationDetailsCSVHeader[idx]);
        }

        /*** Legend ***/

        for(int i = 0; i < LegendInfo.Count; i++)
        {
          XLSLabel2(3 + i, ablationDetailsCSVHeader.Count + 2, worksheet, LegendInfo[i].Item1);
          XLSValue3(3 + i, ablationDetailsCSVHeader.Count + 3, worksheet, LegendInfo[i].Item2);
        }

        worksheet.View.FreezePanes(3, 1);


        worksheet.Cells["A3"].LoadFromText(new FileInfo(csvFileName), format);
        worksheet.Protection.IsProtected = true;

        //if (FileType != "Doctor")
        //{
        worksheet.Column(1).Style.Numberformat.Format = "yyyy/MM/dd HH:mm:ss.000";
        //}
        worksheet.Protection.AllowFormatColumns = true;
        worksheet.Protection.SetPassword(new string(PasswordUtils.DecryptPasscode(bscDataChangePasscode)));
        worksheet.Column(1).Width = 22;

        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
          var cell = worksheet.Cells[2, col];
          if (cell.Value == null) // Assuming data starts from row 2
          {
            break;
          }
          worksheet.Column(col).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
          worksheet.Column(col).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
          if (cell.Value.ToString().Contains(Environment.NewLine))
          {
            string originalValue = cell.Value.ToString();
            string newValue = originalValue.Replace(Environment.NewLine,"\n");
            cell.Value = newValue;
            cell.Style.WrapText = true;
          }
        }

        worksheet.TabColor = Color.Green;
        worksheet.Cells.AutoFitColumns();
        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {

          var cell = worksheet.Cells[2, col];
          switch (cell.Value?.ToString())
          {
            case "Timestamp":
              worksheet.Column(col).Width = 23;
              break;
            case "Time":
              worksheet.Column(col).Width = 5.14;
              break;
            case "ID":
              worksheet.Column(col).Width = 2.29;
              break;
            case "State":
              worksheet.Column(col).Width = 10.57;
              break;
            case "TR":
              worksheet.Column(col).Width = 2.57;
              break;
            case "TC1":
              worksheet.Column(col).Width = 4.29;
              break;
            case "TC1CJ":
              worksheet.Column(col).Width = 6.86;
              break;
            case "PT1":
            case "PT2":
              worksheet.Column(col).Width = 5.86;
              break;
            case "PT3":
              worksheet.Column(col).Width = 4.71;
              break;
            case "PT4":
              worksheet.Column(col).Width = 3.57;
              break;
            case "PT5":
              worksheet.Column(col).Width = 4.71;
              break;
            case "PS1":
              worksheet.Column(col).Width = 4.29;
              break;
            case "FM1":
              worksheet.Column(col).Width = 4.71;
              break;
            case "TS1":
              worksheet.Column(col).Width = 4.29;
              break;
            case "TN2O":
              worksheet.Column(col).Width = 6.57;
              break;
            case "LC1":
              worksheet.Column(col).Width = 3.57;
              break;
            case "IBP":
              worksheet.Column(col).Width = 3.57;
              break;
            case "OBP":
              worksheet.Column(col).Width = 5.43;
              break;
            case "TS1CJ":
              worksheet.Column(col).Width = 5.86;
              break;
            case "IPWM":
              worksheet.Column(col).Width = 6.43;
              break;
            case "BPWM":
              worksheet.Column(col).Width = 6.86;
              break;
            case "BDI":
              worksheet.Column(col).Width = 4.57;
              break;
          }
          if (cell.Value!=null)
          {
            if (cell.Value.ToString().Contains("ESO"))
            {
              worksheet.Column(col).Width = 5.5;
            }else if (cell.Value.ToString().Contains("DMS"))
            {
              worksheet.Column(col).Width = 5.43;
            }
          }

        }
        package.Workbook.Protection.LockStructure = true;
        package.Save(Password);
      }

      File.Delete(DestinationPath + "AblationDetails.csv");

      void XLSHeader1(int row, int startcol, int endcol, object title, ExcelWorksheet worksheet)
      {
        worksheet.Cells[row, startcol, row, endcol].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells[row, startcol, row, endcol].Style.Font.Color.SetColor(Color.White);
        worksheet.Cells[row, startcol, row, endcol].Merge = true;
        worksheet.Cells[row, startcol, row, endcol].Style.Border.BorderAround(ExcelBorderStyle.None);
        worksheet.Cells[row, startcol, row, endcol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Cells[row, startcol, row, endcol].Style.Font.Size = 22;
        worksheet.Cells[row, startcol, row, endcol].Style.Font.Bold = true;
        worksheet.Cells[row, startcol, row, endcol].Value = title;
        worksheet.Row(row).Height = 30;
      }

      void XLSRowStyle(int row, int startcol, int endcol, ExcelWorksheet worksheet)
      {
        worksheet.Cells[row, startcol, row, endcol].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells[row, startcol, row, endcol].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(49, 140, 231));
        worksheet.Cells[row, startcol, row, endcol].Style.Border.BorderAround(ExcelBorderStyle.Thick);
        worksheet.Row(row).Height = 30;
        worksheet.Cells[row, startcol, row, endcol].Style.Font.Size = 12;
        worksheet.Cells[row, startcol, row, endcol].Style.Font.Bold = true;
      }

      void XLSValue1(int row, int col, ExcelWorksheet worksheet, object value)
      {
        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(15, 245, 145));
        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Hair);
        worksheet.Cells[row, col].Value = value;
        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
      }

      void XLSValue2(int row, int col, ExcelWorksheet worksheet, object value)
      {
        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(15, 245, 145));
        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        if (value is int value_ && value_ == 0)
        {
          value = "--";
        }
        worksheet.Cells[row, col].Value = value;
        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
      }

      void XLSValue2_NoDash(int row, int col, ExcelWorksheet worksheet, object value)
      {
        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(15, 245, 145));
        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        worksheet.Cells[row, col].Value = value;
        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
      }

      void XLSValue3(int row, int col, ExcelWorksheet worksheet, object value)
      {
        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(180, 180, 180));
        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        worksheet.Cells[row, col].Value = value;
      }

      void XLSMultiCellValue(int row, int startcol, int endcol, ExcelWorksheet worksheet, object value)
      {
        worksheet.Cells[row, startcol, row, endcol].Merge = true;
        worksheet.Cells[row, startcol, row, endcol].Value = value;
        worksheet.Cells[row, startcol, row, endcol].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells[row, startcol, row, endcol].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(15, 245, 145));
        worksheet.Cells[row, startcol, row, endcol].Style.Border.BorderAround(ExcelBorderStyle.Thin);
      }

      void XLSLargeMultiCellValue(int row, int startcol, int endcol, ExcelWorksheet worksheet, object value)
      {
        worksheet.Cells[row, startcol, row, endcol].Merge = true;
        worksheet.Cells[row, startcol, row, endcol].Value = value;
        worksheet.Cells[row, startcol, row, endcol].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells[row, startcol, row, endcol].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(15, 245, 145));
        worksheet.Cells[row, startcol, row, endcol].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        worksheet.Rows[row].Style.WrapText = true;
        worksheet.Cells[row, startcol, row, endcol].Style.WrapText = true;
        int valuelength = Convert.ToString(value).Length;
        int lines = (int)Math.Ceiling(valuelength / 60.0);
        worksheet.Cells[row, startcol, row, endcol].EntireRow.Height = lines * 15;
        worksheet.Cells[row, startcol, row, endcol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
      }

      void XLSLabel1(int row, int col, ExcelWorksheet worksheet, object label)
      {
        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(49, 140, 231));
        worksheet.Cells[row, col].Style.Font.Bold = true;
        worksheet.Cells[row, col].Value = label;
      }

      void XLSLabel2(int row, int col, ExcelWorksheet worksheet, object label)
      {
        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(49, 140, 231));
        worksheet.Cells[row, col].Style.Font.Bold = true;
        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        worksheet.Cells[row, col].Value = label;
      }

      // Write the GeneralInformation header table and returns the number of rows it takes
      int WriteGeneralInformation(ExcelWorksheet worksheet)
      {
        int generalInfoHeaderCount = GeneralInfo.Count();

        XLSHeader1(1, 1, generalInfoHeaderCount, GeneralInformationTitle, worksheet);

        XLSRowStyle(2, 1, generalInfoHeaderCount, worksheet);

        for(int idx = 0; idx < generalInfoHeaderCount; idx++)
        {
          XLSLabel1(2, idx + 1, worksheet, GeneralInfo[idx].Item1);
          XLSValue1(3, idx + 1, worksheet, GeneralInfo[idx].Item2);
        }
        return 3;
      }

      void WriteTreatmentInformation(ExcelWorksheet worksheet, int startrow, int startcol)
      {
        XLSLabel2(startrow, 1, worksheet, null);

        for(int key = 0; key < treatmentInfoCSVHeader.Count; key++)
        {
          XLSLabel2(startrow + 1 + key, 1, worksheet, treatmentInfoCSVHeader[key]);
        }

        for(int t = 1; t <= numberOfTreatments_; t++)
        {

          XLSLabel2(startrow, t + 1, worksheet, "Treatment " + t);
          XLSValue2(startrow + 1, t + 1, worksheet, Enum.GetName(typeof(AblationSiteEnum), GetLastValue(ablationDataDetails[t - 1], x=> x.AblationSite))); // Ablation Site
          XLSValue2(startrow + 2, t + 1, worksheet, ablationDataDetails[t - 1][0].BalloonSize); // Balloon size
          XLSValue2(startrow + 3, t + 1, worksheet, GetMinimumValue(ablationDataDetails[t - 1], x => x.MaxTemperatureRate)); // Minimum Temperature
          XLSValue2_NoDash(startrow + 4, t + 1, worksheet, ablationDataDetails[t - 1][0].RequiredTargetTemperature); // Cooling Timer Setpoint
          XLSValue2(startrow + 5, t + 1, worksheet, GetLastValue(ablationDataDetails[t - 1], x => x.TimeToTargetTemperature)); // Cooling Time
          XLSValue2(startrow + 6, t + 1, worksheet, ablationDataDetails[t - 1][0].RequiredAblationTime); // Ablation Duration Setpoint
          XLSValue2(startrow + 7, t + 1, worksheet, GetLastValue(ablationDataDetails[t - 1], x => x.TimeToVeinIsolation)); // Time to Vein Isolation

          int? temperatureAtTti_;
          int? timeSinceTti_;
          if(ablationDataDetails[t - 1].Any(x => x.TimeToVeinIsolation > 0))
          {
            temperatureAtTti_ = (int)ablationDataDetails[t - 1].First(x => x.TimeToVeinIsolation > 0).TC1Reading;
            timeSinceTti_ = ablationDataDetails[t - 1].Count(x => 
              x.TimeToVeinIsolation > 0 && 
              (x.SystemState == (int)MessageStateId.CAN_ID_STATE_ABLATION || x.SystemState == (int)MessageStateId.CAN_ID_STATE_TRANSITION)) - 1;
          }
          else
          {
            temperatureAtTti_ = null;
            timeSinceTti_ = null;
          }
          
          XLSValue2(startrow + 8, t + 1, worksheet, (temperatureAtTti_ is null) ? "--" : temperatureAtTti_.ToString()); // Temperature at isolation
          XLSValue2(startrow + 9, t + 1, worksheet, timeSinceTti_ is null ? "--" : timeSinceTti_.ToString()); // Time since isolation
          XLSValue2(startrow + 10, t + 1, worksheet, GetLastValue(ablationDataDetails[t - 1], x => x.TimeToThaw)); // Thaw Time
          XLSValue2_NoDash(startrow + 11, t + 1, worksheet, ablationDataDetails[t - 1][0].ThawTimerToTemperature); // Thaw Timer Setpoint
          XLSValue2(startrow + 12, t + 1, worksheet, ablationDataDetails[t - 1].Count(x => x.SystemState == (int)MessageStateId.CAN_ID_STATE_THAWING));  // Total thawing time
          XLSValue2(startrow + 13, t + 1, worksheet, GetCatheterTypeByID(ablationDataDetails[t - 1][0].CatheterId)); // Catheter ID
          XLSValue2(startrow + 14, t + 1, worksheet, ablationDataDetails[t - 1][0].CatheterLot); // Catheter Lot
          XLSValue2(startrow + 15, t + 1, worksheet, ablationDataDetails[t - 1][0].CatheterSerialNumber); // Catheter Serial Number
          XLSValue2(startrow + 16, t + 1, worksheet, ablationDataDetails[t - 1][0].CatheterContainer); // Catheter Container 
          XLSValue2_NoDash(startrow + 17, t + 1, worksheet, MinsConvert(ablationDataDetails[t - 1].LastOrDefault()?.MinimumDiaphragmMovementValue ?? -1)); // Minimum DMS Value 
          XLSValue2_NoDash(startrow + 18, t + 1, worksheet, MinsConvert(ablationDataDetails[t - 1].LastOrDefault()?.MinimumEsophagusTemperatureValue ?? -1)); // Minimum ESO Temp
        }
      }

      object GetCatheterTypeByID(int id)
      {
        if(User == UserType.BostonBsc || User == UserType.Bsc)
        {
          return id;
        }
        switch(id)
        {
          case 1:
            return "POLARx";
          case 2:
            return "POLARx FIT";
          case 129:
            return "POLARx Test";
          case 130:
            return "POLARx FIT Test";
          default:
            return "Unknown";
        }
      }

      object GetMinimumValue(List<AblationDataDetails> ablationdetailslist, Func<AblationDataDetails, object> property)
      {
        var enumerable = ablationdetailslist.Select(property).Where(x => x.ToString() != "--");
        if(enumerable == null || enumerable.Count() == 0)
        {
          return "--";
        }
        return enumerable.Select(x => Convert.ToInt32(x)).Min();
      }

      object GetLastValue(List<AblationDataDetails> ablationdetailslist, Func<AblationDataDetails, object> property)
      {
        return property(ablationdetailslist[ablationdetailslist.Count - 1]);
      }
    }

    /// <summary>
    /// This function converts Mins value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private string MinsConvert(int value)
    {

      if(value > 100 || value < 0)
      {
        return "--";
      }
      else
      {
        return (System.Convert.ToInt32(value).ToString());
      }
    }

    /// <summary>
    /// This function converts date birth  formate
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// Fix :253#
    private string DateBirthConvert(string value)
    {
      string DateBirth = "--";
      if(value == "01/01/1800" || value == "")
        DateBirth = "--";
      else
        DateBirth = Convert.ToDateTime(value).ToString("MMM dd yyyy");
      return DateBirth;
    }

    /// <summary>
    /// This function converts ESO Temp value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private string GetESOTemp(int _eSOTemp)
    {
      if(_eSOTemp >= 50 || _eSOTemp <= 0)
      {
        return "--";
      }
      return _eSOTemp.ToString();

    }

    /// <summary>
    /// This function converts remote firmware value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private string GetConvertValue(string value)
    {
      if(value == "0" || value == null)
      {
        return "--";
      }
      return value;

    }

    /// <summary>
    /// This function returns base directory of this application
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private string GetBasePath()
    {
      string thePath = "";

      String path = AppDomain.CurrentDomain.BaseDirectory;
      String[] extract = Regex.Split(path, "bin");  //split it in bin
      thePath = extract[0];

      return thePath;
    }

    /// <summary>
    /// Message state ID enumeration
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public enum MessageStateId
    {
      CAN_ID_STATE_UNKNOWN = 0,
      CAN_ID_STATE_IDLE = 256,
      CAN_ID_STATE_READY = 512,
      CAN_ID_STATE_INFLATION = 768,
      CAN_ID_STATE_TRANSITION = 1024,
      CAN_ID_STATE_ABLATION = 1280,
      CAN_ID_STATE_THAWING = 1536,
      CAN_ID_STATE_EXCEPTION = 1792
    }
  }
}