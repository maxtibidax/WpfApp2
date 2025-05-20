using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
{
    public partial class MenuControl : UserControl
    {
        public MenuControl() 
        { 
            InitializeComponent(); 
            ContinueButton.IsEnabled = GameStateModel.LoadGame() != null;
            GlobalMusicManager.Stop(); 
            GlobalMusicManager.PlayMusic(
                "..\\..\\..\\music\\menu.mp3", 
                true, 
                SettingsControl.MusicVolume); 
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
            GlobalMusicManager.Stop();
            Application.Current.Shutdown();
        }
    }

}