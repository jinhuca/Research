using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a USB Drive to a String
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class USBDriveToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value to a target type depending on the object received in parameter
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="value">An object to convert.</param>
        /// <param name="targetType">A Type representing the conversion target type.</param>
        /// <param name="parameter">An object representing the conversion's parameter.</param>
        /// <param name="culture">Provides information about a specific culture (called a locale for unmanaged).</param>
        /// <returns>An object converted to the target type.</returns>
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null)
            {
                if (value is List<DriveInfo>)
                {
                    DriveInfo driveInfo = ((List<DriveInfo>)value)[0];

                    if (parameter.ToString() == "NAME")
                    {
                        return driveInfo.Name;
                    }
                    else if (parameter.ToString() == "ROOTDIRECTORY")
                    {
                        return driveInfo.RootDirectory;
                    }
                    else if (parameter.ToString() == "AVAILABLEFREESPACE")
                    {
                        return driveInfo.AvailableFreeSpace;
                    }
                    else if (parameter.ToString() == "DRIVEFORMAT")
                    {
                        return driveInfo.DriveFormat;
                    }
                    else if (parameter.ToString() == "ISREADY")
                    {
                        return driveInfo.IsReady;
                    }
                    else if (parameter.ToString() == "FREESPACE")
                    {
                        if (driveInfo.TotalSize != 0)
                        {
                            decimal result = (decimal)driveInfo.TotalFreeSpace / (decimal)driveInfo.TotalSize;
                            decimal percentage = result * 100;

                            return driveInfo.TotalFreeSpace.ToString() + " (" + percentage.ToString("0.0") + "%)";
                        }
                        else
                        {
                            return driveInfo.TotalFreeSpace.ToString() + " (-%)";
                        }
                    }
                    else if (parameter.ToString() == "TOTALSIZE")
                    {
                        return driveInfo.TotalSize;
                    }
                    else if (parameter.ToString() == "VOLUMELABEL")
                    {
                        return driveInfo.VolumeLabel;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Converts back an object to a target type depending on the object received in parameter
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="value">An object to convert back.</param>
        /// <param name="targetType">A Type representing the conversion target type.</param>
        /// <param name="parameter">An object representing the conversion's parameter.</param>
        /// <param name="culture">Provides information about a specific culture (called a locale for unmanaged).</param>
        /// <returns>An object converted to the target type.</returns>
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return value;
            }
            catch (Exception ex)
            {
                // TO DO
                ex.ToString();
                return 0;
            }
        }

        #endregion IValueConverter Members
    }
}