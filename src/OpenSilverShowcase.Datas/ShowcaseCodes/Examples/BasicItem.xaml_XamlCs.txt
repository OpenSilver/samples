using OpenSilverShowcase.Datas.Models;
using OpenSilverShowcase.Support.UI.Units;
using System.Collections.ObjectModel;

namespace OpenSilverShowcase.Datas.Examples
{
    public partial class BasicItem : ShowcaseItem
    {
        public ObservableCollection<Member> Members { get; } = new()
        {
            new Member { Name="Han Kang",            Field="Literature",  Year=2024 },
            new Member { Name="Victor Ambros",       Field="Medicine",    Year=2024 },
            new Member { Name="Gary Ruvkun",         Field="Medicine",    Year=2024 },
            new Member { Name="Demis Hassabis",      Field="Chemistry",   Year=2024 },
            new Member { Name="John Jumper",         Field="Chemistry",   Year=2024 },
            new Member { Name="David Baker",         Field="Chemistry",   Year=2024 },
            new Member { Name="Geoffrey Hinton",     Field="Physics",     Year=2024 },
            new Member { Name="John Hopfield",       Field="Physics",     Year=2024 },
        };

        public BasicItem()
        {
            this.InitializeComponent();

            NobelGrid.ItemsSource = Members;
        }
    }
}
