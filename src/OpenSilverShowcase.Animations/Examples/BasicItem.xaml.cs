using OpenSilverShowcase.Support.UI.Units;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace OpenSilverShowcase.Animations.Examples
{
    public partial class BasicItem : ShowcaseItem
    {
        private bool isScaled = false;

        public BasicItem()
        {
            this.InitializeComponent();
        }

        private void OnAnimateClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var scaleXAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(800),
                To = isScaled ? 1.0 : 1.5,
                EasingFunction = new BounceEase
                {
                    EasingMode = EasingMode.EaseOut,
                    Bounces = 3,
                    Bounciness = 2
                }
            };

            var scaleYAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(800),
                To = isScaled ? 1.0 : 1.5,
                EasingFunction = new BounceEase
                {
                    EasingMode = EasingMode.EaseOut,
                    Bounces = 3,
                    Bounciness = 2
                }
            };

            Storyboard.SetTarget(scaleXAnimation, ElementTransform);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("ScaleX"));

            Storyboard.SetTarget(scaleYAnimation, ElementTransform);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("ScaleY"));

            var storyboard = new Storyboard();
            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);

            button.Content = isScaled ? "Animate" : "Reset";
            isScaled = !isScaled;

            storyboard.Begin();
        }
    }
}