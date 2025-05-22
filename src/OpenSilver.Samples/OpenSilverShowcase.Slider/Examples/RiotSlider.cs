using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;

namespace OpenSilverShowcase.Slider.Examples
{
    public class RiotSlider : System.Windows.Controls.Slider
    {
        public RiotSlider()
        {
            DefaultStyleKey = typeof(RiotSlider);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Thumb thumb = (Thumb)GetTemplateChild("HorizontalThumb");
            thumb.DragStarted += Thumb_DragStarted;
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Pressed", true);
        }
    }
}
