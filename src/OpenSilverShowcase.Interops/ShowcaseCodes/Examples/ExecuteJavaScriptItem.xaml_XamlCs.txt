using OpenSilver;
using OpenSilverShowcase.Support.UI.Units;
using System.Windows;

namespace OpenSilverShowcase.Interops.Examples
{
    public partial class ExecuteJavaScriptItem : ShowcaseItem
    {
        public ExecuteJavaScriptItem()
        {
            this.InitializeComponent();
        }
        void SendJavaScriptMessage(object sender, RoutedEventArgs e)
        {
            Interop.ExecuteJavaScript("alert($0);", TextBoxInput.Text);
        }
    }
}
