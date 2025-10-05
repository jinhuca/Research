using PDFReportsGenerator;
using SmartAblationSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmartAblationSystem.Helpers
{
    public class PDFCaseReport
    {
        private PDFTemplate pdfTemplate;
        private ChartGen  chartGen;
        List<CaseYearMonthSummary> caseYearMonthSummary = new List<CaseYearMonthSummary>();
        
        public PDFCaseReport()
        {
            chartGen = new ChartGen();
            pdfTemplate = new PDFTemplate();
        }
        public void GeneratePDFCaseReport(List<ProcedureRecords> procedureRecordsListWhereFrom, string caseReportName, string hospitalName)
        {
            List<ProcedureRecords> procedureRecordsyearList = new List<ProcedureRecords>();
          
            List<PDFElementsTable> ablationCurrentInfo = new List<PDFElementsTable>();

            string PDate = "";
            string AblationSummaryReportField = "Case Summary Report";
            string pageField = "Page";
            string PDFImagePath = ConfigurationManager.AppSettings["PDFImagePath"];
            int currentcaseyear = 0;
            int casemonth = 0;
            List<CaseChartValue> yearcharts = new List<CaseChartValue>();
            List<CaseChartValue> monthcharts = new List<CaseChartValue>();
            foreach (ProcedureRecords pr in procedureRecordsListWhereFrom)
            {
                if (currentcaseyear != pr.ProcedureDate.Year)
                {
                    currentcaseyear = pr.ProcedureDate.Year;
                    procedureRecordsyearList = procedureRecordsListWhereFrom.Where(CR => CR.ProcedureDate.Year == currentcaseyear).ToList();

                    yearcharts.Add(new CaseChartValue
                    {
                        chartKey = currentcaseyear,
                        chartValue = procedureRecordsyearList.Count
                    });
                    for (int i = 1; i < 13; i++)
                    {
                        casemonth = procedureRecordsyearList.Where(CRM => CRM.ProcedureDate.Month == i).ToList().Count();
                        
                        caseYearMonthSummary.Add(new CaseYearMonthSummary
                        {
                            caseyear = currentcaseyear,
                            caseyeartotal = procedureRecordsyearList.Count,
                            casemonth = i,
                            casemonthtotal = casemonth
                        });
                    }
                }
            }


            //caseYearMonthSummary.Count

            currentcaseyear = 0;
            string currentPDFName = GetBasePath() + "PDFFiles\\" + caseReportName + ".pdf";
            if (!File.Exists(@currentPDFName))
            {
                string[][] CoverPageElementValues;
                CoverPageElementValues = new string[1][];
                CoverPageElementValues[0] = new string[] { hospitalName };
                PDate = DateTime.Today.ToString("MMMM dd, yyyy");
                ablationCurrentInfo.Add(new PDFElementsTable { ElementType = "Cover", ElementDispalyName = AblationSummaryReportField + " ", ElementValue = CoverPageElementValues });


                if (yearcharts.Count>1)
                {
                    int yearcount = yearcharts.Count;
                    int yearstotal = 0;
                    string[][] chatYearValues = new string[1][];
                    string[][] CaseyearElementValues = new string[yearcount][];
                    string yeartitle = "";
                    string chatYearName = "";
                    for (int m=0; m< yearcount; m++)
                    {
                        CaseyearElementValues[m] = new string[]
                            {
                                yearcharts[m].chartKey.ToString(),
                                yearcharts[m].chartValue.ToString()
                            };
                        yearstotal += yearcharts[m].chartValue;
                    }
                    chatYearName = yearcharts[yearcount - 1].chartKey.ToString() + yearcharts[0].chartKey.ToString() + "yearChart.png";
                    yeartitle = yearcharts[yearcount - 1].chartKey.ToString() + " ~ " + yearcharts[0].chartKey.ToString();
                    ablationCurrentInfo.Add(new PDFElementsTable { ElementType = "tableBig2-b", ElementDispalyName = yeartitle + " -  Total case: " + yearstotal, ElementValue = CaseyearElementValues });
                    chartGen.SaveChartAsImage(yearcharts, chatYearName, "", 1, "", 0, 12, 1, 0, 0 + 50, 5, 0, 0, "pie");
                    chatYearValues[0] = new string[] { GetBasePath() + @PDFImagePath + chatYearName, currentcaseyear.ToString() };
                    ablationCurrentInfo.Add(new PDFElementsTable { ElementType = "PIECHART", ElementDispalyName = "", ElementValue = chatYearValues });
                }


                int count = caseYearMonthSummary.Count();
                int row = caseYearMonthSummary.Count/12;
                string[][] CaseElementValues = new string[count/row][];
             
                CaseElementValues[0] = new string[] { "Month", "Case" };

                string imagepath = GetBasePath() + @PDFImagePath + "\\CaseReport\\"; // + "\\CaseReport\\20203CasesReport.bmp";
                string[][] ImageElementValues = new string[12][];
                string[][] ImageYearValues = new string[1][];
                string ElementTypeName = "";



                for (int j = 0; j < count; j++)
                {
                    if (currentcaseyear != caseYearMonthSummary[j].caseyear)
                    {
                        currentcaseyear = caseYearMonthSummary[j].caseyear;

                        for (int x = 0; x < 12; x++)
                        {
                            string monthName = new DateTime(2020, x + 1, 1).ToString("MMM", CultureInfo.InvariantCulture);
                            CaseElementValues[x] = new string[]
                            {
                                monthName,
                                caseYearMonthSummary[j + x].casemonthtotal.ToString()
                            };
                            monthcharts.Add(new CaseChartValue
                            {
                                chartKey = x+1 ,
                                chartValue = caseYearMonthSummary[j + x].casemonthtotal
                            });
                            if (caseYearMonthSummary[j + x].casemonthtotal>0)
                                ImageElementValues[x] = new string[] { imagepath + currentcaseyear+(x+1).ToString()+ "CasesReport.bmp", currentcaseyear + "-" + monthName + "   Total Case: " + caseYearMonthSummary[j + x].casemonthtotal };

                        }
                            if (yearcharts.Count >1)
                            {
                                ElementTypeName = "tableBig2-a";
                            }
                            else
                            ElementTypeName = "tableBig2-b";

                        ablationCurrentInfo.Add(new PDFElementsTable { ElementType = ElementTypeName, ElementDispalyName = currentcaseyear + " - Total Case: " + caseYearMonthSummary[j].caseyeartotal.ToString(), ElementValue = CaseElementValues });

                            chartGen.SaveChartAsImage(monthcharts, currentcaseyear+"CaseChart.png", "", 1, "", 0, 12, 1, 0, 0 + 50, 5, 0, 0, "bar");
                            ImageYearValues[0] = new string[] { GetBasePath() + @PDFImagePath  + currentcaseyear + "CaseChart.png", currentcaseyear.ToString() };
                            ablationCurrentInfo.Add(new PDFElementsTable { ElementType = "CHART", ElementDispalyName = "2020-03", ElementValue = ImageYearValues });
                            CaseElementValues = new string[count / row][];
                            ImageYearValues = new string[1][];
                            monthcharts.Clear();
                            ablationCurrentInfo.Add(new PDFElementsTable { ElementType = "IMAGEBIG", ElementDispalyName = "", ElementValue = ImageElementValues });
                    }

                   
                    ImageElementValues = new string[12][];
                }
                caseYearMonthSummary.Clear();
               
                
                pdfTemplate.SaveToPDFTemplate(currentPDFName, ablationCurrentInfo, "", PDate, pageField, PDFImagePath);


            }
        }
        private string GetMonthString(int count)
        {
            string monthstring = "";
            for (int x = 0; x < 12; x++)
            {
                string monthName = new DateTime(2020, x+1,1).ToString("MMM", CultureInfo.InvariantCulture);
                monthstring = monthstring + monthName  + System.Environment.NewLine;

            }
            return monthstring;
        }


        private string GetMonthValue(int count)
        {
            string monthstring = "";
            for (int x = 0; x < 12; x++)
            {
                monthstring = monthstring+caseYearMonthSummary[count + x].casemonthtotal.ToString() + System.Environment.NewLine;
            }
            return monthstring;
        }

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
