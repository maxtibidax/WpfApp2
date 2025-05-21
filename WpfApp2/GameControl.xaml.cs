using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;

namespace WpfApp2
{
    public partial class GameControl : UserControl
    {
        private int[,] gameGrid;
        private Random random = new Random();
        private int score = 0;
        private int highScore = 0;
        private int value = 0;
        private int gridSize;
        private bool isGameOver = false;
        private int moveCount = 0;

        public GameControl(bool continueGame = false)
        {
            InitializeComponent();
            gridSize = SettingsControl.GridSize;
            gameGrid = new int[gridSize, gridSize];
            InitializeGrid();

            highScore = GameStateModel.LoadHighScore();
            highScoreLbl.Content = highScore.ToString();

            if (continueGame)
            {
                var savedState = GameStateModel.LoadGame();
                if (savedState != null)
                {
                    RestoreGameState(savedState);
                    score = savedState.Score;
                    scoreLbl.Content = score.ToString();
                    highScore = savedState.HighScore;
                    highScoreLbl.Content = highScore.ToString();
                }
                else
                {
                    InitializeGame();
                }
            }
            else
            {
                GameStateModel.DeleteSaveFile();
                InitializeGame();
                score = 0;
                scoreLbl.Content = "0";
            }

            UpdateGameGrid();

            GlobalMusicManager.PlayMusic(
                "..\\..\\..\\music\\game.mp3",
                true,
                (float)SettingsControl.MusicVolume
            );
        }

        private void InitializeGrid()
        {
            GameGrid.Children.Clear();
            GameGrid.ColumnDefinitions.Clear();
            GameGrid.RowDefinitions.Clear();

            for (int i = 0; i < gridSize; i++)
            {
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                GameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    double buttonSize = 380.0 / gridSize - 10; // 380 - размер поля, 10 - отступы
                    Button button = new Button
                    {
                        Style = (Style)FindResource("emp"),
                        Width = buttonSize,
                        Height = buttonSize
                    };
                    double fontSize = gridSize == 3 ? 32 : gridSize == 5 ? 24 : 28;
                    button.FontSize = fontSize;
                    Grid grid = new Grid();
                    grid.Children.Add(button);
                    Grid.SetRow(grid, i);
                    Grid.SetColumn(grid, j);
                    GameGrid.Children.Add(grid);
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
        }

        private void InitializeGame()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    gameGrid[i, j] = 0;
                }
            }
            AddNewTile();
            AddNewTile();
            UpdateGameGrid();
            isGameOver = false;
            moveCount = 0;
        }

        private void AddNewTile()
        {
            int emptyCellCount = CountEmptyCells();
            if (emptyCellCount == 0) return;

            int targetCell = random.Next(emptyCellCount);
            int cellIndex = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (gameGrid[i, j] == 0)
                    {
                        if (cellIndex == targetCell)
                        {
                            gameGrid[i, j] = random.Next(10) == 0 ? 4 : 2;
                            return;
                        }
                        cellIndex++;
                    }
                }
            }
        }

        private int CountEmptyCells()
        {
            int count = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (gameGrid[i, j] == 0) count++;
                }
            }
            return count;
        }

        private void UpdateGameGrid()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    int index = i * gridSize + j;
                    Grid grid = (Grid)GameGrid.Children[index];
                    Button button = (Button)grid.Children[0];
                    if (gameGrid[i, j] == 0)
                    {
                        button.Style = (Style)FindResource("emp");
                        button.Content = "";
                    }
                    else
                    {
                        switch (gameGrid[i, j])
                        {
                            case 2:
                                button.Style = (Style)FindResource("cell2");
                                break;
                            case 4:
                                button.Style = (Style)FindResource("cell4");
                                break;
                            case 8:
                                button.Style = (Style)FindResource("cell8");
                                break;
                            case 16:
                                button.Style = (Style)FindResource("cell16");
                                break;
                            case 32:
                                button.Style = (Style)FindResource("cell32");
                                break;
                            case 64:
                                button.Style = (Style)FindResource("cell64");
                                break;
                            case 128:
                                button.Style = (Style)FindResource("cell128");
                                break;
                            case 256:
                                button.Style = (Style)FindResource("cell256");
                                break;
                            case 512:
                                button.Style = (Style)FindResource("cell512");
                                break;
                            case 1024:
                                button.Style = (Style)FindResource("cell1024");
                                break;
                            case 2048:
                                button.Style = (Style)FindResource("cell2048");
                                break;
                            default:
                                button.Style = (Style)FindResource("cell2048");
                                break;
                        }
                        button.Content = gameGrid[i, j].ToString();
                    }
                }
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            bool moved = false;
            value = 0;

            switch (e.Key)
            {
                case Key.Up:
                    MoveUp();
                    moved = true;
                    break;
                case Key.Down:
                    MoveDown();
                    moved = true;
                    break;
                case Key.Left:
                    MoveLeft();
                    moved = true;
                    break;
                case Key.Right:
                    MoveRight();
                    moved = true;
                    break;
            }

            if (moved)
            {
                moveCount++;
                AddScore(value);
                SaveGameState();
                if (!CanMove())
                {
                    GlobalMusicManager.Stop();
                    StatisticsModel.UpdateStatistics(score, moveCount, highScore);
                    MessageBox.Show("Игра окончена!");
                    GameStateModel.DeleteSaveFile();
                    score = 0;
                    highScore = 0;
                    GameStateModel.SaveHighScore(highScore);
                    UpdateScoreLabels();
                    InitializeGame();
                    isGameOver = true;
                }
            }

            UpdateGameGrid();
            e.Handled = true;
        }

        private bool CanMove()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (gameGrid[i, j] == 0)
                    {
                        return true;
                    }
                }
            }

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize - 1; j++)
                {
                    if (gameGrid[i, j] == gameGrid[i, j + 1] ||
                        gameGrid[j, i] == gameGrid[j + 1, i])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void MoveUp()
        {
            bool moved = false;
            for (int j = 0; j < gridSize; j++)
            {
                for (int i = 0; i < gridSize; i++)
                {
                    if (gameGrid[i, j] != 0)
                    {
                        int k = i;
                        while (k > 0 && gameGrid[k - 1, j] == 0)
                        {
                            gameGrid[k - 1, j] = gameGrid[k, j];
                            gameGrid[k, j] = 0;
                            k--;
                            moved = true;
                        }
                        if (k > 0 && gameGrid[k - 1, j] == gameGrid[k, j])
                        {
                            gameGrid[k - 1, j] *= 2;
                            value += gameGrid[k - 1, j];
                            gameGrid[k, j] = 0;
                            moved = true;
                        }
                    }
                }
            }
            if (moved) AddNewTile();
        }

        private void MoveDown()
        {
            bool moved = false;
            for (int j = 0; j < gridSize; j++)
            {
                for (int i = gridSize - 1; i >= 0; i--)
                {
                    if (gameGrid[i, j] != 0)
                    {
                        int k = i;
                        while (k < gridSize - 1 && gameGrid[k + 1, j] == 0)
                        {
                            gameGrid[k + 1, j] = gameGrid[k, j];
                            gameGrid[k, j] = 0;
                            k++;
                            moved = true;
                        }
                        if (k < gridSize - 1 && gameGrid[k + 1, j] == gameGrid[k, j])
                        {
                            gameGrid[k + 1, j] *= 2;
                            value += gameGrid[k + 1, j];
                            gameGrid[k, j] = 0;
                            moved = true;
                        }
                    }
                }
            }
            if (moved) AddNewTile();
        }

        private void MoveLeft()
        {
            bool moved = false;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (gameGrid[i, j] != 0)
                    {
                        int k = j;
                        while (k > 0 && gameGrid[i, k - 1] == 0)
                        {
                            gameGrid[i, k - 1] = gameGrid[i, k];
                            gameGrid[i, k] = 0;
                            k--;
                            moved = true;
                        }
                        if (k > 0 && gameGrid[i, k - 1] == gameGrid[i, k])
                        {
                            gameGrid[i, k - 1] *= 2;
                            value += gameGrid[i, k - 1];
                            gameGrid[i, k] = 0;
                            moved = true;
                        }
                    }
                }
            }
            if (moved) AddNewTile();
        }

        private void MoveRight()
        {
            bool moved = false;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = gridSize - 1; j >= 0; j--)
                {
                    if (gameGrid[i, j] != 0)
                    {
                        int k = j;
                        while (k < gridSize - 1 && gameGrid[i, k + 1] == 0)
                        {
                            gameGrid[i, k + 1] = gameGrid[i, k];
                            gameGrid[i, k] = 0;
                            k++;
                            moved = true;
                        }
                        if (k < gridSize - 1 && gameGrid[i, k + 1] == gameGrid[i, k])
                        {
                            gameGrid[i, k + 1] *= 2;
                            value += gameGrid[i, k + 1];
                            gameGrid[i, k] = 0;
                            moved = true;
                        }
                    }
                }
            }
            if (moved) AddNewTile();
        }

        private void AddScore(int value)
        {
            score += value;
            if (score > highScore)
            {
                highScore = score;
                GameStateModel.SaveHighScore(highScore);
            }
            UpdateScoreLabels();
        }

        private void UpdateScoreLabels()
        {
            scoreLbl.Content = score.ToString();
            highScoreLbl.Content = highScore.ToString();
        }

        private void SaveGameState()
        {
            var saveState = new GameStateModel
            {
                GridSize = gridSize,
                Score = score,
                HighScore = highScore,
                TileValues = new int[gridSize, gridSize]
            };

            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    saveState.TileValues[r, c] = gameGrid[r, c];
                }
            }

            GameStateModel.SaveGame(saveState);
        }

        private void RestoreGameState(GameStateModel savedState)
        {
            gridSize = savedState.GridSize;
            gameGrid = new int[gridSize, gridSize];
            InitializeGrid();

            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    gameGrid[r, c] = savedState.TileValues[r, c];
                }
            }
        }

        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        {
            GlobalMusicManager.Stop();
            if (!isGameOver && UserManager.CurrentUser == null)
            {
                var stats = StatisticsModel.LoadStatistics();
                if (stats.GamesPlayed > 0 || stats.HighScore > 0)
                {
                    ((MainWindow)Application.Current.MainWindow).MainContent.Content = new SaveStatsPromptControl();
                    return;
                }
            }
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new MenuControl();
        }
    }
}