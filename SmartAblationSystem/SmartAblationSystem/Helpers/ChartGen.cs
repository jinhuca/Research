using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;
using FileSerializer;
using SmartAblationSystem.Models;
using System.Drawing;
using System.Globalization;

namespace SmartAblationSystem.Helpers
{
    class ChartGen
    {
       private Chart chartTemperature;

        private Chart pieChart;
        private Chart barChart;
        private readonly System.Drawing.Color GRID_LINES_COLOR = System.Drawing.Color.Gray;
        private readonly System.Drawing.Color ECG_GRID_LINES_COLOR = System.Drawing.Color.DimGray;
        private readonly System.Drawing.Color SERIES_COLOR = System.Drawing.ColorTranslator.FromHtml("#00AFEF");
        private readonly ChartDashStyle TEMPERATURE_GRID_DASH_STYLE = ChartDashStyle.Dot;
    
        Chart chart = new Chart() { BackColor = System.Drawing.Color.Transparent, Name = "ChartTemperature", Dock = System.Windows.Forms.DockStyle.Fill, Enabled = true };


        public void SaveChartAsImage(List<CaseChartValue> CaseDatasListItems, string imageName, string imageTitle, int points, string chartDrawingType, int XMinimum, int XMaximum, int XInterval, int YMinimum, int YMaximum, int YInterval, int XCrossing, int YCrossing, string chartType)          //(List<List<AblationDataDetails>> allAblationDataList)
        {
            myInit();
          
            if (chartType=="bar")
            { 
                barChartGen(CaseDatasListItems);
                SaveToImage(barChart, GetBasePath() + "PDFFiles\\PDFImages\\" + imageName, imageTitle, chartType);
            }
            else if (chartType =="pie")
            {
                  pieChartGen(CaseDatasListItems);
                  SaveToImage(pieChart, GetBasePath() + "PDFFiles\\PDFImages\\" + imageName, imageTitle, chartType);
            }
        }

        private void pieChartGen(List<CaseChartValue> CaseDatasListItems)
        {
            pieChart.Series.Clear();
            pieChart.Palette = ChartColorPalette.Fire;
            pieChart.BackColor = Color.Transparent;
            pieChart.Titles.Add("");
            pieChart.ChartAreas[0].BackColor = Color.Transparent;
            Series series = new Series
            {
                Name = "series1",
                IsVisibleInLegend = true,
                Color = System.Drawing.Color.Green,
                ChartType = SeriesChartType.Pie
            };
            pieChart.Series.Add(series);

            for (int i = 0; i < CaseDatasListItems.Count(); i++)
            {
                series.Points.Add(CaseDatasListItems[i].chartValue);
                var obj = series.Points[i];
     
                if (CaseDatasListItems[i].chartValue > 0)
                {
                    obj.AxisLabel  = CaseDatasListItems[i].chartValue.ToString();
                    obj.LegendText = CaseDatasListItems[i].chartKey.ToString();
                }

            }

            pieChart.Invalidate();
        }
        private void barChartGen(List<CaseChartValue> CaseDatasListItems)
        {
            barChart.Series.Clear();
            barChart.BackColor = Color.Transparent;
            barChart.Palette = ChartColorPalette.Fire;
            barChart.ChartAreas[0].BackColor = Color.Transparent;
            barChart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            barChart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            barChart.ChartAreas[0].AxisX.Interval = 1;
            barChart.ChartAreas[0].AxisX.Maximum = 12.9;
            barChart.ChartAreas[0].AxisY.Interval = 3;
            //   barChart.ChartAreas[0].AxisY.Maximum = 3+ 

            barChart.ChartAreas[0].AxisY.LabelStyle.ForeColor = System.Drawing.Color.Gray;
            barChart.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("Arial", 11f, FontStyle.Regular);
            barChart.ChartAreas[0].AxisX.LabelStyle.ForeColor = System.Drawing.Color.Gray;
            barChart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 11f, FontStyle.Regular);
            Series series = new Series
            {
                Name = "series2",
                IsVisibleInLegend = false,
                ChartType = SeriesChartType.Column
            };

            barChart.Series.Add(series);
            for (int i=0; i<CaseDatasListItems.Count(); i++)
            {
                series.Points.Add(CaseDatasListItems[i].chartValue);
                var obj = series.Points[i];
                obj.Color = Color.DarkOrange;
                obj.Font = new System.Drawing.Font("Arial", 14f, FontStyle.Regular);
                //   string monthName = new DateTime(2020, CaseDatasListItems[i].chartKey, 1).ToString("MMM", CultureInfo.InvariantCulture);
                obj.LegendText = "y:cases x:month";
                if (CaseDatasListItems[i].chartValue > 0)
                {

                    obj.Label = CaseDatasListItems[i].chartValue.ToString();
                }
            }
            barChart.Invalidate();
        }

        private void myInit()
        {
            ChartArea chartArea1 = new ChartArea();
            Legend legend1 = new Legend()
            { BackColor = Color.LightGray, ForeColor = Color.Black, Title = "" };
            Legend legend2 = new Legend()
            { BackColor = Color.Green, ForeColor = Color.White, Title = "" };
            pieChart = new Chart();
            barChart = new Chart();


            //===Pie chart
            chartArea1.Name = "PieChartArea";
            pieChart.ChartAreas.Add(chartArea1);
            pieChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            pieChart.Legends.Add(legend1);
            pieChart.Location = new System.Drawing.Point(0, 50);

            //====Bar Chart
            chartArea1 = new ChartArea();
            chartArea1.Name = "BarChartArea";
            barChart.ChartAreas.Add(chartArea1);
            barChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Name = "Legend3";
            barChart.Legends.Add(legend2);

        }
    
        private string GetBasePath()
        {
            string thePath = "";

            String path = AppDomain.CurrentDomain.BaseDirectory;
            String[] extract = Regex.Split(path, "bin");  //split it in bin
            thePath = extract[0];
            return thePath;
        }
        private void SaveToImage(Chart obj,  string imageName, string imageTitle, string type)
        {

            if (type=="bar")
            { 
                obj.Width = 550;
                obj.Height = 450;
            }
            //else
            //{
            //    obj.Width = 500;
            //    obj.Height = 500;
            //}
            obj.SaveImage(imageName, ChartImageFormat.Png);
          
        }

       

        /// <summary>
        /// This function sets temperature interval for the chart.
        /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void SetTemperatureChartInterval(int xAxisTimeValue)
        {
            if (chartTemperature != null && chartTemperature.ChartAreas != null &&
                chartTemperature.ChartAreas.Count > 0 && chartTemperature.ChartAreas[0].AxisX != null)
            {
                //Preset values set by pressing the timer increase/decrease arrows.
                if (xAxisTimeValue == 12)
                    chartTemperature.ChartAreas[0].AxisX.Interval = 1;
                else if (xAxisTimeValue <= 30)
                {
                    chartTemperature.ChartAreas[0].AxisX.Interval = 5;
                }
                else if (xAxisTimeValue <= 60)
                {
                    chartTemperature.ChartAreas[0].AxisX.Interval = 10;
                }
                else if (xAxisTimeValue <= 150)
                {
                    chartTemperature.ChartAreas[0].AxisX.Interval = 20;
                }
                else if (xAxisTimeValue <= 240)
                {
                    chartTemperature.ChartAreas[0].AxisX.Interval = 30;
                }
                else if (xAxisTimeValue <= 480)
                {
                    chartTemperature.ChartAreas[0].AxisX.Interval = 50;
                }
                else
                {
                    chartTemperature.ChartAreas[0].AxisX.Interval = Math.Ceiling(chartTemperature.ChartAreas[0].AxisX.Maximum / 10);
                }
            }
            else
            {
                chartTemperature.ChartAreas[0].AxisX.Interval = 5;
            }
        }


        private void SetChartYInterval(int yAxisTimeValue)
        {
            if (chartTemperature != null && chartTemperature.ChartAreas != null &&
                chartTemperature.ChartAreas.Count > 0 && chartTemperature.ChartAreas[0].AxisX != null)
            {
                //Preset values set by pressing the timer increase/decrease arrows.
                if (yAxisTimeValue <= 4)
                    chartTemperature.ChartAreas[0].AxisY.Interval = 1;
                else if (yAxisTimeValue <= 10)
                    chartTemperature.ChartAreas[0].AxisY.Interval = 2;
                else if (yAxisTimeValue <= 30)
                {
                    chartTemperature.ChartAreas[0].AxisY.Interval = 5;
                }
                else if (yAxisTimeValue <= 60)
                {
                    chartTemperature.ChartAreas[0].AxisY.Interval = 10;
                }
                else if (yAxisTimeValue <= 150)
                {
                    chartTemperature.ChartAreas[0].AxisY.Interval = 20;
                }
                else if (yAxisTimeValue <= 240)
                {
                    chartTemperature.ChartAreas[0].AxisY.Interval = 30;
                }
                else if (yAxisTimeValue <= 480)
                {
                    chartTemperature.ChartAreas[0].AxisY.Interval = 50;
                }
                else if (yAxisTimeValue <= 20000)
                    chartTemperature.ChartAreas[0].AxisY.Interval = 2000;
                else
                {
                    chartTemperature.ChartAreas[0].AxisY.Interval = Math.Ceiling(chartTemperature.ChartAreas[0].AxisY.Maximum / 10);
                }
            }
            else
            {
                chartTemperature.ChartAreas[0].AxisY.Interval = 5;
            }
        }

   

        /// <summary>
        /// This function initialize temperature value for the chart.
        /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void InitializeTemperatureGraphic(Chart ChartTemperature, int XMinimum, int XMaximum, int XInterval, int YMinimum, int YMaximum, int YInterval, int XCrossing, int YCrossing, string title)
        {
            string cryoballoonTemperature = string.Empty;
            if (Models.Languages.GuiFieldTranslation.ContainsKey("CryoBalloonTemperatureLabel"))
            {
                cryoballoonTemperature = Models.Languages.GuiFieldTranslation["CryoBalloonTemperatureLabel"];
            }

            ChartTemperature.ChartAreas.Add("TemperatureArea");
            ChartTemperature.Titles.Add(cryoballoonTemperature);
            ChartTemperature.Titles[0].Font = new System.Drawing.Font("Courrier New", 8.0f, System.Drawing.FontStyle.Regular);
            ChartTemperature.Titles[0].ForeColor = System.Drawing.Color.White;
            ChartTemperature.Titles[0].Text = title;
            ChartTemperature.ChartAreas[0].BackColor = System.Drawing.Color.Transparent;
            ChartTemperature.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            ChartTemperature.ChartAreas[0].AxisX.Minimum = XMinimum;
            ChartTemperature.ChartAreas[0].AxisX.Maximum = XMaximum;
            ChartTemperature.ChartAreas[0].AxisX.Interval = XInterval;
            ChartTemperature.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            ChartTemperature.ChartAreas[0].AxisX.IsStartedFromZero = true;
            ChartTemperature.ChartAreas[0].AxisX.MajorGrid.LineColor = GRID_LINES_COLOR;
            ChartTemperature.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = TEMPERATURE_GRID_DASH_STYLE;
            ChartTemperature.ChartAreas[0].AxisX.LineColor = GRID_LINES_COLOR;
            ChartTemperature.ChartAreas[0].AxisX.LineDashStyle = TEMPERATURE_GRID_DASH_STYLE;
            ChartTemperature.ChartAreas[0].AxisX.LabelStyle.ForeColor = System.Drawing.Color.WhiteSmoke; //GRID_LINES_COLOR;
            ChartTemperature.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 8.0f, System.Drawing.FontStyle.Regular);

            ChartTemperature.ChartAreas[0].AxisY.MajorGrid.LineColor = GRID_LINES_COLOR;
            ChartTemperature.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = TEMPERATURE_GRID_DASH_STYLE;
            ChartTemperature.ChartAreas[0].AxisY.LineColor = GRID_LINES_COLOR;
            ChartTemperature.ChartAreas[0].AxisY.LabelStyle.ForeColor = System.Drawing.Color.WhiteSmoke; //= GRID_LINES_COLOR;
            ChartTemperature.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("Arial", 8.0f, System.Drawing.FontStyle.Regular);
            ChartTemperature.ChartAreas[0].AxisY.Minimum = YMinimum;
            ChartTemperature.ChartAreas[0].AxisY.Maximum = YMaximum;
            ChartTemperature.ChartAreas[0].AxisY.Interval = YInterval;

            //To make the X-axis appear on Y axis 0.
            ChartTemperature.ChartAreas[0].AxisX.Crossing = XCrossing;
            ChartTemperature.ChartAreas[0].AxisY.Crossing = YCrossing;

            //// Set Antialiasing mode
            ////this can be set lower if there are any performance issues!
            ChartTemperature.AntiAliasing = AntiAliasingStyles.None;
            ChartTemperature.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
        }

    }
}
