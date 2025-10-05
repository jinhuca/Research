using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
	public class MultipleBoolToVisibleConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			bool.TryParse(values[0].ToString(), out var isExporting_);
			bool.TryParse(values[1].ToString(), out var isCanceled_);
			bool.TryParse(values[2].ToString(), out var isExportingCurrentProcedure_);
			
			switch (parameter.ToString())
			{
				case "StartingSelectedProceduresExport":
					return isExporting_ && !isCanceled_ && !isExportingCurrentProcedure_ 
						? Visibility.Visible 
						: Visibility.Hidden;
				case "StartingCurrentProcedureExport":
					return isExporting_ && !isCanceled_ && isExportingCurrentProcedure_
						? Visibility.Visible
						: Visibility.Hidden;

				case "CancelingSelectedProceduresExport":
					return isExporting_ && isCanceled_ && !isExportingCurrentProcedure_
						? Visibility.Visible 
						: Visibility.Hidden;
				case "CanceledSelectedProceduresExport":
					return !isExporting_ && isCanceled_ && !isExportingCurrentProcedure_ 
						? Visibility.Visible 
						: Visibility.Hidden;

				case "FinishedSelectedProceduresExport":
					return !isExporting_ && !isCanceled_ && !isExportingCurrentProcedure_
						? Visibility.Visible
						: Visibility.Hidden;
				case "FinishedCurrentProcedureExport":
					return !isExporting_ && !isCanceled_ && isExportingCurrentProcedure_ 
						? Visibility.Visible 
						: Visibility.Hidden;
			}

			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
