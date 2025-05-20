using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
{
    public partial class LoginControl : UserControl
    {
        private readonly bool _saveStatsMode;

        public LoginControl(bool saveStatsMode = false)
        {
            InitializeComponent();
            _saveStatsMode = saveStatsMode;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите имя пользователя и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = UserModel.Authenticate(username, password);
            if (user != null)
            {
                UserManager.CurrentUser = user;
                if (_saveStatsMode)
                {
                    UserModel.SaveGuestStatsToUser(user);
                    StatisticsModel.ClearGuestStats();
                    GlobalMusicManager.Stop();
                    Application.Current.Shutdown();
                }
                else
                {
                    ((MainWindow)Application.Current.MainWindow).MainContent.Content = new MenuControl();
                }
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите имя пользователя и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (UserModel.Register(username, password))
            {
                MessageBox.Show("Регистрация успешна! Теперь войдите.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                UsernameTextBox.Text = "";
                PasswordBox.Password = "";
            }
            else
            {
                MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (_saveStatsMode)
            {
                ((MainWindow)Application.Current.MainWindow).MainContent.Content = new SaveStatsPromptControl();
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).MainContent.Content = new SettingsControl();
            }
        }
    }
}