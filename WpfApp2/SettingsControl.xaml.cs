using System;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;

namespace WpfApp2
{
    public partial class SettingsControl : UserControl
    {
        public static double MusicVolume { get; private set; } = 0.5; // Громкость по умолчанию

        public SettingsControl()
        {
            InitializeComponent();

            // Загрузка сохраненной громкости
            LoadSettings();

            // Установка текущего значения слайдера
            VolumeSlider.Value = MusicVolume * 100;
        }

        private void LoadSettings()
        {
            try
            {
                // Попытка загрузить громкость из настроек приложения
                //MusicVolume = Properties.Settings.Default.MusicVolume;

                //// Проверка диапазона
                //if (MusicVolume < 0) MusicVolume = 0;
                //if (MusicVolume > 1) MusicVolume = 1;
            }
            catch
            {
                // Если не удалось загрузить - используем значение по умолчанию
                MusicVolume = 0.5;
            }
        }

        private void SaveSettings()
        {
            try
            {
                // Сохранение громкости в настройках приложения
                //Properties.Settings.Default.MusicVolume = MusicVolume;
                //Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения настроек: {ex.Message}");
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Обновляем громкость при изменении слайдера
            if (VolumeSlider != null)
            {
                MusicVolume = VolumeSlider.Value / 100;
                SaveSettings();

                // Обновляем громкость в текущем плеере, если он существует
                UpdateMusicVolume();
            }
        }

        private void UpdateMusicVolume()
        {
            // Логика обновления громкости для текущего музыкального плеера
            // Это будет зависеть от вашей реализации MusicManager
        }

        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        {
            // Возврат в главное меню
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new MenuControl();
        }
    }
}