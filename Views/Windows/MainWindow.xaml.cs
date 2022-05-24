using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Tetris.Views.Windows
{

    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //StartButton.Visibility = Visibility.Collapsed;
            StartButton.Content = "Restart";
        }

    }
}

