using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models.Blocks.Base;

namespace Tetris.Models.Blocks
{
    internal class SmashBoy : Block
    {
        public SmashBoy() : base(new SolidColorBrush(Colors.LightPink), BlockMatrixType._2x2)
        {
        }
        public override object Clone() => new SmashBoy();

        protected override void SetCellsPositions()
        {
            for (int i = 0; i < 4; i++)
            {
                var blockCell = BlockCells[i];
                var cell = blockCell.Cell;
                Grid.SetRow( cell, i % 2 == 0 ? 0 : 1);
                Grid.SetColumn(cell, i > 1 ? 6 : 5);
                var baseRow = i % 2 == 0 ? 0 : 1;
                var baseColumn = i > 1 ? 1 : 0;
                Positions[baseRow, baseColumn] = true;
                blockCell.BaseColumn = baseColumn;
                blockCell.BaseRow= baseRow;
            }
        }
    }
}
