using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Tetris.Models;
using Tetris.Models.Blocks;
using Tetris.Models.Blocks.Base;
namespace Tetris.Services
{
    internal class GameLogic
    {
        private static readonly PlaygroundField _field = new PlaygroundField(21, 11);
        private static int _score;
        private static int _level = 1;
        private Action _onAnyBlockActionEnd;
        private Action<int> _onRowDestroy;
        private Action _onScoreUpdate;
        private Action _onGameOver;
        private Block _nextBlock;
        //private bool _isGameOver;
        #region PrivateMethods
        private void SetActions()
        {
            SetAnyBlockActionEnd();
        }
        private void SetAnyBlockActionEnd() => _onAnyBlockActionEnd = () =>
        {
            _field.ClearCells(CurrentBlock);
            CurrentBlock.UpdatePosition();
            _field.FillCells(CurrentBlock);
            _field.FixField();
        };
        private void MoveBlockDown()
        {
            //OnAnyBlockActionStart?.Invoke();
            _field.ClearCells(CurrentBlock);
            BlockMover.MoveDown(CurrentBlock);
            _onAnyBlockActionEnd?.Invoke();
        }
        private void SetTimersParameters()
        {
            TimeController.SetTimerDoneAction(MoveBlockDown);
            TimeController.AddTimerStartAction(() =>
            {

                CurrentBlock = _nextBlock is null ? BlockCreator.CreateRandomBlock() : _nextBlock.Clone().ToBlock();
                if (_nextBlock is not null)
                    _field.RemoveBlock(_nextBlock);
                _nextBlock = BlockCreator.CreateRandomBlock();
                _field.FillCells(CurrentBlock);
            });
            TimeController.SetTimerCondition(CheckCollisionUnderBlock);
            TimeController.AddTimerFalseConditionAction(() =>
            {
                if (_field.isHighestRowCellFilled())
                {
                    _onGameOver?.Invoke();
                    return;
                }
                _onAnyBlockActionEnd?.Invoke();
                if (PointCounter.UpdateScore(RowDestroyer.UpdateRows(CurrentBlock.BlockCells.Select(x => x.Row), _onRowDestroy), _onScoreUpdate))
                    for (int i = 0; i <= RowDestroyer.GetDestroyedRowsCount() - 1; i++)
                        _field.PutRowsDown(RowDestroyer.LowerDestroyedRow + i);
                TimeController.SetTimer(SpeedController.GetSpeedMultiplier());

            });
        }
        private void SetScoreUpdateAction(Action<int, int> action) => _onScoreUpdate = () => action?.Invoke(LevelUpdater.UpdateProgress(), _level);
        private void SetRowDestroyAction() => _onRowDestroy = (int row) =>
        {
            _field.RemoveRow(row);
            // action?.Invoke(LevelUpdater.UpdateProgress());
        };
        private void SetGameOverAction(Action action) => _onGameOver = action;
        private bool CheckCollisionUnderBlock() => CollisionChecker.CanMoveDown(CurrentBlock);


        #endregion
        #region PublicMethods
        public Block CurrentBlock { get; private set; }
        #region Properties
        public ref Block GetNextBlockReference() => ref _nextBlock;
        public ref int GetScoreReference() => ref _score;
        public ref int GetLevelReference() => ref _level;
        #endregion
        public void StartGame(Grid playground)
        {
            // _isGameOver = false;
            _field.Playground = playground;
            _field.ClearPlayground(playground);
            SetActions();
            SetTimersParameters();
            TimeController.SetTimer(SpeedController.GetSpeedMultiplier());
        }
        public void Rotate()
        {
            //TODO: check block collisions for rotation
            CurrentBlock.Rotate();
            _onAnyBlockActionEnd?.Invoke();
        }
        public void MoveBlockToSide(bool isLeft)
        {
            if (isLeft)
            {
                if (CollisionChecker.CanMoveLeft(CurrentBlock))
                    BlockMover.MoveLeft(CurrentBlock);
            }
            else if (CollisionChecker.CanMoveRight(CurrentBlock))
                BlockMover.MoveRight(CurrentBlock);
            _onAnyBlockActionEnd?.Invoke();
        }
        public void SetViewModelsAction(Action NextFigureFieldAction, Action<int> ScoreFieldAction, Action<int, int> progressBarAction, Action OnGameOver)
        {
            TimeController.AddTimerStartAction(NextFigureFieldAction);
            TimeController.AddTimerStartAction(() => ScoreFieldAction(_score));
            SetRowDestroyAction();
            SetScoreUpdateAction(progressBarAction);
            SetGameOverAction(OnGameOver);
        }
        #endregion
        #region Logic Classes
        private class BlockCreator
        {
            private static Random _random = new Random();
            private static readonly Block[] _blocks = { new OrangeRicky(), new Hero(), new BlueRicky(), new Cleveland(), new RhodeIsland(), new Teewee() };
            private static int GetRandomCollectionNumber() => _random.Next(_blocks.Length);
            public static Block CreateRandomBlock() => _blocks[GetRandomCollectionNumber()].Clone() as Block;
        }
        private class BlockMover
        {
            public static void MoveDown(Block currentBlock) => currentBlock.ForEach(x => Grid.SetRow(x.Cell, x.Row + 1));
            public static void MoveUp(Block currentBlock) => currentBlock.ForEach(x => Grid.SetRow(x.Cell, x.Row - 1));
            public static void MoveLeft(Block currentBlock) => currentBlock.ForEach(x => Grid.SetColumn(x.Cell, x.Column - 1));
            public static void MoveRight(Block currentBlock) => currentBlock.ForEach(x => Grid.SetColumn(x.Cell, x.Column + 1));
        }
        private static class CollisionChecker
        {
            private static bool IsLowerCellFilled(BlockCell blockCell, Block parentBlock)
            {
                if (blockCell.Cell is null) throw new NullReferenceException();
                var row = blockCell.Row + 1;
                var column = blockCell.Column;
                if (row >= _field.FieldCells.GetLength(0)) return true;
                return _field.FieldCells[row, column] && !IsFilledCellSameBlock((row, column), parentBlock);
            }
            private static bool IsFilledCellSameBlock((int, int) position, Block currentBlock) => currentBlock.BlockCells.Any(blockcell => (blockcell.Row, blockcell.Column) == position);
            public static bool CanMoveDown(Block currentBlock) => currentBlock.BlockCells.All(x => x.Row < 20 && !IsLowerCellFilled(x, currentBlock));
            private static bool isSideCellsFilled(Block block, bool isLeft)
            {
                foreach (var cell in block.BlockCells)
                {
                    var row = cell.Row;
                    var column = cell.Column + (isLeft ? -1 : 1);
                    if (row >= _field.FieldCells.GetLength(0) || column >= _field.FieldCells.GetLength(1)) break;
                    if (_field.FieldCells[row, column] && !IsFilledCellSameBlock((row, column), block))
                        return true;
                }
                return false;
            }
            public static bool CanMoveRight(Block currentBlock) => currentBlock.BlockCells.Max(x => x.Column) < 10 && !isSideCellsFilled(currentBlock, false);
            public static bool CanRotate(int row, int column)
            {
                if (_field.FieldCells[row, column]) return false;
                if (row > 20 || row < 1) return false;
                if (column > 10 || column < 1) return false;
                return true;

            }
            public static bool CanMoveLeft(Block currentBlock) => currentBlock.BlockCells.Min(x => x.Column) > 1 && !isSideCellsFilled(currentBlock, true);
            //TODO: Check block Collisions for rotation
        }
        private static class TimeController
        {
            private static Action _onTimerDone;
            private static Action _onTimerStart;
            private static Action _onConditionFalse;
            private static Func<bool> _condition;
            private static bool IsTimerParametersReady => _onTimerDone != null && _condition != null;
            public static void SetTimerDoneAction(Action action) => _onTimerDone = action;
            public static void AddTimerStartAction(Action action) => _onTimerStart += action;
            public static void AddTimerFalseConditionAction(Action action)
            {
                _onConditionFalse -= action;
                _onConditionFalse += action;
            }
            public static void SetTimerCondition(Func<bool> condition) => _condition = condition;
            public static async void SetTimer(float delayMultiplier)
            {
                if (!IsTimerParametersReady) throw new NullReferenceException("one or more timer parameters == null");
                _onTimerStart?.Invoke();
                while (_condition.Invoke())
                {
                    await Task.Delay((int)(400 * delayMultiplier));
                    _onTimerDone?.Invoke();
                }
                _onConditionFalse?.Invoke();
            }
        }
        private static class PointCounter
        {
            private static void AddScore(int destroyedLinesCount)
            {
                switch (destroyedLinesCount)
                {
                    case 1:
                        _score += 100;
                        break;
                    case 2:
                        _score += 300;
                        break;
                    case 3:
                        _score += 700;
                        break;
                    default:
                        _score += 1200;
                        break;
                }
            }
            public static bool UpdateScore(int destroyedLinesCount, Action callback)
            {
                if (destroyedLinesCount == 0) return false;
                AddScore(destroyedLinesCount);
                callback?.Invoke();
                return true;
            }
        }
        private class LevelUpdater
        {

            private static int _pointsForLevelUpdate = 200;
            public static int UpdateProgress()
            {
                int progressScore = _score;
                if (_score >= _pointsForLevelUpdate)
                {
                    _level++;
                    progressScore -= _pointsForLevelUpdate;
                    _pointsForLevelUpdate = _pointsForLevelUpdate * 2 + _level * 100;
                }
                var result = (progressScore * 100) / _pointsForLevelUpdate;
                return result;
            }
            public int Progress { get; private set; }

        }
        private class SpeedController
        {
            public static float GetSpeedMultiplier() => 1f / (float)_level;
        }
        private class RowDestroyer
        {
            private static int _destroyedRowsCount;
            private static int _lowerRow;
            private static bool IsRowFilled(int rowNumber)
            {
                var field = _field.FieldCells;
                for (int j = 1; j < 11; j++)
                    if (!field[rowNumber, j]) return false;
                return true;
            }
            public static int UpdateRows(IEnumerable<int> rowNumbers, Action<int> onRowDestroyed = default)
            {
                _destroyedRowsCount = 0;
                _lowerRow = 0;
                for (int i = 0; i < rowNumbers.Count(); i++)
                {
                    var currentRow = rowNumbers.ElementAt(i);
                    if (IsRowFilled(currentRow))
                    {
                        if (_lowerRow is 0)
                            _lowerRow = currentRow;
                        onRowDestroyed?.Invoke(currentRow);
                        _destroyedRowsCount++;
                    }
                }
                return _destroyedRowsCount;
            }
            public static int GetDestroyedRowsCount() => _destroyedRowsCount;
            public static int LowerDestroyedRow => _lowerRow > 0 ? _lowerRow : throw new Exception("lower destroyed row cannot be null");
        }
        #endregion

    }
}
