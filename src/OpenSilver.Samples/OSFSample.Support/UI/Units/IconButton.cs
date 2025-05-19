using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OSFSample.Support.UI.Units
{
    public class IconButton : Button
    {
        public IconButton()
        {
            DefaultStyleKey = typeof(IconButton);
        }

        public Geometry IconData
        {
            get => (Geometry)GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }

        public static readonly DependencyProperty IconDataProperty =
            DependencyProperty.Register(
                nameof(IconData),
                typeof(Geometry),
                typeof(IconButton),
                new PropertyMetadata(null));
    }
}
