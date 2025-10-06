using OpenSilverShowcase.Support.Local.Helpers;
using System.Windows;

namespace OpenSilverSample
{
    public sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();

            var code = ShowcaseExtractor.ExtractAllControlItemsAsCode();

            var mainPage = new MainPage();
            Window.Current.Content = mainPage;
        }
    }
}
