using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace WpfApp2
{
    [Serializable]
    public class UserModel
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string StatisticsFilePath { get; set; }
        public string GameSaveFilePath { get; set; }
        public string HighScoreFilePath { get; set; }

        private static string UsersFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "2048Game",
            "users.json"
        );

        public static UserModel Authenticate(string username, string password)
        {
            var users = LoadUsers();
            var user = users.Find(u => u.Username == username);
            if (user != null && user.PasswordHash == HashPassword(password))
            {
                return user;
            }
            return null;
        }

        public static bool Register(string username, string password)
        {
            var users = LoadUsers();
            if (users.Exists(u => u.Username == username))
            {
                return false;
            }

            var newUser = new UserModel
            {
                Username = username,
                PasswordHash = HashPassword(password),
                StatisticsFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "2048Game",
                    $"statistics_{username}.json"
                ),
                GameSaveFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "2048Game",
                    $"gamesave_{username}.json"
                ),
                HighScoreFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "2048Game",
                    $"highscore_{username}.json"
                )
            };

            users.Add(newUser);
            SaveUsers(users);
            return true;
        }

        public static void DeleteAccount(string username)
        {
            var users = LoadUsers();
            var user = users.Find(u => u.Username == username);
            if (user != null)
            {
                users.Remove(user);
                SaveUsers(users);
                try
                {
                    if (File.Exists(user.StatisticsFilePath)) File.Delete(user.StatisticsFilePath);
                    if (File.Exists(user.GameSaveFilePath)) File.Delete(user.GameSaveFilePath);
                    if (File.Exists(user.HighScoreFilePath)) File.Delete(user.HighScoreFilePath);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Ошибка удаления файлов аккаунта: {ex.Message}");
                }
            }
        }

        public static void SaveGuestStatsToUser(UserModel user)
        {
            var guestStats = StatisticsModel.GuestStats;
            if (guestStats != null && (guestStats.GamesPlayed > 0 || guestStats.HighScore > 0))
            {
                var userStats = StatisticsModel.LoadStatistics();
                userStats.GamesPlayed += guestStats.GamesPlayed;
                userStats.TotalMoves += guestStats.TotalMoves;
                userStats.TotalScore += guestStats.TotalScore;
                userStats.HighScore = Math.Max(userStats.HighScore, guestStats.HighScore);
                StatisticsModel.SaveStatistics(userStats);
            }
        }

        private static List<UserModel> LoadUsers()
        {
            try
            {
                if (File.Exists(UsersFilePath))
                {
                    string json = File.ReadAllText(UsersFilePath);
                    return JsonConvert.DeserializeObject<List<UserModel>>(json) ?? new List<UserModel>();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}");
            }
            return new List<UserModel>();
        }

        private static void SaveUsers(List<UserModel> users)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(UsersFilePath));
                string json = JsonConvert.SerializeObject(users, Formatting.Indented);
                File.WriteAllText(UsersFilePath, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка сохранения пользователей: {ex.Message}");
            }
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }

    public static class UserManager
    {
        public static UserModel CurrentUser { get; set; }
    }
}