using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models.Blocks.Base;

namespace Tetris.Models.Blocks
{
    internal class BlueRicky : Block
    {
        public BlueRicky() : base(new SolidColorBrush(Colors.SkyBlue), BlockMatrixType._3x3)
        {
        }

        public override object Clone() => new BlueRicky();

        protected override void SetCellsPositions()
        {
            for (int i = 0; i < 4; i++)
            {
                var blockCell = BlockCells[i];
                var cell = blockCell.Cell;
                Grid.SetRow(cell, i == 0 ? 2 : 1);
                Grid.SetColumn(cell, (i == 0) ? 7 : 4 + i);
                var baseRow = i == 0 ? 1 : 0;
                var baseColumn = i == 0 ? 2 : i - 1;
                Positions[baseRow, baseColumn] = true;
                blockCell.BaseColumn = baseColumn;
                blockCell.BaseRow = baseRow;
            }
        }
    }
}
