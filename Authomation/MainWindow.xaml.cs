using System.Windows;
using WinLogger;

namespace Authomation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainwindowViewModel();
            new LogUploader(logConsole,scrollLogConsole);
        }
    }
}
