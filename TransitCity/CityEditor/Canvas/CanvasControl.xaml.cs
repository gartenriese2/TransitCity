namespace CityEditor.Canvas
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for CanvasControl.xaml
    /// </summary>
    public partial class CanvasControl
    {
        /// <summary>
        /// The initial actual size of the control.
        /// </summary>
        private Size _initialSize;

        /// <summary>
        /// Indicates whether the initial size has been set.
        /// </summary>
        private bool _initialSizeIsSet;

        public CanvasControl()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var panelControl = (CanvasControl)sender;
            _initialSize = new Size(panelControl.ActualWidth, panelControl.ActualHeight);

            // DataContext has been set before control has been loaded. Set initial size now.
            if (DataContext != null)
            {
                var panelControlViewModel = (CanvasViewModel)DataContext;
                panelControlViewModel.SetInitialViewSize(_initialSize);
            }
            else
            {
                _initialSizeIsSet = true;
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Control has not been loaded yet. Initial size will be set in the OnLoaded method.
            if (!_initialSizeIsSet)
            {
                return;
            }

            var panelControlViewModel = (CanvasViewModel)e.NewValue;
            panelControlViewModel.SetInitialViewSize(_initialSize);
        }
    }
}
