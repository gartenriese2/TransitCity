namespace TransitCity.Models.Transit
{
    using System.Collections.Generic;

    using MVVM;

    using Utility.Coordinates;

    public class Station : PropertyChangedBase
    {
        private const double MaxWalkingDistanceInWorldCoordinates = 500;

        private uint _peopleEntering;
        private uint _peopleExiting;
        private uint _peopleTransfering;

        public Station(ModelPosition position)
        {
            Position = position;
        }

        public ModelPosition Position { get; }

        public List<Line> Lines { get; } = new List<Line>();

        public uint PeopleEntering
        {
            get { return _peopleEntering; }
            set
            {
                if (value != _peopleEntering)
                {
                    _peopleEntering = value;
                    OnPropertyChanged();
                }
            }
        }

        public uint PeopleExiting
        {
            get { return _peopleExiting; }
            set
            {
                if (value != _peopleExiting)
                {
                    _peopleExiting = value;
                    OnPropertyChanged();
                }
            }
        }

        public uint PeopleTransfering
        {
            get { return _peopleTransfering; }
            set
            {
                if (value != _peopleTransfering)
                {
                    _peopleTransfering = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsWithinWalkingDistance(ModelPosition pos) => pos.GetDistanceTo(Position) <= GetMaxWalkingDistanceInModelCoordinates();

        public double GetMaxWalkingDistanceInModelCoordinates() => new WorldPosition(MaxWalkingDistanceInWorldCoordinates, 0).ToModelPosition().X;
    }
}
