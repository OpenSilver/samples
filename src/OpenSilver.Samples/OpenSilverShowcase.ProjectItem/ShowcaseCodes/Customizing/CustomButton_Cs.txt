using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenSilverShowcase.ProjectItem.Customizing
{
    public class CustomButton : Button
    {
        #region Dependency Properties
        public static readonly DependencyProperty IconDataProperty =
            DependencyProperty.Register(
                "IconData",
                typeof(Geometry),
                typeof(CustomButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register(
                "IconWidth",
                typeof(double),
                typeof(CustomButton),
                new PropertyMetadata(16.0));

        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register(
                "IconHeight",
                typeof(double),
                typeof(CustomButton),
                new PropertyMetadata(16.0));

        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register(
                "IconMargin",
                typeof(Thickness),
                typeof(CustomButton),
                new PropertyMetadata(new Thickness(0, 0, 4, 0)));

        public static readonly DependencyProperty IconFillProperty =
            DependencyProperty.Register(
                "IconFill",
                typeof(Brush),
                typeof(CustomButton),
                new PropertyMetadata(null));
        #endregion

        #region Properties
        public Geometry IconData
        {
            get { return (Geometry)GetValue(IconDataProperty); }
            set { SetValue(IconDataProperty, value); }
        }

        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }

        public Brush IconFill
        {
            get { return (Brush)GetValue(IconFillProperty); }
            set { SetValue(IconFillProperty, value); }
        }
        #endregion

        public CustomButton()
        {
            DefaultStyleKey = typeof(CustomButton);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateButtonContent();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomButton button)
            {
                button.UpdateButtonContent();
            }
        }

        private void UpdateButtonContent()
        {
        }
    }
}