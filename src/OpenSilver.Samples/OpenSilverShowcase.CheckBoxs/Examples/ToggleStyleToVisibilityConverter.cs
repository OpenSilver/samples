using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OpenSilverShowcase.CheckBoxs.Examples
{
    public class ToggleStyleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ToggleStyleType toggleStyle && parameter is string targetStyle)
            {
                return toggleStyle.ToString() == targetStyle ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}