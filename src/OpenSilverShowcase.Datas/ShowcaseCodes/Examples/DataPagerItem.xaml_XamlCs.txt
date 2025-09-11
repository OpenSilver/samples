using OpenSilverShowcase.Datas.Models;
using OpenSilverShowcase.Support.UI.Units;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace OpenSilverShowcase.Datas.Examples
{
    public partial class DataPagerItem : ShowcaseItem
    {
        public DataPagerItem()
        {
            this.InitializeComponent();

            var pagedView = new PagedCollectionView(AtomicElement.GetElements());
            DataGrid1.ItemsSource = pagedView;

            dataPager.Source = pagedView;
        }
    }
}
