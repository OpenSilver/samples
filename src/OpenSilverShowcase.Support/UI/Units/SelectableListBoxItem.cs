using System.Windows;
using OpenSilverShowcase.Support.UI.Primitives;

namespace OpenSilverShowcase.Support.UI.Units
{
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
}