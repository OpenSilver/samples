using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace OpenSilverShowcase.Support.UI.Units
{
    
    public class TabMenuBar : Control
    {
        private double _lastIndicatorX = 0;
        private ListBox _menuListBox;
        private Border _indicatorBorder;
        private TranslateTransform _indicatorTransform;
        private List<string> _menuItems = new();
        private int _selectedIndex = -1;
        private Storyboard _animationStoryboard;

        public event EventHandler<MenuItemSelectedEventArgs> MenuItemSelected;

        public List<string> MenuItems
        {
            get => _menuItems;
            set
            {
                _menuItems = value;
                if (_menuListBox != null)
                    UpdateMenuItems();
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != value && value >= 0 && value < _menuItems.Count)
                {
                    _selectedIndex = value;
                    if (_menuListBox != null)
                    {
                        _menuListBox.SelectedIndex = _selectedIndex;
                        MoveIndicatorToSelectedItem();
                    }
                }
            }
        }

        public Brush BackgroundColor
        {
            get => (Brush)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Brush), typeof(TabMenuBar),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 241, 234, 240))));

        public Brush IndicatorColor
        {
            get => (Brush)GetValue(IndicatorColorProperty);
            set => SetValue(IndicatorColorProperty, value);
        }

        public static readonly DependencyProperty IndicatorColorProperty =
            DependencyProperty.Register(nameof(IndicatorColor), typeof(Brush), typeof(TabMenuBar),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 120, 215))));

        public Brush SelectedTextColor
        {
            get => (Brush)GetValue(SelectedTextColorProperty);
            set => SetValue(SelectedTextColorProperty, value);
        }

        public static readonly DependencyProperty SelectedTextColorProperty =
            DependencyProperty.Register(nameof(SelectedTextColor), typeof(Brush), typeof(TabMenuBar),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush NormalTextColor
        {
            get => (Brush)GetValue(NormalTextColorProperty);
            set => SetValue(NormalTextColorProperty, value);
        }

        public static readonly DependencyProperty NormalTextColorProperty =
            DependencyProperty.Register(nameof(NormalTextColor), typeof(Brush), typeof(TabMenuBar),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public double AnimationDuration
        {
            get => (double)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(nameof(AnimationDuration), typeof(double), typeof(TabMenuBar),
                new PropertyMetadata(300.0));

        public double SelectedFontSize
        {
            get => (double)GetValue(SelectedFontSizeProperty);
            set => SetValue(SelectedFontSizeProperty, value);
        }

        public static readonly DependencyProperty SelectedFontSizeProperty =
            DependencyProperty.Register(nameof(SelectedFontSize), typeof(double), typeof(TabMenuBar),
                new PropertyMetadata(15.0));

        public double NormalFontSize
        {
            get => (double)GetValue(NormalFontSizeProperty);
            set => SetValue(NormalFontSizeProperty, value);
        }

        public static readonly DependencyProperty NormalFontSizeProperty =
            DependencyProperty.Register(nameof(NormalFontSize), typeof(double), typeof(TabMenuBar),
                new PropertyMetadata(13.0));

        public TabMenuBar()
        {
            DefaultStyleKey = typeof(TabMenuBar);
            _animationStoryboard = new Storyboard();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _menuListBox = GetTemplateChild("PART_MenuListBox") as ListBox;
            _indicatorBorder = GetTemplateChild("PART_Indicator") as Border;

            if (_indicatorBorder != null)
            {
                _indicatorTransform = new TranslateTransform();
                _indicatorBorder.RenderTransform = _indicatorTransform;
            }

            if (_menuListBox != null)
            {
                _menuListBox.SelectionChanged += MenuListBox_SelectionChanged;
                UpdateMenuItems();
                _menuListBox.SelectedIndex = _selectedIndex;
            }
        }

        private void UpdateMenuItems()
        {
            if (_menuListBox == null) return;

            _menuListBox.Items.Clear();
            foreach (var item in _menuItems)
                _menuListBox.Items.Add(item);
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_menuListBox.SelectedIndex >= 0)
            {
                _selectedIndex = _menuListBox.SelectedIndex;
                MoveIndicatorToSelectedItem();

                MenuItemSelected?.Invoke(this, new MenuItemSelectedEventArgs
                {
                    SelectedIndex = _selectedIndex,
                    SelectedItem = _menuItems[_selectedIndex]
                });
            }
        }

        public void MoveIndicatorToSelectedItem()
        {
            if (_menuListBox == null || _indicatorBorder == null || _indicatorTransform == null) return;

            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    var selectedItem = _menuListBox.ItemContainerGenerator.ContainerFromIndex(_selectedIndex) as ListBoxItem;
                    if (selectedItem == null) return;

                    var transform = selectedItem.TransformToVisual(_menuListBox);
                    var relativePoint = transform.Transform(new Point(0, 0));
                    var targetX = relativePoint.X;
                    var targetWidth = selectedItem.ActualWidth;

                    _animationStoryboard.Stop();
                    _animationStoryboard.Children.Clear();

                    var moveAnimation = new DoubleAnimation
                    {
                        From = _lastIndicatorX,
                        To = targetX,
                        Duration = TimeSpan.FromMilliseconds(AnimationDuration)
                    };

                    var widthAnimation = new DoubleAnimation
                    {
                        From = _lastIndicatorWidth <= 0 ? targetWidth : _lastIndicatorWidth,
                        To = targetWidth,
                        Duration = TimeSpan.FromMilliseconds(AnimationDuration)
                    };

                    try
                    {
                        var easing = new CubicEase { EasingMode = EasingMode.EaseOut };
                        moveAnimation.EasingFunction = easing;
                        widthAnimation.EasingFunction = easing;
                    }
                    catch { }

                    Storyboard.SetTarget(moveAnimation, _indicatorTransform);
                    Storyboard.SetTargetProperty(moveAnimation, new PropertyPath("X"));

                    Storyboard.SetTarget(widthAnimation, _indicatorBorder);
                    Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("Width"));

                    _animationStoryboard.Children.Add(moveAnimation);
                    _animationStoryboard.Children.Add(widthAnimation);
                    _animationStoryboard.Begin();

                    // 둘 다 저장
                    _lastIndicatorX = targetX;
                    _lastIndicatorWidth = targetWidth;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"애니메이션 오류: {ex.Message}");

                    var selectedItem = _menuListBox.ItemContainerGenerator.ContainerFromIndex(_selectedIndex) as ListBoxItem;
                    if (selectedItem != null)
                    {
                        var transform = selectedItem.TransformToVisual(_menuListBox);
                        var relativePoint = transform.Transform(new Point(0, 0));
                        var targetX = relativePoint.X;
                        var targetWidth = selectedItem.ActualWidth;

                        _indicatorTransform.X = targetX;
                        _indicatorBorder.Width = targetWidth;

                        _lastIndicatorX = targetX;
                        _lastIndicatorWidth = targetWidth;
                    }
                }
            });
        }
        private double _lastIndicatorWidth = 0;

    }

    public class MenuItemSelectedEventArgs : EventArgs
    {
        public int SelectedIndex { get; set; }
        public string SelectedItem { get; set; }
    }
}
