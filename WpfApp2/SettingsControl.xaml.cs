using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace WpfApp2
{
    public partial class SettingsControl : UserControl
    {
        public static double MusicVolume { get; private set; } = 0.5;
        public static int GridSize { get; private set; } = 4;

        private static string SettingsFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "2048Game",
            "settings.json"
        );

        public SettingsControl()
        {
            InitializeComponent();
            LoadSettings();
            UpdateAccountUI();
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    var settings = JsonConvert.DeserializeObject<SettingsData>(json);
                    if (settings != null)
                    {
                        MusicVolume = settings.MusicVolume;
                        GridSize = settings.GridSize;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки настроек: {ex.Message}");
            }

            VolumeSlider.Value = MusicVolume * 100;
            GridSizeComboBox.SelectedItem = FindGridSizeComboBoxItem(GridSize);
            GlobalMusicManager.SetVolume((float)MusicVolume);
        }

        private ComboBoxItem FindGridSizeComboBoxItem(int size)
        {
            foreach (ComboBoxItem item in GridSizeComboBox.Items)
            {
                if (item.Tag != null && int.Parse(item.Tag.ToString()) == size)
                {
                    return item;
                }
            }
            return GridSizeComboBox.Items[1] as ComboBoxItem; // Default to 4x4
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new SettingsData
                {
                    MusicVolume = MusicVolume,
                    GridSize = GridSize
                };
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath));
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения настроек: {ex.Message}");
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VolumeSlider != null)
            {
                MusicVolume = VolumeSlider.Value / 100;
                SaveSettings();
                GlobalMusicManager.SetVolume((float)MusicVolume);
            }
        }

        private void GridSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridSizeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                GridSize = int.Parse(selectedItem.Tag.ToString());
                SaveSettings();
            }
        }

        private void UpdateAccountUI()
        {
            if (UserManager.CurrentUser != null)
            {
                AccountStatusTextBlock.Text = $"Аккаунт: {UserManager.CurrentUser.Username}";
                LoginButton.Visibility = Visibility.Collapsed;
                RegisterButton.Visibility = Visibility.Collapsed;
                LogoutButton.Visibility = Visibility.Visible;
                DeleteAccountButton.Visibility = Visibility.Visible;
            }
            else
            {
                AccountStatusTextBlock.Text = "Гость";
                LoginButton.Visibility = Visibility.Visible;
                RegisterButton.Visibility = Visibility.Visible;
                LogoutButton.Visibility = Visibility.Collapsed;
                DeleteAccountButton.Visibility = Visibility.Collapsed;
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new LoginControl();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new LoginControl();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            UserManager.CurrentUser = null;
            UpdateAccountUI();
            MessageBox.Show("Вы вышли из аккаунта.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить аккаунт? Это действие нельзя отменить.",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                UserModel.DeleteAccount(UserManager.CurrentUser.Username);
                UserManager.CurrentUser = null;
                UpdateAccountUI();
                MessageBox.Show("Аккаунт успешно удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new MenuControl();
        }
    }

    [Serializable]
    public class SettingsData
    {
        public double MusicVolume { get; set; }
        public int GridSize { get; set; }
    }
}