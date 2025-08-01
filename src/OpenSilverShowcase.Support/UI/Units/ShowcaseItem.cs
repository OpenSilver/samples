﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Jamesnet.Foundation;

namespace OpenSilverShowcase.Support.UI.Units;

public class ShowcaseItem : Control
{

    public static readonly DependencyProperty CodeSourcesProperty =
    DependencyProperty.Register(
        nameof(CodeSources),
        typeof(ObservableCollection<CodeSource>),
        typeof(ShowcaseItem),
        new PropertyMetadata(null)
    );


    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(ShowcaseItem), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(ShowcaseItem), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DemoContentProperty =
        DependencyProperty.Register(nameof(DemoContent), typeof(object), typeof(ShowcaseItem), new PropertyMetadata(null));

    public static readonly DependencyProperty XamlCodeProperty =
        DependencyProperty.Register(nameof(XamlCode), typeof(string), typeof(ShowcaseItem), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CSharpCodeProperty =
        DependencyProperty.Register(nameof(CSharpCode), typeof(string), typeof(ShowcaseItem), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ViewModelCodeProperty =
        DependencyProperty.Register(nameof(ViewModelCode), typeof(string), typeof(ShowcaseItem), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ShowXamlCodeProperty =
        DependencyProperty.Register(nameof(ShowXamlCode), typeof(bool), typeof(ShowcaseItem), new PropertyMetadata(true));

    public static readonly DependencyProperty ShowCSharpCodeProperty =
        DependencyProperty.Register(nameof(ShowCSharpCode), typeof(bool), typeof(ShowcaseItem), new PropertyMetadata(true));

    public static readonly DependencyProperty ShowViewModelCodeProperty =
        DependencyProperty.Register(nameof(ShowViewModelCode), typeof(bool), typeof(ShowcaseItem), new PropertyMetadata(false));

    public static readonly DependencyProperty OrderProperty =
        DependencyProperty.Register(nameof(Order), typeof(int), typeof(ShowcaseItem), new PropertyMetadata(0));

    public static readonly DependencyProperty SelectedCodeTabProperty =
        DependencyProperty.Register(nameof(SelectedCodeTab), typeof(CodeSource), typeof(ShowcaseItem), new PropertyMetadata(null, OnSelectedCodeTabChanged));

    private static void OnSelectedCodeTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ShowcaseItem ShowcaseItem)
        {
            var selectedTab = e.NewValue as CodeSource;

            // 모든 CodeSources의 IsSelected 업데이트
            foreach (var codeSource in ShowcaseItem.CodeSources)
            {
                if (codeSource is CodeSource cs)
                {
                    cs.IsSelected = string.Equals(cs.Key, selectedTab.Key, StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }

    public static readonly DependencyProperty AvailableCodeTabsProperty =
        DependencyProperty.Register(nameof(AvailableCodeTabs), typeof(ObservableCollection<string>), typeof(ShowcaseItem), new PropertyMetadata(new ObservableCollection<string>()));
    public static readonly DependencyProperty DemoBackgroundProperty =
        DependencyProperty.Register(
            nameof(DemoBackground),
            typeof(object),
            typeof(ShowcaseItem),
            new PropertyMetadata(null)
        );
    public static readonly DependencyProperty DemoBorderBrushProperty =
        DependencyProperty.Register(
            nameof(DemoBorderBrush),
            typeof(object),
            typeof(ShowcaseItem),
            new PropertyMetadata(null)
        );

    public static readonly DependencyProperty FullScreenCommandProperty =
        DependencyProperty.Register(nameof(FullScreenCommand), typeof(ICommand), typeof(ShowcaseItem), new PropertyMetadata(null));

    private Grid _fullScreen;

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

    public object DemoBackground
    {
        get => GetValue(DemoBackgroundProperty);
        set => SetValue(DemoBackgroundProperty, value);
    }

    public object DemoBorderBrush
    {
        get => GetValue(DemoBorderBrushProperty);
        set => SetValue(DemoBorderBrushProperty, value);
    }

    public ICommand FullScreenCommand
    {
        get => (ICommand)GetValue(FullScreenCommandProperty);
        set => SetValue(FullScreenCommandProperty, value);
    }

    public ShowcaseItem()
    {
        DefaultStyleKey = typeof(ShowcaseItem);
        CodeSources = new();

        FullScreenCommand = new RelayCommand<CodeSource>(OnFullScreen);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_CopyButton") is CopyIconButton copyBtn)
        {
            copyBtn.Click += OnCopyClick;
        }

        if (GetTemplateChild("PART_GitHubButton") is IconButton gitHubBtn)
        {
            gitHubBtn.Click += OnGitHubClick;
        }

        if (GetTemplateChild("PART_ShareButton") is IconButton shareBtn)
        {
            shareBtn.Click += OnShareClick;
        }

        if (GetTemplateChild("PART_FullScreen") is Grid fullScreen)
        {
            _fullScreen = fullScreen;
        }
    }

    private void OnFullScreen(CodeSource obj)
    {

        var parent = FindParentShowcaseContent(this);
        parent.CodeItem(obj);
    }

    public void OnCopyClick(object sender, RoutedEventArgs e)
    {
        if (SelectedCodeTab != null && !string.IsNullOrEmpty(SelectedCodeTab.Code))
        {
            System.Windows.Clipboard.SetText(SelectedCodeTab.Code);
        }
    }
    public void OnGitHubClick(object sender, RoutedEventArgs e)
    {
        if (SelectedCodeTab != null && SelectedCodeTab.Source != null)
        {
            string ns = this.GetType().Namespace;
            string[] parts = ns?.Split('.') ?? Array.Empty<string>();
            string projectName = (parts.Length >= 2) ? $"{parts[0]}.{parts[1]}" : "Unknown";

            string path = SelectedCodeTab.Source.OriginalString.TrimStart('/');

            string prefix = $"{projectName};component/";
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                path = path.Substring(prefix.Length);

            if (!path.StartsWith("Examples/", StringComparison.OrdinalIgnoreCase))
            {
                string fileName = System.IO.Path.GetFileName(path);
                path = $"Examples/{fileName}";
            }

            string githubUrl = $"https://github.com/OpenSilver/samples/blob/master/src/OpenSilver.Samples/{projectName}/{path}";

            HtmlPage.Window.Navigate(new Uri(githubUrl, UriKind.Absolute), "_blank");
        }
    }





    public void OnShareClick(object sender, RoutedEventArgs e)
    {
        var parent = FindParentShowcaseContent(this);
        if (parent != null)
        {
            parent.ShareItem(SelectedCodeTab);
        }
    }

    private ShowcaseContent FindParentShowcaseContent(DependencyObject obj)
    {
        DependencyObject parent = VisualTreeHelper.GetParent(obj);
        while (parent != null && !(parent is ShowcaseContent))
            parent = VisualTreeHelper.GetParent(parent);

        return parent as ShowcaseContent;
    }
}
