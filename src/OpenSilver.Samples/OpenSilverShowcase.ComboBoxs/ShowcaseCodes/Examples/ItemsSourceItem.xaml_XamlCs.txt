using System;
using System.Collections.Generic;
using System.Windows.Controls;
using OSFSample.Support.UI.Units;

namespace OpenSilverShowcase.ComboBoxs.Examples
{
    public partial class ItemsSourceItem : ShowcaseItem
    {
        public ItemsSourceItem()
        {
            InitializeComponent();
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            var countries = new List<string>
            {
                "South Korea",
                "United States",
                "Japan",
                "Germany",
                "France",
                "United Kingdom"
            };
            CountriesComboBox.ItemsSource = countries;

            var fruits = new List <string>
            {
                "Apple",
                "Banana",
                "Orange",
                "Grape",
                "Strawberry"
            };
            FruitsComboBox.ItemsSource = fruits;

            var colors = new List<string>
            {
                "Red",
                "Blue",
                "Green",
                "Yellow",
                "Purple"
            };
            ColorsComboBox.ItemsSource = colors;
            ColorsComboBox.SelectedIndex = 1; 

            var numbers = new List<string>();
            for (int i = 1; i
                <= 10; i++)
            {
                numbers.Add($"Number {i}");
            }
            NumbersComboBox.ItemsSource = numbers;
        }
    }
}