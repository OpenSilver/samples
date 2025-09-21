using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace OpenSilverShowcase.CustomPanels.Examples
{
    public class RadialPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement elem in Children)
            {
                elem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return finalSize;

            double _angle = 0;

            double _incrementalAngularSpace = (360.0 / Children.Count) * (Math.PI / 180);

            double radiusX = finalSize.Width / 2.4;
            double radiusY = finalSize.Height / 2.4;

            foreach (UIElement elem in Children)
            {

                Point childPoint = new Point(Math.Cos(_angle) * radiusX, -Math.Sin(_angle) * radiusY);
                Point actualChildPoint = new Point(finalSize.Width / 2 + childPoint.X - elem.DesiredSize.Width / 2, finalSize.Height / 2 + childPoint.Y - elem.DesiredSize.Height / 2);

                elem.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, elem.DesiredSize.Width, elem.DesiredSize.Height));


                _angle += _incrementalAngularSpace;
            }
            return finalSize;
        }
    }
}

