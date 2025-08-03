using System.Collections.ObjectModel;
using System.Windows;
using OpenSilverShowcase.Support.UI.Units;

namespace OpenSilverShowcase.Support.UI.Primitives
{
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
}