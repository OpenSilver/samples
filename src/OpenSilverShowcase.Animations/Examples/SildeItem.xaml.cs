using OpenSilverShowcase.Support.UI.Units;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace OpenSilverShowcase.Animations.Examples
{
    public partial class SildeItem : ShowcaseItem
    {
        private bool isSlided = false;

        public SildeItem()
        {
            this.InitializeComponent();
        }

        private void OnAnimateClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var slideAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(800),
                To = isSlided ? 0 : 200,
                EasingFunction = new BackEase
                {
                    EasingMode = EasingMode.EaseOut,
                    Amplitude = 0.3
                }
            };

            Storyboard.SetTarget(slideAnimation, ElementTransform);
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("TranslateX"));

            var storyboard = new Storyboard();
            storyboard.Children.Add(slideAnimation);

            button.Content = isSlided ? "Slide" : "Reset";
            isSlided = !isSlided;

            storyboard.Begin();
        }
    }
}