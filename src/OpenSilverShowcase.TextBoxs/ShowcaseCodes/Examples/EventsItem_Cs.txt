using System;
using System.Windows.Controls;
using System.Windows.Input;
using OSFSample.Support.UI.Units;

namespace OpenSilverShowcase.TextBoxs.Examples
{
    public partial class EventsItem : ShowcaseItem
    {
        public EventsItem()
        {
            InitializeComponent();
        }

        private void EventTextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            GotFocusText.Text = "GotFocus: Triggered!";
            GotFocusText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
        }

        private void EventTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            LostFocusText.Text = "LostFocus: Triggered!";
            LostFocusText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
        }

        private void EventTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChangedText.Text = $"TextChanged: Triggered! Length: {EventTextBox.Text.Length}";
            TextChangedText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
        }

        private void EventTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDownText.Text = $"KeyDown: {e.Key}";
            KeyDownText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Purple);
        }

        private void EventTextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseEnterText.Text = "MouseEnter: Mouse over TextBox!";
            MouseEnterText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
        }

        private void EventTextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseLeaveText.Text = "MouseLeave: Mouse left TextBox!";
            MouseLeaveText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
        }
    }
}