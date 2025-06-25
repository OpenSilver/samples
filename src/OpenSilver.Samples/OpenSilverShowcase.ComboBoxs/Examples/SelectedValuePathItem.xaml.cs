using System.Collections.Generic;
using System.Windows.Controls;
using OSFSample.Support.UI.Units;

namespace OpenSilverShowcase.ComboBoxs.Examples
{
    public partial class SelectedValuePathItem : ShowcaseItem
    {
        public SelectedValuePathItem()
        {
            InitializeComponent();
            LoadComboBoxData();
            SetupEventHandlers();
        }

        private void LoadComboBoxData()
        {
            var movies = new List<Movie>
            {
                new Movie { MovieId = 101, Title = "Avengers: Endgame" },
                new Movie { MovieId = 102, Title = "The Dark Knight" },
                new Movie { MovieId = 103, Title = "Inception" },
                new Movie { MovieId = 104, Title = "Interstellar" }
            };
            MoviesComboBox.ItemsSource = movies;

            var books = new List<Book>
            {
                new Book { ISBN = "978-0134685991", Title = "Effective Java" },
                new Book { ISBN = "978-0596007126", Title = "Head First Design Patterns" },
                new Book { ISBN = "978-0135957059", Title = "The Pragmatic Programmer" },
                new Book { ISBN = "978-0132350884", Title = "Clean Code" }
            };
            BooksComboBox.ItemsSource = books;

            var cars = new List<Car>
            {
                new Car { VIN = "1HGCM82633A123456", Model = "Honda Accord" },
                new Car { VIN = "KMHD84LF5DA123456", Model = "Hyundai Elantra" },
                new Car { VIN = "5NPE34AF2FH123456", Model = "Hyundai Sonata" },
                new Car { VIN = "1N4AL3AP1FC123456", Model = "Nissan Altima" }
            };
            CarsComboBox.ItemsSource = cars;

            var courses = new List<Course>
            {
                new Course { CourseCode = "CS101", CourseName = "Introduction to Programming" },
                new Course { CourseCode = "CS201", CourseName = "Data Structures" },
                new Course { CourseCode = "CS301", CourseName = "Database Systems" },
                new Course { CourseCode = "CS401", CourseName = "Software Engineering" }
            };
            CoursesComboBox.ItemsSource = courses;
        }

        private void SetupEventHandlers()
        {
            MoviesComboBox.SelectionChanged += (s, e) =>
                MovieValueText.Text = $"Selected Value: {MoviesComboBox.SelectedValue}";

            BooksComboBox.SelectionChanged += (s, e) =>
                BookValueText.Text = $"Selected Value: {BooksComboBox.SelectedValue}";

            CarsComboBox.SelectionChanged += (s, e) =>
                CarValueText.Text = $"Selected Value: {CarsComboBox.SelectedValue}";

            CoursesComboBox.SelectionChanged += (s, e) =>
                CourseValueText.Text = $"Selected Value: {CoursesComboBox.SelectedValue}";
        }
    }

    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
    }

    public class Book
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
    }

    public class Car
    {
        public string VIN { get; set; }
        public string Model { get; set; }
    }

    public class Course
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
    }
}