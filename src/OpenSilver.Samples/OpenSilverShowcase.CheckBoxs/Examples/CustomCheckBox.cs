using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenSilverShowcase.CheckBoxs.Examples
{
    public class CustomCheckBox : CheckBox
    {
        static CustomCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomCheckBox),
                new FrameworkPropertyMetadata(typeof(CustomCheckBox)));
        }

        #region CheckedBackground
        public static readonly DependencyProperty CheckedBackgroundProperty =
            DependencyProperty.Register(nameof(CheckedBackground), typeof(Brush), typeof(CustomCheckBox),
                new PropertyMetadata(new SolidColorBrush(Colors.DodgerBlue)));

        public Brush CheckedBackground
        {
            get => (Brush)GetValue(CheckedBackgroundProperty);
            set => SetValue(CheckedBackgroundProperty, value);
        }
        #endregion

        #region CheckedForeground
        public static readonly DependencyProperty CheckedForegroundProperty =
            DependencyProperty.Register(nameof(CheckedForeground), typeof(Brush), typeof(CustomCheckBox),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush CheckedForeground
        {
            get => (Brush)GetValue(CheckedForegroundProperty);
            set => SetValue(CheckedForegroundProperty, value);
        }
        #endregion

        #region ToggleStyle
        public static readonly DependencyProperty ToggleStyleProperty =
            DependencyProperty.Register(nameof(ToggleStyle), typeof(ToggleStyleType), typeof(CustomCheckBox),
                new PropertyMetadata(ToggleStyleType.Default));

        public ToggleStyleType ToggleStyle
        {
            get => (ToggleStyleType)GetValue(ToggleStyleProperty);
            set => SetValue(ToggleStyleProperty, value);
        }
        #endregion

        #region AnimationDuration
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(nameof(AnimationDuration), typeof(Duration), typeof(CustomCheckBox),
                new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(200))));

        public Duration AnimationDuration
        {
            get => (Duration)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }
        #endregion

        #region IconSize
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(CustomCheckBox),
                new PropertyMetadata(16.0));

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }
        #endregion

        #region CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(CustomCheckBox),
                new PropertyMetadata(new CornerRadius(4)));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        #endregion

        #region CheckIcon
        public static readonly DependencyProperty CheckIconProperty =
            DependencyProperty.Register(nameof(CheckIcon), typeof(string), typeof(CustomCheckBox),
                new PropertyMetadata("✓"));

        public string CheckIcon
        {
            get => (string)GetValue(CheckIconProperty);
            set => SetValue(CheckIconProperty, value);
        }
        #endregion

        #region EnableRippleEffect
        public static readonly DependencyProperty EnableRippleEffectProperty =
            DependencyProperty.Register(nameof(EnableRippleEffect), typeof(bool), typeof(CustomCheckBox),
                new PropertyMetadata(true));

        public bool EnableRippleEffect
        {
            get => (bool)GetValue(EnableRippleEffectProperty);
            set => SetValue(EnableRippleEffectProperty, value);
        }
        #endregion

        #region Description
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(CustomCheckBox),
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
        Modern
    }
}