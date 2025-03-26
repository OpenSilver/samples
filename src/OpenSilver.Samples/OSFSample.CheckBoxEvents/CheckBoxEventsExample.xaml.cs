using Jamesnet.Foundation;
using OSFSample.Support.UI.Units;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace OSFSample.CheckBoxEvents;

public partial class CheckBoxEventsExample : ExampleBase
{
    public CheckBoxEventsExample()
    {
        this.InitializeComponent();
    }
    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        txtStatus.Content = "Notifications enabled";
        rectIndicator.Fill = new SolidColorBrush(Colors.Green);
        MessageBox.Show("You will receive email notifications");
    }
    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        txtStatus.Content = "Notifications disabled";
        rectIndicator.Fill = new SolidColorBrush(Colors.Red);
        MessageBox.Show("Email notifications have been turned off");
    }
}
