using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Tetris.ViewModels.Base;

namespace Tetris.ViewModels
{
    internal class LevelFieldVM : ViewModel
    {
        private bool _isStarted;
        private RadialProgressBarUC _progressBar;
        private int _progress;
        private int _level;
        public LevelFieldVM(RadialProgressBarUC progressBar)
        {
            _progressBar = progressBar;
        }
        DispatcherTimer _timer = new DispatcherTimer();
        private int _counter = 0;
        private void TimerTick(object sender, EventArgs e)
        {
            _counter++;
            if (_counter >= _progress)
            {
                StopTimer();
                StopAnimation();
                if (_progress == 0)
                    _isStarted = false;
                /// TimerLabel.Text = "0".ToString();
            }
        }
        private void StartTimer()
        {
            _progressBar.Visibility = Visibility.Visible;
            if (_counter >= 100|| !_isStarted)
                _counter = 0;
            
            if (!_isStarted)
            {
                _timer.Tick -= TimerTick;
                _timer.Interval = TimeSpan.FromMilliseconds(188);
                _timer.Tick += TimerTick;
            }
            _timer.Start();
        }
        private void StopTimer()
        {
            //_timer.Tick -= TimerTick;
            _timer.Stop();
        }
        private void StartAnimation()
        {
            var animation = ((Storyboard)_progressBar.Resources["ProgressBarAnimation"]);

            if (!_isStarted)
            {
                animation.Begin();
                _isStarted = true;
                _progressBar.Visibility = Visibility.Visible;
                return;
            }
            animation.Resume();
        }
        private void StopAnimation()
        {
            ((Storyboard)_progressBar.Resources["ProgressBarAnimation"]).Pause();
        }
        private void UpdateProgress(int value) => _progress = value;
        public void Start(int progress)
        {
            UpdateProgress(progress);
            StartTimer();
            StartAnimation();
        }
        public void SetLevel(int value) => Set(ref _level, ref value, "Level");
        public int Level { get => _level; set => Set(ref _level, ref value); }
    }
}

