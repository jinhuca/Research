using Spire.Pdf;
using System;
using System.Management;
using System.Windows.Forms;


namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class manages printing
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PrinterManager
    {
        /// <summary>
        /// Constructor that initialize printer manager
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public PrinterManager()
        {

        }

        /// <summary>
        /// This function prints a pdf file
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        //public void PrinterStatus(string pdfFileName)
        //{
        //     PdfDocument doc = new PdfDocument();
        //    doc.LoadFromFile(@pdfFileName);
        //    //string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        //    //if (System.Security.Principal.WindowsIdentity.GetCurrent().Name == "SMARTFREEZE\\Boston")
        //    //{
        //    //    PrintDialog dialogPrint = new PrintDialog();

        //    //    dialogPrint.AllowPrintToFile = false;
        //    //    dialogPrint.AllowSomePages = true;
        //    //    dialogPrint.PrinterSettings.MinimumPage = 1;
        //    //    dialogPrint.PrinterSettings.MaximumPage = doc.Pages.Count;
        //    //    dialogPrint.PrinterSettings.FromPage = 1;
        //    //    dialogPrint.AllowSelection = false;            
        //    //    dialogPrint.PrinterSettings.ToPage = doc.Pages.Count;
        //    //    if (dialogPrint.ShowDialog() == DialogResult.OK)
        //    //    {
        //    //        doc.PrintSettings.SelectPageRange(dialogPrint.PrinterSettings.FromPage, dialogPrint.PrinterSettings.ToPage);
        //    //        doc.PrintSettings.PrinterName = dialogPrint.PrinterSettings.PrinterName;
        //    //        doc.Print();
        //    //    }
        //    //}
        //    //else
        //    //{
        //        doc.PrintSettings.SelectPageRange(1, doc.Pages.Count);
        //        doc.PrintSettings.PrinterName = "HP LaserJet Pro M402-M403 n-dne PCL 6"; //"RICOH Class Driver";  //"MTL001";   // 
        //        doc.Print();
        //    //}
        //}



        public void PrinterStatus(string pdfFileName)
        {
            string defaultBostonPrinterName = "HP LaserJet Pro M402-M403 n-dne PCL 6";
            //string defaultBostonPrinterName = "Hewlett-Packard HP LaserJet M402n";
            // string defaultBostonPrinterName = "follow-you";
            //string t = Environment.OSVersion.ToString();
            //string releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();


            string onLineBostonPrinterName = "";
            onLineBostonPrinterName = GetOnLineBostonPrinterName(defaultBostonPrinterName.ToLower());
            if (onLineBostonPrinterName != "")
            {
                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(@pdfFileName);
                // string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //  if (System.Security.Principal.WindowsIdentity.GetCurrent().Name == "SMARTFREEZE\\Boston")
                //  {
                //PrintDialog dialogPrint = new PrintDialog();

                //dialogPrint.AllowPrintToFile = false;
                //dialogPrint.AllowSomePages = true;
                //dialogPrint.PrinterSettings.MinimumPage = 1;
                //dialogPrint.PrinterSettings.MaximumPage = doc.Pages.Count;
                //dialogPrint.PrinterSettings.FromPage = 1;
                //dialogPrint.AllowSelection = false;
                //dialogPrint.PrinterSettings.ToPage = doc.Pages.Count;

                //   if (dialogPrint.ShowDialog() == DialogResult.OK)
                //   {

                //doc.PrintSettings.SelectPageRange(dialogPrint.PrinterSettings.FromPage, dialogPrint.PrinterSettings.ToPage);
           
               // doc.PrintSettings.PrinterName = dialogPrint.PrinterSettings.PrinterName;
            doc.PrintSettings.SelectPageRange(1, doc.Pages.Count);
            doc.PrintSettings.PrinterName = onLineBostonPrinterName;
            doc.Print();
           }

          //  }
            //doc.PrintSettings.SelectPageRange(1, doc.Pages.Count);
            //    doc.PrintSettings.PrinterName = onLineBostonPrinterName; //"RICOH Class Driver";  //"MTL001";   // 
            //    doc.Print();
            //}
            //else
            //{
            //    ;
            //}
        }
        /// <summary>
        /// This function prints a pdf file
        /// </summary>
        /// <param name="defaultHPLaserJetName"></param>
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// <returns>Returns an on line HP Laser printer name.</returns>
        private string GetOnLineBostonPrinterName(string defaultBostonPrinterName)
        {
            string printername = "";
            string x = "";
            ManagementScope scope = new ManagementScope(@"\root\cimv2");
            scope.Connect();
            // Select Printers from WMI Object Collections
            ManagementObjectSearcher searcher = new
            ManagementObjectSearcher("SELECT * FROM Win32_Printer");

            string printerName = "";
            foreach (ManagementObject printer in searcher.Get())
            {
                printerName = printer["Name"].ToString().ToLower();
                int startIndex = printerName.IndexOf(@defaultBostonPrinterName);
                if (startIndex > -1)
                {
                    x = "Printer = " + printer["Name"];
                    if (printer["WorkOffline"].ToString().ToLower().Equals("true"))
                    {
                        x = printer["Name"] + " printer is not connected.";
                    }
                    else
                    {
                        printername = printer["Name"].ToString();
                        break;
                    }
                }
            }
            return printername;
        }
    }
}
