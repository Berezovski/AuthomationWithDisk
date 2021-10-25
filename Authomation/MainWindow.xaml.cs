using System.Windows;
using Disk.Interfaces;
using DryIoc;
using WinLogger;

namespace Authomation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static IContainer Container = new Container();

        public MainWindow()
        {
            InitializeComponent();
            new LogUploader(logConsole,scrollLogConsole);
            LogUploader.LogInformation("Запуск службы конфигурации...");
            var config = Startup.BuildConfiguration();

            LogUploader.LogInformation("Регистрация конфигураций...");
            Container.RegisterInstance(config);

            LogUploader.LogInformation("Добавление модулей...");
            Container.AddModules();

            LogUploader.LogSuccess("Успешно...");

            //не работает
            IDiskUploader diskUploader = Container.Resolve<IDiskUploader>();
        }


    }
}
