namespace TransitCity.UI
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    using Models;
    using MVVM;

    using Utility;

    public class GameViewModel : PropertyChangedBase
    {
        private bool _showResidents = true;
        private bool _showJobs = true;
        private bool _showConnections;

        private ICommand _openMenuCommand;

        public GameViewModel(uint numResidentialBuildings)
        {
            UiModel = new UiModel();
            CityModel = new CityModel(numResidentialBuildings, 10000);
            CityCanvasViewModel = new CityCanvasViewModel(CityModel, UiModel);
        }

        public event EventHandler EscapePressedEvent;

        public UiModel UiModel { get; }

        public CityModel CityModel { get; }

        public CityCanvasViewModel CityCanvasViewModel { get; }

        public bool Active { get; private set; }

        public bool ShowResidents
        {
            get { return _showResidents; }
            set
            {
                if (_showResidents != value)
                {
                    _showResidents = value;
                    OnPropertyChanged();

                    SetConnectionsVisibility(CityCanvasViewModel.Residents, _showResidents ? Visibility.Visible : Visibility.Collapsed);
                }
            }
        }

        public bool ShowJobs
        {
            get { return _showJobs; }
            set
            {
                if (_showJobs != value)
                {
                    _showJobs = value;
                    OnPropertyChanged();

                    SetConnectionsVisibility(CityCanvasViewModel.Jobs, _showJobs ? Visibility.Visible : Visibility.Collapsed);
                }
            }
        }

        public bool ShowConnections
        {
            get { return _showConnections; }
            set
            {
                if (_showConnections != value)
                {
                    _showConnections = value;
                    OnPropertyChanged();

                    SetConnectionsVisibility(CityCanvasViewModel.Connections, _showConnections ? Visibility.Visible : Visibility.Collapsed);
                }
            }
        }

        public ICommand OpenMenuCommand => _openMenuCommand ?? (_openMenuCommand = new RelayCommand(p => OpenMenu()));

        public void Initialize()
        {
            var win = new BusyWindow
            {
                Owner = Application.Current.MainWindow,
                Width = Application.Current.MainWindow.Width,
                Height = Application.Current.MainWindow.Height
            };
            win.Show();
            CityModel.Initialize();
            CityCanvasViewModel.Initialize();
            SetConnectionsVisibility(CityCanvasViewModel.Connections, Visibility.Collapsed);
            Active = true;
            win.Close();
        }

        private void SetConnectionsVisibility(IEnumerable<DrawableViewModel> drawableViewModels, Visibility visibility)
        {
            foreach (var drawableViewModel in drawableViewModels)
            {
                drawableViewModel.IsVisible = visibility;
            }
        }

        private void OpenMenu()
        {
            EscapePressedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
