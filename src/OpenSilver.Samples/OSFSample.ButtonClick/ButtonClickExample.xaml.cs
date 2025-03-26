using OSFSample.Support.UI.Units;
using System.Windows;

namespace OSFSample.ButtonClick;

public partial class ButtonClickExample : ExampleBase
{
    int count = 0;
    public ButtonClickExample()
    {
        this.InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        count++;
        btnCount.Content = $"Click Count: {count}";
        System.Windows.MessageBox.Show("Button Clicked!");
    }
}
