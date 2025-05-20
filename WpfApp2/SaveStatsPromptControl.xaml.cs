using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
{
    public partial class SaveStatsPromptControl : UserControl
    {
        public SaveStatsPromptControl()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new LoginControl(true);
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new LoginControl(true);
        }

        private void ExitWithoutSaving_Click(object sender, RoutedEventArgs e)
        {
            StatisticsModel.ClearGuestStats();
            GlobalMusicManager.Stop();
            Application.Current.Shutdown();
        }
    }
}