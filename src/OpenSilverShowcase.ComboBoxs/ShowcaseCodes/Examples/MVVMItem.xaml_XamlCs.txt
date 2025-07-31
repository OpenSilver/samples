using OpenSilverShowcase.Support.UI.Units;

namespace OpenSilverShowcase.ComboBoxs.Examples
{
    public partial class MVVMItem : ShowcaseItem
    {
        public MVVMItem()
        {
            this.InitializeComponent();
            DataContext = new ComboBoxViewModel();
        }
    }
}
