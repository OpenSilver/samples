using System.Collections.Generic;
using OSFSample.Support.UI.Units;

namespace OpenSilverShowcase.ComboBoxs.Examples
{
    public partial class BindingItem : ShowcaseItem
    {
        public BindingItem()
        {
            InitializeComponent();
            LoadSportsData();
        }

        private void LoadSportsData()
        {
            var sports = new List<Sport>
            {
                new Sport { SportId = 1, SportName = "Basketball", Category = "Team Sport" },
                new Sport { SportId = 2, SportName = "Tennis", Category = "Individual Sport" },
                new Sport { SportId = 3, SportName = "Swimming", Category = "Individual Sport" },
                new Sport { SportId = 4, SportName = "Football", Category = "Team Sport" },
                new Sport { SportId = 5, SportName = "Baseball", Category = "Team Sport" },
                new Sport { SportId = 6, SportName = "Golf", Category = "Individual Sport" },
                new Sport { SportId = 7, SportName = "Volleyball", Category = "Team Sport" },
                new Sport { SportId = 8, SportName = "Boxing", Category = "Combat Sport" }
            };

            SportsComboBox.ItemsSource = sports;
        }
    }

    public class Sport
    {
        public int SportId { get; set; }
        public string SportName { get; set; }
        public string Category { get; set; }

        public override string ToString()
        {
            return $"{SportName} (ID: {SportId}, Category: {Category})";
        }
    }
}