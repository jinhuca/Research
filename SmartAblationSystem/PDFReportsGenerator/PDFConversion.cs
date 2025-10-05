using iTextSharp.text.pdf;
using LogSystem;
using System;
using System.IO;

namespace PDFReportsGenerator
{
	public class PDFConversion
	{
		/// <summary>
		/// Adds protection to PDF file
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public void Protect(string inputFile, string outputFile, string password)
		{
			string InputF = inputFile;
			string outputF = outputFile;

			using(Stream input = new FileStream(InputF, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using(Stream output = new FileStream(outputF, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					var reader = new PdfReader(input);
					try
					{
						PdfEncryptor.Encrypt(
							reader,
							output,
							true,
							password,
							string.Empty,
							PdfWriter.AllowScreenReaders | PdfWriter.AllowPrinting);
					}
					catch(Exception e)
					{
						LogService.LogException(e);
					}
				}
			}
		}

		public string Encrypt(string inputFile, string outputFile, string password)
		{
			using(Stream input = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using(Stream output = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					using (var reader_ = new PdfReader(input))
					{
						try
						{
							PdfEncryptor.Encrypt(
								reader_,
								output,
								true,
								password,
								string.Empty,
								PdfWriter.AllowScreenReaders | PdfWriter.AllowPrinting);
						}
						catch(Exception e)
						{
							LogService.LogException(e);
						}
					}
				}
			}

			return File.Exists(outputFile) ? outputFile : null;
		}
	}
}