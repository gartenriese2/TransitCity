namespace TransitCity
{
    using System;
    using System.Windows;

    using MVVM;

    using UI;

    public class MainWindowViewModel : PropertyChangedBase
    {
        private readonly MainMenu _mainMenuControl;
        private readonly StartNewGameControl _startNewGameControl;
        private GameControl _gameControl;

        private object _content;

        public MainWindowViewModel()
        {
            MainMenuViewModel = new MainMenuViewModel();
            _mainMenuControl = new MainMenu { DataContext = MainMenuViewModel };
            StartNewGameViewModel = new StartNewGameViewModel();
            _startNewGameControl = new StartNewGameControl { DataContext = StartNewGameViewModel };

            Content = _mainMenuControl;

            MainMenuViewModel.StartNewGameEvent += (sender, args) => Content = _startNewGameControl;
            MainMenuViewModel.EscapePressedEvent += EscapePressed;
            MainMenuViewModel.ContinueGameEvent += (sender, args) => Content = _gameControl;
            StartNewGameViewModel.StartGameEvent += (sender, args) =>
            {
                GameViewModel = new GameViewModel(((StartNewGameViewModel)sender).NumResidentialBuildings);
                GameViewModel.EscapePressedEvent += EscapePressed;
                _gameControl = new GameControl
                {
                    DataContext = GameViewModel
                };
                _gameControl.CanvasLoaded += (o, eventArgs) => GameViewModel.Initialize();
                Content = _gameControl;
                MainMenuViewModel.ContinueVisibility = Visibility.Visible;
            };
            StartNewGameViewModel.CancelEvent += (sender, args) => Content = _mainMenuControl;
        }

        public MainMenuViewModel MainMenuViewModel { get; }

        public StartNewGameViewModel StartNewGameViewModel { get; }

        public object Content
        {
            get { return _content; }
            set
            {
                if (value != _content)
                {
                    _content = value;
                    OnPropertyChanged();
                }
            }
        }

        private GameViewModel GameViewModel { get; set; }

        private void EscapePressed(object sender, EventArgs args)
        {
            if (sender is GameViewModel)
            {
                Content = _mainMenuControl;
            }
            else if (sender is MainMenuViewModel)
            {
                if (GameViewModel == null || !GameViewModel.Active)
                {
                    MainMenuViewModel.ExitCommand.Execute(null);
                }
                else if (GameViewModel.Active)
                {
                    MainMenuViewModel.ContinueCommand.Execute(null);
                }
            }
        }
    }
}
