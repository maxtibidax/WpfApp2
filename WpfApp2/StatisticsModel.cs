using System;
using System.IO;
using Newtonsoft.Json;

namespace WpfApp2
{
    [Serializable]
    public class StatisticsModel
    {
        public int GamesPlayed { get; set; } // Количество сыгранных игр
        public int HighScore { get; set; } // Лучший счёт
        public int TotalMoves { get; set; } // Общее количество ходов
        public long TotalScore { get; set; } // Суммарный счёт для вычисления среднего

        private static string StatisticsFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "2048Game",
            "statistics.json"
        );

        public static void SaveStatistics(StatisticsModel stats)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(StatisticsFilePath));
                string json = JsonConvert.SerializeObject(stats, Formatting.Indented);
                File.WriteAllText(StatisticsFilePath, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка сохранения статистики: {ex.Message}");
            }
        }

        public static StatisticsModel LoadStatistics()
        {
            try
            {
                if (File.Exists(StatisticsFilePath))
                {
                    string json = File.ReadAllText(StatisticsFilePath);
                    return JsonConvert.DeserializeObject<StatisticsModel>(json);
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
    }
}