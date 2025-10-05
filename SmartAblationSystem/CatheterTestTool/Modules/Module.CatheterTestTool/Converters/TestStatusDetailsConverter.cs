using System;
using System.Globalization;
using System.Windows.Data;
using Module.CatheterTestTool.Services;

namespace Module.CatheterTestTool.Converters
{
    internal class TestStatusDetailsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var testStatus = value as TestStatus;
            string testDetail = string.Empty;

            if (testStatus == null) return testDetail;

            return testStatus.CurrentTestStep == 0 
                 ? $"{testStatus.Description}"
                 : $"Step {testStatus.CurrentTestStep} : {testStatus.Description}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
