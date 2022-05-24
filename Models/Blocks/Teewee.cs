using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models.Blocks.Base;

namespace Tetris.Models.Blocks
{
    internal class Teewee : Block
    {
        public Teewee() : base(new SolidColorBrush(Colors.LightGray), BlockMatrixType._3x3)
        {

        }

        public override object Clone() => new Teewee();

        protected override void SetCellsPositions()
        {
            for (int i = 0; i < 4; i++)
            {
                var blockCell = BlockCells[i];
                var cell = blockCell.Cell;
                Grid.SetColumn(cell, i == 0 ? 6 : 4 + i);
                Grid.SetRow(cell, i == 0 ? 1 : 0);
                var baseRow = i == 0 ? 1 : 0;
                var baseColumn = i == 0 ? 1 : i - 1;
                Positions[baseRow, baseColumn] = true;
                blockCell.BaseColumn = baseColumn;
                blockCell.BaseRow = baseRow;
            }
        }
    }
}
