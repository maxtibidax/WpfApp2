using System.Windows;
using System.IO;
using System.Windows.Controls;

namespace WpfApp2
{
    public partial class MenuControl : UserControl
    {
        public MenuControl()
        {
            InitializeComponent();
            ContinueButton.IsEnabled = GameStateModel.LoadGame() != null;
            UserTextBlock.Text = UserManager.CurrentUser?.Username ?? "Гость";
            GlobalMusicManager.Stop();
            string musicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "music", "menu.mp3");
            GlobalMusicManager.PlayMusic(musicPath, true, (float)SettingsControl.MusicVolume);
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new GameControl();
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new GameControl(true);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new SettingsControl();
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new StatisticsControl();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (UserManager.CurrentUser == null)
            {
                var stats = StatisticsModel.LoadStatistics();
                if (stats.GamesPlayed > 0 || stats.HighScore > 0)
                {
                    ((MainWindow)Application.Current.MainWindow).MainContent.Content = new SaveStatsPromptControl();
                    return;
                }
            }
            GlobalMusicManager.Stop();
            Application.Current.Shutdown();
        }
    }
}