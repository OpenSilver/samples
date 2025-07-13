using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace OSFSample.Support.Local.Converters;

public class SelectionBrushConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 3 || !(values[0] is bool) || !(values[1] is Brush) || !(values[2] is Brush))
        {
            return new SolidColorBrush(Colors.Black);
        }

        bool isSelected = (bool)values[0];
        Brush selectedBrush = (Brush)values[1];
        Brush normalBrush = (Brush)values[2];

        return isSelected ? selectedBrush : normalBrush;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SelectionFontSizeConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (values[0] is bool isSelected &&
            values[1] is double selectedSize &&
            values[2] is double normalSize)
        {
            return isSelected ? selectedSize : normalSize;
        }

        // fallback: values[2] 직접 접근
        return values.Length > 2 && values[2] is double fallbackSize ? fallbackSize : 13.0; // 기본값
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}


public class SelectionFontWeightConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (values[0] is bool isSelected &&
            values[1] is FontWeight selectedWeight &&
            values[2] is FontWeight normalWeight)
        {
            return isSelected ? selectedWeight : normalWeight;
        }
        return values.Length > 2 && values[2] is FontWeight fallback ? fallback : FontWeights.Normal;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool booleanValue)
        {
            return booleanValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }
        return System.Windows.Visibility.Collapsed; // 기본값
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class CodeTabToVisibilityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // DependencyProperty.UnsetValue 체크 추가
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == DependencyProperty.UnsetValue)
                return System.Windows.Visibility.Collapsed;
        }

        if (values.Length >= 2 && values[0] != null && values[1] != null)
        {
            string thisTab = values[0].ToString();
            string selectedTab = values[1].ToString();

            // 명시적으로 System.Windows.Visibility 사용
            return string.Equals(thisTab, selectedTab, StringComparison.OrdinalIgnoreCase)
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;
        }
        return System.Windows.Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}