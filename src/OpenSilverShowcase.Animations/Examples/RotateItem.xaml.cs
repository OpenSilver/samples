using OpenSilverShowcase.Support.UI.Units;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace OpenSilverShowcase.Animations.Examples
{
    public partial class RotateItem : ShowcaseItem
    {
        private bool isRotated = false;

        public RotateItem()
        {
            this.InitializeComponent();
        }

        private void OnAnimateClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var rotateAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(1000),
                To = isRotated ? 0 : 360,
                EasingFunction = new CubicEase
                {
                    EasingMode = EasingMode.EaseInOut
                }
            };

            Storyboard.SetTarget(rotateAnimation, ElementTransform);
            Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("Rotation"));

            var storyboard = new Storyboard();
            storyboard.Children.Add(rotateAnimation);

            button.Content = isRotated ? "Rotate" : "Reset";
            isRotated = !isRotated;

            storyboard.Begin();
        }
    }
}
