using OpenSilverShowcase.Support.UI.Units;
using System.Windows;

namespace OpenSilverShowcase.LayoutTransformer.Examples
{
    public partial class LayoutItem : ShowcaseItem
    {
        public LayoutItem()
        {
            this.InitializeComponent();
        }
        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            layoutTransformer.ApplyLayoutTransform();
        }
    }
}
