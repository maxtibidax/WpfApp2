using System.Windows;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Загружаем меню при запуске
            MainContent.Content = new MenuControl();
        }
    }
}