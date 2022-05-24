using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tetris.Models.Blocks.Base
{
    internal abstract class Block : ICloneable
    {
        private readonly Brush _color;
        private int _blockHeight;
        protected Block(Brush color, BlockMatrixType matrixType)
        {
            _color = color;
            MatrixType = matrixType;
            Init();
            _blockHeight = BlockCells.Max(x => x.Column) - BlockCells.Min(x => x.Column);
        }
        private void CreateCells() => ForEach(x => x.Fill(_color));
        private void Init()
        {
            CreateCells();
            SetCellsPositions();
        }
        protected abstract void SetCellsPositions();
        public abstract object Clone();
        public void UpdatePosition() => ForEach(x => x.UpdatePosition());
        public Block ForEach(Action<BlockCell> action1, Action<BlockCell> action2 = default)
        {
            foreach (var cell in BlockCells)
            {
                action1(cell);
                action2?.Invoke(cell);
            }
            return this;
        }
        public void Rotate()
        {
            BlockRotator.RotateBlock(this);
        }
        public bool[,] Positions { get; set; } = new bool[4, 4];
        public int BlockHeight => _blockHeight;
        public IEnumerator GetEnumerator() => BlockCells.GetEnumerator();
        public BlockMatrixType MatrixType { get; }
        public bool IsToVertical => BlockCells.Max(x => x.Row) - BlockCells.Min(x => x.Row) == 0;
        public BlockCell[] BlockCells { get; protected set; } = new BlockCell[4].Select(x => x = new BlockCell()).ToArray();
        private class BlockRotator
        {
            public static bool[,] TransportMatrix(Block block)
            {
                var matrix = block.Positions;
                var rowCount = matrix.GetLength(0);
                var result = block.MatrixType == BlockMatrixType._4x4 ? new bool[4, 4] : new bool[3, 3];
                for (int i = 0; i < result.GetLength(0); i++)
                    for (int j = 0; j < result.GetLength(0); j++)
                        result[j, i] = matrix[rowCount - i - 1, j];
                return result;
            }
            private static void ConvertLocalPositionToGlobal(bool[,] oldPositions, bool[,] newPositions, Block block)
            {
                var blockCells = block.BlockCells;
                var rowMover = 0;
                var columnMover = 0;
                var blockHeight = block.BlockHeight;
                List<(int, int)> newGlobalPositions = new List<(int, int)>();
                var groupedCells = blockCells.OrderBy(x => x.Row).ThenBy(x => x.Column);
                bool isToVerical = block.IsToVertical;
                foreach (var cell in groupedCells)
                {
                    bool isChanged = false;

                    bool isFirstCell = cell == groupedCells.First();
                    (int?, int) newCellPosition = (null, 0);
                    (int?, int) oldCellPosition = (null, 0);
                    for (int i = 0; i < newPositions.GetLength(0); i++)
                    {
                        for (int j = 0; j < newPositions.GetLength(1); j++)
                        {
                            if (newPositions[i, j] && newCellPosition.Item1 is null)
                            {
                                newCellPosition = (i, j);
                                newPositions[i, j] = false;
                            }
                            if (oldPositions[i, j] && oldCellPosition.Item1 is null)
                            {
                                oldCellPosition = (i, j);
                                oldPositions[i, j] = false;
                            }
                            if (newCellPosition.Item1 is not null && oldCellPosition.Item1 is not null)
                            {
                                var newColumn = cell.Column;
                                var newRow = cell.Row;
                                while (oldCellPosition.Item1 != newCellPosition.Item1)
                                {
                                    newRow += newCellPosition.Item1 < oldCellPosition.Item1 ? -1 : 1;
                                    oldCellPosition.Item1 += newCellPosition.Item1 < oldCellPosition.Item1 ? -1 : 1;
                                }
                                while (oldCellPosition.Item2 != newCellPosition.Item2)
                                {
                                    newColumn += newCellPosition.Item2 < oldCellPosition.Item2 ? -1 : 1;
                                    oldCellPosition.Item2 += newCellPosition.Item2 < oldCellPosition.Item2 ? -1 : 1;
                                }
                                if (isFirstCell)
                                {
                                    if (newColumn + blockHeight > 10 && columnMover == 0 && !isToVerical)
                                        columnMover = newColumn - 10 - (block.MatrixType == BlockMatrixType._4x4 ? 1 : 0);
                                    else if (newColumn - blockHeight < 1 && columnMover == 0 && !isToVerical)
                                        columnMover = blockHeight - newColumn + (block.MatrixType == BlockMatrixType._4x4 ? -1 : 0);
                                    if (newRow + blockHeight > 20 && rowMover == 0 && isToVerical)
                                        rowMover = newRow - 21;
                                    else if (newRow - blockHeight + 1 < 0 && rowMover == 0 && isToVerical)
                                        rowMover = blockHeight - newRow + (block.MatrixType == BlockMatrixType._4x4 ? -2 : -1); ;

                                }

                                Grid.SetColumn(cell.Cell, newColumn + (columnMover < -2 ? -2 : columnMover));
                                Grid.SetRow(cell.Cell, newRow + rowMover);
                                isChanged = true;
                                newGlobalPositions.Add((newRow, newColumn));
                                newCellPosition = (null, 0);
                                oldCellPosition = (null, 0);
                                break;
                            }
                        }
                        if (isChanged)
                            break;
                    }
                }
            }
            private static bool[,] CreateMatrix(Block block)
            {
                var matrix = block.Positions;
                var result = block.MatrixType == BlockMatrixType._4x4 ? new bool[4, 4] : new bool[3, 3];
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        switch (block.MatrixType)
                        {
                            case BlockMatrixType._3x3:
                                if (i < 3 && j < 3)
                                    result[i, j] = matrix[i, j];
                                break;
                            case BlockMatrixType._4x4:
                                result[i, j] = matrix[i, j];
                                break;
                            default:
                                break;
                        }
                    }
                }
                return result;
            }
            public static void RotateBlock(Block block)
            {
                if (block.MatrixType == BlockMatrixType._2x2) return;
                var oldpos = CreateMatrix(block);
                block.Positions = CreateMatrix(block);
                block.Positions = TransportMatrix(block);
                var newPosition = CreateMatrix(block);
                ConvertLocalPositionToGlobal(oldpos, newPosition, block);
            }
        }
    }
    internal class BlockCell
    {
        private int _row;
        private int _column;
        public BlockCell()
        {
            Cell = new Rectangle();
        }
        private void UpdateRow() => _row = Grid.GetRow(Cell);
        private void UpdateColumn() => _column = Grid.GetColumn(Cell);
        public void Fill(Brush color) => Cell.Fill = color;
        public void UpdatePosition()
        {
            UpdateColumn();
            UpdateRow();
        }
        public Rectangle Cell { get; }
        public int Row
        {
            get => _row > 0 ? _row : Grid.GetRow(Cell);
        }

        public int Column
        {
            get => _column > 0 ? _column : Grid.GetColumn(Cell);
        }
        public int BaseRow { get; set; }
        public int BaseColumn { get; set; }

    }
    internal static class TetrisExtensions
    {
        public static Block ToBlock(this object obj) => obj as Block is null ? throw new Exception($"it's impossible to transform{obj.GetType()} to block")
            : obj as Block;

    }
    internal enum BlockMatrixType
    {
        _3x3,
        _4x4,
        _2x2,
    }

}