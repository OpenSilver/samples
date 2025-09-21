using OpenSilverShowcase.Support.UI.Units;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace OpenSilverShowcase.Animations.Examples
{
    public partial class QuarticEaseItem : ShowcaseItem
    {
        private bool isAnimated = false;

        public QuarticEaseItem()
        {
            this.InitializeComponent();
        }

        private void OnAnimateClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var slideAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(1),
                To = isAnimated ? 0 : 200,
                EasingFunction = new QuarticEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            Storyboard.SetTarget(slideAnimation, ElementTransform);
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("TranslateX"));

            var storyboard = new Storyboard();
            storyboard.Children.Add(slideAnimation);

            button.Content = isAnimated ? "Slide" : "Reset";
            isAnimated = !isAnimated;

            storyboard.Begin();
        }
    }
}