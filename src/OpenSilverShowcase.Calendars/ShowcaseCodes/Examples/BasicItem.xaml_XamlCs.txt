using OpenSilverShowcase.Support.UI.Units;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;

namespace OpenSilverShowcase.Calendars.Examples
{
    public partial class BasicItem : ShowcaseItem
    {
        public BasicItem()
        {
            this.InitializeComponent();
        }

        private void OnPastDatesChanged(object sender, RoutedEventArgs e)
        {
            if (sampleCalendar == null)
            {
                return;
            }

            if ((bool)chkPastDateSelection.IsChecked)
            {
                sampleCalendar.BlackoutDates.Clear();
            }
            else
            {
                try
                {
                    sampleCalendar.BlackoutDates.AddDatesInPast();
                }
                catch
                {
                    chkPastDateSelection.IsChecked = true;
                }
            }
        }
    }
}
