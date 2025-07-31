using OpenSilverShowcase.Support.UI.Units;

namespace OpenSilverShowcase.CheckBoxs.Examples
{
    public partial class MVVMItem : ShowcaseItem
    {
        public MVVMItem()
        {
            this.InitializeComponent();
            DataContext = new CheckBoxViewModel();
        }
    }
}
