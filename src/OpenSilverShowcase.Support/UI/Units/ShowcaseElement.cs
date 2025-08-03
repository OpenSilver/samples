using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Jamesnet.Foundation;

namespace OpenSilverShowcase.Support.UI.Units
{
    public class ShowcaseContentElement : ShowcaseFrameworkElement, IView
    {
        public static readonly DependencyProperty AutoSelectFirstItemProperty = DependencyProperty.Register("AutoSelectFirstItem", typeof(bool), typeof(ShowcaseContentElement), new PropertyMetadata(false));
        public static readonly DependencyProperty FirstSelectedItemNameProperty = DependencyProperty.Register("FirstSelectedItemName", typeof(string), typeof(ShowcaseContentElement), new PropertyMetadata(null));
        public static readonly DependencyProperty FullScreenCommandProperty = DependencyProperty.Register(nameof(FullScreenCommand), typeof(ICommand), typeof(ShowcaseContentElement), new PropertyMetadata(null));
        
        public bool AutoSelectFirstItem
        {
            get => (bool)GetValue(AutoSelectFirstItemProperty);
            set => SetValue(AutoSelectFirstItemProperty, value);
        }

        public string FirstSelectedItemName
        {
            get => (string)GetValue(FirstSelectedItemNameProperty);
            set => SetValue(FirstSelectedItemNameProperty, value);
        }

        public ICommand FullScreenCommand
        {
            get => (ICommand)GetValue(FullScreenCommandProperty);
            set => SetValue(FullScreenCommandProperty, value);
        }
    }

    public class ShowcaseItemElement : ShowcaseFrameworkElement
    {
        public static readonly DependencyProperty OrderProperty = DependencyProperty.Register(nameof(Order), typeof(int), typeof(ShowcaseItemElement), new PropertyMetadata(0));
        public static readonly DependencyProperty CodeSourcesProperty = DependencyProperty.Register(nameof(CodeSources), typeof(ObservableCollection<CodeSource>), typeof(ShowcaseItemElement), new PropertyMetadata(null));
        public static readonly DependencyProperty DemoContentProperty = DependencyProperty.Register(nameof(DemoContent), typeof(object), typeof(ShowcaseItemElement), new PropertyMetadata(null));
        public static readonly DependencyProperty DemoBackgroundProperty = DependencyProperty.Register(nameof(DemoBackground), typeof(object), typeof(ShowcaseItemElement), new PropertyMetadata(null));
        public static readonly DependencyProperty DemoBorderBrushProperty = DependencyProperty.Register(nameof(DemoBorderBrush), typeof(object), typeof(ShowcaseItemElement), new PropertyMetadata(null));
        
        public int Order
        {
            get => (int)GetValue(OrderProperty);
            set => SetValue(OrderProperty, value);
        }

        public ObservableCollection<CodeSource> CodeSources
        {
            get => (ObservableCollection<CodeSource>)GetValue(CodeSourcesProperty);
            set => SetValue(CodeSourcesProperty, value);
        }

        public object DemoContent
        {
            get => GetValue(DemoContentProperty);
            set => SetValue(DemoContentProperty, value);
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

        public ShowcaseItemElement()
        {
            CodeSources = new ObservableCollection<CodeSource>();
        }
    }

    public class ShowcaseFrameworkElement : ContentControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(ShowcaseFrameworkElement), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(ShowcaseFrameworkElement), new PropertyMetadata(string.Empty));

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
    }
}