namespace TransitCity.UI
{
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu
    {
        public MainMenu()
        {
            InitializeComponent();
            Focusable = true;
            Loaded += (sender, args) => Keyboard.Focus(this);
        }
    }
}
