using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OpenSilverShowcase.Support.UI.Units;

namespace OpenSilverShowcase.RadioButtons.Examples
{
    public partial class EventsItem : ShowcaseItem
    {
        public EventsItem()
        {
            try
            {
                InitializeComponent();
                if (RadioNone == null || RadioImportant == null || RadioAll == null)
                {
                    System.Diagnostics.Debug.WriteLine("Radio buttons are null!");
                }
                if (CurrentSelectionText == null || LastEventText == null || EventHistoryPanel == null)
                {
                    System.Diagnostics.Debug.WriteLine("Text or panel is null!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitializeComponent failed: {ex.Message}");
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var radioButton = sender as RadioButton;
                if (radioButton == null) return;

                string content = radioButton.Content?.ToString() ?? "Unknown";
                CurrentSelectionText.Text = $"Current: {content}";
                LastEventText.Text = $"Last event: {content} Checked";
                AddHistory($"[{DateTime.Now:HH:mm:ss}] {content} Checked", Color.FromRgb(128, 128, 128)); // Colors.Gray
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RadioButton_Checked failed: {ex.Message}");
            }
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var radioButton = sender as RadioButton;
                if (radioButton == null) return;

                string content = radioButton.Content?.ToString() ?? "Unknown";
                LastEventText.Text = $"Last event: {content} Unchecked";
                AddHistory($"[{DateTime.Now:HH:mm:ss}] {content} Unchecked", Color.FromRgb(211, 211, 211)); // Colors.LightGray
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RadioButton_Unchecked failed: {ex.Message}");
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var radioButton = sender as RadioButton;
                if (radioButton == null) return;

                string content = radioButton.Content?.ToString() ?? "Unknown";
                string state = radioButton.IsChecked == true ? "selected" : "unselected";
                LastEventText.Text = $"Last event: {content} Clicked";
                AddHistory($"[{DateTime.Now:HH:mm:ss}] {content} Clicked - state is {state}", Color.FromRgb(123, 104, 238)); // Colors.MediumSlateBlue
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RadioButton_Click failed: {ex.Message}");
            }
        }

        private void AddHistory(string message, Color color)
        {
            try
            {
                var textBlock = new TextBlock
                {
                    Text = message,
                    FontSize = 11,
                    Foreground = new SolidColorBrush(color),
                    Margin = new Thickness(0, 0, 0, 1)
                };
                EventHistoryPanel.Children.Add(textBlock);

                if (EventHistoryPanel.Parent is ScrollViewer sv)
                {
                    sv.ScrollToVerticalOffset(sv.ScrollableHeight);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddHistory failed: {ex.Message}");
            }
        }
    }
}