using Jamesnet.Foundation;
using OSFSample.Support.UI.Units;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace OSFSample.CheckBoxGroup;

public partial class CheckBoxGroupExample : ExampleBase
{
    public CheckBoxGroupExample()
    {
        this.InitializeComponent();
        UpdateSummary();
    }
    private void NotificationOption_Changed(object sender, RoutedEventArgs e)
    {
        UpdateSummary();
    }

    private void UpdateSummary()
    {
        List<string> selectedOptions = new List<string>();

        if (chkEmail.IsChecked == true) selectedOptions.Add("Email");
        if (chkSMS.IsChecked == true) selectedOptions.Add("SMS");
        if (chkPush.IsChecked == true) selectedOptions.Add("Push");
        if (chkInApp.IsChecked == true) selectedOptions.Add("In-App");

        if (selectedOptions.Count > 0)
        {
            txtSummary.Content = $"You will receive notifications via: {string.Join(", ", selectedOptions)}";
            btnSave.IsEnabled = true;
        }
        else
        {
            txtSummary.Content = "No notification methods selected";
            btnSave.IsEnabled = false;
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Notification preferences saved successfully!");
    }
}