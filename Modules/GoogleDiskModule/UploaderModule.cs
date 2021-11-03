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
