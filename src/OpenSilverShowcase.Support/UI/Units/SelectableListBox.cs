using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using OpenSilverShowcase.Support.Local.Models;
using OpenSilverShowcase.Support.UI.Primitives;

namespace OpenSilverShowcase.Support.UI.Units
{
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
}