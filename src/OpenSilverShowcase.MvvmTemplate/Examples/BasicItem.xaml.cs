using OpenSilverShowcase.Support.UI.Units;

namespace OpenSilverShowcase.MvvmTemplate.Examples
{
    public partial class BasicItem : ShowcaseItem
    {
        public BasicItem()
        {
            this.InitializeComponent();

            DataContext = new BasicViewModel();
        }
    }
}
