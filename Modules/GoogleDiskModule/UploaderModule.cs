using DryIoc;
using GoogleDisk.Settings;
using Microsoft.Extensions.Configuration;
using Disk.Interfaces;
using GoogleDisk.Uploader;
using DryIocModules;

namespace GoogleDiskModule
{
    public class UploaderModule : IModule
    {

        // TODO: надо реализовать всех этих дядечек
        // резолвишь IConfiguration из IContainer и смотришь чё в сеттингах валяется
        // а вообще, по-хорошему стоит сделать базовый класс модуля, чтобы всё это гавно не реализовывать каждый раз (=
        public string Name { get; set; } = nameof(UploaderModule);
        public string AssemblyName { get; set; }
        public bool Enabled { get; set; }

        public void RegisterTypes(IContainer container)
        {
            var configuration = container.Resolve<IConfiguration>(IfUnresolved.ReturnDefault);

            if (configuration != default)
            {
                var settings = configuration.GetSection(nameof(GoogleDiskSettings)).Get<GoogleDiskSettings>();
                container.RegisterInstance(settings);
            }

            container.Register<IDiskUploader, GoogleDiskUploader>(Reuse.Singleton);
        }
    }
}
