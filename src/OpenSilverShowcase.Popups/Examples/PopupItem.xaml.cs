using OpenSilverShowcase.Support.UI.Units;
using System.Windows;

namespace OpenSilverShowcase.Popups.Examples
{
    public partial class PopupItem : ShowcaseItem
    {
        public PopupItem()
        {
            this.InitializeComponent();
        }

        private void OpenPopupButton_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = true;
        }

        private void PopupButtonClose_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = false;
        }
    }
}
