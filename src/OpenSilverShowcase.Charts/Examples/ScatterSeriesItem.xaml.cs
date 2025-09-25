using OpenSilverShowcase.Support.UI.Units;
using OpenSilverShowcase.Charts.Examples;

namespace OpenSilverShowcase.Charts.Examples
{
    public partial class ScatterSeriesItem : ShowcaseItem
    {
        public ScatterSeriesItem()
        {
            this.InitializeComponent();

            ChairsSeries.ItemsSource = Sales.Chairs;
            TablesSeries.ItemsSource = Sales.Tables;
        }
    }
}
