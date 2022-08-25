using System.Windows;
using System.Windows.Controls;
using Disk.Interfaces;
using DryIoc;
using WinLogger;
using static Authomation.MainwindowViewModel;
using GalaSoft.MvvmLight.Command;

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
