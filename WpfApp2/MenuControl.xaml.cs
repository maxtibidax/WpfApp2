using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
{
    public partial class MenuControl : UserControl
    {
        public MenuControl()
        {
            InitializeComponent();

            // Проверяем, есть ли сохраненное состояние игры
            ContinueButton.IsEnabled = GameStateModel.LoadGame() != null;

            // Останавливаем предыдущую музыку
            GlobalMusicManager.Stop();

            // Играем музыку меню
            GlobalMusicManager.PlayMusic(
                "..\\..\\..\\music\\menu.mp3",
                true,
                SettingsControl.MusicVolume
            );
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // Переход к игровому экрану
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new GameControl();
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            // Переход к игровому экрану с продолжением
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new GameControl(true);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            // Переход к экрану настроек
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new SettingsControl();
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Статистика - в разработке");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            GlobalMusicManager.Stop(); // Останавливаем музыку перед выходом
            Application.Current.Shutdown();
        }
    }
}