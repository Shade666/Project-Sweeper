using System;
using System.Globalization;
using System.Windows.Data;

namespace PKHL.ProjectSweeper.Converters
{
    /// <summary>
    /// Converts integer values to boolean (non-zero = true, zero = false)
    /// </summary>
    [ValueConversion(typeof(int), typeof(bool))]
    public class IntegerToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue != 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? 1 : 0;
            }
            return 0;
        }
    }
}
