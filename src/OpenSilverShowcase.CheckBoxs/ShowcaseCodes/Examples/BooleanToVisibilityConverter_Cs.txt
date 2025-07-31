using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OpenSilverShowcase.CheckBoxs.Examples
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                bool invert = parameter?.ToString() == "Invert";
                bool result = invert ? !boolValue : boolValue;
                return result ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool invert = parameter?.ToString() == "Invert";
                bool result = visibility == Visibility.Visible;
                return invert ? !result : result;
            }
            return false;
        }
    }
}