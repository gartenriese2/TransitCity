namespace TransitCity.UI
{
    using System;
    using System.Windows.Input;

    using MVVM;

    public class StartNewGameViewModel : PropertyChangedBase
    {
        private uint _numResidentialBuildings;

        private ICommand _startGameCommand;
        private ICommand _cancelCommand;

        public event EventHandler StartGameEvent;

        public event EventHandler CancelEvent;

        public uint NumResidentialBuildings
        {
            get { return _numResidentialBuildings; }
            set
            {
                if (value != _numResidentialBuildings)
                {
                    _numResidentialBuildings = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand StartGameCommand => _startGameCommand ?? (_startGameCommand = new RelayCommand(p => StartGameEvent?.Invoke(this, null)));

        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(p => CancelEvent?.Invoke(null, null)));
    }
}
