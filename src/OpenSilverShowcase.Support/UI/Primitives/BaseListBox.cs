using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
namespace OpenSilverShowcase.Support.UI.Primitives
{
    public abstract class BaseListBox : ListBox
    {
        protected bool _isInitializing = false;

        public BaseListBox()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e) { }
        protected virtual void OnUnloaded(object sender, RoutedEventArgs e) { }

        protected override async void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            await HandleItemsChanged(e);
        }

        protected virtual async Task HandleItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                await HandleItemsReset();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                await HandleItemsAdded();
            }
        }

        protected virtual async Task HandleItemsReset()
        {
            _isInitializing = true;
            await Task.Delay(10);
            _isInitializing = false;
        }

        protected virtual async Task HandleItemsAdded() { }
    }
}