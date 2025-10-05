using SmartAblationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartAblationSystem.ViewModels;
using System.ComponentModel;
using SmartAblationSystem.Helpers;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for CaseSummaryReport.xaml
    /// </summary>
    public partial class CaseSummaryReport :Window
    {

        List<ProcedureRecords> procedureRecordsListWhereFrom = new List<ProcedureRecords>();
        public CaseSummaryReport(List<ProcedureRecords> t)
        {
            CaseCalendarCollection = new ObservableCollection<CaseCalendar>();
            procedureRecordsListWhereFrom = t;
            InitializeComponent();
        }
        public ObservableCollection<CaseCalendar> CaseCalendarCollection
        {
            get;
            set;
       }
        public List<CaseCalendar> caseCalendar { get; set; }
        private void dataGrid1_Loaded(object sender, RoutedEventArgs e)
        {

            
            BindData();
           
           this.Close();

        }

        private void BindData()
        {

            string filename = "";
            int colheight = 0;
            List<int> yearList = new List<int>();
            List<CaseReport> listofCaseReport = new List<CaseReport>();
            List<CaseReport> listofCaseReportbyYear = new List<CaseReport>();
            string currentYear = "";
            foreach (ProcedureRecords procRecwhere in procedureRecordsListWhereFrom)
            {
                listofCaseReport.Add(new CaseReport
                {
                    caseyear = int.Parse(procRecwhere.ProcedureDate.Year.ToString()),
                    casemonth = int.Parse(procRecwhere.ProcedureDate.Month.ToString()),
                    caseday = int.Parse(procRecwhere.ProcedureDate.Day.ToString()),
                    caseablations = procRecwhere.Procedure.Ablations.Count,
                    caseprocedureId = procRecwhere.Procedure.Id
                });
                if (currentYear != procRecwhere.ProcedureDate.Year.ToString())
                { 
                    currentYear = procRecwhere.ProcedureDate.Year.ToString();
                    yearList.Add(int.Parse(procRecwhere.ProcedureDate.Year.ToString()));
                }
            }

            foreach (int year in yearList)
            {
                listofCaseReportbyYear = (listofCaseReport.Where(CR => CR.caseyear == year).ToList()).OrderBy(CaseReport => CaseReport.casemonth).ToList();
                List<CaseReport> listofCaseReportbyYearMonth;
                for (int i = 1; i < 13; i++)
                {
                    listofCaseReportbyYearMonth = (listofCaseReportbyYear.Where(CR => CR.casemonth == i).ToList()).OrderBy(CaseReport => CaseReport.caseday).ToList();       
                    if (listofCaseReportbyYearMonth.Count >0)
                    {

                        colheight=CaseMonthCalendarGen(listofCaseReportbyYearMonth);
                        dataGrid1.Items.Refresh();
                        canvasCalendarReport.UpdateLayout();
                        filename = listofCaseReportbyYearMonth[0].caseyear.ToString() + i.ToString() + "CasesReport.bmp";
                        FileAction fileAction = new FileAction();
                        fileAction.CreateNewFolder(fileAction.GetBasePath() + "PDFFiles\\PDFImages\\CaseReport");
                        string filePath = fileAction.GetBasePath() + "PDFFiles\\PDFImages\\CaseReport\\" + filename;
                       
                        colheight = colheight * 80 + 70; 
                        fileAction.SaveImage(canvasCalendarReport, 605, colheight, filePath, 96d);
                        CaseCalendarCollection.Clear();
                    }
                }
                

            }
        }


        private int CaseMonthCalendarGen(List<CaseReport> listofCaseReportbyYearMonth)
        {

            DateTime firstdayofmonth = new DateTime(listofCaseReportbyYearMonth[0].caseyear, listofCaseReportbyYearMonth[0].casemonth, 1);
            int rownumber=0;
            int daysInMonth = DateTime.DaysInMonth(listofCaseReportbyYearMonth[0].caseyear, listofCaseReportbyYearMonth[0].casemonth)  ;
            int days = 0;
            DateTime lastdayofmonth = new DateTime(listofCaseReportbyYearMonth[0].caseyear, listofCaseReportbyYearMonth[0].casemonth, daysInMonth);
            int dayoflastdateofmonth = (int)lastdayofmonth.DayOfWeek;
            int firstdayofMonth = (int)firstdayofmonth.DayOfWeek;
            
            days = daysInMonth + 7 - dayoflastdateofmonth;
            bool outofmonth = false ;
            //int currentDay = 0;
            string Tsun = "";
            string Tmon = "";
            string Ttue = "";
            string Twed = "";
            string Tthr = "";
            string Tfri = "";
            string Tsat = "";

            string Tsunvalue = "";
            string Tmonvalue = "";
            string Ttuevalue = "";
            string Twedvalue = "";
            string Tthrvalue = "";
            string Tfrivalue = "";
            string Tsatvalue = "";
            int daynum = 0;
            int weekcount = 0;
            int j = 0;
            int startNum = 0;
            startNum = 0 - firstdayofMonth;
            days = daysInMonth + 7 - dayoflastdateofmonth + firstdayofMonth + startNum-1;
            for (int i = startNum; i < days; i++)
            {
                int sameDayofmonth = 0;
                firstdayofmonth = (new DateTime(listofCaseReportbyYearMonth[0].caseyear, listofCaseReportbyYearMonth[0].casemonth, 1)).AddDays(i);   //lastdayofmonth.AddDays(i);
                int casyday = firstdayofmonth.Day;
                if (j < listofCaseReportbyYearMonth.Count()) daynum = listofCaseReportbyYearMonth[j].caseday ;
                if (casyday == daynum)
                {
                    sameDayofmonth = listofCaseReportbyYearMonth.Where(CR => CR.caseday == daynum).ToList().Count();
                    j = j + sameDayofmonth;
                }
                string Tvalue = "";
                if (sameDayofmonth > 0)
                    Tvalue = sameDayofmonth.ToString();
                if (i >= daysInMonth)
                {
                  //  firstdayofmonth = (new DateTime(listofCaseReportbyYearMonth[0].caseyear, listofCaseReportbyYearMonth[0].casemonth, 1)).AddDays(i);   //lastdayofmonth.AddDays(i);
                    outofmonth = true;
                }
                else if (i < 0)
                {
                  //  firstdayofmonth = (new DateTime(listofCaseReportbyYearMonth[0].caseyear, listofCaseReportbyYearMonth[0].casemonth, 1)).AddDays(i);
                    outofmonth = true;
                }
                else
                {
                    //firstdayofmonth = new DateTime(listofCaseReportbyYearMonth[0].caseyear, listofCaseReportbyYearMonth[0].casemonth, i+1);
                 //   firstdayofmonth = (new DateTime(listofCaseReportbyYearMonth[0].caseyear, listofCaseReportbyYearMonth[0].casemonth, 1)).AddDays(i);
                    outofmonth = false;
                }

                int caseSwitch = (int)firstdayofmonth.DayOfWeek;
               // int casyday = firstdayofmonth.Day;
                switch (caseSwitch)
                {
                    case 1:
                        if (outofmonth)
                        {
                            Tmon = "";
                            Tmonvalue = "";
                        }
                        else
                        {
                            Tmon = casyday.ToString();
                            Tmonvalue = Tvalue;
                        }
                        break;
                    case 2:
                        if (outofmonth)
                        {
                            Ttue = "";
                            Ttuevalue = "";
                        }
                        else
                        {
                            Ttue = casyday.ToString();
                            Ttuevalue = Tvalue;
                        }
                        break;
                    case 3:
                        if (outofmonth)
                        {
                            Twed = "";
                            Twedvalue = "";
                        }
                        else
                        {

                            Twed = casyday.ToString();
                            Twedvalue = Tvalue;
                        }
                        break;
                    case 4:
                        if (outofmonth)
                        {
                            Tthr = "";
                            Tthrvalue = "";
                        }
                        else
                        {
                            Tthr = casyday.ToString();
                            Tthrvalue = Tvalue;
                        }
                        break;
                    case 5:
                        if (outofmonth)
                        {
                            Tfri = "";
                            Tfrivalue = "";
                        }
                        else
                        { 
                            Tfri = casyday.ToString();
                            Tfrivalue = Tvalue;
                        }
                        break;
                    case 6:
                        if (outofmonth)
                        {
                            Tsat = "";
                            Tsatvalue = "";
                        }
                        else
                        { 
                            Tsat = casyday.ToString();
                            Tsatvalue = Tvalue;
                        }
                        break;
                    case 0:
                        if (outofmonth)
                        {
                            Tsun = "";
                            Tsunvalue = "";
                        }
                        else
                        { 
                            Tsun = casyday.ToString();
                            Tsunvalue = Tvalue;
                        }
                        break;
                    default:
                        break;

                }
                weekcount++;
                if (weekcount==7)
                { 
                    CaseCalendarCollection.Add(new CaseCalendar(Tmon, Tmonvalue, Ttue, Ttuevalue, Twed, Twedvalue, Tthr, Tthrvalue, Tfri, Tfrivalue, Tsat, Tsatvalue, Tsun, Tsunvalue));
                    weekcount = 0;
                    Tsun = "";
                    Tmon = "";
                    Ttue = "";
                    Twed = "";
                     Tthr = "";
                    Tfri = "";
                    Tsat = "";

                    Tsunvalue = "";
                    Tmonvalue = "";
                    Ttuevalue = "";
                    Twedvalue = "";
                    Tthrvalue = "";
                    Tfrivalue = "";
                    Tsatvalue = "";
                }
            }
            rownumber=CaseCalendarCollection.Count;
            return rownumber;
        }
    }


}

