using OpenSilverShowcase.Support.UI.Units;

namespace OpenSilverShowcase.RadioButtons.Examples
{
    public partial class MVVMItem : ShowcaseItem
    {
        public MVVMItem()
        {
            this.InitializeComponent();
            DataContext = new RadioButtonViewModel();
        }
    }
}
