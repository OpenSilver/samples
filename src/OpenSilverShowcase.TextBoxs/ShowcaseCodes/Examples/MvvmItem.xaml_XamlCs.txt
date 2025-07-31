using OpenSilverShowcase.Support.UI.Units;

namespace OpenSilverShowcase.TextBoxs.Examples
{
    public partial class MvvmItem : ShowcaseItem
    {
        public MvvmItem()
        {
            this.InitializeComponent();
            DataContext = new TextBoxViewModel();
        }
    }
}
