using System.Windows;
using System.Windows.Media;

namespace WpfDrawing.Objects
{
    public class PanelObject : FrameworkElement
    {
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill",
            typeof(Brush),
            typeof(PanelObject),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            "X",
            typeof(double),
            typeof(PanelObject),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
            "Y",
            typeof(double),
            typeof(PanelObject),
            new PropertyMetadata(0.0));

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public double X
        {
            get => (double)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public double Y
        {
            get => (double)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }
    }
}
