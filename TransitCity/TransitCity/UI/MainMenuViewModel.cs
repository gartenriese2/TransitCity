namespace TransitCity.UI
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using MVVM;

    public class MainMenuViewModel : PropertyChangedBase
    {
        private Visibility _continueVisibility = Visibility.Collapsed;

        private ICommand _continueCommand;
        private ICommand _newGameCommand;
        private ICommand _exitCommand;
        private ICommand _escapePressedCommand;

        public event EventHandler StartNewGameEvent;

        public event EventHandler ContinueGameEvent;

        public event EventHandler EscapePressedEvent;

        public Visibility ContinueVisibility
        {
            get { return _continueVisibility; }
            set
            {
                if (_continueVisibility != value)
                {
                    _continueVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ContinueCommand => _continueCommand ?? (_continueCommand = new RelayCommand(p => ContinueGame()));

        public ICommand NewGameCommand => _newGameCommand ?? (_newGameCommand = new RelayCommand(p => StartNewGame()));

        public ICommand ExitCommand => _exitCommand ?? (_exitCommand = new RelayCommand(p => Environment.Exit(0)));

        public ICommand EscapePressedCommand => _escapePressedCommand ?? (_escapePressedCommand = new RelayCommand(p => EscapePressed()));

        private void ContinueGame()
        {
            ContinueGameEvent?.Invoke(null, null);
        }

        private void StartNewGame()
        {
            StartNewGameEvent?.Invoke(null, null);
        }

        private void EscapePressed()
        {
            EscapePressedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
