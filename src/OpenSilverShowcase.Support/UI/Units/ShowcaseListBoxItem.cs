using System.Windows;
using System.Windows.Controls;

namespace OpenSilverShowcase.Support.UI.Units
{
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