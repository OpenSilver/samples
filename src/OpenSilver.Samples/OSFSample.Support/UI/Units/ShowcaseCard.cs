using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

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


public class ShowcaseCard : Control
{
    public static readonly DependencyProperty CodeSourcesProperty =
    DependencyProperty.Register(
        nameof(CodeSources),
        typeof(ObservableCollection<CodeSource>),
        typeof(ShowcaseCard),
        new PropertyMetadata(null)
    );


    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(ShowcaseCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(ShowcaseCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DemoContentProperty =
        DependencyProperty.Register(nameof(DemoContent), typeof(object), typeof(ShowcaseCard), new PropertyMetadata(null));

    public static readonly DependencyProperty XamlCodeProperty =
        DependencyProperty.Register(nameof(XamlCode), typeof(string), typeof(ShowcaseCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CSharpCodeProperty =
        DependencyProperty.Register(nameof(CSharpCode), typeof(string), typeof(ShowcaseCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ViewModelCodeProperty =
        DependencyProperty.Register(nameof(ViewModelCode), typeof(string), typeof(ShowcaseCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ShowXamlCodeProperty =
        DependencyProperty.Register(nameof(ShowXamlCode), typeof(bool), typeof(ShowcaseCard), new PropertyMetadata(true));

    public static readonly DependencyProperty ShowCSharpCodeProperty =
        DependencyProperty.Register(nameof(ShowCSharpCode), typeof(bool), typeof(ShowcaseCard), new PropertyMetadata(true));

    public static readonly DependencyProperty ShowViewModelCodeProperty =
        DependencyProperty.Register(nameof(ShowViewModelCode), typeof(bool), typeof(ShowcaseCard), new PropertyMetadata(false));

    public static readonly DependencyProperty OrderProperty =
        DependencyProperty.Register(nameof(Order), typeof(int), typeof(ShowcaseCard), new PropertyMetadata(0));

    public static readonly DependencyProperty SelectedCodeTabProperty =
        DependencyProperty.Register(nameof(SelectedCodeTab), typeof(CodeSource), typeof(ShowcaseCard), new PropertyMetadata(null, OnSelectedCodeTabChanged));

    private static void OnSelectedCodeTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ShowcaseCard showcaseCard)
        {
            var selectedTab = e.NewValue as CodeSource;

            // 모든 CodeSources의 IsSelected 업데이트
            foreach (var codeSource in showcaseCard.CodeSources)
            {
                if (codeSource is CodeSource cs)
                {
                    cs.IsSelected = string.Equals(cs.Key, selectedTab.Key, StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }

    public static readonly DependencyProperty AvailableCodeTabsProperty =
        DependencyProperty.Register(nameof(AvailableCodeTabs), typeof(ObservableCollection<string>), typeof(ShowcaseCard), new PropertyMetadata(new ObservableCollection<string>()));

    public ObservableCollection<CodeSource> CodeSources
    {
        get => (ObservableCollection<CodeSource>)GetValue(CodeSourcesProperty);
        set => SetValue(CodeSourcesProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public object DemoContent
    {
        get => GetValue(DemoContentProperty);
        set => SetValue(DemoContentProperty, value);
    }

    public string XamlCode
    {
        get => (string)GetValue(XamlCodeProperty);
        set => SetValue(XamlCodeProperty, value);
    }

    public string CSharpCode
    {
        get => (string)GetValue(CSharpCodeProperty);
        set => SetValue(CSharpCodeProperty, value);
    }

    public string ViewModelCode
    {
        get => (string)GetValue(ViewModelCodeProperty);
        set => SetValue(ViewModelCodeProperty, value);
    }

    public bool ShowXamlCode
    {
        get => (bool)GetValue(ShowXamlCodeProperty);
        set => SetValue(ShowXamlCodeProperty, value);
    }

    public bool ShowCSharpCode
    {
        get => (bool)GetValue(ShowCSharpCodeProperty);
        set => SetValue(ShowCSharpCodeProperty, value);
    }

    public bool ShowViewModelCode
    {
        get => (bool)GetValue(ShowViewModelCodeProperty);
        set => SetValue(ShowViewModelCodeProperty, value);
    }

    public int Order
    {
        get => (int)GetValue(OrderProperty);
        set => SetValue(OrderProperty, value);
    }

    public CodeSource SelectedCodeTab
    {
        get { return (CodeSource)GetValue(SelectedCodeTabProperty); }
        set { SetValue(SelectedCodeTabProperty, value); }
    }

    public ObservableCollection<string> AvailableCodeTabs
    {
        get => (ObservableCollection<string>)GetValue(AvailableCodeTabsProperty);
        set => SetValue(AvailableCodeTabsProperty, value);
    }

    public ShowcaseCard()
    {
        DefaultStyleKey = typeof(ShowcaseCard);
        CodeSources = new();
    }
}
