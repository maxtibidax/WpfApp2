using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
{
    public partial class StatisticsControl : UserControl
    {
        public StatisticsControl()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var stats = StatisticsModel.LoadStatistics();
            GamesPlayedTextBlock.Text = stats.GamesPlayed.ToString();
            HighScoreTextBlock.Text = stats.HighScore.ToString();
            TotalMovesTextBlock.Text = stats.TotalMoves.ToString();
            AverageScoreTextBlock.Text = stats.GamesPlayed > 0
                ? (stats.TotalScore / stats.GamesPlayed).ToString("F0")
                : "0";
        }

        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new MenuControl();
        }
    }
}