namespace TransitCity.Models.Transit
{
    using System.Collections.Generic;

    using TransitCity.MVVM;

    public class Line : PropertyChangedBase
    {
        private uint _ridership;

        public Line(uint number, string name = null)
        {
            Name = name ?? number.ToString();
        }

        public List<StationConnection> StationConnections { get; } = new List<StationConnection>();

        public string Name { get; }

        public uint Number { get; }

        public int NumStations => GetStations().Count;

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

        public List<Station> GetStations()
        {
            if (StationConnections == null)
            {
                return null;
            }

            var list = new List<Station>(StationConnections.Count + 1) { StationConnections[0].StationA };
            foreach (var stationConnection in StationConnections)
            {
                list.Add(stationConnection.StationB);
            }

            return list;
        }
    }
}
