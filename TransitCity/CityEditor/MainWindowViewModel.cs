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

        public MainWindowViewModel()
        {
            CanvasViewModel = new CanvasViewModel();

            CreateNewCityCommand = new RelayCommand(o => CreateNewCity());
            LoadCityCommand = new RelayCommand(o => LoadCity());
            SaveCityCommand = new RelayCommand(o => SaveCity(), o => !string.IsNullOrWhiteSpace(CityName) && _city != null);
        }

        public CanvasViewModel CanvasViewModel { get; }

        public RelayCommand CreateNewCityCommand { get; }

        public RelayCommand LoadCityCommand { get; }

        public RelayCommand SaveCityCommand { get; }

        public string CityName
        {
            get => _cityName;
            set
            {
                _cityName = value;
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
    }
}
