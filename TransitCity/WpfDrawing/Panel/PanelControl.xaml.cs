using System.Windows;
using WpfDrawing.Utility;

namespace WpfDrawing.Panel
{
    /// <summary>
    /// Interaction logic for PanelControl.xaml
    /// </summary>
    public partial class PanelControl
    {
        public static readonly DependencyProperty ObjectsProperty = DependencyProperty.Register(
            "Objects",
            typeof(ObservableNotifiableCollection<PanelObject>),
            typeof(PanelControl),
            new PropertyMetadata(new ObservableNotifiableCollection<PanelObject>()));

        public PanelControl()
        {
            InitializeComponent();
        }

        public ObservableNotifiableCollection<PanelObject> Objects
        {
            get => (ObservableNotifiableCollection<PanelObject>)GetValue(ObjectsProperty);
            set => SetValue(ObjectsProperty, value);
        }
    }
}
