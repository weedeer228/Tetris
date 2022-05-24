using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Tetris.Infracstructure.Commands;
using Tetris.Infrastructure.Commands;
using Tetris.ViewModels.Base;
using System.Diagnostics;
using System.Windows;

namespace Tetris.ViewModels
{
    public class MainWindowVievModel : ViewModel
    {
        #region privateFields
        private int _width = 320;
        private int _height = 520;
        private Grid _mainWindowGrid;
        private Visibility _playgroundVisibility = Visibility.Collapsed;
        private bool _isStarted;
        #region ViewModels
        private PlaygroundVM _playground = new PlaygroundVM();
        private NextFigureFieldVM _nextFigureFieldVM = new NextFigureFieldVM();
        private ScoreFieldVM _scoreFieldVM = new ScoreFieldVM();
        private LevelFieldVM _levelFieldVM;
        #endregion
        #endregion
        #region Commands
        private ICommand _startCommand;
        private ICommand _rotateCommand;
        private ICommand _moveToSideCommand;
        private bool CanStartCommandExecute(object o) => o is Grid;
        private bool CanRotateCommandExecute(object o) => _isStarted;
        private bool CanMoveToSideCommandExecute(object o) => bool.TryParse(o.ToString(), out var res) && _isStarted;
        private void OnStartCommandExecuted(object o)
        {
            if (_isStarted)
                Restart();
            _mainWindowGrid = o as Grid;
            PlaygroundVisibility = Visibility.Visible;
            StartButtonVisibility = Visibility.Collapsed;
            OnPropertyChanged(nameof(StartButtonVisibility));
            _isStarted = true;
            StartGame();
        }
        private void OnRotateCommandExecuted(object o) => _playground.RotateBlock();
        private void OnMoveToSideCommandExecuted(object o) => _playground.MoveBlockToSide(Convert.ToBoolean(o));
        public ICommand StartCommand
        {
            get
            {
                if (_startCommand is null)
                    _startCommand = new LambdaCommand(OnStartCommandExecuted, CanStartCommandExecute);
                return _startCommand;
            }
        }
        public ICommand RotateCommand
        {
            get
            {
                if (_rotateCommand is null)
                    _rotateCommand = new UIRelayCommand(OnRotateCommandExecuted, CanRotateCommandExecute, Key.Up, ModifierKeys.None);
                return _rotateCommand;
            }
        }
        public ICommand MoveToSideCommand
        {
            get
            {
                if (_moveToSideCommand is null)
                    _moveToSideCommand = new LambdaCommand(OnMoveToSideCommandExecuted, CanMoveToSideCommandExecute);
                return _moveToSideCommand;
            }
        }
        #endregion
        private void UpdateNextFigureField()
        {
            _nextFigureFieldVM.ClearNextFigureField();
            _nextFigureFieldVM.ShowBlock(_playground.GetNextFigureRef(), (uIElement) => _mainWindowGrid.Children.Add(uIElement),RemoveBlockAction);
        }
        private void StartGame()
        {

            Action<int> updateScoreAction = CreateUpdateScoreAction();
            Action<int, int> updateLevelAction = CreateUpdateLevelAction();
            _playground.StartGame(_mainWindowGrid);
            SetLevelField();
            _playground.SetGameSettings(UpdateNextFigureField, updateScoreAction, updateLevelAction, OnGameOver);
            SetSCoreField();
            SetNextFigureField();
        }
        private void RemoveBlockAction(UIElement element)
        {
            if (element is Rectangle && _mainWindowGrid.Children.Contains(element))
                _mainWindowGrid.Children.Remove(element);
        }
        private void SetLevelField()
        {
            var progressBar = _mainWindowGrid.FindName("ProgressBar");
            _levelFieldVM = new LevelFieldVM(progressBar as RadialProgressBarUC);
            _levelFieldVM.Level = _playground.GetLevelRef();
        }
        private void SetNextFigureField()
        {
            _nextFigureFieldVM.NextBlock = _playground.GetNextFigureRef();
            _nextFigureFieldVM.ShowBlock(_playground.GetNextFigureRef(), (uIElement) => _mainWindowGrid.Children.Add(uIElement), (uIElement) => _mainWindowGrid.Children.Remove(uIElement));
        }
        private void SetSCoreField() => _scoreFieldVM.Score = _playground.GetScoreRef();
        private void Restart()
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }
        private Action<int> CreateUpdateScoreAction() => (int score) => Score = score.ToString();
        private Action<int, int> CreateUpdateLevelAction() => (int progress, int level) =>
        {
            Level = level;
            _levelFieldVM.Start(progress);
        };
        private void OnGameOver()
        {
            GameOverBlockVisibility = Visibility.Visible;

            StartButtonVisibility = Visibility.Visible;
            PlaygroundVisibility = Visibility.Collapsed;
            OnPropertyChanged("");
        }
        #region Properties
        public int Width => _width;
        public int Height => _height;
        public Visibility PlaygroundVisibility
        {
            get => _playgroundVisibility;
            set => Set(ref _playgroundVisibility, ref value);
        }
        public string Score
        {
            get => _scoreFieldVM.Score.ToString();
            set
            {
                _scoreFieldVM.SetValueFromString(value);
                OnPropertyChanged(nameof(Score));
            }
        }
        public int NextFigureFieldRow => _nextFigureFieldVM.Row;
        public int NextFigureFieldColumn => _nextFigureFieldVM.Column;
        public int Level
        {
            get => _levelFieldVM is null ? 1 : _levelFieldVM.Level;
            set
            {
                _levelFieldVM.SetLevel(value);
                OnPropertyChanged(nameof(Level));
            }
        }
        public Visibility GameOverBlockVisibility { get; private set; } = Visibility.Collapsed;
        public Visibility StartButtonVisibility { get; private set; } = Visibility.Visible;
        #endregion
    }
}
