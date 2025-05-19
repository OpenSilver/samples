using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OSFSample.Support.UI.Units;

namespace OpenSilverShowcase.Buttons.Event
{
    public partial class ButtonEventContent : ShowcaseCard
    {
        public ButtonEventContent()
        {
            this.InitializeComponent();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Background = new SolidColorBrush(Colors.Brown);
                button.Content = "Click Successed!";
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                btnSubmit.Background = new SolidColorBrush(Colors.Chocolate);
                btnSubmit.Content = "Submit";
            }
        }
    }
}
