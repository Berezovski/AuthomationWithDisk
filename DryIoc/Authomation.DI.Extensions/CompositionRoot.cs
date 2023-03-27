using DryIoc;
using Microsoft.Extensions.Configuration;

namespace Authomation
{
    public static class CompositionRoot
    {
        public static void AddModules(this IContainer container)
        {
            var configuration = container.Resolve<IConfiguration>();
            // добавить список модулей из appsettings 
            container.AddModules(configuration.GetSection("Modules"));
        }
    }
}
