using OpenSilverShowcase.Support.UI.Units;
using OpenSilverShowcase.Charts.Examples;

namespace OpenSilverShowcase.Charts.Examples
{
    public partial class PieSeriesItem : ShowcaseItem
    {
        public PieSeriesItem()
        {
            this.InitializeComponent();

            ProductSales.ItemsSource = ProductSalesData.GetSalesData();
        }
    }
}
