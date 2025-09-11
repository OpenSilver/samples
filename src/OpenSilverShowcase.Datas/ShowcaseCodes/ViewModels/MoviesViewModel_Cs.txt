using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenSilverShowcase.Datas.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OpenSilverShowcase.Datas.ViewModels;

public partial class MoviesViewModel : ObservableObject
{
    public ObservableCollection<Movie> Movies { get; } = [];

    public MoviesViewModel()
    {
        Add();
    }

    [RelayCommand]
    private void Add()
    {
        Movies.Add(new Movie { Title = "New Movie" });
    }

    [RelayCommand]
    private void Remove(Movie movie)
    {
        Movies.Remove(movie);
    }
}
