using System;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;

namespace WpfApp2
{
    public partial class SettingsControl : UserControl
    {
        public static double MusicVolume { get; private set; } = 0.5; // Громкость по умолчанию
        public static int GridSize { get; private set; } = 4; // Размер сетки по умолчанию

        public SettingsControl()
        {
            InitializeComponent();

            // Загрузка сохраненных настроек
            LoadSettings();

            // Установка текущего значения слайдера и размера сетки
            VolumeSlider.Value = MusicVolume * 100;
            GridSizeComboBox.SelectedItem = FindGridSizeComboBoxItem(GridSize);
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
            return null;
        }

        private void LoadSettings()
        {
            try
            {
                // Здесь можно добавить загрузку из постоянных настроек
                // Пока используем значения по умолчанию
            }
            catch
            {
                MusicVolume = 0.5;
                GridSize = 4;
            }
        }

        private void SaveSettings()
        {
            try
            {
                // Здесь можно добавить сохранение в постоянные настройки
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
                UpdateMusicVolume();
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

        private void UpdateMusicVolume()
        {
            // Логика обновления громкости для текущего музыкального плеера
        }

        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new MenuControl();
        }
    }
}