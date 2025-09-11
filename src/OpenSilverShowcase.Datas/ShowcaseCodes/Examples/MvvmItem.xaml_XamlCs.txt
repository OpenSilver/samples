using OpenSilverShowcase.Datas.ViewModels;
using OpenSilverShowcase.Support.UI.Units;

namespace OpenSilverShowcase.Datas.Examples
{
    public partial class MvvmItem : ShowcaseItem
    {
        public MvvmItem()
        {
            this.InitializeComponent();

            DataContext = new MoviesViewModel();
        }
    }
}
