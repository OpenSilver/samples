using System.Windows.Controls;

namespace OSFSample.Support.UI.Units;

public class CodeTypeListBox : ListBox
{
    public CodeTypeListBox()
    {
        DefaultStyleKey = typeof(CodeTypeListBox);
        SelectionChanged += CodeTypeListBox_SelectionChanged;
    }

    private void CodeTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is ShowcaseItem card && SelectedItem is TextBlock selectedTextBlock)
        {
            //card.SelectedCodeTab = selectedTextBlock.;
        }
    }
}
