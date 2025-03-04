using System;
using System.IO;
using Newtonsoft.Json;

namespace WpfApp2
{
    [Serializable]
    public class GameStateModel
    {
        public int GridSize { get; set; }
        public int[,] TileValues { get; set; }
        public int Score { get; set; }

        // Статические методы для работы с файлом сохранения
        private static string SaveFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "2048Game",
            "gamesave.json"
        );

        public static void SaveGame(GameStateModel state)
        {
            try
            {
                // Создаем директорию, если она не существует
                Directory.CreateDirectory(Path.GetDirectoryName(SaveFilePath));

                // Сериализуем и сохраняем состояние
                string json = JsonConvert.SerializeObject(state, Formatting.Indented);
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        public static GameStateModel LoadGame()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    string json = File.ReadAllText(SaveFilePath);
                    return JsonConvert.DeserializeObject<GameStateModel>(json);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
            return null;
        }

        public static void DeleteSaveFile()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка удаления файла сохранения: {ex.Message}");
            }
        }
    }
}