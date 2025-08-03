using System;

namespace OpenSilverShowcase.Support.Local.Models
{
    public class ItemClickedEventArgs : EventArgs
    {
        public object Item { get; }

        public ItemClickedEventArgs(object item)
        {
            Item = item;
        }
    }
}
