using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models.Blocks.Base;

namespace Tetris.Models.Blocks
{
    internal class RhodeIsland : Block
    {
        public RhodeIsland() : base(new SolidColorBrush(Colors.LightCoral), BlockMatrixType._3x3)
        {
        }

        public override object Clone() => new RhodeIsland();

        protected override void SetCellsPositions()
        {
            for (int i = 0; i < 4; i++)
            {
                var blockCell=BlockCells[i];
                var cell = blockCell.Cell;
                Grid.SetRow(cell, i <= 1 ? 1 : 0);
                Grid.SetColumn(cell, i <= 1 ? 5 + i : 4 + i);
                var baseRow = i <= 1 ? 1 : 0;
                var baseColumn = i <= 1 ? i : i - 1;
                Positions[baseRow, baseColumn] = true;
                blockCell.BaseColumn = baseColumn;
                blockCell.BaseRow = baseRow;

            }
        }
    }
}
