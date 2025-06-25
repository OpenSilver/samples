using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace OpenSilverShowcase.ToggleButtons.Examples
{
    public class CustomToggleButton : ToggleButton
    {
        static CustomToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomToggleButton),
                new FrameworkPropertyMetadata(typeof(CustomToggleButton)));
        }

        #region CheckedBackground
        public static readonly DependencyProperty CheckedBackgroundProperty =
            DependencyProperty.Register(nameof(CheckedBackground), typeof(Brush), typeof(CustomToggleButton),
                new PropertyMetadata(new SolidColorBrush(Colors.DodgerBlue)));

        public Brush CheckedBackground
        {
            get => (Brush)GetValue(CheckedBackgroundProperty);
            set => SetValue(CheckedBackgroundProperty, value);
        }
        #endregion

        #region CheckedForeground
        public static readonly DependencyProperty CheckedForegroundProperty =
            DependencyProperty.Register(nameof(CheckedForeground), typeof(Brush), typeof(CustomToggleButton),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush CheckedForeground
        {
            get => (Brush)GetValue(CheckedForegroundProperty);
            set => SetValue(CheckedForegroundProperty, value);
        }
        #endregion

        #region UncheckedBackground
        public static readonly DependencyProperty UncheckedBackgroundProperty =
            DependencyProperty.Register(nameof(UncheckedBackground), typeof(Brush), typeof(CustomToggleButton),
                new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public Brush UncheckedBackground
        {
            get => (Brush)GetValue(UncheckedBackgroundProperty);
            set => SetValue(UncheckedBackgroundProperty, value);
        }
        #endregion

        #region UncheckedForeground
        public static readonly DependencyProperty UncheckedForegroundProperty =
            DependencyProperty.Register(nameof(UncheckedForeground), typeof(Brush), typeof(CustomToggleButton),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush UncheckedForeground
        {
            get => (Brush)GetValue(UncheckedForegroundProperty);
            set => SetValue(UncheckedForegroundProperty, value);
        }
        #endregion

        #region ToggleStyle
        public static readonly DependencyProperty ToggleStyleProperty =
            DependencyProperty.Register(nameof(ToggleStyle), typeof(ToggleStyleType), typeof(CustomToggleButton),
                new PropertyMetadata(ToggleStyleType.Default));

        public ToggleStyleType ToggleStyle
        {
            get => (ToggleStyleType)GetValue(ToggleStyleProperty);
            set => SetValue(ToggleStyleProperty, value);
        }
        #endregion

        #region AnimationDuration
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(nameof(AnimationDuration), typeof(Duration), typeof(CustomToggleButton),
                new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(200))));

        public Duration AnimationDuration
        {
            get => (Duration)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }
        #endregion

        #region CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(CustomToggleButton),
                new PropertyMetadata(new CornerRadius(4)));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        #endregion

        #region IconData
        public static readonly DependencyProperty IconDataProperty =
            DependencyProperty.Register(nameof(IconData), typeof(Geometry), typeof(CustomToggleButton),
                new PropertyMetadata(null));

        public Geometry IconData
        {
            get => (Geometry)GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }
        #endregion

        #region IconWidth
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(CustomToggleButton),
                new PropertyMetadata(16.0));

        public double IconWidth
        {
            get => (double)GetValue(IconWidthProperty);
            set => SetValue(IconWidthProperty, value);
        }
        #endregion

        #region IconHeight
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(CustomToggleButton),
                new PropertyMetadata(16.0));

        public double IconHeight
        {
            get => (double)GetValue(IconHeightProperty);
            set => SetValue(IconHeightProperty, value);
        }
        #endregion

        #region IconMargin
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register(nameof(IconMargin), typeof(Thickness), typeof(CustomToggleButton),
                new PropertyMetadata(new Thickness(0, 0, 8, 0)));

        public Thickness IconMargin
        {
            get => (Thickness)GetValue(IconMarginProperty);
            set => SetValue(IconMarginProperty, value);
        }
        #endregion

        #region Description
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(CustomToggleButton),
                new PropertyMetadata(string.Empty));

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
        #endregion
    }

    public enum ToggleStyleType
    {
        Default,
        Switch,
        Card,
        Pill,
        Modern,
        Icon
    }
}