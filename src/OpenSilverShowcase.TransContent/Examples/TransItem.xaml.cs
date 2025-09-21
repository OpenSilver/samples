using OpenSilverShowcase.Support.UI.Units;
using System;
using System.Windows;
using System.Windows.Controls;

namespace OpenSilverShowcase.TransContent.Examples
{
    public partial class TransItem : ShowcaseItem
    {
        public TransItem()
        {
            this.InitializeComponent();
        }

        private void ChangeContentWithDefaultTransition(object sender, RoutedEventArgs e)
        {
            SetContentWithTransition(TransitioningContentControl.DefaultTransitionState);
        }

        private void ChangeContentWithDownTransition(object sender, RoutedEventArgs e)
        {
            SetContentWithTransition("DownTransition");
        }

        private void ChangeContentWithUpTransition(object sender, RoutedEventArgs e)
        {
            SetContentWithTransition("UpTransition");
        }

        private void SetContentWithTransition(string transition)
        {
            defaultTCC.Transition = transition;
            defaultTCC.Content = DateTime.Now.Ticks;
        }
    }
}
