using System.Windows.Media;
using System.Windows;

namespace OpenSilverShowcase.Support.Local.Helpers
{
    public static class VisualTreeHelperExt
    {
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            while (parentObject != null)
            {
                if (parentObject is T parent)
                    return parent;
                parentObject = VisualTreeHelper.GetParent(parentObject);
            }
            return null;
        }
    }
}
