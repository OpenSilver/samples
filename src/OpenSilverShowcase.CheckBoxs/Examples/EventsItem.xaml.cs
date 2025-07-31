using System;
using System.Windows;
using System.Windows.Controls;
using OpenSilverShowcase.Support.UI.Units;

namespace OpenSilverShowcase.CheckBoxs.Examples
{
    public partial class EventsItem : ShowcaseItem
    {
        public EventsItem()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            UpdateEventLog($"Checked: '{checkBox?.Content}' is now checked");
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            UpdateEventLog($"Unchecked: '{checkBox?.Content}' is now unchecked");
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var state = checkBox?.IsChecked == true ? "checked" : "unchecked";
            UpdateEventLog($"Click: '{checkBox?.Content}' clicked - state is {state}");
        }

        private void ThreeState_Checked(object sender, RoutedEventArgs e)
        {
            UpdateEventLog("ThreeState: Changed to Checked state");
        }

        private void ThreeState_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateEventLog("ThreeState: Changed to Unchecked state");
        }

        private void ThreeState_Indeterminate(object sender, RoutedEventArgs e)
        {
            UpdateEventLog("ThreeState: Changed to Indeterminate state");
        }

        private void UpdateEventLog(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            EventLog.Text = $"[{timestamp}] {message}";
        }
    }
}