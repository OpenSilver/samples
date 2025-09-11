using OpenSilverShowcase.Datas.Models;
using OpenSilverShowcase.Support.UI.Units;
using System.Windows.Data;

namespace OpenSilverShowcase.Datas.Examples
{
    public partial class GroupingItem : ShowcaseItem
    {
        public GroupingItem()
        {
            this.InitializeComponent();

            var pcv = new PagedCollectionView(Contact.People);
            pcv.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Contact.State)));
            pcv.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Contact.City)));
            dataGrid.ItemsSource = pcv;
        }
    }
}
