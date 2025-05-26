# 2048 Game
A desktop version of the classic 2048 game, built using Windows Presentation Foundation (WPF).

## Features
- **Classic 2048 Gameplay**: Slide tiles on a grid, merge identical numbers to score, and try to reach the 2048 tile.
- **Configurable Grid Size**: Choose different grid sizes (e.g., 3x3, 4x4, 5x5) in the settings.
- **User Accounts**: Register and log in to save your progress and track your statistics separately.
- **Game State Persistence**:
    - Save your current game and continue later.
    - Game states are saved per user.
    - Guest sessions also support saving the current game (in-memory for the session) and high scores.
- **High Score Tracking**: Your highest score for each grid size is saved.
- **Statistics**: Track your games played, overall high score, total moves, and average score. Generate an HTML report of all user statistics.
- **Music & Volume Control**: Enjoy background music while playing, with adjustable volume settings.

## How to Build and Run

This project is a Visual Studio solution.

**Prerequisites:**
- Visual Studio (e.g., 2019 or later)
- .NET Desktop Development workload installed in Visual Studio Installer.

**Running the Game:**
1. Clone or download this repository.
2. Open `WpfApp2.sln` with Visual Studio.
3. Ensure `WpfApp2` is set as the startup project.
4. Press F5 or click the "Start" button in Visual Studio to build and run the application.

## Project Structure
- **`WpfApp2/`**: Contains the main source code for the WPF 2048 game application. This includes all game logic, UI elements (XAML and C# code-behind), data models, and utility classes.
- **`Setup1/`**: Contains a Visual Studio Installer Project (`.vdproj`). This project is used to create an MSI installer package for easy distribution and installation of the game on Windows systems.
- **`README.md`**: This file, providing information about the project.
