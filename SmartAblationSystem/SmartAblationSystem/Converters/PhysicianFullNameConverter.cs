using System;
using System.Globalization;
using System.Windows.Data;
using DataAccessLayer;
using SmartAblationSystem.ViewModels;
using Type = System.Type;

namespace SmartAblationSystem.Converters
{
	[ValueConversion(typeof(Physician), typeof(string))]
	public class PhysicianFullNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var fullName_ = string.Empty;

			if(value?.GetType().BaseType != typeof(Physician))
      {
        return fullName_;
      }

      if (CommonViewModel.Current.IsBSCADMINUser || CommonViewModel.Current.IsCryterionUser)
      {
        return UIConstants.DoubleDash;
      }

			var p_ = (Physician)value;
			fullName_ = "Dr. " + p_.FirstName + " " + p_.LastName;

			return fullName_;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
