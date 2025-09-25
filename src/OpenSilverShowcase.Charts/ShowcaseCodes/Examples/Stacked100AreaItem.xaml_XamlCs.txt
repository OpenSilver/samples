using OpenSilverShowcase.Support.UI.Units;
using OpenSilverShowcase.Charts.Examples;

namespace OpenSilverShowcase.Charts.Examples
{
    public partial class Stacked100AreaItem : ShowcaseItem
    {
        public Stacked100AreaItem()
        {
            this.InitializeComponent();

            ChairsSeries.ItemsSource = Sales.Chairs;
            TablesSeries.ItemsSource = Sales.Tables;
        }
    }
}
