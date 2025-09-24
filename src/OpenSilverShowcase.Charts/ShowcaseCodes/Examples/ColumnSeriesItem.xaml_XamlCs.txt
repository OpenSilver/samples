using OpenSilverShowcase.Support.UI.Units;

namespace OpenSilverShowcase.Charts.Examples
{
    public partial class ColumnSeriesItem : ShowcaseItem
    {
        public ColumnSeriesItem()
        {
            this.InitializeComponent();

            ChairsSeries.ItemsSource = Sales.Chairs;
            TablesSeries.ItemsSource = Sales.Tables;
        }
    }
}
