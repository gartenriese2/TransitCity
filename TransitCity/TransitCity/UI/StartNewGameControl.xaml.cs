using System.Windows.Input;

namespace TransitCity.UI
{
    /// <summary>
    /// Interaction logic for StartNewGameControl.xaml
    /// </summary>
    public partial class StartNewGameControl
    {
        public StartNewGameControl()
        {
            InitializeComponent();
            Focusable = true;
            Loaded += (sender, args) => Keyboard.Focus(this);
        }
    }
}
