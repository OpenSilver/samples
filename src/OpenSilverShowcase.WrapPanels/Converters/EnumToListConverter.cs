using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace OpenSilverShowcase.WarpPanels.Converters;

public class EnumToListConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Type enumType = null;

        if (parameter is Type type)
        {
            enumType = type;
        }
        else if (parameter is DependencyProperty dependencyProperty)
        {
            enumType = dependencyProperty.PropertyType;
        }

        if (enumType != null && enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            enumType = Nullable.GetUnderlyingType(enumType);
        }

        if (enumType != null && enumType.IsEnum)
        {
            return Enum.GetValues(enumType).Cast<object>().ToList();
        }

        return new List<object>();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
