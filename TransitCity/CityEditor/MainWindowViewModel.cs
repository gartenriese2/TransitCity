namespace CityEditor
{
    using System.Collections.Generic;

    using CityEditor.Canvas;

    using CitySimulation;

    using Utility.MVVM;

    public class MainWindowViewModel : PropertyChangedBase
    {
        private City _city;

        private string _cityName = string.Empty;
        private double _mouseX;
        private double _mouseY;

        public MainWindowViewModel()
        {
            CanvasViewModel = new CanvasViewModel();
            CanvasViewModel.MouseMoved += (sender, point) =>
            {
                MouseX = point.X;
                MouseY = point.Y;
            };

            CreateNewCityCommand = new RelayCommand(o => CreateNewCity());
            LoadCityCommand = new RelayCommand(o => LoadCity());
            SaveCityCommand = new RelayCommand(o => SaveCity(), o => !string.IsNullOrWhiteSpace(CityName) && _city != null);

            CreateNewDistrictCommand = new RelayCommand(o => CreateNewDistrict(), o => !CanvasViewModel.IsInDrawingMode);
        }

        public CanvasViewModel CanvasViewModel { get; }

        public RelayCommand CreateNewCityCommand { get; }

        public RelayCommand LoadCityCommand { get; }

        public RelayCommand SaveCityCommand { get; }

        public RelayCommand CreateNewDistrictCommand { get; }

        public string CityName
        {
            get => _cityName;
            set
            {
                _cityName = value;
                OnPropertyChanged();
            }
        }

        public double MouseX
        {
            get => _mouseX;
            set
            {
                _mouseX = value;
                OnPropertyChanged();
            }
        }

        public double MouseY
        {
            get => _mouseY;
            set
            {
                _mouseY = value;
                OnPropertyChanged();
            }
        }

        private void CreateNewCity()
        {
            CityName = "NewCity";
            _city = new City(CityName, new List<IDistrict>());
        }

        private void LoadCity()
        {

        }

        private void SaveCity()
        {

        }

        private void CreateNewDistrict()
        {
            CanvasViewModel.StartDrawingPolygon();
        }
    }
}
