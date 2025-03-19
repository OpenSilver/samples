using Jamesnet.Foundation;
using OSFSample.Support.UI.Units;
using System.Windows.Controls;
using System.Windows;

namespace OSFSample.CheckBoxThreeState;

public partial class CheckBoxThreeStateExample : ExampleBase
{
    public CheckBoxThreeStateExample()
    {
        this.InitializeComponent();
    }
    private void ParentCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
    {
        // Modify child checkboxes based on parent state
        bool? isChecked = parentCheckBox.IsChecked;

        childCheckBox1.IsChecked = isChecked;
        childCheckBox2.IsChecked = isChecked;
        childCheckBox3.IsChecked = isChecked;

        UpdateStatus();
    }
    private void ChildCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
    {
        // Update parent checkbox based on children
        bool allChecked = (childCheckBox1.IsChecked == true) &&
                         (childCheckBox2.IsChecked == true) &&
                         (childCheckBox3.IsChecked == true);

        bool allUnchecked = (childCheckBox1.IsChecked == false) &&
                           (childCheckBox2.IsChecked == false) &&
                           (childCheckBox3.IsChecked == false);

        if (allChecked)
        {
            parentCheckBox.IsChecked = true;
        }
        else if (allUnchecked)
        {
            parentCheckBox.IsChecked = false;
        }
        else
        {
            parentCheckBox.IsChecked = null; // Indeterminate state
        }

        UpdateStatus();
    }

    private void UpdateStatus()
    {
        int checkedCount = 0;
        if (childCheckBox1.IsChecked == true) checkedCount++;
        if (childCheckBox2.IsChecked == true) checkedCount++;
        if (childCheckBox3.IsChecked == true) checkedCount++;

        txtStatus.Text = $"Selected items: {checkedCount} of 3";
    }
}