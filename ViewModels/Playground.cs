using System;
using System.Windows;
using System.Windows.Controls;
using Tetris.Models.Blocks.Base;
using Tetris.Services;
using Tetris.ViewModels.Base;

namespace Tetris.ViewModels
{
    internal class PlaygroundVM : ViewModel
    {
        private Visibility _visibility;
        private GameLogic _gameLogic;

        public PlaygroundVM()
        {
            _visibility = Visibility.Visible;
        }
        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                if (_visibility != value)
                    _visibility = value;
            }
        }
        public Grid Playground { get; private set; }
        public void StartGame(Grid playground)
        {
            Playground = playground;
            _gameLogic = new GameLogic();
            _gameLogic.StartGame(playground);
        }
        public void RotateBlock() => _gameLogic.Rotate();
        public ref Block GetNextFigureRef() => ref _gameLogic.GetNextBlockReference();
        public ref int GetScoreRef() => ref _gameLogic.GetScoreReference();
        public ref int GetLevelRef() => ref _gameLogic.GetLevelReference();
        public void MoveBlockToSide(bool isLeft) => _gameLogic.MoveBlockToSide(isLeft);
        public void SetGameSettings(Action ShowBlockAction, Action<int> UpdateSccoreAction, Action<int,int> progressBarAction, Action OnGameOver)
        {
            _gameLogic.SetViewModelsAction(ShowBlockAction, UpdateSccoreAction,progressBarAction, OnGameOver);

        }
       
    }
}
