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
            if (UserManager.CurrentUser == null)
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "2048Game",
                    "gamesave_guest.json"
                );
            }
            return UserManager.CurrentUser.GameSaveFilePath;
        }

        private static string GetHighScoreFilePath()
        {
            if (UserManager.CurrentUser == null)
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "2048Game",
                    "highscore_guest.json"
                );
            }
            return UserManager.CurrentUser.HighScoreFilePath;
        }

        public static void SaveGame(GameStateModel state)
        {
            try
            {
                if (UserManager.CurrentUser == null)
                {
                    GuestGameState = state;
                    return;
                }

                string saveFilePath = GetSaveFilePath();
                Directory.CreateDirectory(Path.GetDirectoryName(saveFilePath));

                int currentHighScore = LoadHighScore();
                state.HighScore = Math.Max(currentHighScore, state.Score);
                SaveHighScore(state.HighScore);

                string json = JsonConvert.SerializeObject(state, Formatting.Indented);
                File.WriteAllText(saveFilePath, json);
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
                if (UserManager.CurrentUser == null)
                {
                    return GuestGameState;
                }

                string saveFilePath = GetSaveFilePath();
                if (File.Exists(saveFilePath))
                {
                    string json = File.ReadAllText(saveFilePath);
                    var state = JsonConvert.DeserializeObject<GameStateModel>(json);
                    state.HighScore = LoadHighScore();
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
                System.Windows.MessageBox.Show($"Ошибка удаления файла сохранения: {ex.Message}");
            }
        }

        public static void SaveHighScore(int highScore)
        {
            try
            {
                string highScoreFilePath = GetHighScoreFilePath();
                Directory.CreateDirectory(Path.GetDirectoryName(highScoreFilePath));
                File.WriteAllText(highScoreFilePath, highScore.ToString());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка сохранения рекорда: {ex.Message}");
            }
        }

        public static int LoadHighScore()
        {
            try
            {
                string highScoreFilePath = GetHighScoreFilePath();
                if (File.Exists(highScoreFilePath))
                {
                    return int.Parse(File.ReadAllText(highScoreFilePath));
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