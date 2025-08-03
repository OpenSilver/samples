using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading.Tasks;

namespace OpenSilverShowcase.Support.UI.Units
{
    public class ItemClickedEventArgs : EventArgs
    {
        public object Item { get; }

        public ItemClickedEventArgs(object item)
        {
            Item = item;
        }
    }

    public abstract class BaseListBox : ListBox
    {
        protected bool _isInitializing = false;

        public BaseListBox()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e) { }
        protected virtual void OnUnloaded(object sender, RoutedEventArgs e) { }

        protected override async void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            await HandleItemsChanged(e);
        }

        protected virtual async Task HandleItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                await HandleItemsReset();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                await HandleItemsAdded();
            }
        }

        protected virtual async Task HandleItemsReset()
        {
            _isInitializing = true;
            await Task.Delay(10);
            _isInitializing = false;
        }

        protected virtual async Task HandleItemsAdded() { }
    }

    public abstract class SelectableListBox<TItem> : BaseListBox where TItem : class
    {
        internal TItem CurrentSelectedItem { get; set; }
        internal bool IsFirstItemSelected { get; set; } = false;
        private string _lastSelectedItemName;

        public event EventHandler<ItemClickedEventArgs> ItemClicked;

        public virtual void OnItemClicked(object item)
        {
            ItemClicked?.Invoke(this, new ItemClickedEventArgs(item));
        }

        public static readonly DependencyProperty PendingSelectedItemNameProperty =
            DependencyProperty.Register(
                "PendingSelectedItemName",
                typeof(string),
                typeof(SelectableListBox<TItem>),
                new PropertyMetadata(null, OnPendingSelectedItemNameChanged));

        public string PendingSelectedItemName
        {
            get => (string)GetValue(PendingSelectedItemNameProperty);
            set => SetValue(PendingSelectedItemNameProperty, value);
        }

        public static readonly DependencyProperty AutoSelectFirstItemProperty =
            DependencyProperty.Register(
                "AutoSelectFirstItem",
                typeof(bool),
                typeof(SelectableListBox<TItem>),
                new PropertyMetadata(false));

        public bool AutoSelectFirstItem
        {
            get => (bool)GetValue(AutoSelectFirstItemProperty);
            set => SetValue(AutoSelectFirstItemProperty, value);
        }

        private static void OnPendingSelectedItemNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SelectableListBox<TItem> listBox && e.NewValue is string newValue)
            {
                _ = listBox.OnPendingSelectedItemNameChanged(newValue);
            }
        }

        protected override async void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);

            if (!string.IsNullOrEmpty(_lastSelectedItemName))
            {
                PendingSelectedItemName = _lastSelectedItemName;
                await OnPendingSelectedItemNameChanged(_lastSelectedItemName);
            }
            else if (AutoSelectFirstItem)
            {
                await SelectFirstItemIfNeeded();
            }
        }

        protected override void OnUnloaded(object sender, RoutedEventArgs e)
        {
            base.OnUnloaded(sender, e);

            if (CurrentSelectedItem != null)
            {
                _lastSelectedItemName = GetItemName(CurrentSelectedItem);
            }
            else
            {
                _lastSelectedItemName = null;
            }
        }

        protected override async Task HandleItemsReset()
        {
            await base.HandleItemsReset();

            if (CurrentSelectedItem != null)
            {
                SetItemState(CurrentSelectedItem, "CustomNormal");
            }

            CurrentSelectedItem = null;
            IsFirstItemSelected = false;

            if (!string.IsNullOrEmpty(PendingSelectedItemName))
            {
                await OnPendingSelectedItemNameChanged(PendingSelectedItemName);
            }
            else if (AutoSelectFirstItem)
            {
                await SelectFirstItemIfNeeded();
            }
        }

        protected override async Task HandleItemsAdded()
        {
            await base.HandleItemsAdded();

            if (!IsFirstItemSelected && Items.Count > 0)
            {
                if (!string.IsNullOrEmpty(PendingSelectedItemName))
                {
                    await OnPendingSelectedItemNameChanged(PendingSelectedItemName);
                }
                else if (AutoSelectFirstItem)
                {
                    await SelectFirstItemIfNeeded();
                }
            }
        }

        private async Task OnPendingSelectedItemNameChanged(string pendingItemName)
        {
            if (string.IsNullOrEmpty(pendingItemName) || Items.Count == 0)
                return;

            await Task.Delay(100);

            var targetIndex = FindItemIndexByName(pendingItemName);
            if (targetIndex >= 0)
            {
                var targetContainer = ItemContainerGenerator.ContainerFromIndex(targetIndex) as TItem;
                if (targetContainer != null)
                {
                    SelectTargetItem(targetContainer, false);
                }
                else if (AutoSelectFirstItem)
                {
                    await SelectFirstItemIfNeeded();
                }
            }
            else if (AutoSelectFirstItem)
            {
                await SelectFirstItemIfNeeded();
            }
        }

        private int FindItemIndexByName(string itemName)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                string itemString = string.Empty;

                if (item is Type type)
                {
                    itemString = type.Name;
                }
                else
                {
                    itemString = item?.ToString() ?? string.Empty;
                }

                if (string.Equals(itemString, itemName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        private async Task SelectFirstItemIfNeeded()
        {
            if (IsFirstItemSelected || Items.Count == 0 || _isInitializing)
                return;

            await Task.Delay(50);

            var firstContainer = ItemContainerGenerator.ContainerFromIndex(0) as TItem;
            if (firstContainer != null)
            {
                SelectFirstItem(firstContainer);
            }
        }

        public async Task<bool> SelectItemByName(string itemName, bool executeCommand = false)
        {
            if (string.IsNullOrEmpty(itemName) || Items.Count == 0)
                return false;

            var targetIndex = FindItemIndexByName(itemName);
            if (targetIndex >= 0)
            {
                var targetContainer = ItemContainerGenerator.ContainerFromIndex(targetIndex) as TItem;
                if (targetContainer != null)
                {
                    SelectTargetItem(targetContainer, executeCommand);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> SelectItemByDataContext(object dataContext, bool executeCommand = false)
        {
            if (dataContext == null || Items.Count == 0)
                return false;

            for (int i = 0; i < Items.Count; i++)
            {
                if (Equals(Items[i], dataContext))
                {
                    var targetContainer = ItemContainerGenerator.ContainerFromIndex(i) as TItem;
                    if (targetContainer != null)
                    {
                        SelectTargetItem(targetContainer, executeCommand);
                        return true;
                    }
                }
            }
            return false;
        }

        protected abstract string GetItemName(TItem item);
        protected abstract void SetItemState(TItem item, string state);
        protected abstract void SelectFirstItem(TItem item);
        protected abstract void SelectTargetItem(TItem item, bool executeCommand);
    }

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

    public abstract class SelectableListBoxItem<TListBox> : BaseListBoxItem where TListBox : class
    {
        protected override void UpdateSelection()
        {
            var listBox = GetParentListBox();
            if (listBox == null) return;

            var currentSelected = GetCurrentSelectedItem(listBox);
            if (currentSelected != null && !ReferenceEquals(currentSelected, this))
            {
                VisualStateManager.GoToState(currentSelected as BaseListBoxItem, "CustomNormal", true);
            }

            SetCurrentSelection(listBox);
            VisualStateManager.GoToState(this, "CustomSelected", true);
        }

        protected override void RestoreSelectionState()
        {
            var listBox = GetParentListBox();
            if (listBox == null) return;

            var currentSelected = GetCurrentSelectedItem(listBox);
            if (ReferenceEquals(currentSelected, this))
            {
                VisualStateManager.GoToState(this, "CustomSelected", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "CustomNormal", true);
            }
        }

        protected override void NotifyItemClicked(object item)
        {
            var listBox = GetParentListBox();
            if (listBox is SelectableListBox<SelectableListBoxItem<TListBox>> selectableListBox)
            {
                selectableListBox.OnItemClicked(item);
            }
        }

        public void SelectAsFirstItem()
        {
            var listBox = GetParentListBox();
            if (listBox == null || IsFirstItemAlreadySelected(listBox)) return;

            PerformSelection(listBox, true);
        }

        public void SelectAsTargetItem(bool executeCommand)
        {
            var listBox = GetParentListBox();
            if (listBox == null) return;

            PerformSelection(listBox, executeCommand);
        }

        private void PerformSelection(TListBox listBox, bool executeCommand)
        {
            ClearPreviousSelection(listBox);
            SetCurrentSelection(listBox);
            VisualStateManager.GoToState(this, "CustomSelected", false);
            MarkFirstItemSelected(listBox);

            if (executeCommand)
            {
                var command = MouseLeftButtonUpCommand;
                var parameter = DataContext;
                if (command?.CanExecute(parameter) == true)
                {
                    command.Execute(parameter);
                }

                // ItemClicked 이벤트도 발생시키기
                NotifyItemClicked(parameter);
            }
        }

        protected abstract TListBox GetParentListBox();
        protected abstract bool IsFirstItemAlreadySelected(TListBox listBox);
        protected abstract object GetCurrentSelectedItem(TListBox listBox);
        protected abstract void SetCurrentSelection(TListBox listBox);
        protected abstract void ClearPreviousSelection(TListBox listBox);
        protected abstract void MarkFirstItemSelected(TListBox listBox);
    }

    public class ShowcaseListBox : SelectableListBox<ShowcaseListBoxItem>
    {
        public ShowcaseListBox()
        {
            DefaultStyleKey = typeof(ShowcaseListBox);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ShowcaseListBoxItem();
        }

        protected override string GetItemName(ShowcaseListBoxItem item)
        {
            return item?.DataContext?.ToString();
        }

        protected override void SetItemState(ShowcaseListBoxItem item, string state)
        {
            VisualStateManager.GoToState(item, state, false);
        }

        protected override void SelectFirstItem(ShowcaseListBoxItem item)
        {
            item.SelectAsFirstItem();
        }

        protected override void SelectTargetItem(ShowcaseListBoxItem item, bool executeCommand)
        {
            item.SelectAsTargetItem(executeCommand);
        }
    }

    public class ShowcaseListBoxItem : SelectableListBoxItem<ShowcaseListBox>
    {
        public ShowcaseListBoxItem()
        {
            DefaultStyleKey = typeof(ShowcaseListBoxItem);
        }

        protected override bool IsCurrentlySelected()
        {
            var listBox = GetParentListBox();
            return listBox?.CurrentSelectedItem == this;
        }

        protected override ShowcaseListBox GetParentListBox()
        {
            return ItemsControl.ItemsControlFromItemContainer(this) as ShowcaseListBox;
        }

        protected override bool IsFirstItemAlreadySelected(ShowcaseListBox listBox)
        {
            return listBox.IsFirstItemSelected;
        }

        protected override object GetCurrentSelectedItem(ShowcaseListBox listBox)
        {
            return listBox.CurrentSelectedItem;
        }

        protected override void SetCurrentSelection(ShowcaseListBox listBox)
        {
            listBox.CurrentSelectedItem = this;
        }

        protected override void ClearPreviousSelection(ShowcaseListBox listBox)
        {
            if (listBox.CurrentSelectedItem != null)
            {
                VisualStateManager.GoToState(listBox.CurrentSelectedItem, "CustomNormal", false);
            }
        }

        protected override void MarkFirstItemSelected(ShowcaseListBox listBox)
        {
            listBox.IsFirstItemSelected = true;
        }

        protected override void NotifyItemClicked(object item)
        {
            var listBox = GetParentListBox();
            listBox?.OnItemClicked(item);
        }
    }
}