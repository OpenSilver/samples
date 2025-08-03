using System.Windows;
using System.Windows.Controls;

namespace OpenSilverShowcase.Support.UI.Units;

public class ShowcaseHeadline : ContentControl
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(ShowcaseHeadline), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(ShowcaseHeadline), new PropertyMetadata(string.Empty));

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

    public ShowcaseHeadline()
    {
        DefaultStyleKey = typeof(ShowcaseHeadline);
    }
}
