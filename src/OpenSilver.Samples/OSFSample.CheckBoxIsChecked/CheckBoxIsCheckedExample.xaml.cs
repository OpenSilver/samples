using Jamesnet.Foundation;
using OSFSample.Support.UI.Units;

namespace OSFSample.CheckBoxIsChecked;

public partial class CheckBoxIsCheckedExample : ExampleBase
{
    public CheckBoxIsCheckedExample()
    {
        this.InitializeComponent();
        DataContext = new CheckBoxIsCheckedViewModel();
    }

    public class CheckBoxIsCheckedViewModel : ViewModelBase
    {
        private bool _isTermsAccepted;

        public bool IsTermsAccepted
        {
            get => _isTermsAccepted;
            set
            {
                if (SetProperty(ref _isTermsAccepted, value))
                {
                    UpdateStatusText();
                }
            }
        }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public CheckBoxIsCheckedViewModel()
        {
            UpdateStatusText();
        }

        private void UpdateStatusText()
        {
            StatusText = IsTermsAccepted ? "Terms accepted, you may continue" : "Please accept terms to continue";
        }
    }
}
