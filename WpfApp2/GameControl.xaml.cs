﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Newtonsoft.Json;

namespace WpfApp2
{
    public class GameState
    {
        public int GridSize { get; set; }
        public Tile[,] BoardTiles { get; set; }
        public int Score { get; set; }
    }
    public static class ArrayExtensions
    {
        public static Tile[,] сlone(this Tile[,] source)
        {
            int rows = source.GetLength(0);
            int cols = source.GetLength(1);
            Tile[,] clone = new Tile[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (source[r, c] != null)
                    {
                        clone[r, c] = new Tile
                        {
                            Value = source[r, c].Value,
                            Row = source[r, c].Row,
                            Col = source[r, c].Col,
                            Merged = source[r, c].Merged,
                            UIElement = source[r, c].UIElement
                        };
                    }
                }
            }
            return clone;
        }
    }
    public partial class GameControl : UserControl
    {

        private int gridSize;
        private const int cellSize = 80; // Уменьшил размер для поддержки разных сеток
        private Tile[,] boardTiles;
        private Random rand = new Random();
        public static GameState LastGameState { get;  set; }
        private int currentScore = 0;
        private int highScore = 0;

        // Метод для сохранения состояния игры
        private void SaveGameState()
        {
            var saveState = new GameStateModel
            {
                GridSize = gridSize,
                Score = currentScore,
                TileValues = new int[gridSize, gridSize]
            };

            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    saveState.TileValues[r, c] = boardTiles[r, c]?.Value ?? 0;
                }
            }

            GameStateModel.SaveGame(saveState);
        }

        // Метод для подсчета очков
        private int CalculateScore()
        {
            int score = 0;
            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    if (boardTiles[r, c] != null)
                    {
                        score += boardTiles[r, c].Value;
                    }
                }
            }
            return score;
        }

        // Конструктор с опциональной загрузкой состояния
        public GameControl(bool continueGame = false)
        {
            InitializeComponent();
            gridSize = SettingsControl.GridSize;
            boardTiles = new Tile[gridSize, gridSize];

            // Загружаем текущий рекорд
            highScore = GameStateModel.LoadHighScore();
            HighScoreTextBlock.Text = highScore.ToString();

            if (continueGame)
            {
                var savedState = GameStateModel.LoadGame();
                if (savedState != null)
                {
                    RestoreGameState(savedState);
                    currentScore = savedState.Score;
                    ScoreTextBlock.Text = currentScore.ToString();
                    highScore = savedState.HighScore;
                    HighScoreTextBlock.Text = highScore.ToString();
                }
                else
                {
                    SpawnTile();
                    SpawnTile();
                }
            }
            else
            {
                GameStateModel.DeleteSaveFile();
                SpawnTile();
                SpawnTile();
                currentScore = 0;
                ScoreTextBlock.Text = "0";
            }

            GameCanvas.Width = GameCanvas.Height = gridSize * cellSize;
            this.Loaded += (s, e) => { Keyboard.Focus(this); };

            GlobalMusicManager.PlayMusic(
                "..\\..\\..\\music\\game.mp3",
                true,
                SettingsControl.MusicVolume
            );
        }


        private void RestoreGameState(GameStateModel savedState)
        {
            gridSize = savedState.GridSize;
            boardTiles = new Tile[gridSize, gridSize];

            // Очищаем предыдущие элементы на Canvas
            GameCanvas.Children.Clear();

            // Восстанавливаем плитки
            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    int value = savedState.TileValues[r, c];
                    if (value > 0)
                    {
                        Tile tile = CreateTile(value, r, c);
                        boardTiles[r, c] = tile;
                        GameCanvas.Children.Add(tile.UIElement);
                    }
                }
            }
        }

        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        { 
            

            // Возврат к меню
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new MenuControl();
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool moved = false;
            switch (e.Key)
            {
                case Key.Left:
                    moved = MoveLeft();
                    break;
                case Key.Right:
                    moved = MoveRight();
                    break;
                case Key.Up:
                    moved = MoveUp();
                    break;
                case Key.Down:
                    moved = MoveDown();
                    break;
            }
            if (moved)
            {
                SpawnTile();
                UpdateUI();
                SaveGameState(); // Сохраняем состояние игры в файл
                if (IsGameOver())
                {
                    MessageBox.Show("Игра окончена!");
                    GameStateModel.DeleteSaveFile(); // Удаляем файл сохранения при окончании игры
                }
            }
            e.Handled = true;
        }



        #region Логика игры (перемещения, анимация, создание плиток и т.д.)

        private bool MoveLeft()
        {
            bool moved = false;
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 1; col < gridSize; col++)
                {
                    if (boardTiles[row, col] != null)
                    {
                        int newCol = col;
                        while (newCol > 0 && boardTiles[row, newCol - 1] == null)
                        {
                            newCol--;
                        }
                        if (newCol > 0 && boardTiles[row, newCol - 1] != null &&
                            boardTiles[row, newCol - 1].Value == boardTiles[row, col].Value &&
                            !boardTiles[row, newCol - 1].Merged)
                        {
                            var target = boardTiles[row, newCol - 1];
                            target.Value *= 2;
                            target.Merged = true;
                            currentScore += target.Value; // Добавляем очки
                            ScoreTextBlock.Text = currentScore.ToString();
                            // Обновляем рекорд если нужно
                            UpdateHighScore();
                            AnimateTile(boardTiles[row, col], row, col, row, newCol - 1);
                            AnimateMerge(target);
                            RemoveTile(boardTiles[row, col]);
                            boardTiles[row, col] = null;
                            moved = true;
                        }
                        else if (newCol != col)
                        {
                            boardTiles[row, newCol] = boardTiles[row, col];
                            boardTiles[row, col] = null;
                            AnimateTile(boardTiles[row, newCol], row, col, row, newCol);
                            boardTiles[row, newCol].Col = newCol;
                            moved = true;
                        }
                    }
                }
            }
            ResetMergedFlags();
            return moved;
        }

        private bool MoveRight()
        {
            bool moved = false;
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = gridSize - 2; col >= 0; col--)
                {
                    if (boardTiles[row, col] != null)
                    {
                        int newCol = col;
                        while (newCol < gridSize - 1 && boardTiles[row, newCol + 1] == null)
                        {
                            newCol++;
                        }
                        if (newCol < gridSize - 1 && boardTiles[row, newCol + 1] != null &&
                            boardTiles[row, newCol + 1].Value == boardTiles[row, col].Value &&
                            !boardTiles[row, newCol + 1].Merged)
                        {
                            var target = boardTiles[row, newCol + 1];
                            target.Value *= 2;
                            target.Merged = true;
                            currentScore += target.Value; // Добавляем очки
                            ScoreTextBlock.Text = currentScore.ToString();
                            // Обновляем рекорд если нужно
                            UpdateHighScore();
                            AnimateTile(boardTiles[row, col], row, col, row, newCol + 1);
                            AnimateMerge(target);
                            RemoveTile(boardTiles[row, col]);
                            boardTiles[row, col] = null;
                            moved = true;
                        }
                        else if (newCol != col)
                        {
                            boardTiles[row, newCol] = boardTiles[row, col];
                            boardTiles[row, col] = null;
                            AnimateTile(boardTiles[row, newCol], row, col, row, newCol);
                            boardTiles[row, newCol].Col = newCol;
                            moved = true;
                        }
                    }
                }
            }
            ResetMergedFlags();
            return moved;
        }

        private bool MoveUp()
        {
            bool moved = false;
            for (int col = 0; col < gridSize; col++)
            {
                for (int row = 1; row < gridSize; row++)
                {
                    if (boardTiles[row, col] != null)
                    {
                        int newRow = row;
                        while (newRow > 0 && boardTiles[newRow - 1, col] == null)
                        {
                            newRow--;
                        }
                        if (newRow > 0 && boardTiles[newRow - 1, col] != null &&
                            boardTiles[newRow - 1, col].Value == boardTiles[row, col].Value &&
                            !boardTiles[newRow - 1, col].Merged)
                        {
                            var target = boardTiles[newRow - 1, col];
                            target.Value *= 2;
                            target.Merged = true;
                            currentScore += target.Value; // Добавляем очки
                            ScoreTextBlock.Text = currentScore.ToString();
                            // Обновляем рекорд если нужно
                            UpdateHighScore();
                            AnimateTile(boardTiles[row, col], row, col, newRow - 1, col);
                            AnimateMerge(target);
                            RemoveTile(boardTiles[row, col]);
                            boardTiles[row, col] = null;
                            moved = true;
                        }
                        else if (newRow != row)
                        {
                            boardTiles[newRow, col] = boardTiles[row, col];
                            boardTiles[row, col] = null;
                            AnimateTile(boardTiles[newRow, col], row, col, newRow, col);
                            boardTiles[newRow, col].Row = newRow;
                            moved = true;
                        }
                    }
                }
            }
            ResetMergedFlags();
            return moved;
        }

        private bool MoveDown()
        {
            bool moved = false;
            for (int col = 0; col < gridSize; col++)
            {
                for (int row = gridSize - 2; row >= 0; row--)
                {
                    if (boardTiles[row, col] != null)
                    {
                        int newRow = row;
                        while (newRow < gridSize - 1 && boardTiles[newRow + 1, col] == null)
                        {
                            newRow++;
                        }
                        if (newRow < gridSize - 1 && boardTiles[newRow + 1, col] != null &&
                            boardTiles[newRow + 1, col].Value == boardTiles[row, col].Value &&
                            !boardTiles[newRow + 1, col].Merged)
                        {
                            var target = boardTiles[newRow + 1, col];
                            target.Value *= 2;
                            target.Merged = true;
                            currentScore += target.Value; // Добавляем очки
                            ScoreTextBlock.Text = currentScore.ToString();
                            // Обновляем рекорд если нужно
                            UpdateHighScore();
                            AnimateTile(boardTiles[row, col], row, col, newRow + 1, col);
                            AnimateMerge(target);
                            RemoveTile(boardTiles[row, col]);
                            boardTiles[row, col] = null;
                            moved = true;
                        }
                        else if (newRow != row)
                        {
                            boardTiles[newRow, col] = boardTiles[row, col];
                            boardTiles[row, col] = null;
                            AnimateTile(boardTiles[newRow, col], row, col, newRow, col);
                            boardTiles[newRow, col].Row = newRow;
                            moved = true;
                        }
                    }
                }
            }
            ResetMergedFlags();
            return moved;
        }
        private void UpdateHighScore()
        {
            if (currentScore > highScore)
            {
                highScore = currentScore;
                HighScoreTextBlock.Text = highScore.ToString();
                GameStateModel.SaveHighScore(highScore);
            }
        }
        private void AnimateTile(Tile tile, int oldRow, int oldCol, int newRow, int newCol)
        {
            double oldX = oldCol * cellSize;
            double oldY = oldRow * cellSize;
            double newX = newCol * cellSize;
            double newY = newRow * cellSize;
            double deltaX = newX - oldX;
            double deltaY = newY - oldY;

            // Устанавливаем новую позицию на Canvas
            Canvas.SetLeft(tile.UIElement, newX);
            Canvas.SetTop(tile.UIElement, newY);

            // Добавляем анимацию перемещения
            TranslateTransform tt = new TranslateTransform();
            tile.UIElement.RenderTransform = tt;
            Duration duration = new Duration(TimeSpan.FromMilliseconds(200));
            DoubleAnimation animX = new DoubleAnimation(-deltaX, 0, duration);
            DoubleAnimation animY = new DoubleAnimation(-deltaY, 0, duration);
            tt.BeginAnimation(TranslateTransform.XProperty, animX);
            tt.BeginAnimation(TranslateTransform.YProperty, animY);
        }

        private void AnimateMerge(Tile tile)
        {
            TransformGroup group;
            if (tile.UIElement.RenderTransform is TransformGroup existingGroup)
            {
                group = existingGroup;
            }
            else
            {
                group = new TransformGroup();
                if (tile.UIElement.RenderTransform != null)
                    group.Children.Add(tile.UIElement.RenderTransform);
                tile.UIElement.RenderTransform = group;
            }

            ScaleTransform scaleTransform = group.Children.OfType<ScaleTransform>().FirstOrDefault();
            if (scaleTransform == null)
            {
                scaleTransform = new ScaleTransform(1.0, 1.0);
                group.Children.Add(scaleTransform);
            }

            var scaleXAnimation = new DoubleAnimationUsingKeyFrames();
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.2, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(150))));
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300))));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation);

            var scaleYAnimation = new DoubleAnimationUsingKeyFrames();
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.2, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(150))));
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300))));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);
        }

        private void UpdateUI()
        {
            foreach (UIElement element in GameCanvas.Children)
            {
                if (element is Border border && border.Child is TextBlock tb && border.Tag is Tile tile)
                {
                    tb.Text = tile.Value.ToString();
                }
            }
        }

        private void RemoveTile(Tile tile)
        {
            GameCanvas.Children.Remove(tile.UIElement);
        }

        private Tile CreateTile(int value, int row, int col)
        {
            Tile tile = new Tile
            {
                Value = value,
                Row = row,
                Col = col,
                Merged = false
            };
            Border border = new Border
            {
                Width = cellSize - 10,
                Height = cellSize - 10,
                CornerRadius = new CornerRadius(5),
                Background = new SolidColorBrush(Colors.Orange),
                BorderBrush = new SolidColorBrush(Colors.Brown),
                BorderThickness = new Thickness(2)
            };
            TextBlock tb = new TextBlock
            {
                Text = value.ToString(),
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            border.Child = tb;
            border.Tag = tile;
            tile.UIElement = border;
            Canvas.SetLeft(border, col * cellSize);
            Canvas.SetTop(border, row * cellSize);
            return tile;
        }

        private void SpawnTile()
        {
            List<(int row, int col)> emptyCells = new List<(int, int)>();
            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    if (boardTiles[r, c] == null)
                        emptyCells.Add((r, c));
                }
            }
            if (emptyCells.Count > 0)
            {
                var (row, col) = emptyCells[rand.Next(emptyCells.Count)];
                int value = rand.NextDouble() < 0.9 ? 2 : 4;
                Tile newTile = CreateTile(value, row, col);
                boardTiles[row, col] = newTile;
                GameCanvas.Children.Add(newTile.UIElement);
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(200)));
                newTile.UIElement.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }
        }

        private bool IsGameOver()
        {
            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    if (boardTiles[r, c] == null)
                        return false;
                    int val = boardTiles[r, c].Value;
                    if (r < gridSize - 1 && boardTiles[r + 1, c] != null && boardTiles[r + 1, c].Value == val)
                        return false;
                    if (c < gridSize - 1 && boardTiles[r, c + 1] != null && boardTiles[r, c + 1].Value == val)
                        return false;
                }
            }
            GlobalMusicManager.Stop(); // Останавливаем музыку
            GameStateModel.SaveHighScore(highScore); // Сохраняем рекорд
            return true;
        }

        private void ResetMergedFlags()
        {
            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    if (boardTiles[r, c] != null)
                        boardTiles[r, c].Merged = false;
                }
            }
        }

        #endregion
    }

    // Класс плитки (можно вынести в отдельный файл)
    public class Tile
    {
        public int Value { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        // Флаг, чтобы не сливалась плитка дважды за один ход
        public bool Merged { get; set; } = false;
        public Border UIElement { get; set; }
    }
}