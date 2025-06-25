using System.ComponentModel;

namespace OpenSilverShowcase.TextBoxs.Examples
{
    public class TextBoxViewModel : INotifyPropertyChanged
    {
        private string _userName = "Type your name here";
        private string _email = "user@example.com";

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
                OnPropertyChanged(nameof(CharacterCount));
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(IsValidEmail));
            }
        }

        public int CharacterCount
        {
            get { return UserName?.Length ?? 0; }
        }

        public bool IsValidEmail
        {
            get { return !string.IsNullOrEmpty(Email) && Email.Contains("@") && Email.Contains("."); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
