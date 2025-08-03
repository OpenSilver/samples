using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenSilverShowcase.Support.UI.Primitives
{
    public abstract class BaseListBoxItem : ListBoxItem
    {
        private double _lastKnownScrollOffset = 0;
        private const double ScrollThreshold = 1.0;

        public static readonly DependencyProperty MouseLeftButtonUpCommandProperty =
            DependencyProperty.Register(
                "MouseLeftButtonUpCommand",
                typeof(ICommand),
                typeof(BaseListBoxItem),
                new PropertyMetadata(null));

        public ICommand MouseLeftButtonUpCommand
        {
            get => (ICommand)GetValue(MouseLeftButtonUpCommandProperty);
            set => SetValue(MouseLeftButtonUpCommandProperty, value);
        }

        public BaseListBoxItem()
        {
            Loaded += OnLoaded;
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "CustomNormal", false);

            var scrollViewer = FindAncestorScrollViewer(this);
            if (scrollViewer != null)
            {
                _lastKnownScrollOffset = scrollViewer.HorizontalOffset;
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var scrollViewer = FindAncestorScrollViewer(this);
            if (scrollViewer != null)
            {
                _lastKnownScrollOffset = scrollViewer.HorizontalOffset;
            }

            if (!IsCurrentlySelected())
            {
                VisualStateManager.GoToState(this, "CustomHover", true);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsCurrentlySelected())
            {
                VisualStateManager.GoToState(this, "CustomNormal", true);
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var scrollViewer = FindAncestorScrollViewer(this);
            if (scrollViewer != null)
            {
                _lastKnownScrollOffset = scrollViewer.HorizontalOffset;
            }
            VisualStateManager.GoToState(this, "CustomPressed", true);
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var command = MouseLeftButtonUpCommand;
            var parameter = DataContext;

            var scrollViewer = FindAncestorScrollViewer(this);
            bool hasScrolled = false;

            if (scrollViewer != null)
            {
                double currentOffset = scrollViewer.HorizontalOffset;
                double scrollDifference = Math.Abs(currentOffset - _lastKnownScrollOffset);
                hasScrolled = scrollDifference >= ScrollThreshold;
                _lastKnownScrollOffset = currentOffset;
            }

            if (!hasScrolled)
            {
                if (command?.CanExecute(parameter) == true)
                {
                    command.Execute(parameter);
                }

                NotifyItemClicked(parameter);
                UpdateSelection();
            }
            else
            {
                RestoreSelectionState();
            }
        }

        private ScrollViewer FindAncestorScrollViewer(DependencyObject element)
        {
            while (element != null)
            {
                if (element is ScrollViewer viewer)
                    return viewer;
                element = VisualTreeHelper.GetParent(element);
            }
            return null;
        }

        protected abstract bool IsCurrentlySelected();
        protected abstract void UpdateSelection();
        protected abstract void RestoreSelectionState();
        protected abstract void NotifyItemClicked(object item);
    }
}