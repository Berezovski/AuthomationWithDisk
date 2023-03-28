using Authomation.GUI.ViewModels;
using Authomation.WinLogger;
using System.Windows;

namespace Authomation.GUI
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
