using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace OpenSilverShowcase.ComboBoxs.Examples
{
    public class ComboBoxViewModel : INotifyPropertyChanged
    {
        private Genre _selectedGenre;
        private Game _selectedGame;
        private ObservableCollection<Game> _availableGames;

        public ComboBoxViewModel()
        {
            LoadData();
        }

        public ObservableCollection<Genre> Genres { get; set; }

        public ObservableCollection<Game> AvailableGames
        {
            get => _availableGames;
            set
            {
                _availableGames = value;
                OnPropertyChanged(nameof(AvailableGames));
            }
        }

        public Genre SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                _selectedGenre = value;
                OnPropertyChanged(nameof(SelectedGenre));
                FilterGamesByGenre();
            }
        }

        public Game SelectedGame
        {
            get => _selectedGame;
            set
            {
                _selectedGame = value;
                OnPropertyChanged(nameof(SelectedGame));
            }
        }

        private List<Game> AllGames { get; set; }

        private void LoadData()
        {
            Genres = new ObservableCollection<Genre>
            {
                new Genre { GenreId = 1, GenreName = "Action" },
                new Genre { GenreId = 2, GenreName = "RPG" },
                new Genre { GenreId = 3, GenreName = "Strategy" },
                new Genre { GenreId = 4, GenreName = "Simulation" },
                new Genre { GenreId = 5, GenreName = "Sports" }
            };

            AllGames = new List<Game>
            {
                new Game { GameId = 1, GameTitle = "Call of Duty", GenreId = 1, ReleaseYear = 2003 },
                new Game { GameId = 2, GameTitle = "Grand Theft Auto", GenreId = 1, ReleaseYear = 1997 },
                new Game { GameId = 3, GameTitle = "Assassin's Creed", GenreId = 1, ReleaseYear = 2007 },

                new Game { GameId = 4, GameTitle = "The Elder Scrolls", GenreId = 2, ReleaseYear = 1994 },
                new Game { GameId = 5, GameTitle = "Final Fantasy", GenreId = 2, ReleaseYear = 1987 },
                new Game { GameId = 6, GameTitle = "The Witcher", GenreId = 2, ReleaseYear = 2007 },

                new Game { GameId = 7, GameTitle = "Civilization", GenreId = 3, ReleaseYear = 1991 },
                new Game { GameId = 8, GameTitle = "Age of Empires", GenreId = 3, ReleaseYear = 1997 },
                new Game { GameId = 9, GameTitle = "StarCraft", GenreId = 3, ReleaseYear = 1998 },

                new Game { GameId = 10, GameTitle = "SimCity", GenreId = 4, ReleaseYear = 1989 },
                new Game { GameId = 11, GameTitle = "The Sims", GenreId = 4, ReleaseYear = 2000 },

                new Game { GameId = 12, GameTitle = "FIFA", GenreId = 5, ReleaseYear = 1993 },
                new Game { GameId = 13, GameTitle = "NBA 2K", GenreId = 5, ReleaseYear = 1999 }
            };

            AvailableGames = new ObservableCollection<Game>();
        }

        private void FilterGamesByGenre()
        {
            if (SelectedGenre == null)
            {
                AvailableGames.Clear();
                SelectedGame = null;
                return;
            }

            var filteredGames = AllGames.Where(g => g.GenreId == SelectedGenre.GenreId).ToList();

            AvailableGames.Clear();
            foreach (var game in filteredGames)
            {
                AvailableGames.Add(game);
            }

            SelectedGame = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Genre
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }

    public class Game
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public int GenreId { get; set; }
        public int ReleaseYear { get; set; }
    }
}