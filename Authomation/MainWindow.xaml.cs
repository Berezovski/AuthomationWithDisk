using System.Windows;
using Disk.Interfaces;
using DryIoc;

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
            var config = Startup.BuildConfiguration();

            Container.RegisterInstance(config);
            Container.AddModules();
            //не работает
            IDiskUploader diskUploader = Container.Resolve<IDiskUploader>();
        }
    }
}
