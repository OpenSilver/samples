using System.Collections.Generic;
using OSFSample.Support.UI.Units;

namespace OpenSilverShowcase.ComboBoxs.Examples
{
    public partial class CustomItem : ShowcaseItem
    {
        public CustomItem()
        {
            InitializeComponent();

            var champions = new List<Champion>
            {
                new Champion { Name = "Ahri", Role = "Mage", Difficulty = 3 },
                new Champion { Name = "Garen", Role = "Fighter", Difficulty = 1 },
                new Champion { Name = "Lux", Role = "Mage", Difficulty = 2 },
                new Champion { Name = "Jinx", Role = "Marksman", Difficulty = 2 },
                new Champion { Name = "Thresh", Role = "Support", Difficulty = 4 }
            };
            CustomRiotComboBox.ItemsSource = champions;
            CustomRiotComboBox.SelectedIndex = 0;
        }
    }

    public class Champion
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public int Difficulty { get; set; }
        public override string ToString()
        {
            return $"{Name} - {Role} (Difficulty: {Difficulty})";
        }
    }
}