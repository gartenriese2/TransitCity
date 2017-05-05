// ItemPropertyChangedEventArgs.cs by Charles Petzold, December 2008

using System.ComponentModel;

namespace WpfTestApp
{
    public class ItemPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public ItemPropertyChangedEventArgs(object item, string propertyName)
            : base(propertyName)
        {
            Item = item;
        }

        public object Item { get; }
    }

    public delegate void ItemPropertyChangedEventHandler(object sender, ItemPropertyChangedEventArgs args);
}
