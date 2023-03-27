using Authomation.DI.Modules;
using Authomation.Disk.Interfaces;
using Authomation.GoogleDisk.Settings;
using Authomation.GoogleDisk.Uploader;
using DryIoc;
using Microsoft.Extensions.Configuration;

namespace Authomation.GoogleDiskModule
{
    public class UploaderModule : IModule
    {
        public string Name { get; set; } = nameof(UploaderModule);
        public string AssemblyName { get; set; }
        public bool Enabled { get; set; }

        public void RegisterTypes(IContainer container)
        {
            var configuration = container.Resolve<IConfiguration>(IfUnresolved.ReturnDefault);
            if (configuration != default)
            {
                var settings = GoogleDiskSettings.FromConfiguration(configuration);
                container.RegisterInstance(settings);
            }

            container.Register<IDiskUploader, GoogleDiskUploader>(Reuse.Singleton);
        }
    }
}
