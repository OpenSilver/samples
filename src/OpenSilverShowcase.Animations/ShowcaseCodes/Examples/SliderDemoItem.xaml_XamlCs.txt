using OpenSilverShowcase.Support.UI.Units;
using System.Windows.Media.Animation;
using System.Windows;

namespace OpenSilverShowcase.Animations.Examples
{
    public partial class SliderDemoItem : ShowcaseItem
    {
        public SliderDemoItem()
        {
            this.InitializeComponent();
        }
        void ButtonToStartAnimationOpen_Click(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)CanvasForAnimationsDemo.Resources["AnimationToOpen"];
            storyboard.Begin();
            ButtonToStartAnimationOpen.Visibility = Visibility.Collapsed;
            ButtonToStartAnimationClose.Visibility = Visibility.Visible;
        }

        void ButtonToStartAnimationClose_Click(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)CanvasForAnimationsDemo.Resources["AnimationToClose"];
            storyboard.Begin();
            ButtonToStartAnimationOpen.Visibility = Visibility.Visible;
            ButtonToStartAnimationClose.Visibility = Visibility.Collapsed;
        }
    }
}
