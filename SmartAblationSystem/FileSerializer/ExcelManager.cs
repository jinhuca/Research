using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using Excel = Microsoft.Office.Interop.Excel;
using System.Drawing;
using OfficeOpenXml.Style;
using Shared;

namespace FileSerializer
{
    public class ExcelManager
    {
        private static byte[] excelFilePasscode = { 0xdb, 0xcb, 0x89, 0xa0, 0x18, 0x10, 0xb0, 0xa9, 0x6, 0xae, 0x3d, 0x3e, 0x35 };
        private string header;
        private string BridgeFolderPath;
        string tmpFileName = "ErrorLog.csv";

        public ExcelManager()
        {

        }

        /// <summary>
        /// This function generates error log excel file
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void GenerateErrorLogExcelFile(DataSet errorLogData, string filename, string path, string password)
        {
            BridgeFolderPath = GetBasePath() + "ExcelData";
            GenerateErrorLogHeader();
            DataProgress.CurrentDataProgressStates = DataProgressStates.GENERATING_DATA;
            GenerateErrorLogContent(errorLogData, BridgeFolderPath);
            DataProgress.CurrentDataProgressStates = DataProgressStates.CONVERTING_TOXLS;
            ConvertCSVtoXLS(filename, path, password);
            DataProgress.CurrentDataProgressStates = DataProgressStates.ENDING;
        }

        /// <summary>
        /// This function generates the CSV file header (first line)
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void GenerateErrorLogHeader()
        {
            header = "Log Id, Error Code, DB Error Code, Error Date, Error Type, Message, Solution Message, Engineering Message, SystemStates,UserID,"+ 
                "Catheter DBID, Catheter ID, Serial, Container, Lot, Catheter First Use Date, Catheter Firmware, "+
                "IsUsingICB, IsUsingRemote, "+
                "Version ID, Version First Use Date, GUI, DataBase,Control MCU A, Control MCU B, CPLD, Patient MCU A, Patient MCU B, Remote MCU A, Remote MCU B,Repeater MCU A, Repeater MCU B,ICB MCU A, ICB MCU B ";                        
        }

        /// <summary>
        /// This function generates the  errorlog content.
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void GenerateErrorLogContent(DataSet errorLogData, string folderpath)
        {
           // BridgeFolderPath = Path.Combine(GetBasePath(), "ExcelData");

            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }

            string dataLine = string.Empty;

            if (errorLogData != null)
            {
                try
                {
                    if (File.Exists((folderpath + tmpFileName)))
                    {
                        File.Delete((BridgeFolderPath + tmpFileName));
                    }

                   // File.Create((BridgeFolderPath + "\\" + tmpFileName));



                    using (StreamWriter file = new StreamWriter(BridgeFolderPath + "\\" + tmpFileName, false, Encoding.UTF8))
                    {
                        file.WriteLine(header);

                        //For each ablation treaments


                        if(errorLogData.Tables[0].Rows.Count>0)
                        {
                            int count = errorLogData.Tables[0].Rows.Count;
                            for (int i =0; i<count; i++)
                            {
                                dataLine = string.Empty;
                                dataLine = GenerateDataLine(errorLogData.Tables[0].Rows[i]);
                                file.WriteLine(dataLine);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    throw;
                }
            }
        }

        /// <summary>
        /// This function generates a CSV line from properties contained in AblationDataDetails for a given timestamp.
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
      
        public string GenerateDataLine(DataRow readErrorLogData)
        {
            string dataLine = string.Empty;
            string serialN="--";
            string lotN = "--";
            string catheterContainer = "--";
            string fUseDate = "--";
            string oLCatherID = "--";
            string catID = "--";
            string catheterfirmware = "--";

            try
            {

                if (readErrorLogData["CatheterID"].ToString() != "")
                { 
                    catID =readErrorLogData["CatheterID"].ToString();
                    serialN = readErrorLogData["SerialNumber"].ToString();
                    lotN = readErrorLogData["Lot"].ToString();
                    fUseDate = readErrorLogData["LastUseDate"].ToString();
                    oLCatherID = readErrorLogData["OverloadedCatheterID"].ToString();
                    catheterfirmware = readErrorLogData["CatheterFirmware"].ToString();
                    catheterContainer = (readErrorLogData["CatheterContainer"] as string)??string.Empty;
                }

                dataLine = readErrorLogData["Id"] + "," +
                "\"" + readErrorLogData["ErrorInformation"] + "\"," +
                readErrorLogData["ErrorCode"] + "," +
                readErrorLogData["ErrorDate"] + "," +
                readErrorLogData["ErrorType"] + "," +
                "\"" + readErrorLogData["Message"] + "\"," +
                "\"" + readErrorLogData["SolutionMessage"] + "\"," +
                "\"" + readErrorLogData["CryterionMessage"] + "\"," +
                "\"" + readErrorLogData["SystemStates"] + "\"," +
                readErrorLogData["UserID"] + "," +
                //System Info End
                // Catheter Info Etart
                catID + "," +
                oLCatherID + "," +
                serialN + "," +
                catheterContainer + "," +
                lotN + "," +
                fUseDate + "," +
                catheterfirmware + "," +
                // Catheter Info End
                readErrorLogData["IsUsingICB"] + "," +
                readErrorLogData["IsUsingRemote"] + "," +
                // Version Info Start         
                readErrorLogData["VersionId"] + "," +
                readErrorLogData["StartDate"] + "," +
                "\"" + readErrorLogData["Software"] + "\"," +
               "\"" + readErrorLogData["DataBaseVersion"] + "\"," +
                "\"" + readErrorLogData["ControlFirmware"] + "\"," +
                "\"" + readErrorLogData["ControlFirmwareBootLoader"] + "\"," +
                "\"" + readErrorLogData["CPLDFirmware"] + "\"," +
                "\"" + readErrorLogData["PatientFirmware"] + "\"," +
                "\"" + readErrorLogData["PatientFirmwareBootLoader"] + "\"," +
                "\"" + readErrorLogData["RemoteFirmware"] + "\"," +
                "\"" + readErrorLogData["RemoteFirmwareBootLoader"] + "\"," +
                "\"" + readErrorLogData["RepeaterFirmware"] + "\"," +
                "\"" + readErrorLogData["RepeaterFirmwareBootLoader"] + "\"," +
                "\"" + readErrorLogData["ICBFirmware"] + "\"," +
                "\"" + readErrorLogData["ICBFirmwareBootLoader"] + "\"";
                return dataLine;
            }
            catch(Exception e)
            {
                return dataLine;
            }
        }



        /// <summary>
        /// This function converts CSV to XLS
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ConvertCSVtoXLS(string filename, string path, string password)
        {

            string worksheetsName = "ErrorLog";

            bool firstRowIsHeader = false;

            var format = new ExcelTextFormat();
            format.Delimiter = ',';
            format.TextQualifier = '"';
            format.EOL = "\r";              // DEFAULT IS "\r\n";
                                            // format.TextQualifier = '"';  

            string csvFileName = BridgeFolderPath + "\\" + tmpFileName;
            string excelFileName = path + "\\" + filename + ".xlsx";  //USBPath;   

            

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (File.Exists(excelFileName)) File.Delete(excelFileName);



      ExcelPackage.LicenseContext = LicenseContext.Commercial;
      using (ExcelPackage package = new ExcelPackage(new FileInfo(excelFileName)))
            {

               ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetsName);

               //worksheet.DeleteColumn
 
               worksheet.Cells["A2"].LoadFromText(new FileInfo(csvFileName), format, OfficeOpenXml.Table.TableStyles.None, firstRowIsHeader);
                worksheet.Protection.IsProtected = true;
                           
                worksheet.Column(4).Style.Numberformat.Format = "MMM dd yyyy HH:mm:ss.000";
                worksheet.Column(16).Style.Numberformat.Format = "MMM dd yyyy HH:mm:ss.000";
                worksheet.Column(21).Style.Numberformat.Format = "MMM dd yyyy HH:mm:ss.000";

              //  worksheet.Column(2).Style.Font.Bold = true;
                worksheet.Column(2).Style.Font.Size  = 14;
                worksheet.Column(2).Style.Font.Color.SetColor(Color.OrangeRed);
                worksheet.Cells[2, 2].Style.Font.Color.SetColor(Color.White);

                //  worksheet.Column(11).Style.Font.Bold = true;
                worksheet.Column(11).Style.Font.Size = 14;
                worksheet.Column(11).Style.Font.Color.SetColor(Color.OrangeRed);
                worksheet.Cells[2,11].Style.Font.Color.SetColor(Color.White);
                //  worksheet.Column(20).Style.Font.Bold = true;
                worksheet.Column(20).Style.Font.Size = 14;
                worksheet.Column(20).Style.Font.Color.SetColor(Color.OrangeRed);
                worksheet.Cells[2,20].Style.Font.Color.SetColor(Color.White);
                //worksheet.Column(20).Style.Fill.PatternType = ExcelFillStyle.Solid;
                //worksheet.Column(20).Style.Fill.BackgroundColor.SetColor(Color.Black);
                //worksheet.Column(20).Style.Font.Color.SetColor(Color.White);

                worksheet.Protection.AllowFormatColumns = true;
                worksheet.Protection.SetPassword(new string(PasswordUtils.DecryptPasscode(excelFilePasscode)));
                worksheet.Column(1).Width = 22;

                worksheet.Row(2).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Row(2).Style.Fill.BackgroundColor.SetColor(Color.FromArgb(49, 140, 231));

                worksheet.TabColor = Color.Green;

                worksheet.Row(2).Style.Border.BorderAround(ExcelBorderStyle.Thick);
                worksheet.Row(2).Height = 30;
                worksheet.Row(2).Style.Font.Size = 12;
                worksheet.Row(2).Style.Font.Bold = true;

                /*** Header ***/
                worksheet.Cells[1, 7].Value = "Error Log Detail";
                worksheet.Cells[1, 7].Style.Font.Color.SetColor(Color.White);
                worksheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;


                worksheet.Row(1).Style.Border.BorderAround(ExcelBorderStyle.None);
                worksheet.Row(1).Height = 40;
                worksheet.Row(1).Style.Font.Size = 22;
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Column(22).Width = 60;
                worksheet.Cells.AutoFitColumns();

                
                
               package.Workbook.Protection.LockStructure = true;
                package.Save(password);
            }


            //if (File.Exists(filename+".csv")) 
            File.Delete(csvFileName);
            // ReadXls(Filename + ".xlsx");
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
    }
}
