namespace TransitCity.Models
{
    using System.Collections.Generic;

    using MVVM;

    public class MetroLineModel : PropertyChangedBase
    {
        private uint _ridership;

        public List<MetroStationModel> Stations { get; } = new List<MetroStationModel>();

        public int Number { get; set; }

        public int NumStations => Stations.Count;

        public uint Ridership
        {
            get { return _ridership; }
            set
            {
                if (value != _ridership)
                {
                    _ridership = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
