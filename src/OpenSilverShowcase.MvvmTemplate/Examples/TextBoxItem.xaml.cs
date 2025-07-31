using OpenSilverShowcase.Support.UI.Units;

namespace OpenSilverShowcase.MvvmTemplate.Examples
{
    public partial class TextBoxItem : ShowcaseItem
    {
        public TextBoxItem()
        {
            this.InitializeComponent();
            DataContext = new TextBoxViewModel();
        }
    }
}
