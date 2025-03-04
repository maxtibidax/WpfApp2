using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
{
    public partial class MenuControl : UserControl
    {
        public MenuControl()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // Переход к игровому экрану
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new GameControl();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Настройки - в разработке");
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Статистика - в разработке");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}