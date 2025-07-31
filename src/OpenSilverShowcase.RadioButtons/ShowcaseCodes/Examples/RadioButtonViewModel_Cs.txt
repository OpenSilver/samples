using System;
using System.ComponentModel;
using System.Windows.Input;

namespace OpenSilverShowcase.RadioButtons.Examples
{
    public enum SizeOption
    {
        Small,
        Medium,
        Large
    }

    public class RadioButtonViewModel : INotifyPropertyChanged
    {
        #region Private Fields
        private SizeOption _selectedSize = SizeOption.Medium;
        private string _selectedColor = "Green";
        private bool _isBasicPlan = true;
        private bool _isPremiumPlan = false;
        private bool _isEnterprisePlan = false;
        private bool _isViewMode = true;
        private bool _isEditMode = false;
        private bool _isAdminMode = false;
        #endregion

        #region Properties

        public SizeOption SelectedSize
        {
            get => _selectedSize;
            set
            {
                if (_selectedSize != value)
                {
                    _selectedSize = value;
                    OnPropertyChanged(nameof(SelectedSize));
                    UpdateStatusMessage();
                }
            }
        }

        public string SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    OnPropertyChanged(nameof(SelectedColor));
                    UpdateStatusMessage();
                }
            }
        }

        public bool IsBasicPlan
        {
            get => _isBasicPlan;
            set
            {
                if (_isBasicPlan != value)
                {
                    _isBasicPlan = value;
                    if (value)
                    {
                        IsPremiumPlan = false;
                        IsEnterprisePlan = false;
                    }
                    OnPropertyChanged(nameof(IsBasicPlan));
                    OnPropertyChanged(nameof(SelectedPlan));
                    UpdateStatusMessage();
                }
            }
        }

        public bool IsPremiumPlan
        {
            get => _isPremiumPlan;
            set
            {
                if (_isPremiumPlan != value)
                {
                    _isPremiumPlan = value;
                    if (value)
                    {
                        IsBasicPlan = false;
                        IsEnterprisePlan = false;
                    }
                    OnPropertyChanged(nameof(IsPremiumPlan));
                    OnPropertyChanged(nameof(SelectedPlan));
                    UpdateStatusMessage();
                }
            }
        }

        public bool IsEnterprisePlan
        {
            get => _isEnterprisePlan;
            set
            {
                if (_isEnterprisePlan != value)
                {
                    _isEnterprisePlan = value;
                    if (value)
                    {
                        IsBasicPlan = false;
                        IsPremiumPlan = false;
                    }
                    OnPropertyChanged(nameof(IsEnterprisePlan));
                    OnPropertyChanged(nameof(SelectedPlan));
                    UpdateStatusMessage();
                }
            }
        }

        public bool IsViewMode
        {
            get => _isViewMode;
            set
            {
                if (_isViewMode != value)
                {
                    _isViewMode = value;
                    OnPropertyChanged(nameof(IsViewMode));
                    OnPropertyChanged(nameof(CurrentMode));
                    UpdateStatusMessage();
                }
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                if (_isEditMode != value)
                {
                    _isEditMode = value;
                    OnPropertyChanged(nameof(IsEditMode));
                    OnPropertyChanged(nameof(CurrentMode));
                    UpdateStatusMessage();
                }
            }
        }

        public bool IsAdminMode
        {
            get => _isAdminMode;
            set
            {
                if (_isAdminMode != value)
                {
                    _isAdminMode = value;
                    OnPropertyChanged(nameof(IsAdminMode));
                    OnPropertyChanged(nameof(CurrentMode));
                    UpdateStatusMessage();
                }
            }
        }

        public string SelectedPlan
        {
            get
            {
                if (IsBasicPlan) return "Basic";
                if (IsPremiumPlan) return "Premium";
                if (IsEnterprisePlan) return "Enterprise";
                return "None";
            }
        }

        public string CurrentMode
        {
            get
            {
                if (IsViewMode) return "View";
                if (IsEditMode) return "Edit";
                if (IsAdminMode) return "Admin";
                return "None";
            }
        }

        private string _statusMessage = "Welcome to RadioButton MVVM Demo!";
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        #endregion

        #region Commands

        public ICommand SetViewModeCommand { get; }
        public ICommand SetEditModeCommand { get; }
        public ICommand SetAdminModeCommand { get; }
        public ICommand ResetCommand { get; }

        #endregion

        #region Constructor

        public RadioButtonViewModel()
        {
            SetViewModeCommand = new RelayCommand(() => SetMode("View"));
            SetEditModeCommand = new RelayCommand(() => SetMode("Edit"));
            SetAdminModeCommand = new RelayCommand(() => SetMode("Admin"));
            ResetCommand = new RelayCommand(Reset);

            UpdateStatusMessage();
        }

        #endregion

        #region Private Methods

        private void SetMode(string mode)
        {
            IsViewMode = mode == "View";
            IsEditMode = mode == "Edit";
            IsAdminMode = mode == "Admin";

            StatusMessage = $"Mode changed to {mode} via Command!";
        }

        private void Reset()
        {
            SelectedSize = SizeOption.Medium;
            SelectedColor = "Green";
            IsBasicPlan = true;
            SetMode("View");
            StatusMessage = "All selections have been reset!";
        }

        private void UpdateStatusMessage()
        {
            StatusMessage = $"Current: {SelectedSize} {SelectedColor} {SelectedPlan} in {CurrentMode} mode";
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}