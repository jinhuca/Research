using Spire.Pdf;
using Spire.Pdf.AutomaticFields;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using Spire.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PDFReportsGenerator
{
	/// <summary>
	/// This class provides functions to generate a PDF file
	///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
	/// </summary>
	public class PDFTemplate
	{
		PdfDocument doc;
		PdfBrush brush1 = PdfBrushes.Black;
		PdfBrush brush2 = PdfBrushes.Transparent;
		PdfBrush brush3 = PdfBrushes.Red;
		PdfBrush brush4 = PdfBrushes.Green;
		PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("Arial", 14f, FontStyle.Bold));
		PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("Arial", 12f, FontStyle.Regular));
		PdfTrueTypeFont font3 = new PdfTrueTypeFont(new Font("Arial", 8f, FontStyle.Regular));
		PdfTrueTypeFont font4 = new PdfTrueTypeFont(new Font("Arial", 16f, FontStyle.Regular));
		PdfTrueTypeFont fontTitle = new PdfTrueTypeFont(new Font("Arial", 28f, FontStyle.Regular));
		PdfTrueTypeFont fontTitle2 = new PdfTrueTypeFont(new Font("Arial", 18f, FontStyle.Regular));
		PdfTrueTypeFont fontTitle3 = new PdfTrueTypeFont(new Font("Arial", 22f, FontStyle.Regular));
		PdfStringFormat format2 = new PdfStringFormat(PdfTextAlignment.Left);
		PdfStringFormat format1 = new PdfStringFormat(PdfTextAlignment.Center);
		PdfPageBase page;
		PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
		PdfMargins margin = new PdfMargins();
		PdfMargins margins = new PdfMargins(40, 60, 40, 60);
		PdfRGBColor pdfRGBColor = Color.Black;
		float x = 0;
		float positionY = 10;

		/// <summary>
		/// This function saves info into PDF File
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public void SaveToPDFTemplate(
			string PDFFileName, 
			List<PDFElementsTable> currentInfo, 
			string pinfo, 
			string PDate, 
			string PageField, 
			string PDFImagePath, 
			string consolesn = "", 
			bool headerbannertemplate = false)
		{
      try
      {
        doc = new PdfDocument();

        doc.PageSettings.Size = PdfPageSize.A4;

        doc.PageSettings.Margins = new PdfMargins(0);

        //create a PdfMargins object, the parameters indicate the page margins you want to set

        margin.Top = unitCvtr.ConvertUnits(2.54f, PdfGraphicsUnit.Pixel, PdfGraphicsUnit.Point);
        margin.Bottom = margin.Top;
        margin.Left = unitCvtr.ConvertUnits(3.17f, PdfGraphicsUnit.Pixel, PdfGraphicsUnit.Point);
        margin.Right = margin.Left;

        doc.Template.Left = new PdfPageTemplateElement(margins.Left, doc.PageSettings.Size.Height);
        doc.Template.Right = new PdfPageTemplateElement(margins.Right, doc.PageSettings.Size.Height);

        //save border templates
				if(headerbannertemplate)
        {
          PDFImagePath = @"Images";
          doc.Template.Top = CreateHeaderTemplate(doc, margins, PDFImagePath);
          doc.Template.Bottom = CreateFooterTemplate(doc, margins, pinfo, PDate, PageField, consolesn);

          PdfSection sec = doc.Sections.Add();
          sec.PageSettings.Width = PdfPageSize.A4.Width;
          page = sec.Pages.Add();
        }
        else
        {
          doc.Template.Top = new PdfPageTemplateElement(doc.PageSettings.Size.Width, margins.Top);
          doc.Template.Bottom = CreateFooterTemplate(doc, margins, pinfo, PDate, PageField);
        }

        // foreach (KeyValuePair<PDFItemsDefinition, List<PDFElementsDefinition>> definition in currentInfo)
        int currentInfoCount = currentInfo.Count;
				for(int i = 0; i < currentInfoCount; i++)
        {
					if(currentInfo[i].ElementType.ToUpper() == "COVER")
          {
            AddCoverPage(currentInfo[i].ElementDispalyName, currentInfo[i].ElementValue[0][0], pinfo, PDFImagePath);
            PdfSection sec = doc.Sections.Add();
            sec.PageSettings.Width = PdfPageSize.A4.Width;
            page = sec.Pages.Add();
          }

					else if(currentInfo[i].ElementType.ToUpper() == "COVER2")
          {
            PDFImagePath = @"Images";
						AddCoverPage2(currentInfo[i].ElementDispalyName, currentInfo[i].ElementValue[0][0], currentInfo[i].ElementValue[2][0], PDFImagePath);
            PdfSection sec = doc.Sections.Add();
            sec.PageSettings.Width = PdfPageSize.A4.Width;
            page = sec.Pages.Add();
          }
					else if(currentInfo[i].ElementType.ToUpper() == "TABLESMALL")
          {
            pdfRGBColor = Color.Transparent;
            positionY = PdfTableGen(page, currentInfo[i], positionY, x, format2, format2, 3);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "TABLE")
          {
            pdfRGBColor = Color.Transparent;
            positionY = PdfTableGen(page, currentInfo[i], positionY, x, format2, format2, 7);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "TABLEIMAGE")
          {
            pdfRGBColor = Color.Transparent;
            positionY = PdfTableImageGen(page, currentInfo[i], positionY, x, format2, format2, 1);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "TABLEIMAGERESULT")
          {
            pdfRGBColor = Color.Transparent;
            positionY = PdfTableImageGen(page, currentInfo[i], positionY, x, format2, format2, 7);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "TABLEBIG")
          {
            pdfRGBColor = Color.Black;
            positionY = PdfTableGen(page, currentInfo[i], positionY, x, format2, format1, 2);
          }
					else if(currentInfo[i].ElementType.ToUpper().Contains("TABLEBIG2"))
          {
            pdfRGBColor = Color.Black;
						if(currentInfo[i].ElementType.ToUpper().Substring(currentInfo[i].ElementType.ToUpper().Length - 1, 1) == "A")
            {
              page = doc.Pages.Add(PdfPageSize.A4, margin);
              positionY = 10;
            }
            positionY = PdfTableGen(page, currentInfo[i], positionY, x, format2, format1, 6);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "NEWPAGE")
          {
            page = doc.Pages.Add(PdfPageSize.A4, margin);
            positionY = 10;
          }
          else if (currentInfo[i].ElementType.ToUpper() == "TABLETREATMENTNOTE")
          {
            pdfRGBColor = Color.Black;
						positionY = PdfTableGen(page, currentInfo[i], positionY, x, format2, format2, 8);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "TABLEBIGNEWPAGE")
          {
            pdfRGBColor = Color.Black;
            page = doc.Pages.Add(PdfPageSize.A4, margin);
            positionY = 10;
            positionY = PdfTableGen(page, currentInfo[i], positionY, x, format2, format2, 2);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "TABLENOHEADER")
          {
            pdfRGBColor = Color.Black;
            positionY = PdfTableGen(page, currentInfo[i], positionY, x, format2, format1, 4);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "TABLENOHEADERNEWPAGE")
          {
            page = doc.Pages.Add(PdfPageSize.A4, margin);
            positionY = 10;
            positionY = PdfTableGen(page, currentInfo[i], positionY, x, format2, format1, 4);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "STRING")
          {
            pdfRGBColor = Color.Black;
            positionY = StringAdd(page, currentInfo[i], positionY, x, format2, format2, 2);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "STRINGANDTITLE")
          {
            pdfRGBColor = Color.Black;
            positionY = StringAdd(page, currentInfo[i], positionY, x, format2, format2, 1);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "IMAGES")
          {
            //page = doc.Pages.Add(PdfPageSize.A4, margin);
            pdfRGBColor = Color.Transparent;
            positionY = 10;
            ImagePage(currentInfo[i]);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "IMAGEBIG")
          {
            pdfRGBColor = Color.Transparent;
            positionY = 10;
            ImageBig(currentInfo[i]);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "CHART")
          {
            pdfRGBColor = Color.Transparent;
            positionY = 10;
            Chart(currentInfo[i]);
          }
					else if(currentInfo[i].ElementType.ToUpper() == "PIECHART")
          {
            pdfRGBColor = Color.Transparent;
            positionY = 10;
            PieChart(currentInfo[i]);

          }
        }

        positionY = 10;
      }
      catch(Exception e)
      {
				LogSystem.LogService.LogException(e);
      }
      finally
      {
        doc.SaveToFile(PDFFileName);
        doc.Close();
      }
    }

		/// <summary>
		/// This function generates PDF format table
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private float PdfTableGen(PdfPageBase page, PDFElementsTable elementTable, float y, float x, PdfStringFormat formatstring, PdfStringFormat formattable, int type)
		{

			page.Canvas.DrawString(elementTable.ElementDispalyName, font1, brush1, x, y, formatstring);
			y = y + font1.MeasureString(elementTable.ElementDispalyName, formattable).Height;
			PdfLayoutResult result = TableCreation(elementTable, type, y);
			y = y + result.Bounds.Height + 15;
			return y;
		}

		/// <summary>
		/// This function generates PDF format table where element display name is a grid of hyphen separated string and image
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private float PdfTableImageGen(PdfPageBase page, PDFElementsTable elementTable, float y, float x, PdfStringFormat formatstring, PdfStringFormat formattable, int type)
		{
			// Get string and image path
			string[] displayelements = elementTable.ElementDispalyName.Split('-');
			int elementsize = displayelements.Length;
			string imagepath = displayelements[elementsize - 1];

			// initialize grid
			PdfGrid grid = new PdfGrid();
			grid.Rows.Add();
			grid.Columns.Add(elementsize);
			// col1: title; col2: Pass; col3: image

			float width = page.Canvas.ClientSize.Width - (grid.Columns.Count + 1);
			if(elementsize == 3)
			{
				grid.Columns[0].Width = width * 0.508f;
				grid.Columns[1].Width = width * 0.251f;
				grid.Columns[2].Width = width * 0.25f;
			}
			else if(elementsize == 2)
			{
				grid.Columns[0].Width = width * 0.749f;
				grid.Columns[1].Width = width * 0.251f;
			}
			else
			{
				throw new Exception("Wrong Number of Display Elements");
			}

			// load image path
			PdfGridCellContentList lst = new PdfGridCellContentList();
			PdfGridCellContent textAndStyle = new PdfGridCellContent
			{
				Image = PdfImage.FromFile(imagepath),
				ImageSize = new SizeF(15, 15)
			};
			lst.List.Add(textAndStyle);

			// set style for text cell
			grid.Rows[0].Cells[0].Value = displayelements[0];
			grid.Rows[0].Cells[0].Style.Font = font1;
			grid.Rows[0].Cells[0].Style.TextBrush = brush1;
			grid.Rows[0].Cells[0].Style.StringFormat = formatstring;

			if(elementsize == 3)
			{
				grid.Rows[0].Cells[1].Value = displayelements[1];
				grid.Rows[0].Cells[1].Style.Font = font1;
				grid.Rows[0].Cells[1].Style.TextBrush = displayelements[1] == "Pass" ? brush4 : brush3;
				grid.Rows[0].Cells[1].Style.StringFormat = formatstring;

				// add image to image cell
				grid.Rows[0].Cells[2].Value = lst;
			}
			else if(elementsize == 2)
			{
				grid.Rows[0].Cells[1].Value = lst;
      }

			// transparent borders
			PdfBorders border = new PdfBorders
			{
				All = new PdfPen(Color.Transparent)
			};
			foreach(PdfGridCell pgc in grid.Rows[0].Cells)
			{
				pgc.Style.Borders = border;
			}

			// draw grid to page
			grid.Draw(page, new PointF(x, y));

			y = y + font1.MeasureString(elementTable.ElementDispalyName, formattable).Height;
			PdfLayoutResult valueresult = TableCreation(elementTable, type, y);
			y = y + valueresult.Bounds.Height + 15;
			return y;
		}

		/// <summary>
		/// This function provides different pdf table layout based on type
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private PdfLayoutResult TableCreation(PDFElementsTable elementTable, int type, float y)
		{
			PdfTable table = new PdfTable();
			PdfLayoutResult result;
      table.DataSource = elementTable.ElementValue;
			if(elementTable.ElementValue[0][0] == null)
			{
				table.Columns[0].Width = 5;
			}

			if(type == 2) table.BeginRowLayout += new BeginRowLayoutEventHandler(table_BeginRowLayoutWithBorder);
			else if(type == 6) table.BeginRowLayout += new BeginRowLayoutEventHandler(table_BeginRowLayoutWithBorderBigFont);
			else if(type == 4) table.BeginRowLayout += new BeginRowLayoutEventHandler(table_BeginRowLayoutWithBorderLeft);
			else if(type == 7) table.BeginRowLayout += new BeginRowLayoutEventHandler(table_BeginRowResultLayout);
			else if(type == 8)
			{
				table.Columns[0].Width = 10;
				table.Columns[1].Width = 45;
        table.BeginRowLayout += table_BeginRowResultLayoutWithBorder;
			}
			else table.BeginRowLayout += new BeginRowLayoutEventHandler(table_BeginRowLayout);

			PdfTableLayoutFormat tableLayout = new PdfTableLayoutFormat();
			tableLayout.Break = PdfLayoutBreakType.FitPage;
			tableLayout.Layout = PdfLayoutType.Paginate;

			table.Style.CellPadding = 1;
			table.Style.IsFixWidth = false;

			if(type == 3 || type == 4)
				table.Style.ShowHeader = false;
			else
				table.Style.ShowHeader = true;
			if(type == 6)
			{
				//table.Style.DefaultStyle.Font = font1;
				//table.Style.HeaderStyle.Font = font1;
				result = table.Draw(page, new RectangleF(0, 30, 400, 300), tableLayout);
			}
			else
				result = table.Draw(page, new PointF(0, y), tableLayout);
			y = result.Bounds.Bottom + 5;
			return result;
		}

		/// <summary>
		/// This function provides table row layout
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public static void table_BeginRowResultLayout(object sender, BeginRowLayoutEventArgs args)
		{
			PdfCellStyle cellStyle = new PdfCellStyle();
			cellStyle.BorderPen = new PdfPen(Color.Transparent, 0.5f);
			args.CellStyle = cellStyle;
			// header style
			if(args.RowIndex == 0)
			{
				args.CellStyle.Font = new PdfTrueTypeFont(new Font("Arial", 8f, FontStyle.Bold));
				//args.CellStyle.BackgroundBrush = PdfBrushes.CadetBlue;
			}
			else
			{
				args.CellStyle.Font = new PdfTrueTypeFont(new Font("Arial", 8f, FontStyle.Regular));
			}
		}

		public static void table_BeginRowResultLayoutWithBorder(object sender, BeginRowLayoutEventArgs args)
    {
			PdfCellStyle cellStyle = new PdfCellStyle
			{
				BorderPen = new PdfPen(Color.Black, 0.5f),
				StringFormat = new PdfStringFormat(PdfTextAlignment.Left)
			};
      if (args.RowIndex == 0)
      {
				args.Skip = true;
      }
			args.CellStyle = cellStyle;
		}

		/// <summary>
		/// This function provides table row layout
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public static void table_BeginRowLayout(object sender, BeginRowLayoutEventArgs args)
		{
			PdfCellStyle cellStyle = new PdfCellStyle();
			cellStyle.BorderPen = new PdfPen(Color.Transparent, 0.5f);
			args.CellStyle = cellStyle;
		}

		/// <summary>
		/// This function provides table row layout
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public static void table_BeginRowLayoutWithBorder(object sender, BeginRowLayoutEventArgs args)
		{
			PdfCellStyle cellStyle = new PdfCellStyle();
			cellStyle.BorderPen = new PdfPen(Color.Black, 0.5f);
			cellStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			args.CellStyle = cellStyle;
		}


		public static void table_BeginRowLayoutWithBorderBigFont(object sender, BeginRowLayoutEventArgs args)
		{
			PdfCellStyle cellStyle = new PdfCellStyle();
			cellStyle.BorderPen = new PdfPen(Color.Black, 0.5f);
			cellStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			args.CellStyle = cellStyle;
			args.MinimalHeight = 20f;
			args.CellStyle.Font = new PdfTrueTypeFont(new Font("Arial", 12f, FontStyle.Regular));
		}



		/// <summary>
		/// This function provides table row layout
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public static void table_BeginRowLayoutWithBorderLeft(object sender, BeginRowLayoutEventArgs args)
		{
			PdfCellStyle cellStyle = new PdfCellStyle();
			cellStyle.BorderPen = new PdfPen(Color.Black, 0.5f);
			cellStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Left);
			args.CellStyle = cellStyle;
		}

		/// <summary>
		/// This function controls PDF elements
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private PdfPageTemplateElement CreateHeaderTemplate(PdfDocument doc, PdfMargins margins, string PdfImagePath)
		{
			//get page size
			SizeF pageSize = doc.PageSettings.Size;

			//create a PdfPageTemplateElement object which works as header space
			PdfPageTemplateElement headerSpace = new PdfPageTemplateElement(pageSize.Width, margins.Top);
			headerSpace.Foreground = false;

			float x = margins.Left;
			float y = 0;

			string headerBSCpath = GetBasePath() + @PdfImagePath + "\\BSC_black_RGB_s.jpg";
			string headerSMpath = GetBasePath() + @PdfImagePath + "\\Smartfreeze.png";

			PdfImage headerBSC = PdfImage.FromFile(headerBSCpath);
			PdfImage headerSM = PdfImage.FromFile(headerSMpath);

			//PdfImage.FromImage()

			float height = 30;
			float widthBSC = 60;
			float widthSM = 180;

			headerSpace.Graphics.DrawImage(headerSM, x, margins.Top - height - 17, widthSM, height);
			headerSpace.Graphics.DrawImage(headerBSC, pageSize.Width - x - widthBSC, margins.Top - height - 17, widthBSC, height);

			return headerSpace;
		}

		/// <summary>
		/// This function controls PDF elements
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private PdfPageTemplateElement CreateFooterTemplate(PdfDocument doc, PdfMargins margins, string pinfo, string PDate, string PageField, string consolesn = "")
		{
			//get page size
			SizeF pageSize = doc.PageSettings.Size;

			//create a PdfPageTemplateElement object which works as footer space
			PdfPageTemplateElement footerSpace = new PdfPageTemplateElement(pageSize.Width, margins.Bottom);
			footerSpace.Foreground = false;

			//declare two float variables
			float x = margins.Left;
			float y = 0;

			//draw line in footer space
			PdfPen pen = new PdfPen(PdfBrushes.Gray, 1);
			footerSpace.Graphics.DrawLine(pen, x, y, pageSize.Width - 50, y);

			//draw text in footer space
			y = y + 5;
			PdfTrueTypeFont font = new PdfTrueTypeFont(new Font("Arial", 9f), true);
			PdfStringFormat formatleft = new PdfStringFormat(PdfTextAlignment.Left);
			PdfStringFormat formatcenter = new PdfStringFormat(PdfTextAlignment.Center);
			string footerDate = PDate;
			string footerConsoleSN = consolesn;
			footerSpace.Graphics.DrawString(footerDate, font, PdfBrushes.Gray, x, y, formatleft);
			footerSpace.Graphics.DrawString(footerConsoleSN, font, PdfBrushes.Gray, pageSize.Width / 2, y, formatcenter);

			//draw dynamic field in footer space
			PdfPageNumberField number = new PdfPageNumberField();
			PdfPageCountField count = new PdfPageCountField();
			PdfCompositeField compositeField = new PdfCompositeField(font, PdfBrushes.Gray, PageField + " {0} of {1}", number, count);
			compositeField.StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Top);
			SizeF size = font.MeasureString(compositeField.Text);
			compositeField.Bounds = new RectangleF(pageSize.Width - x - 40, y, size.Width, size.Height);
			compositeField.Draw(footerSpace.Graphics);

			return footerSpace;
		}

		/// <summary>
		/// This function controls PDF string format and returns measure value of the string
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private float StringAdd(PdfPageBase page, PDFElementsTable elementTable, float y, float x, PdfStringFormat formatstring, PdfStringFormat formattable, int type)
		{

			if(type == 1)
			{
				page.Canvas.DrawString(elementTable.ElementDispalyName, font1, brush1, x, y, formatstring);
				y = y + font1.MeasureString(elementTable.ElementDispalyName, formattable).Height;
				page.Canvas.DrawString(elementTable.ElementValue[0][0], font3, brush1, x, y, formatstring);
			}
			else
				page.Canvas.DrawString(elementTable.ElementValue[0][0], font2, brush1, x, y, formatstring);
			y = y + 25;
			return y;
		}
		/// <summary>
		/// This function adds image to PDF page
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void ImagePage(PDFElementsTable elementTable)
		{
			PdfGrid grid = new PdfGrid();
			int rowcount = elementTable.ElementValue.Length;
			//float stringHeight = 0;
			float y = 10;
			//float objH = 0;
			for(int i = 0; i < rowcount; i++)
			{
				if(i % 3 == 0)
				{
					page = doc.Pages.Add(PdfPageSize.A4, margin);
					positionY = 10;
					y = 10;
				}

				grid = InsertImage(elementTable.ElementValue[i][0], 400, 145, 0.98f);
				PdfLayoutResult result = grid.Draw(page, new PointF(0, y));
				y += 145;
				//page.Canvas.DrawString(elementTable.ElementValue[i][1], font4, brush1, 30, y , format2);

				if(elementTable.ElementValue[i][1].Length > 0)
				{
					string[][] treatmentTemp = new string[2][];
					treatmentTemp[0] = new string[] { " " };
					treatmentTemp[1] = new string[] { elementTable.ElementValue[i][1] };
					PDFElementsTable elementTableTemp = new PDFElementsTable { ElementType = "table", ElementDispalyName = " ", ElementValue = treatmentTemp };

					result = TableCreation(elementTableTemp, 3, y);
					y += result.Bounds.Height;
				}

				if(File.Exists(elementTable.ElementValue[i][0]))
				{
					File.Delete(elementTable.ElementValue[i][0]);
				}
				y += 20;
			}
		}

		private void Chart(PDFElementsTable elementTable)
		{
			PdfGrid grid = new PdfGrid();
			int rowcount = elementTable.ElementValue.Length;
			float y = 10;
			for(int i = 0; i < rowcount; i++)
			{

				if(elementTable.ElementValue[i] != null)
				{
					y = 350;
					grid = InsertImage(elementTable.ElementValue[i][0], 420, 250, 0.98f);
					PdfLayoutResult result;
					string[][] treatmentTemp = new string[2][];
					treatmentTemp[0] = new string[] { " " };
					treatmentTemp[1] = new string[] { elementTable.ElementValue[i][1] };
					page.Canvas.DrawString(elementTable.ElementValue[i][1] + " Charts ", font1, brush1, 0, y, format2);

					result = grid.Draw(page, new PointF(0, y + 20));

					if(File.Exists(elementTable.ElementValue[i][0]))
					{
						File.Delete(elementTable.ElementValue[i][0]);
					}
					y += 20;
				}
			}
		}


		private void PieChart(PDFElementsTable elementTable)
		{
			PdfGrid grid = new PdfGrid();
			int rowcount = elementTable.ElementValue.Length;
			float y = 10;
			for(int i = 0; i < rowcount; i++)
			{

				if(elementTable.ElementValue[i] != null)
				{
					y = 50;
					grid = InsertImage(elementTable.ElementValue[i][0], 350, 350, 0.98f);
					PdfLayoutResult result;
					string[][] treatmentTemp = new string[2][];
					treatmentTemp[0] = new string[] { " " };
					treatmentTemp[1] = new string[] { elementTable.ElementValue[i][1] };


					result = grid.Draw(page, new PointF(0, y + 20));

					if(File.Exists(elementTable.ElementValue[i][0]))
					{
						File.Delete(elementTable.ElementValue[i][0]);
					}
					y += 20;
				}
			}
		}



		private void ImageBig(PDFElementsTable elementTable)
		{
			PdfGrid grid = new PdfGrid();
			int rowcount = elementTable.ElementValue.Length;
			float y = 10;
			int j = 0;
			for(int i = 0; i < rowcount; i++)
			{

				if(elementTable.ElementValue[i] != null)
				{

					if(j % 2 == 0)
					{
						page = doc.Pages.Add(PdfPageSize.A4, margin);
						positionY = 10;
						y = 10;
						j++;
					}
					else
					{
						j++;
						y = 350;
					}
					grid = InsertImage(elementTable.ElementValue[i][0], 400, 250, 0.98f);
					PdfLayoutResult result;
					// y += 50;

					string[][] treatmentTemp = new string[2][];
					treatmentTemp[0] = new string[] { " " };
					treatmentTemp[1] = new string[] { elementTable.ElementValue[i][1] };
					page.Canvas.DrawString(elementTable.ElementValue[i][1], font1, brush1, 0, y, format2);

					result = grid.Draw(page, new PointF(0, y + 20));

					if(File.Exists(elementTable.ElementValue[i][0]))
					{
						File.Delete(elementTable.ElementValue[i][0]);
					}
					y += 20;
				}
			}
		}

		/// <summary>
		/// This function adds cover page to PDF page
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void AddCoverPage(string ReportName, string HospitalName, string ProcedureID, string PDFImagePath)
		{

			page = doc.Pages.Add(PdfPageSize.A4, margin);
			float y = 10;

			PdfBorders border = new PdfBorders();
			border.All = new PdfPen(Color.Transparent);

			string imagepath1 = GetBasePath() + @PDFImagePath + "BSC_black_RGB_s.jpg";  //  "PDFFiles/PDFImages/BSC_black_RGB_s.jpg";
			string imagepath = GetBasePath() + @PDFImagePath + "Smartfreeze.png"; //  "PDFFiles /PDFImages/Smartfreeze.png";
																																						//Create a pdf grid
			PdfGrid grid2 = new PdfGrid();

			grid2 = InsertImage(imagepath, 460, 70, 0.98f);
			PdfLayoutResult result = grid2.Draw(page, new PointF(10, 0));

			PdfGrid grid = new PdfGrid();
			grid.Columns.Add(1);
			float width = page.Canvas.ClientSize.Width - 30;
			grid.Columns[0].Width = width;
			grid.Rows.Add();
			grid.Rows[0].Style.Font = fontTitle3;
			grid.Rows[0].Cells[0].Style.Borders = border;
			grid.Rows[0].Cells[0].StringFormat.Alignment = PdfTextAlignment.Center;
			grid.Rows[0].Height = 100;
			grid.Rows[0].Cells[0].Value = HospitalName;
			grid.Rows.Add();
			grid.Rows[1].Cells[0].Value = ReportName;
			grid.Rows[1].Style.Font = fontTitle;
			grid.Rows[1].Cells[0].Style.Borders = border;
			grid.Rows[1].Cells[0].StringFormat.Alignment = PdfTextAlignment.Center;
			grid.Rows.Add();
			grid.Rows[2].Cells[0].Value = ProcedureID;
			grid.Rows[2].Style.Font = font1;
			grid.Rows[2].Cells[0].Style.Borders = border;
			grid.Rows[2].Cells[0].StringFormat.Alignment = PdfTextAlignment.Center;
			grid.Draw(page.Canvas, new PointF(10, 250));

			//if(HospitalName.Length>30)
			//page.Canvas.DrawString(HospitalName, fontTitle2, brush1, page.Canvas.ClientSize.Width / 2, y+200, format1);
			//else
			//    page.Canvas.DrawString(HospitalName, fontTitle3, brush1, page.Canvas.ClientSize.Width / 2, y + 200, format1);


			//page.Canvas.DrawString(ReportName, fontTitle, brush1, page.Canvas.ClientSize.Width / 2, y + 250, format1);
			//page.Canvas.DrawString(PatientID, font1, brush1, page.Canvas.ClientSize.Width / 2, y+300, format1);

			PdfGrid grid1 = new PdfGrid();
			grid1 = InsertImage(imagepath1, 100, 50, 0.5f);
			PdfLayoutResult result1 = grid1.Draw(page, new PointF(190, 530));


		}



		/// <summary>
		/// This function adds cover page to PDF page
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void AddCoverPage2(string ReportName, string HospitalName, string TesterName, string PDFImagePath)
		{

			page = doc.Pages.Add(PdfPageSize.A4, margin);
			float y = 10;

			PdfBorders border = new PdfBorders();
			border.All = new PdfPen(Color.Transparent);

			string imagepath1 = GetBasePath() + @PDFImagePath + "\\BSC_black_RGB_s.jpg";  //  "PDFFiles/PDFImages/BSC_black_RGB_s.jpg";
			string imagepath = GetBasePath() + @PDFImagePath + "\\Smartfreeze.png"; //  "PDFFiles /PDFImages/Smartfreeze.png";
																																							//Create a pdf grid
			PdfGrid grid2 = new PdfGrid();

			grid2 = InsertImage(imagepath, 460, 70, 0.98f);
			PdfLayoutResult result = grid2.Draw(page, new PointF(10, 0));

			PdfGrid grid = new PdfGrid();
			grid.Columns.Add(1);
			float width = page.Canvas.ClientSize.Width - 30;
			grid.Columns[0].Width = width;
			grid.Rows.Add();
			grid.Rows[0].Style.Font = fontTitle3;
			grid.Rows[0].Cells[0].Style.Borders = border;
			grid.Rows[0].Cells[0].StringFormat.Alignment = PdfTextAlignment.Center;
			grid.Rows[0].Height = 50;
			grid.Rows[0].Cells[0].Value = HospitalName + " " + ReportName;
			grid.Rows.Add();
			grid.Rows[1].Cells[0].Value = "";
			grid.Rows[1].Style.Font = fontTitle;
			grid.Rows[1].Cells[0].Style.Borders = border;
			grid.Rows[1].Cells[0].StringFormat.Alignment = PdfTextAlignment.Center;
			grid.Rows.Add();
			grid.Rows[2].Cells[0].Value = TesterName;
			grid.Rows[2].Style.Font = font1;
			grid.Rows[2].Cells[0].Style.Borders = border;
			grid.Rows[2].Cells[0].StringFormat.Alignment = PdfTextAlignment.Center;
			grid.Rows[2].Height = 50;
			grid.Draw(page.Canvas, new PointF(10, 250));

			PdfGrid grid1 = new PdfGrid();
			grid1 = InsertImage(imagepath1, 100, 50, 0.5f);
			PdfLayoutResult result1 = grid1.Draw(page, new PointF(190, 530));


		}





		/// <summary>
		/// This function was called by ImagePage, it can insert multiple images into image page
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private PdfGrid InsertImage(string image, int x, int y, float gridX)
		{
			//Create a pdf grid
			PdfGrid grid = new PdfGrid();
			//Set the cell padding of pdf grid
			// grid.Style.CellPadding = new PdfPaddings(1, 1, 1, 1);

			//Add a row for pdf grid
			PdfGridRow row = grid.Rows.Add();
			//Add two columns for pdf grid 
			grid.Columns.Add(1);
			float width = page.Canvas.ClientSize.Width - (grid.Columns.Count + 1);
			//Set the width of the first column
			grid.Columns[0].Width = width * gridX;

			PdfBorders border = new PdfBorders();
			//Set borders color to transparent
			border.All = new PdfPen(Color.Transparent);

			foreach(PdfGridRow pgr in grid.Rows)
			{
				foreach(PdfGridCell pgc in pgr.Cells)
				{
					pgc.Style.Borders = border;
				}
			}

			//Add a image
			PdfGridCellContentList lst = new PdfGridCellContentList();
			PdfGridCellContent textAndStyle = new PdfGridCellContent();
			textAndStyle.Image = PdfImage.FromFile(@image);
			//Set the size of image
			textAndStyle.ImageSize = new SizeF(x, y);
			// textAndStyle.StringFormat.LineAlignment=
			lst.List.Add(textAndStyle);
			//Add a image into the first cell. 
			row.Cells[0].Value = lst;
			return grid;
		}

		/// <summary>
		/// This function returns base directory of application
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