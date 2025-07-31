using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenSilverShowcase.ComboBoxs.Examples
{
    public class CustomComboBox : ComboBox
    {
        public static readonly DependencyProperty LightDismissOverlayBackgroundProperty = DependencyProperty.Register("LightDismissOverlayBackground", typeof(Brush), typeof(CustomComboBox), new PropertyMetadata(null));

        public Brush LightDismissOverlayBackground
        {
            get { return (Brush)GetValue(LightDismissOverlayBackgroundProperty); }
            set { SetValue(LightDismissOverlayBackgroundProperty, value); }
        }

        public CustomComboBox()
        {
            DefaultStyleKey = typeof(CustomComboBox);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomComboBoxItem();
        }
    }
    public class CustomComboBoxItem : ComboBoxItem
    {
        public CustomComboBoxItem()
        { 
            DefaultStyleKey = typeof(CustomComboBoxItem);
        }
    }
}