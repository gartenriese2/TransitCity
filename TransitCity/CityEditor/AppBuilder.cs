namespace CityEditor
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Windows;

    public static class AppBuilder
    {
        /// <summary>
        /// The WPF starting point.
        /// </summary>
        private static App _app;

        /// <summary>
        /// The main window view model.
        /// </summary>
        private static MainWindowViewModel _mainWindowViewModel;

        /// <summary>
        /// The function object to create the main window view model.
        /// </summary>
        private static Func<MainWindowViewModel> _mainWindowViewModelCreator;

        /// <summary>
        /// Variable entry point to allow derived models.
        /// </summary>
        /// <param name="mainWindowViewModelCreator">The function to create the <see cref="MainWindowViewModel"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="mainWindowViewModelCreator"/> is <see langword="null"/>.</exception>
        public static void CreateApp(Func<MainWindowViewModel> mainWindowViewModelCreator)
        {
            _mainWindowViewModelCreator = mainWindowViewModelCreator ?? throw new ArgumentNullException(nameof(mainWindowViewModelCreator));

            // Kill process if already started to avoid malfunction
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Length > 1)
            {
                Process.GetCurrentProcess().Kill();
            }

            // Setting invariant culture to avoid culture bugs
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // Setup application
            _app = new App { MainWindow = new MainWindow() };
            _app.Startup += OnAppStartup;

            if (_app.MainWindow != null)
            {
                _app.MainWindow.Closed += OnMainWindowClosed;
            }

            // Start application
            _app.Run();
        }

        /// <summary>
        /// Creates the main window view model and sets it as the data context of the main window. Then shows the main window.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="startupEventArgs">The startup event arguments.</param>
        private static void OnAppStartup(object sender, StartupEventArgs startupEventArgs)
        {
            if (_app.MainWindow == null)
            {
                return;
            }

            // Initialize ViewModel
            _mainWindowViewModel = _mainWindowViewModelCreator();

            // Set DataContext
            _app.MainWindow.DataContext = _mainWindowViewModel;

            // Show MainWindow
            _app.MainWindow.Show();
        }

        /// <summary>
        /// Shuts the WPF application down.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnMainWindowClosed(object sender, EventArgs e)
        {
            _app.Shutdown();
        }
    }
}
