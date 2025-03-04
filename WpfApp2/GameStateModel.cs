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
        public int HighScore { get; set; } // Новое свойство для лучшего результата

        // Статические методы для работы с файлом сохранения
        private static string SaveFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "2048Game",
            "gamesave.json"
        );

        private static string HighScoreFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "2048Game",
            "highscore.json"
        );

        public static void SaveGame(GameStateModel state)
        {
            try
            {
                // Создаем директорию, если она не существует
                Directory.CreateDirectory(Path.GetDirectoryName(SaveFilePath));

                // Обновляем высший балл, если текущий больше
                int currentHighScore = LoadHighScore();
                state.HighScore = Math.Max(currentHighScore, state.Score);

                // Сохраняем высший балл
                SaveHighScore(state.HighScore);

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
                    var state = JsonConvert.DeserializeObject<GameStateModel>(json);
                    state.HighScore = LoadHighScore(); // Загружаем текущий высший балл
                    return state;
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

        // Новый метод для сохранения высшего балла
        public static void SaveHighScore(int highScore)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(HighScoreFilePath));
                File.WriteAllText(HighScoreFilePath, highScore.ToString());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка сохранения рекорда: {ex.Message}");
            }
        }

        // Новый метод для загрузки высшего балла
        public static int LoadHighScore()
        {
            try
            {
                if (File.Exists(HighScoreFilePath))
                {
                    return int.Parse(File.ReadAllText(HighScoreFilePath));
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки рекорда: {ex.Message}");
            }
            return 0;
        }
    }
}