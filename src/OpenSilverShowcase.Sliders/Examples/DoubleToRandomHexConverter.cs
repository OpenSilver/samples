using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace OpenSilverShowcase.Sliders.Examples
{
    public class DoubleToRandomHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Generate a random color based on the double value as a seed
            if (value is double d)
            {
                // Use the double value to seed the random generator for deterministic output
                int seed = d.GetHashCode();
                var random = new Random(seed);

                byte r = (byte)random.Next(0, 256);
                byte g = (byte)random.Next(0, 256);
                byte b = (byte)random.Next(0, 256);

                // Return as hex string, e.g. "#FFAABB"
                return $"#{r:X2}{g:X2}{b:X2}";
            }
            return "#000000";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
