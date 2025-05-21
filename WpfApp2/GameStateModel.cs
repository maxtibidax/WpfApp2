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
        public int HighScore { get; set; }

        private static GameStateModel GuestGameState { get; set; }

        private static string GetSaveFilePath()
        {
            string basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "2048Game"
            );
            if (UserManager.CurrentUser == null)
            {
                return Path.Combine(basePath, "gamesave_guest.json");
            }
            return UserManager.CurrentUser.GameSaveFilePath ?? Path.Combine(basePath, $"gamesave_{UserManager.CurrentUser.Username}.json");
        }

        private static string GetHighScoreFilePath()
        {
            string basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "2048Game"
            );
            if (UserManager.CurrentUser == null)
            {
                return Path.Combine(basePath, "highscore_guest.json");
            }
            return UserManager.CurrentUser.HighScoreFilePath ?? Path.Combine(basePath, $"highscore_{UserManager.CurrentUser.Username}.json");
        }

        public static void SaveGame(GameStateModel state)
        {
            try
            {
                if (state == null)
                {
                    throw new ArgumentNullException(nameof(state), "Состояние игры не может быть null.");
                }

                if (UserManager.CurrentUser == null)
                {
                    GuestGameState = state;
                    return;
                }

                string saveFilePath = GetSaveFilePath();
                Directory.CreateDirectory(Path.GetDirectoryName(saveFilePath) ?? throw new InvalidOperationException("Не удалось определить директорию для сохранения."));

                int currentHighScore = LoadHighScore();
                state.HighScore = Math.Max(currentHighScore, state.Score);
                SaveHighScore(state.HighScore);

                string json = JsonConvert.SerializeObject(state, Formatting.Indented);
                File.WriteAllText(saveFilePath, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public static GameStateModel LoadGame()
        {
            try
            {
                if (UserManager.CurrentUser == null)
                {
                    return GuestGameState;
                }

                string saveFilePath = GetSaveFilePath();
                if (File.Exists(saveFilePath))
                {
                    string json = File.ReadAllText(saveFilePath);
                    var state = JsonConvert.DeserializeObject<GameStateModel>(json);
                    if (state != null)
                    {
                        state.HighScore = LoadHighScore();
                        return state;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            return null;
        }

        public static void DeleteSaveFile()
        {
            try
            {
                if (UserManager.CurrentUser == null)
                {
                    GuestGameState = null;
                    return;
                }

                string saveFilePath = GetSaveFilePath();
                if (File.Exists(saveFilePath))
                {
                    File.Delete(saveFilePath);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка удаления файла сохранения: {ex.Message}", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public static void SaveHighScore(int highScore)
        {
            try
            {
                string highScoreFilePath = GetHighScoreFilePath();
                Directory.CreateDirectory(Path.GetDirectoryName(highScoreFilePath) ?? throw new InvalidOperationException("Не удалось определить директорию для сохранения рекорда."));
                File.WriteAllText(highScoreFilePath, highScore.ToString());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка сохранения рекорда: {ex.Message}", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public static int LoadHighScore()
        {
            try
            {
                string highScoreFilePath = GetHighScoreFilePath();
                if (File.Exists(highScoreFilePath))
                {
                    string content = File.ReadAllText(highScoreFilePath);
                    if (int.TryParse(content, out int highScore))
                    {
                        return highScore;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки рекорда: {ex.Message}", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            return 0;
        }
    }
}