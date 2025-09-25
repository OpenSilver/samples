using OpenSilverShowcase.Support.UI.Units;
using System.Windows;

namespace OpenSilverShowcase.Interops.Examples
{
    public partial class HtmlPresenterItem : ShowcaseItem
    {
        public HtmlPresenterItem()
        {
            this.InitializeComponent();
        }
        private void ButtonClickHere_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The value is: " + NumericTextBox1.Value.ToString());
        }
    }
}
