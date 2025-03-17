using OSFSample.Support.UI.Units;
using System.Windows.Input;
using System.Windows;
using Jamesnet.Foundation;

namespace OSFSample.ButtonCommandCanExecute;

public partial class ButtonCommandCanExecuteExample : ExampleBase
{
    public ButtonCommandCanExecuteExample()
    {
        this.InitializeComponent();
        DataContext = new ButtonCommandCanExecuteExampleViewModel();
    }
}

public class ButtonCommandCanExecuteExampleViewModel : ViewModelBase
{
    private string _email;

    [CommandTrigger(nameof(CompleteCommand))]
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public ICommand CompleteCommand { get; set; }

    public ButtonCommandCanExecuteExampleViewModel()
    {
        CompleteCommand = new RelayCommand(Complete, CanComplete);
    }

    private bool CanComplete()
    {
        return !string.IsNullOrEmpty(Email);
    }

    private void Complete()
    {
        MessageBox.Show("Execute Completed!");
    }
}
