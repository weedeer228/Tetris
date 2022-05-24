using Tetris.Models.Blocks.Base;
using System.Windows.Controls;

using System.Windows.Media;

namespace Tetris.Models.Blocks
{




    internal class Hero : Block
    {
        public Hero() : base(new SolidColorBrush(Colors.Aquamarine), BlockMatrixType._4x4)
        {
        }

        public override object Clone() => new Hero();
        protected override void SetCellsPositions()
        {
            ForEach(x => Grid.SetRow(x.Cell, 1));
            for (int i = 0; i < BlockCells.Length; i++)
            {
                Grid.SetColumn(BlockCells[i].Cell, i + 4);
                Positions[1, i] = true;
                BlockCells[i].BaseColumn = i;
                BlockCells[i].BaseRow = 1;
            }
        }
    }
}
