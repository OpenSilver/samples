using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OSFSample.Support.UI.Units;

public class CodeSource : INotifyPropertyChanged
{
    private bool _isSelected;

    public string Key { get; set; } = "";    
    public Uri Source { get; set; } = null!; 
    public string Code { get; set; } = "";

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
