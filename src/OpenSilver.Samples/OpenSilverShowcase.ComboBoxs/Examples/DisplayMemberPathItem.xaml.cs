using System.Collections.Generic;
using OSFSample.Support.UI.Units;

namespace OpenSilverShowcase.ComboBoxs.Examples
{
    public partial class DisplayMemberPathItem : ShowcaseItem
    {
        public DisplayMemberPathItem()
        {
            InitializeComponent();
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            var people = new List<Person>
            {
                new Person { Name = "John Smith", Age = 30 },
                new Person { Name = "Jane Doe", Age = 25 },
                new Person { Name = "Mike Johnson", Age = 35 },
                new Person { Name = "Sarah Wilson", Age = 28 }
            };
            PeopleComboBox.ItemsSource = people;
            PeopleComboBox.SelectedIndex = 0;

            var cities = new List<City>
            {
                new City { CityName = "Seoul", Country = "South Korea" },
                new City { CityName = "New York", Country = "USA" },
                new City { CityName = "Tokyo", Country = "Japan" },
                new City { CityName = "London", Country = "UK" }
            };
            CitiesComboBox.ItemsSource = cities;
            CitiesComboBox.SelectedIndex = 0;

            var products = new List<Product>
            {
                new Product { Title = "Laptop Computer", Price = 1200.00m },
                new Product { Title = "Wireless Mouse", Price = 25.99m },
                new Product { Title = "Mechanical Keyboard", Price = 89.99m },
                new Product { Title = "USB Monitor", Price = 299.99m }
            };
            ProductsComboBox.ItemsSource = products;
            ProductsComboBox.SelectedIndex = 0;

            var categories = new List<Category>
            {
                new Category { Description = "Electronics & Gadgets", Code = "ELEC" },
                new Category { Description = "Books & Education", Code = "BOOK" },
                new Category { Description = "Clothing & Fashion", Code = "CLTH" },
                new Category { Description = "Home & Garden", Code = "HOME" }
            };
            CategoriesComboBox.ItemsSource = categories;
            CategoriesComboBox.SelectedIndex = 0;
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class City
    {
        public string CityName { get; set; }
        public string Country { get; set; }
    }

    public class Product
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
    }

    public class Category
    {
        public string Description { get; set; }
        public string Code { get; set; }
    }
}