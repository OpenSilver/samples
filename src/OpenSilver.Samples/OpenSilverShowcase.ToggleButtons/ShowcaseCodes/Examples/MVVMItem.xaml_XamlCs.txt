using OSFSample.Support.UI.Units;

namespace OpenSilverShowcase.ToggleButtons.Examples
{
    public partial class MVVMItem : ShowcaseItem
    {
        public MVVMItem()
        {
            this.InitializeComponent();
            this.DataContext = new ToggleButtonViewModel();
        }
    }
}
