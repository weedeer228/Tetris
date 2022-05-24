using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models.Blocks.Base;

namespace Tetris.Models.Blocks
{
    internal class Cleveland : Block
    {
        public Cleveland() : base(new SolidColorBrush(Colors.Lime), BlockMatrixType._3x3)
        {
        }

        public override object Clone() => new Cleveland();


        protected override void SetCellsPositions()
        {
            for (int i = 0; i < 4; i++)
            {
                var blockCell = BlockCells[i];
                var cell = blockCell.Cell;
                Grid.SetRow(cell, i > 1 ? 1 : 0);
                Grid.SetColumn(cell, i > 1 ? 4 + i : 5 + i);
                var baseRow = i > 1 ? 1 : 0;
                var baseColumn = i > 1 ? i - 1 : i;
                Positions[baseRow, baseColumn] = true;
                blockCell.BaseColumn = baseColumn;
                blockCell.BaseRow = baseRow;
            }
        }
    }
}
