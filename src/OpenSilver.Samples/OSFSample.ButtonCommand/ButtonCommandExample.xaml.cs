using OSFSample.Support.UI.Units;
using System.Windows.Input;
using System.Windows;
using Jamesnet.Foundation;

namespace OSFSample.ButtonCommand;

public partial class ButtonCommandExample : ExampleBase
{
    public ButtonCommandExample()
    {
        this.InitializeComponent();
        DataContext = new ButtonCommandExampleViewModel();
    }
}

public class ButtonCommandExampleViewModel : ViewModelBase
{
    public ICommand SimpleCommand { get; set; }

    public ButtonCommandExampleViewModel()
    {
        SimpleCommand = new RelayCommand(ExecuteSimpleCommand);
    }
    private void ExecuteSimpleCommand()
    {
        MessageBox.Show("Simple Command Executed!");
    }
}
