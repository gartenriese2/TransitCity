namespace TransitCity.Models
{
    using System;

    using MVVM;

    public class UiModel : PropertyChangedBase
    {
        private bool _buildMetroChecked;

        public event EventHandler MetroBuildFinished;

        public bool BuildMetroChecked
        {
            get { return _buildMetroChecked; }
            set
            {
                if (value != _buildMetroChecked)
                {
                    _buildMetroChecked = value;
                    OnPropertyChanged();

                    if (!value)
                    {
                        MetroBuildFinished?.Invoke(null, null);
                    }
                }
            }
        }
    }
}
