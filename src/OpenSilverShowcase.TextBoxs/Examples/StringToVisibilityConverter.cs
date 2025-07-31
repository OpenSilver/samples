using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace OpenSilverShowcase.TextBoxs.Examples
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
