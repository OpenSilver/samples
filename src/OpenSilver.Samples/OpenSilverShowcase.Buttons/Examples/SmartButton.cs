using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Input;

namespace OpenSilverShowcase.Buttons.Examples
{
    public enum IconPosition
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public enum ContentAlignment
    {
        Left,
        Top,
        Right,
        Bottom,
        Center,
        Stretch
    }

    public enum DisplayMode
    {
        IconAndText,
        IconOnly,
        TextOnly
    }

    public class SmartButton : Button
    {
        private Border _rootBorder;
        private Grid _contentGrid;
        private Path _iconPath;
        private TextBlock _textBlock;
        private DropShadowEffect _shadowEffect;
        private bool _isHovered;
        private bool _isPressed;
        private bool _templateApplied;
        private Thickness _originalBorderThickness;
        private TransformGroup _transformGroup;
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;

        #region Dependency Properties
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(SmartButton),
                new PropertyMetadata(new CornerRadius(0), OnCornerRadiusChanged));

        public static readonly DependencyProperty IconGeometryProperty =
            DependencyProperty.Register("IconGeometry", typeof(Geometry), typeof(SmartButton),
                new PropertyMetadata(null, OnIconGeometryChanged));

        public static readonly DependencyProperty IconPositionProperty =
            DependencyProperty.Register("IconPosition", typeof(IconPosition), typeof(SmartButton),
                new PropertyMetadata(IconPosition.Left, OnLayoutPropertyChanged));

        public static readonly DependencyProperty IconAlignmentProperty =
            DependencyProperty.Register("IconAlignment", typeof(ContentAlignment), typeof(SmartButton),
                new PropertyMetadata(ContentAlignment.Center, OnLayoutPropertyChanged));

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(ContentAlignment), typeof(SmartButton),
                new PropertyMetadata(ContentAlignment.Center, OnLayoutPropertyChanged));

        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(DisplayMode), typeof(SmartButton),
                new PropertyMetadata(DisplayMode.IconAndText, OnDisplayModeChanged));

        public static readonly DependencyProperty HasShadowProperty =
            DependencyProperty.Register("HasShadow", typeof(bool), typeof(SmartButton),
                new PropertyMetadata(false, OnShadowChanged));

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(double), typeof(SmartButton),
                new PropertyMetadata(16.0, OnIconSizeChanged));

        public static readonly DependencyProperty IconBrushProperty =
            DependencyProperty.Register("IconBrush", typeof(Brush), typeof(SmartButton),
                new PropertyMetadata(new SolidColorBrush(Colors.Black), OnIconBrushChanged));

        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(SmartButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty HoverBorderBrushProperty =
            DependencyProperty.Register("HoverBorderBrush", typeof(Brush), typeof(SmartButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty HoverForegroundProperty =
            DependencyProperty.Register("HoverForeground", typeof(Brush), typeof(SmartButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty HoverIconBrushProperty =
            DependencyProperty.Register("HoverIconBrush", typeof(Brush), typeof(SmartButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(SmartButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty PressedBorderBrushProperty =
            DependencyProperty.Register("PressedBorderBrush", typeof(Brush), typeof(SmartButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty PressedForegroundProperty =
            DependencyProperty.Register("PressedForeground", typeof(Brush), typeof(SmartButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty PressedIconBrushProperty =
            DependencyProperty.Register("PressedIconBrush", typeof(Brush), typeof(SmartButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(TimeSpan), typeof(SmartButton),
                new PropertyMetadata(TimeSpan.FromMilliseconds(150)));

        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(SmartButton),
                new PropertyMetadata(new Thickness(0), OnLayoutPropertyChanged));

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(SmartButton),
                new PropertyMetadata(new Thickness(0), OnLayoutPropertyChanged));

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(SmartButton),
                new PropertyMetadata("", OnButtonTextChanged));

        public static readonly DependencyProperty IconTextSpacingProperty =
            DependencyProperty.Register("IconTextSpacing", typeof(double), typeof(SmartButton),
                new PropertyMetadata(8.0, OnLayoutPropertyChanged));

        public static readonly DependencyProperty ContentPaddingProperty =
            DependencyProperty.Register("ContentPadding", typeof(Thickness), typeof(SmartButton),
                new PropertyMetadata(new Thickness(12, 6, 12, 6), OnLayoutPropertyChanged));

        public static readonly DependencyProperty PressedScaleProperty =
            DependencyProperty.Register("PressedScale", typeof(double), typeof(SmartButton),
                new PropertyMetadata(1));

        public static readonly DependencyProperty PressedOffsetProperty =
            DependencyProperty.Register("PressedOffset", typeof(Point), typeof(SmartButton),
                new PropertyMetadata(new Point(0, 0)));
        #endregion

        #region Properties
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Geometry IconGeometry
        {
            get => (Geometry)GetValue(IconGeometryProperty);
            set => SetValue(IconGeometryProperty, value);
        }

        public IconPosition IconPosition
        {
            get => (IconPosition)GetValue(IconPositionProperty);
            set => SetValue(IconPositionProperty, value);
        }

        public ContentAlignment IconAlignment
        {
            get => (ContentAlignment)GetValue(IconAlignmentProperty);
            set => SetValue(IconAlignmentProperty, value);
        }

        public ContentAlignment TextAlignment
        {
            get => (ContentAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        public DisplayMode DisplayMode
        {
            get => (DisplayMode)GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }

        public bool HasShadow
        {
            get => (bool)GetValue(HasShadowProperty);
            set => SetValue(HasShadowProperty, value);
        }

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public Brush IconBrush
        {
            get => (Brush)GetValue(IconBrushProperty);
            set => SetValue(IconBrushProperty, value);
        }

        public Brush HoverBackground
        {
            get => (Brush)GetValue(HoverBackgroundProperty);
            set => SetValue(HoverBackgroundProperty, value);
        }

        public Brush HoverBorderBrush
        {
            get => (Brush)GetValue(HoverBorderBrushProperty);
            set => SetValue(HoverBorderBrushProperty, value);
        }

        public Brush HoverForeground
        {
            get => (Brush)GetValue(HoverForegroundProperty);
            set => SetValue(HoverForegroundProperty, value);
        }

        public Brush HoverIconBrush
        {
            get => (Brush)GetValue(HoverIconBrushProperty);
            set => SetValue(HoverIconBrushProperty, value);
        }

        public Brush PressedBackground
        {
            get => (Brush)GetValue(PressedBackgroundProperty);
            set => SetValue(PressedBackgroundProperty, value);
        }

        public Brush PressedBorderBrush
        {
            get => (Brush)GetValue(PressedBorderBrushProperty);
            set => SetValue(PressedBorderBrushProperty, value);
        }

        public Brush PressedForeground
        {
            get => (Brush)GetValue(PressedForegroundProperty);
            set => SetValue(PressedForegroundProperty, value);
        }

        public Brush PressedIconBrush
        {
            get => (Brush)GetValue(PressedIconBrushProperty);
            set => SetValue(PressedIconBrushProperty, value);
        }

        public TimeSpan AnimationDuration
        {
            get => (TimeSpan)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }

        public Thickness IconMargin
        {
            get => (Thickness)GetValue(IconMarginProperty);
            set => SetValue(IconMarginProperty, value);
        }

        public Thickness TextMargin
        {
            get => (Thickness)GetValue(TextMarginProperty);
            set => SetValue(TextMarginProperty, value);
        }

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public double IconTextSpacing
        {
            get => (double)GetValue(IconTextSpacingProperty);
            set => SetValue(IconTextSpacingProperty, value);
        }

        public Thickness ContentPadding
        {
            get => (Thickness)GetValue(ContentPaddingProperty);
            set => SetValue(ContentPaddingProperty, value);
        }

        public double PressedScale
        {
            get => (double)GetValue(PressedScaleProperty);
            set => SetValue(PressedScaleProperty, value);
        }

        public Point PressedOffset
        {
            get => (Point)GetValue(PressedOffsetProperty);
            set => SetValue(PressedOffsetProperty, value);
        }
        #endregion

        public SmartButton()
        {
            DefaultStyleKey = typeof(SmartButton);
            // Initialize transforms
            _scaleTransform = new ScaleTransform();
            _scaleTransform.ScaleX = 1.0;
            _scaleTransform.ScaleY = 1.0;
            _translateTransform = new TranslateTransform();
            _translateTransform.X = 0;
            _translateTransform.Y = 0;
            _transformGroup = new TransformGroup();
            _transformGroup.Children.Add(_scaleTransform);
            _transformGroup.Children.Add(_translateTransform);

            SetupEventHandlers();
            Loaded += SmartButton_Loaded;
            // Register MouseDown, MouseUp, and MouseEnter handlers with forced event handling
            AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
            AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseUp), true);
            AddHandler(UIElement.MouseEnterEvent, new MouseEventHandler(OnMouseEnter), true);
            AddHandler(UIElement.MouseLeaveEvent, new MouseEventHandler(OnMouseLeave), true);
        }

        private void SmartButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_templateApplied)
            {
                CreateTemplate();
            }
        }

        private void CreateTemplate()
        {
            _originalBorderThickness = BorderThickness.Left > 0 ? BorderThickness : new Thickness(1);

            // Create Point for RenderTransformOrigin explicitly
            Point transformOrigin = new Point();
            transformOrigin.X = 0.5;
            transformOrigin.Y = 0.5;

            _rootBorder = new Border
            {
                CornerRadius = CornerRadius,
                Background = Background ?? new SolidColorBrush(Colors.LightGray),
                BorderBrush = BorderBrush ?? new SolidColorBrush(Colors.Gray),
                BorderThickness = _originalBorderThickness,
                Padding = ContentPadding,
                RenderTransform = _transformGroup,
                RenderTransformOrigin = transformOrigin
            };

            _contentGrid = new Grid();

            _iconPath = new Path
            {
                Data = IconGeometry,
                Fill = IconBrush,
                Width = IconSize,
                Height = IconSize,
                Stretch = Stretch.Uniform,
                Margin = IconMargin
            };

            var textContent = !string.IsNullOrEmpty(ButtonText) ? ButtonText : (Content?.ToString() ?? "");
            _textBlock = new TextBlock
            {
                Text = textContent,
                Foreground = Foreground ?? new SolidColorBrush(Colors.Black),
                FontFamily = FontFamily,
                FontSize = FontSize,
                FontWeight = FontWeight,
                Margin = TextMargin
            };

            _shadowEffect = new DropShadowEffect
            {
                BlurRadius = 8,
                Direction = 315,
                ShadowDepth = 2,
                Opacity = 0.3,
                Color = Colors.Black
            };

            _rootBorder.Child = _contentGrid;
            Content = _rootBorder;

            UpdateLayout();
            _templateApplied = true;
        }

        private void UpdateLayout()
        {
            if (_contentGrid == null) return;

            _contentGrid.Children.Clear();
            _contentGrid.RowDefinitions.Clear();
            _contentGrid.ColumnDefinitions.Clear();

            switch (DisplayMode)
            {
                case DisplayMode.IconOnly:
                    SetupIconOnlyLayout();
                    break;
                case DisplayMode.TextOnly:
                    SetupTextOnlyLayout();
                    break;
                case DisplayMode.IconAndText:
                    SetupIconAndTextLayout();
                    break;
            }

            UpdateShadow();
        }

        private void SetupIconOnlyLayout()
        {
            if (_iconPath != null && _iconPath.Data != null)
            {
                SetAlignment(_iconPath, IconAlignment);
                _contentGrid.Children.Add(_iconPath);
            }
        }

        private void SetupTextOnlyLayout()
        {
            if (_textBlock != null)
            {
                SetAlignment(_textBlock, TextAlignment);
                _contentGrid.Children.Add(_textBlock);
            }
        }

        private void SetupIconAndTextLayout()
        {
            if (_iconPath?.Data == null)
            {
                SetupTextOnlyLayout();
                return;
            }

            switch (IconPosition)
            {
                case IconPosition.Left:
                    _contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    _contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    Grid.SetColumn(_iconPath, 0);
                    Grid.SetColumn(_textBlock, 1);
                    _iconPath.Margin = new Thickness(IconMargin.Left, IconMargin.Top, IconMargin.Right + IconTextSpacing, IconMargin.Bottom);
                    _textBlock.Margin = TextMargin;
                    SetAlignment(_iconPath, IconAlignment);
                    SetAlignment(_textBlock, TextAlignment);
                    break;

                case IconPosition.Right:
                    _contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    _contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    Grid.SetColumn(_textBlock, 0);
                    Grid.SetColumn(_iconPath, 1);
                    _textBlock.Margin = new Thickness(TextMargin.Left, TextMargin.Top, TextMargin.Right + IconTextSpacing, TextMargin.Bottom);
                    _iconPath.Margin = IconMargin;
                    SetAlignment(_textBlock, TextAlignment);
                    SetAlignment(_iconPath, IconAlignment);
                    break;

                case IconPosition.Top:
                    _contentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    _contentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    Grid.SetRow(_iconPath, 0);
                    Grid.SetRow(_textBlock, 1);
                    _iconPath.Margin = new Thickness(IconMargin.Left, IconMargin.Top, IconMargin.Right, IconMargin.Bottom + IconTextSpacing);
                    _textBlock.Margin = TextMargin;
                    SetAlignment(_iconPath, IconAlignment);
                    SetAlignment(_textBlock, TextAlignment);
                    break;

                case IconPosition.Bottom:
                    _contentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    _contentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    Grid.SetRow(_textBlock, 0);
                    Grid.SetRow(_iconPath, 1);
                    _textBlock.Margin = new Thickness(TextMargin.Left, TextMargin.Top, TextMargin.Right, TextMargin.Bottom + IconTextSpacing);
                    _iconPath.Margin = IconMargin;
                    SetAlignment(_textBlock, TextAlignment);
                    SetAlignment(_iconPath, IconAlignment);
                    break;
            }

            if (_iconPath != null) _contentGrid.Children.Add(_iconPath);
            if (_textBlock != null) _contentGrid.Children.Add(_textBlock);
        }

        private void SetAlignment(FrameworkElement element, ContentAlignment alignment)
        {
            if (element == null) return;

            switch (alignment)
            {
                case ContentAlignment.Left:
                    element.HorizontalAlignment = HorizontalAlignment.Left;
                    element.VerticalAlignment = VerticalAlignment.Center;
                    break;
                case ContentAlignment.Top:
                    element.HorizontalAlignment = HorizontalAlignment.Center;
                    element.VerticalAlignment = VerticalAlignment.Top;
                    break;
                case ContentAlignment.Right:
                    element.HorizontalAlignment = HorizontalAlignment.Right;
                    element.VerticalAlignment = VerticalAlignment.Center;
                    break;
                case ContentAlignment.Bottom:
                    element.HorizontalAlignment = HorizontalAlignment.Center;
                    element.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                case ContentAlignment.Center:
                    element.HorizontalAlignment = HorizontalAlignment.Center;
                    element.VerticalAlignment = VerticalAlignment.Center;
                    break;
                case ContentAlignment.Stretch:
                    element.HorizontalAlignment = HorizontalAlignment.Stretch;
                    element.VerticalAlignment = VerticalAlignment.Stretch;
                    break;
            }
        }

        private void UpdateShadow()
        {
            if (_rootBorder == null) return;
            _rootBorder.Effect = HasShadow ? _shadowEffect : null;
        }

        private void GoToNormalState()
        {
            if (_rootBorder == null) return;

            _rootBorder.Background = Background ?? new SolidColorBrush(Colors.LightGray);
            _rootBorder.BorderBrush = BorderBrush ?? new SolidColorBrush(Colors.Gray);
            _rootBorder.BorderThickness = _originalBorderThickness;
            _rootBorder.Margin = new Thickness(0);
            _rootBorder.Opacity = 1.0;

            if (_scaleTransform != null)
            {
                _scaleTransform.ScaleX = 1.0;
                _scaleTransform.ScaleY = 1.0;
            }
            if (_translateTransform != null)
            {
                _translateTransform.X = 0;
                _translateTransform.Y = 0;
            }

            if (_textBlock != null)
            {
                _textBlock.Foreground = Foreground ?? new SolidColorBrush(Colors.Black);
            }

            if (_iconPath != null)
            {
                _iconPath.Fill = IconBrush ?? new SolidColorBrush(Colors.Black);
            }

            if (HasShadow && _shadowEffect != null)
            {
                _shadowEffect.ShadowDepth = 2;
                _shadowEffect.Opacity = 0.3;
            }
        }

        private void GoToHoverState()
        {
            if (_rootBorder == null) return;

            var hoverBg = HoverBackground ?? GetCalculatedHoverColor(Background ?? new SolidColorBrush(Colors.LightGray));
            var hoverBorder = HoverBorderBrush ?? GetCalculatedHoverColor(BorderBrush ?? new SolidColorBrush(Colors.Gray));
            var hoverFg = HoverForeground ?? GetCalculatedHoverColor(Foreground ?? new SolidColorBrush(Colors.Black));
            var hoverIcon = HoverIconBrush ?? GetCalculatedHoverColor(IconBrush ?? new SolidColorBrush(Colors.Black));

            _rootBorder.Background = hoverBg;
            _rootBorder.BorderBrush = hoverBorder;
            _rootBorder.BorderThickness = _originalBorderThickness;
            _rootBorder.Margin = new Thickness(0);
            _rootBorder.Opacity = 1.0;

            if (_scaleTransform != null)
            {
                _scaleTransform.ScaleX = 1.0;
                _scaleTransform.ScaleY = 1.0;
            }
            if (_translateTransform != null)
            {
                _translateTransform.X = 0;
                _translateTransform.Y = 0;
            }

            if (_textBlock != null)
            {
                _textBlock.Foreground = hoverFg;
            }

            if (_iconPath != null)
            {
                _iconPath.Fill = hoverIcon;
            }

            if (HasShadow && _shadowEffect != null)
            {
                _shadowEffect.ShadowDepth = 2;
                _shadowEffect.Opacity = 0.3;
            }
        }

        private void GoToPressedState()
        {
            if (_rootBorder == null) return;

            var pressedBg = PressedBackground ?? GetCalculatedPressedColor(Background ?? new SolidColorBrush(Colors.LightGray));
            var pressedBorder = PressedBorderBrush ?? GetCalculatedPressedColor(BorderBrush ?? new SolidColorBrush(Colors.Gray));
            var pressedFg = PressedForeground ?? GetCalculatedPressedColor(Foreground ?? new SolidColorBrush(Colors.Black));
            var pressedIcon = PressedIconBrush ?? GetCalculatedPressedColor(IconBrush ?? new SolidColorBrush(Colors.Black));

            _rootBorder.Background = pressedBg;
            _rootBorder.BorderBrush = pressedBorder;

            if (_scaleTransform != null)
            {
                _scaleTransform.ScaleX = PressedScale;
                _scaleTransform.ScaleY = PressedScale;
            }
            if (_translateTransform != null)
            {
                _translateTransform.X = PressedOffset.X;
                _translateTransform.Y = PressedOffset.Y;
            }

            if (_textBlock != null)
            {
                _textBlock.Foreground = pressedFg;
            }

            if (_iconPath != null)
            {
                _iconPath.Fill = pressedIcon;
            }

            if (HasShadow && _shadowEffect != null)
            {
                _shadowEffect.ShadowDepth = 1;
                _shadowEffect.Opacity = 0.2;
            }
        }

        private Brush GetCalculatedHoverColor(Brush originalBrush)
        {
            if (originalBrush is SolidColorBrush solidBrush)
            {
                var color = solidBrush.Color;
                return new SolidColorBrush(Color.FromArgb(
                    color.A,
                    (byte)Math.Min(255, color.R + 30),
                    (byte)Math.Min(255, color.G + 30),
                    (byte)Math.Min(255, color.B + 30)
                ));
            }
            return originalBrush;
        }

        private Brush GetCalculatedPressedColor(Brush originalBrush)
        {
            if (originalBrush is SolidColorBrush solidBrush)
            {
                var color = solidBrush.Color;
                return new SolidColorBrush(Color.FromArgb(
                    color.A,
                    (byte)Math.Max(0, color.R - 80),
                    (byte)Math.Max(0, color.G - 80),
                    (byte)Math.Max(0, color.B - 80)
                ));
            }
            return originalBrush;
        }

        private void SetupEventHandlers()
        {
            Click += OnClick;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (IsEnabled && !_isPressed)
            {
                _isHovered = true;
                GoToHoverState();
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (IsEnabled)
            {
                _isHovered = false;
                _isPressed = false;
                GoToNormalState();
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEnabled)
            {
                _isPressed = true;
                GoToPressedState();
                e.Handled = false; // Allow Click event to proceed
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsEnabled && _isPressed)
            {
                _isPressed = false;
                if (_isHovered)
                    GoToHoverState();
                else
                    GoToNormalState();
            }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (IsEnabled)
            {
                if (_isHovered)
                    GoToHoverState();
                else
                    GoToNormalState();
            }
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SmartButton button && button._rootBorder != null)
            {
                button._rootBorder.CornerRadius = (CornerRadius)e.NewValue;
            }
        }

        private static void OnIconGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SmartButton button && button._iconPath != null)
            {
                button._iconPath.Data = (Geometry)e.NewValue;
                button.UpdateLayout();
            }
        }

        private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SmartButton button)
            {
                button.UpdateLayout();
            }
        }

        private static void OnDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SmartButton button)
            {
                button.UpdateLayout();
            }
        }

        private static void OnShadowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SmartButton button)
            {
                button.UpdateShadow();
            }
        }

        private static void OnIconSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SmartButton button && button._iconPath != null)
            {
                button._iconPath.Width = (double)e.NewValue;
                button._iconPath.Height = (double)e.NewValue;
            }
        }

        private static void OnIconBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SmartButton button && button._iconPath != null)
            {
                button._iconPath.Fill = (Brush)e.NewValue;
            }
        }

        private static void OnButtonTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SmartButton button && button._textBlock != null)
            {
                button._textBlock.Text = e.NewValue?.ToString() ?? "";
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (_textBlock != null && string.IsNullOrEmpty(ButtonText))
            {
                _textBlock.Text = newContent?.ToString() ?? "";
            }
        }
    }
}