using System.Windows;

namespace OpenSilverShowcase.Support.UI.Units
{
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
}