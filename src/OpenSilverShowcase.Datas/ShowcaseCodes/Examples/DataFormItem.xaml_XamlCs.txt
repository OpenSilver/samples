using OpenSilverShowcase.Datas.Models;
using OpenSilverShowcase.Support.UI.Units;
using System.Windows.Controls;

namespace OpenSilverShowcase.Datas.Examples
{
    public partial class DataFormItem : ShowcaseItem
    {
        public DataFormItem()
        {
            this.InitializeComponent();

            DataForm1.ItemsSource = Planet.GetListOfPlanets();
        }
    }
}
