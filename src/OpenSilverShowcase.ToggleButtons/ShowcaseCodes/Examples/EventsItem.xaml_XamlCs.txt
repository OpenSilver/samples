using System.Windows;
using System.Windows.Controls.Primitives;
using OpenSilverShowcase.Support.UI.Units;

namespace OpenSilverShowcase.ToggleButtons.Examples
{
    public partial class EventsItem : ShowcaseItem
    {
        private int clickCount = 0;

        public EventsItem()
        {
            this.InitializeComponent();
        }

        private void OnToggleButtonClick(object sender, RoutedEventArgs e)
        {
            clickCount++;
            ClickCountText.Text = $"Click Count: {clickCount}";
        }

        private void OnToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            StateText.Text = "State: Checked";
        }

        private void OnToggleButtonUnchecked(object sender, RoutedEventArgs e)
        {
            StateText.Text = "State: Unchecked";
        }

        private void OnThreeStateChecked(object sender, RoutedEventArgs e)
        {
            ThreeStateText.Text = "State: Checked";
        }

        private void OnThreeStateUnchecked(object sender, RoutedEventArgs e)
        {
            ThreeStateText.Text = "State: Unchecked";
        }

        private void OnThreeStateIndeterminate(object sender, RoutedEventArgs e)
        {
            ThreeStateText.Text = "State: Indeterminate";
        }
    }
}