namespace TransitCity.City.Transit
{
    using System.Collections.ObjectModel;

    public class NetworkViewModel
    {
        public ObservableCollection<StationViewModel> Stations { get; } = new ObservableCollection<StationViewModel>();
    }
}
