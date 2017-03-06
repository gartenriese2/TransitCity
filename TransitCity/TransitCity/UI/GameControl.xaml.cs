namespace TransitCity.UI
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using Utility.Coordinates;

    /// <summary>
    /// Interaction logic for GameControl.xaml
    /// </summary>
    public partial class GameControl
    {
        public GameControl()
        {
            InitializeComponent();
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }

        public event EventHandler CanvasLoaded;

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            var cityCanvas = sender as CityCanvas;
            if (cityCanvas != null)
            {
                ViewPosition.Size = Math.Min(cityCanvas.ActualHeight, cityCanvas.ActualWidth);
                CanvasLoaded?.Invoke(null, null);
            }
        }
    }
}
