using System.ComponentModel;
using System.Windows;

namespace Utility.MVVM
{
    public class ViewModelBase : PropertyChangedBase
    {
        public bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());
    }
}
