namespace TransitCity.Models
{
    using MVVM;

    public class ResidentModel : PropertyChangedBase
    {
        private bool _usesTransit;

        public ResidentModel(Connection connection)
        {
            Connection = connection;
        }

        public Connection Connection { get; set; }

        public bool UsesTransit
        {
            get { return _usesTransit; }
            set
            {
                if (value != _usesTransit)
                {
                    _usesTransit = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
