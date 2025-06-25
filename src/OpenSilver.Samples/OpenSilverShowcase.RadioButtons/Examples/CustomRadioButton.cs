using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace OpenSilverShowcase.RadioButtons.Examples
{
    public class CustomRadioButton : RadioButton
    {
        public CustomRadioButton()
        {
            DefaultStyleKey = typeof(CustomRadioButton);
        }

        public Brush CircleBorderBrush
        {
            get => (Brush)GetValue(CircleBorderBrushProperty);
            set => SetValue(CircleBorderBrushProperty, value);
        }
        public static readonly DependencyProperty CircleBorderBrushProperty =
            DependencyProperty.Register(nameof(CircleBorderBrush), typeof(Brush), typeof(CustomRadioButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 209, 213, 219)))); // #D1D5DB

        public Brush CircleBorderBrushChecked
        {
            get => (Brush)GetValue(CircleBorderBrushCheckedProperty);
            set => SetValue(CircleBorderBrushCheckedProperty, value);
        }
        public static readonly DependencyProperty CircleBorderBrushCheckedProperty =
            DependencyProperty.Register(nameof(CircleBorderBrushChecked), typeof(Brush), typeof(CustomRadioButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 99, 102, 241)))); // #6366F1

        public Brush CircleBorderBrushHover
        {
            get => (Brush)GetValue(CircleBorderBrushHoverProperty);
            set => SetValue(CircleBorderBrushHoverProperty, value);
        }
        public static readonly DependencyProperty CircleBorderBrushHoverProperty =
            DependencyProperty.Register(nameof(CircleBorderBrushHover), typeof(Brush), typeof(CustomRadioButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 99, 102, 241)))); // #6366F1

        public Brush CircleFill
        {
            get => (Brush)GetValue(CircleFillProperty);
            set => SetValue(CircleFillProperty, value);
        }
        public static readonly DependencyProperty CircleFillProperty =
            DependencyProperty.Register(nameof(CircleFill), typeof(Brush), typeof(CustomRadioButton),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush CircleFillHover
        {
            get => (Brush)GetValue(CircleFillHoverProperty);
            set => SetValue(CircleFillHoverProperty, value);
        }
        public static readonly DependencyProperty CircleFillHoverProperty =
            DependencyProperty.Register(nameof(CircleFillHover), typeof(Brush), typeof(CustomRadioButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 243, 244, 246)))); // #F3F4F6

        public Brush CheckedDotFill
        {
            get => (Brush)GetValue(CheckedDotFillProperty);
            set => SetValue(CheckedDotFillProperty, value);
        }
        public static readonly DependencyProperty CheckedDotFillProperty =
            DependencyProperty.Register(nameof(CheckedDotFill), typeof(Brush), typeof(CustomRadioButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 99, 102, 241)))); // #6366F1

        public double CircleSize
        {
            get => (double)GetValue(CircleSizeProperty);
            set => SetValue(CircleSizeProperty, value);
        }
        public static readonly DependencyProperty CircleSizeProperty =
            DependencyProperty.Register(nameof(CircleSize), typeof(double), typeof(CustomRadioButton),
                new PropertyMetadata(24.0));

        public double DotSize
        {
            get => (double)GetValue(DotSizeProperty);
            set => SetValue(DotSizeProperty, value);
        }
        public static readonly DependencyProperty DotSizeProperty =
            DependencyProperty.Register(nameof(DotSize), typeof(double), typeof(CustomRadioButton),
                new PropertyMetadata(12.0));

        public Brush BackgroundChecked
        {
            get => (Brush)GetValue(BackgroundCheckedProperty);
            set => SetValue(BackgroundCheckedProperty, value);
        }
        public static readonly DependencyProperty BackgroundCheckedProperty =
            DependencyProperty.Register(nameof(BackgroundChecked), typeof(Brush), typeof(CustomRadioButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(25, 99, 102, 241)))); // #6366F1 with 10% opacity

        public Brush BackgroundHover
        {
            get => (Brush)GetValue(BackgroundHoverProperty);
            set => SetValue(BackgroundHoverProperty, value);
        }
        public static readonly DependencyProperty BackgroundHoverProperty =
            DependencyProperty.Register(nameof(BackgroundHover), typeof(Brush), typeof(CustomRadioButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(13, 99, 102, 241)))); // #6366F1 with 5% opacity

        public Brush BorderBrushChecked
        {
            get => (Brush)GetValue(BorderBrushCheckedProperty);
            set => SetValue(BorderBrushCheckedProperty, value);
        }
        public static readonly DependencyProperty BorderBrushCheckedProperty =
            DependencyProperty.Register(nameof(BorderBrushChecked), typeof(Brush), typeof(CustomRadioButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 99, 102, 241)))); // #6366F1
        
        public Brush BorderBrushHover
        {
            get => (Brush)GetValue(BorderBrushHoverProperty);
            set => SetValue(BorderBrushHoverProperty, value);
        }
        public static readonly DependencyProperty BorderBrushHoverProperty =
            DependencyProperty.Register(nameof(BorderBrushHover), typeof(Brush), typeof(CustomRadioButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 99, 102, 241)))); // #6366F1 with 50% opacity

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(CustomRadioButton),
                new PropertyMetadata(new CornerRadius(8)));

        public double IconTextSpacing
        {
            get => (double)GetValue(IconTextSpacingProperty);
            set => SetValue(IconTextSpacingProperty, value);
        }
        public static readonly DependencyProperty IconTextSpacingProperty =
            DependencyProperty.Register(nameof(IconTextSpacing), typeof(double), typeof(CustomRadioButton),
                new PropertyMetadata(8.0));

        public Duration AnimationDuration
        {
            get => (Duration)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(nameof(AnimationDuration), typeof(Duration), typeof(CustomRadioButton),
                new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(200))));
    }
}