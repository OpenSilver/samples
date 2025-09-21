using OpenSilverShowcase.Support.UI.Units;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenSilverShowcase.RenderTransforms.Examples
{
    public partial class RenderItem : ShowcaseItem
    {
        public RenderItem()
        {
            this.InitializeComponent();
        }

        // Scale Transform Hover Effects
        private void OnScaleMouseEnter(object sender, MouseEventArgs e)
        {
            var element = sender as FrameworkElement;
            var scaleTransform = new ScaleTransform { ScaleX = 1.4, ScaleY = 1.4 };
            element.RenderTransform = scaleTransform;
            element.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private void OnScaleMouseLeave(object sender, MouseEventArgs e)
        {
            var element = sender as FrameworkElement;
            element.RenderTransform = null;
        }

        // Rotate Transform Hover Effects
        private void OnRotateMouseEnter(object sender, MouseEventArgs e)
        {
            var element = sender as FrameworkElement;
            var rotateTransform = new RotateTransform { Angle = 45 };
            element.RenderTransform = rotateTransform;
        }

        private void OnRotateMouseLeave(object sender, MouseEventArgs e)
        {
            var element = sender as FrameworkElement;
            element.RenderTransform = null;
        }

        // Translate Transform Hover Effects
        private void OnTranslateMouseEnter(object sender, MouseEventArgs e)
        {
            var element = sender as FrameworkElement;
            var translateTransform = new TranslateTransform { X = 15, Y = 0 };
            element.RenderTransform = translateTransform;
        }

        private void OnTranslateMouseLeave(object sender, MouseEventArgs e)
        {
            var element = sender as FrameworkElement;
            element.RenderTransform = null;
        }

        // Composite Transform Hover Effects
        private void OnCompositeMouseEnter(object sender, MouseEventArgs e)
        {
            var element = sender as FrameworkElement;
            var compositeTransform = new CompositeTransform
            {
                TranslateX = 10,
                Rotation = 20,
                ScaleX = 1.2,
                ScaleY = 1.2
            };
            element.RenderTransform = compositeTransform;
        }

        private void OnCompositeMouseLeave(object sender, MouseEventArgs e)
        {
            var element = sender as FrameworkElement;
            element.RenderTransform = null;
        }

        // Live Transform Controls
        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LiveTransform != null)
            {
                LiveTransform.ScaleX = ScaleSlider.Value;
                LiveTransform.ScaleY = ScaleSlider.Value;
                LiveTransform.Rotation = RotationSlider.Value;
                LiveTransform.TranslateX = TranslateXSlider.Value;
                LiveTransform.TranslateY = TranslateYSlider.Value;
            }
        }
    }
}