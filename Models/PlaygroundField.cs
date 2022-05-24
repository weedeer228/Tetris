using System.Windows.Controls;
using Tetris.Models.Blocks.Base;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using System;

namespace Tetris.Models
{
    internal class PlaygroundField
    {
        public bool[,] FieldCells { get; private set; }
        private Grid _playground;
        public Grid Playground
        {
            get => _playground;
            set
            {
                if (_playground == null)
                    _playground = value;
            }
        }
        public PlaygroundField(int rows, int columns)
        {
            FieldCells = new bool[rows, columns];
        }
        private void FillCell(int row, int column) => FieldCells[row, column] = true;
        private void ClearCell(int row, int column) => FieldCells[row, column] = false;
        private void RemoveCell(BlockCell cell) => Playground.Children.Remove(cell.Cell);
        private void RemoveCell(int row, int column)
        {
            var cells = from cell in Playground.Children.OfType<Rectangle>()
                        where Grid.GetRow(cell) == row && Grid.GetColumn(cell) == column
                        select cell;
            var cellCount = cells.Count();
            for (int i = 0; i < cellCount; i++)
                Playground.Children.Remove(cells.First());
        }
        private void AddCell(BlockCell cell) => Playground.Children.Add(cell.Cell);
        private void ClearCell(BlockCell cell) => ClearCell(cell.Row, cell.Column);
        private void ClearField() => FieldCells = new bool[21, 11];
        private void FillCell(BlockCell cell) => FillCell(cell.Row, cell.Column);
        private int GetMaxRow() => _playground.Children.OfType<Rectangle>().Max(x => Grid.GetRow(x));
        public void ClearCells(Block block) => block.ForEach(ClearCell, RemoveCell);
        public void FillCells(Block block) => block.ForEach(FillCell).ForEach(AddCell);
        public void RemoveRow(int row, Action callback = default)
        {
            for (int i = 1; i < 11; i++)
            {
                ClearCell(row, i);
                RemoveCell(row, i);
            }
            callback?.Invoke();
        }
        public void RemoveBlock(Block block) => block.ForEach(RemoveCell);
        public void FixField()
        {
            var cells = from children in _playground.Children.OfType<Rectangle>()
                        select (Grid.GetRow(children), Grid.GetColumn(children));

            foreach (var cell in cells.Where(x => x.Item2 < 11))
                if (!FieldCells[cell.Item1, cell.Item2])
                    FieldCells[cell.Item1, cell.Item2] = true;

            for (int i = 0; i < FieldCells.GetLength(0); i++)
                for (int j = 0; j < FieldCells.GetLength(1); j++)
                    if (FieldCells[i, j])
                        if (!cells.Any(x => x == (i, j)))
                            FieldCells[i, j] = false;



        }
        public void PutRowsDown(int MinDestroyedRow)
        {
            var cellsToMoveDown = _playground.Children.OfType<Rectangle>().Where(x => /*Grid.GetRow(x) <= GetMaxRow()
            &&*/ Grid.GetRow(x) <= MinDestroyedRow && Grid.GetColumn(x) < 11);
            //var tmp2 = GetMaxRow();
            foreach (var cell in cellsToMoveDown)
            {
                var row = Grid.GetRow(cell);
                var column = Grid.GetColumn(cell);
                ClearCell(row, column);
                row++;
                Grid.SetRow(cell, row);
                FillCell(row, column);
            }
        }

        public void ClearPlayground(Grid playground)
        {
            var cells = (from children in playground.Children.OfType<UIElement>()
                         where children is Rectangle
                         select children).ToList();
            cells.ForEach(x => playground.Children.Remove(x));
            ClearField();
        }

        public bool isHighestRowCellFilled()
        {
            for (int i = 1; i < 11; i++)
                if (FieldCells[1, i] && FieldCells[2, i])
                    return true;
            return false;
        }
    }
}
