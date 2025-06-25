using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace OpenSilverShowcase.ToggleButtons.Examples
{
    public class ToggleButtonViewModel : INotifyPropertyChanged
    {
        private bool _isFeatureEnabled;
        private string _commandResult = "Ready";
        private bool _canSave = true;

        public ToggleButtonViewModel()
        {
            FeatureName = "Auto Backup";
            ToggleCommand = new RelayCommand(ExecuteToggle);
            SaveCommand = new RelayCommand(ExecuteSave, () => CanSave);

            ToggleOptions = new ObservableCollection<ToggleOption>
            {
                new ToggleOption { Name = "Email Alerts", Color = "#E74C3C", IsSelected = true },
                new ToggleOption { Name = "Weekly Reports", Color = "#3498DB", IsSelected = false },
                new ToggleOption { Name = "Data Sync", Color = "#27AE60", IsSelected = true }
            };
        }

        public string FeatureName { get; set; }

        public bool IsFeatureEnabled
        {
            get => _isFeatureEnabled;
            set
            {
                _isFeatureEnabled = value;
                OnPropertyChanged(nameof(IsFeatureEnabled));
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public string StatusMessage => IsFeatureEnabled ? "Backup is Active" : "Backup is Disabled";

        public string CommandResult
        {
            get => _commandResult;
            set
            {
                _commandResult = value;
                OnPropertyChanged(nameof(CommandResult));
            }
        }

        public bool CanSave
        {
            get => _canSave;
            set
            {
                _canSave = value;
                OnPropertyChanged(nameof(CanSave));
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<ToggleOption> ToggleOptions { get; set; }

        public ICommand ToggleCommand { get; }
        public ICommand SaveCommand { get; }

        private void ExecuteToggle()
        {
            CommandResult = $"System refreshed at {System.DateTime.Now:HH:mm:ss}";
        }

        private void ExecuteSave()
        {
            CommandResult = "Configuration saved successfully!";
            CanSave = false;

            // Re-enable after 2 seconds (simulated)
            System.Threading.Tasks.Task.Delay(2000).ContinueWith(_ => CanSave = true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ToggleOption : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Name { get; set; }
        public string Color { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}