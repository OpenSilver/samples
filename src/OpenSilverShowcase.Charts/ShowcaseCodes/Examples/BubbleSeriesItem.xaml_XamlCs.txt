using OpenSilverShowcase.Support.UI.Units;
using OpenSilverShowcase.Charts.Examples;

namespace OpenSilverShowcase.Charts.Examples
{
    public partial class BubbleSeriesItem : ShowcaseItem
    {
        public BubbleSeriesItem()
        {
            this.InitializeComponent();

            ChairsSeries.ItemsSource = Sales.Chairs;
            TablesSeries.ItemsSource = Sales.Tables;
        }
    }
}
