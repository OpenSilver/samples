using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace OpenSilverShowcase.TextBoxs.Examples
{
    public class CustomTextBox : TextBox
    {
        public CustomTextBox()
        {
            DefaultStyleKey = typeof(CustomTextBox);
            Loaded += CustomTextBox_Loaded;
            TextChanged += CustomTextBox_TextChanged;
        }

        // Custom focus tracking
        public static readonly DependencyProperty IsControlFocusedProperty =
            DependencyProperty.Register("IsControlFocused", typeof(bool), typeof(CustomTextBox),
                new PropertyMetadata(false, OnIsControlFocusedChanged));

        public bool IsControlFocused
        {
            get => (bool)GetValue(IsControlFocusedProperty);
            set => SetValue(IsControlFocusedProperty, value);
        }

        private static void OnIsControlFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomTextBox textBox)
            {
                textBox.UpdateVisualStates();
            }
        }

        // Other Dependency Properties
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(CustomTextBox), new PropertyMetadata(new CornerRadius(12)));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(CustomTextBox), new PropertyMetadata(string.Empty));

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(CustomTextBox), new PropertyMetadata(string.Empty));

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public static readonly DependencyProperty ShowSearchIconProperty =
            DependencyProperty.Register("ShowSearchIcon", typeof(bool), typeof(CustomTextBox), new PropertyMetadata(false));

        public bool ShowSearchIcon
        {
            get => (bool)GetValue(ShowSearchIconProperty);
            set => SetValue(ShowSearchIconProperty, value);
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(CustomTextBox), new PropertyMetadata(string.Empty));

        public string IconPath
        {
            get => (string)GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value);
        }

        public static readonly DependencyProperty IconColorProperty =
            DependencyProperty.Register("IconColor", typeof(Brush), typeof(CustomTextBox), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public Brush IconColor
        {
            get => (Brush)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        }

        private void CustomTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateVisualStates();
        }

        private void CustomTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateVisualStates();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            IsControlFocused = true;
            VisualStateManager.GoToState(this, "Focused", true);
            // 포커스를 받으면 즉시 플레이스홀더 숨기기
            UpdatePlaceholderVisibility();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            IsControlFocused = false;
            VisualStateManager.GoToState(this, "Normal", true);
            // 포커스를 잃으면 텍스트 상태에 따라 플레이스홀더 표시/숨기기
            UpdatePlaceholderVisibility();
        }

        private void UpdateVisualStates()
        {
            // 텍스트 상태만 업데이트 (포커스와 분리)
            bool hasText = !string.IsNullOrEmpty(Text);
            VisualStateManager.GoToState(this, hasText ? "HasText" : "EmptyText", true);

            // 플레이스홀더 가시성도 별도로 업데이트
            UpdatePlaceholderVisibility();

            // 기타 요소들의 가시성 업데이트
            UpdateElementsVisibility();
        }

        private void UpdatePlaceholderVisibility()
        {
            if (Template?.FindName("PlaceholderTextBlock", this) is FrameworkElement placeholderElement)
            {
                // 텍스트가 있거나 포커스가 있으면 플레이스홀더 숨기기
                bool shouldHidePlaceholder = !string.IsNullOrEmpty(Text) || IsControlFocused;
                double targetOpacity = shouldHidePlaceholder ? 0 : 0.7;

                // 부드러운 애니메이션으로 opacity 변경
                AnimateOpacity(placeholderElement, targetOpacity, TimeSpan.FromMilliseconds(150));
            }
        }

        private void AnimateOpacity(FrameworkElement element, double targetOpacity, TimeSpan duration)
        {
            var animation = new DoubleAnimation
            {
                To = targetOpacity,
                Duration = duration,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            element.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void UpdateElementsVisibility()
        {
            if (Template != null)
            {
                if (Template.FindName("HeaderLabel", this) is FrameworkElement headerLabel)
                {
                    headerLabel.Visibility = string.IsNullOrEmpty(HeaderText) ? Visibility.Collapsed : Visibility.Visible;
                }
                if (Template.FindName("CustomIcon", this) is FrameworkElement customIcon)
                {
                    customIcon.Visibility = string.IsNullOrEmpty(IconPath) ? Visibility.Collapsed : Visibility.Visible;
                }
                if (Template.FindName("SearchIcon", this) is FrameworkElement searchIcon)
                {
                    searchIcon.Visibility = ShowSearchIcon ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
    }
}