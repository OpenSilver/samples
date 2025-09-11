using OpenSilverShowcase.Support.UI.Units;
using System.Globalization;
using System.Windows.Controls;

namespace OpenSilverShowcase.Calendars.Examples
{
    public partial class GlobalItem : ShowcaseItem
    {
        public GlobalItem()
        {
            this.InitializeComponent();
        }

        private void OnCultureChanged(object sender, SelectionChangedEventArgs e)
        {
            if (culturesCombo.SelectedItem is not ComboBoxItem selected)
            {
                return;
            }

            var culture = new CultureInfo(selected.Tag as string);
            globalCalendar.CalendarInfo = new CultureCalendarInfo(culture);
        }
    }
}
