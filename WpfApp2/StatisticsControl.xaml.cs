using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
{
    public partial class StatisticsControl : UserControl
    {
        public StatisticsControl()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var stats = StatisticsModel.LoadStatistics();
            GamesPlayedTextBlock.Text = stats.GamesPlayed.ToString();
            HighScoreTextBlock.Text = stats.HighScore.ToString();
            TotalMovesTextBlock.Text = stats.TotalMoves.ToString();
            AverageScoreTextBlock.Text = stats.GamesPlayed > 0
                ? (stats.TotalScore / stats.GamesPlayed).ToString("F0")
                : "0";
        }

        private void ResetStatistics_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите сбросить статистику?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var resetStats = new StatisticsModel();
                StatisticsModel.SaveStatistics(resetStats);
                LoadStatistics();
            }
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            string reportFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "2048Game",
                "Reports",
                "statistics_report.html"
            );

            try
            {
                // Получаем список всех пользователей
                var users = UserModel.LoadUsers();
                var statsList = new List<(string Username, StatisticsModel Stats)>();

                // Собираем статистику зарегистрированных пользователей
                foreach (var user in users)
                {
                    UserManager.CurrentUser = user; // Временно устанавливаем пользователя для загрузки его статистики
                    var stats = StatisticsModel.LoadStatistics();
                    statsList.Add((user.Username, stats));
                }

                // Добавляем статистику гостя, если она существует
                UserManager.CurrentUser = null; // Сбрасываем пользователя для гостевого режима
                var guestStats = StatisticsModel.LoadStatistics();
                if (guestStats.GamesPlayed > 0 || guestStats.HighScore > 0)
                {
                    statsList.Add(("Guest", guestStats));
                }

                // Восстанавливаем текущего пользователя
                UserManager.CurrentUser = null; // Или восстанавливаем актуального пользователя, если нужно

                // Генерируем HTML
                StringBuilder html = new StringBuilder();
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html lang='ru'>");
                html.AppendLine("<head>");
                html.AppendLine("<meta charset='UTF-8'>");
                html.AppendLine("<title>Отчёт по статистике игры 2048</title>");
                html.AppendLine("<style>");
                html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
                html.AppendLine("h1 { text-align: center; }");
                html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
                html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
                html.AppendLine("th { background-color: #f2f2f2; }");
                html.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
                html.AppendLine("tr:hover { background-color: #f5f5f5; }");
                html.AppendLine("</style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");
                html.AppendLine("<h1>Отчёт по статистике игры 2048</h1>");
                html.AppendLine($"<p>Дата создания: {DateTime.Now:dd.MM.yyyy HH:mm:ss}</p>");
                html.AppendLine("<table>");
                html.AppendLine("<tr>");
                html.AppendLine("<th>Пользователь</th>");
                html.AppendLine("<th>Сыграно игр</th>");
                html.AppendLine("<th>Лучший счёт</th>");
                html.AppendLine("<th>Общее кол-во ходов</th>");
                html.AppendLine("<th>Средний счёт</th>");
                html.AppendLine("</tr>");

                foreach (var (username, stats) in statsList)
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{username}</td>");
                    html.AppendLine($"<td>{stats.GamesPlayed}</td>");
                    html.AppendLine($"<td>{stats.HighScore}</td>");
                    html.AppendLine($"<td>{stats.TotalMoves}</td>");
                    html.AppendLine($"<td>{(stats.GamesPlayed > 0 ? (stats.TotalScore / stats.GamesPlayed).ToString("F0") : "0")}</td>");
                    html.AppendLine("</tr>");
                }

                html.AppendLine("</table>");
                html.AppendLine("</body>");
                html.AppendLine("</html>");

                // Сохраняем отчёт
                Directory.CreateDirectory(Path.GetDirectoryName(reportFilePath));
                File.WriteAllText(reportFilePath, html.ToString());
                MessageBox.Show($"Отчёт успешно создан: {reportFilePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчёта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new MenuControl();
        }
    }
}