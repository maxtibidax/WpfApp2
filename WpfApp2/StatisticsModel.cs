using System;
using System.IO;
using Newtonsoft.Json;

namespace WpfApp2
{
    [Serializable]
    public class StatisticsModel
    {
        public int GamesPlayed { get; set; }
        public int HighScore { get; set; }
        public int TotalMoves { get; set; }
        public long TotalScore { get; set; }

        public static StatisticsModel GuestStats { get; private set; } = new StatisticsModel();

        public static void SaveStatistics(StatisticsModel stats)
        {
            if (UserManager.CurrentUser == null)
            {
                GuestStats = stats;
                return;
            }

            try
            {
                string filePath = UserManager.CurrentUser.StatisticsFilePath;
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                string json = JsonConvert.SerializeObject(stats, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка сохранения статистики: {ex.Message}");
            }
        }

        public static StatisticsModel LoadStatistics()
        {
            if (UserManager.CurrentUser == null)
            {
                return GuestStats;
            }

            try
            {
                string filePath = UserManager.CurrentUser.StatisticsFilePath;
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<StatisticsModel>(json) ?? new StatisticsModel();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки статистики: {ex.Message}");
            }
            return new StatisticsModel();
        }

        public static void UpdateStatistics(int score, int moves, int highScore)
        {
            var stats = LoadStatistics();
            stats.GamesPlayed++;
            stats.TotalMoves += moves;
            stats.TotalScore += score;
            stats.HighScore = Math.Max(stats.HighScore, highScore);
            SaveStatistics(stats);
        }

        public static void ClearGuestStats()
        {
            GuestStats = new StatisticsModel();
        }
    }
}