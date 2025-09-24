using OpenSilverShowcase.Support.UI.Units;
using OpenSilverShowcase.Charts.Examples;

namespace OpenSilverShowcase.Charts.Examples
{
    public partial class LineSeriesItem : ShowcaseItem
    {
        public LineSeriesItem()
        {
            this.InitializeComponent();

            ChairsSeries.ItemsSource = Sales.Chairs;
            TablesSeries.ItemsSource = Sales.Tables;
        }
    }
}
