using System;
using SmartAblationSystem.Helpers;

namespace SmartAblationSystem.ViewModels
{
	internal class FileType
	{
		public FileTypeEnum Type { get; set; }
		public string Name { get; set; }
		public string Extension { get; set; }

		public FileType(FileTypeEnum type)
		{
			switch(type)
			{
				case FileTypeEnum.PDF:
					Type = type;
					Name = FileTypeEnum.PDF.ToString();
					Extension = ".pdf";
					break;
				case FileTypeEnum.Excel:
					Type = type;
					Name = FileTypeEnum.Excel.ToString();
					Extension = ".xlsx";
					break;
				case FileTypeEnum.Json:
					Type = type;
					Name = FileTypeEnum.Json.ToString();
					Extension = ".json";
					break;
				case FileTypeEnum.CaseReport:
					Type = type;
					Name = FileTypeEnum.CaseReport.ToString();
					Extension = ".pdf";
					break;
				case FileTypeEnum.Log:
					Type = type;
					Name = FileTypeEnum.Log.ToString();
					Extension = ".log";
					break;
				case FileTypeEnum.Unknown:
					Type = type;
					Name = FileTypeEnum.Unknown.ToString();
					Extension = string.Empty;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
	}
}