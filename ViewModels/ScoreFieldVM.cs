using System.ComponentModel;
using Tetris.ViewModels.Base;

namespace Tetris.ViewModels
{
    internal class ScoreFieldVM : ViewModel
    {
        private int _score = 0;
        public int Score
        {
            get => _score;
            set
            {
                Set(ref _score, ref value, "Score");
            }
        }

        public void SetValueFromString(string value)
        {
            int intValue;
            try
            {
                intValue = int.Parse(value);
                Set(ref _score, ref intValue);

            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
