using System;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models.Blocks.Base;

namespace Tetris.Models.Blocks
{
    internal class OrangeRicky : Block
    {
        public OrangeRicky() : base(new SolidColorBrush(Colors.Orange), BlockMatrixType._3x3)
        {
        }
        public override object Clone() => new OrangeRicky();
        protected override void SetCellsPositions()
        {
            for (int i = 0; i < 4; i++)
            {
                var blockcell=BlockCells[i];
                var cell=blockcell.Cell;
                Grid.SetRow(cell, i == 0 ? 2 : 1);
                Grid.SetColumn(cell, (i == 0) ? 5 : 4 + i);
                var baseRow = i == 0 ? 1 : 0;
                var baseColumn = i == 0 ? i : i - 1;
                Positions[baseRow, baseColumn] = true;
                blockcell.BaseColumn= baseColumn;
                blockcell.BaseRow = baseRow;
            }
        }
    }
}