using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Jamesnet.Foundation;

namespace OpenSilverShowcase.Support.UI.Primitives
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
}