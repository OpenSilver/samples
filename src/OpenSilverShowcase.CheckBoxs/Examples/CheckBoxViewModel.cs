using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace OpenSilverShowcase.CheckBoxs.Examples
{
    public class CheckBoxViewModel : INotifyPropertyChanged
    {
        private bool _isShuffleEnabled;
        private bool _isOfflineDownloadEnabled;
        private bool _isDiscoveryEnabled;
        private bool _isLyricsEnabled;

        public bool IsShuffleEnabled
        {
            get => _isShuffleEnabled;
            set
            {
                _isShuffleEnabled = value;
                OnPropertyChanged(nameof(IsShuffleEnabled));
                UpdateStatus();
            }
        }

        public bool IsOfflineDownloadEnabled
        {
            get => _isOfflineDownloadEnabled;
            set
            {
                _isOfflineDownloadEnabled = value;
                OnPropertyChanged(nameof(IsOfflineDownloadEnabled));
                UpdateStatus();
            }
        }

        public bool IsDiscoveryEnabled
        {
            get => _isDiscoveryEnabled;
            set
            {
                _isDiscoveryEnabled = value;
                OnPropertyChanged(nameof(IsDiscoveryEnabled));
                UpdateStatus();
            }
        }

        public bool IsLyricsEnabled
        {
            get => _isLyricsEnabled;
            set
            {
                _isLyricsEnabled = value;
                OnPropertyChanged(nameof(IsLyricsEnabled));
                UpdateStatus();
            }
        }

        public string StatusMessage { get; private set; }
        public int EnabledFeaturesCount { get; private set; }

        public ICommand ResetCommand { get; }

        public CheckBoxViewModel()
        {
            ResetCommand = new RelayCommand(ResetSettings);
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            var features = new[]
            {
                IsShuffleEnabled ? "Shuffle" : null,
                IsOfflineDownloadEnabled ? "Offline Downloads" : null,
                IsDiscoveryEnabled ? "Music Discovery" : null,
                IsLyricsEnabled ? "Karaoke Mode" : null
            }.Where(f => f != null).ToArray();

            EnabledFeaturesCount = features.Length;
            StatusMessage = features.Length > 0
                ? $"Jamming with: {string.Join(", ", features)}"
                : "Basic listening mode - time to spice it up! 🎶";

            OnPropertyChanged(nameof(StatusMessage));
            OnPropertyChanged(nameof(EnabledFeaturesCount));
        }

        private void ResetSettings()
        {
            IsShuffleEnabled = false;
            IsOfflineDownloadEnabled = false;
            IsDiscoveryEnabled = false;
            IsLyricsEnabled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly System.Action _execute;

        public RelayCommand(System.Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute();

        public event System.EventHandler CanExecuteChanged;
    }
}