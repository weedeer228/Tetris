using System.Windows.Controls;
using System.Linq;
using Tetris.Models.Blocks.Base;
using Tetris.ViewModels.Base;
using System;
using System.Windows;

namespace Tetris.ViewModels
{
    internal class NextFigureFieldVM : ViewModel
    {
        private Block _nextBlock;
        private int _fieldRow = 1;
        private int _fieldColumn = 12;
        private void SetNextBlockPosition()
        {
            var blockCells = _nextBlock.BlockCells;
            foreach (var blockCell in blockCells)
            {
                Grid.SetColumn(blockCell.Cell, blockCell.BaseColumn + _fieldColumn + 2);
                Grid.SetRow(blockCell.Cell, blockCell.BaseRow + _fieldRow + 3);
            }
        }
        private UIElement[] GetCells() => _nextBlock.BlockCells.Select(x => x.Cell).ToArray();
        public Block NextBlock { get => _nextBlock; set => Set(ref _nextBlock, ref value); }
        public void ShowBlock(Block nextBlock, Action<UIElement> AddBlockAction = default, Action<UIElement> RemoveBlockAction = default)
        {
            if (nextBlock is null) return;
            _nextBlock = nextBlock;
            SetNextBlockPosition();
            var cells = GetCells();

            foreach (var cell in cells)
            {
                RemoveBlockAction?.Invoke(cell);
                AddBlockAction?.Invoke(cell);
            }

        }
        public void ClearNextFigureField(Action<UIElement> RemoveCellAction = default)
        {
            var cells = GetCells();
            foreach (var cell in cells)
                RemoveCellAction?.Invoke(cell);
        }
        public int Row => _fieldRow;
        public int Column => _fieldColumn;
    }
}