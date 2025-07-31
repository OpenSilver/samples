using System.Windows;
using System.Windows.Controls;

namespace OpenSilverShowcase.Support.UI.Units
{
    public class SimpleCodeTextBox : ContentControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(SimpleCodeTextBox),
                new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public SimpleCodeTextBox()
        {
            DefaultStyleKey = typeof(SimpleCodeTextBox);
        }
    }
}
