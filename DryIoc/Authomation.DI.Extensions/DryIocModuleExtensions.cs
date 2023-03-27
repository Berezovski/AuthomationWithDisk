using DryIoc;
using DryIocModules;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Authomation
{
    public static class DryIocModuleExtensions
    {
        public static IContainer AddModules(this IContainer container, IEnumerable<IModuleInfo> modulesInfo)
        {
            var modulesInfoArray = modulesInfo as IModuleInfo[] ?? modulesInfo.ToArray();
            var activeModules = modulesInfoArray.Where(i => i.Enabled).ToArray();

            if (!activeModules.Any())
            {
                return container;
            }

            foreach (var moduleInfo in activeModules)
            {
                container.AddModule(moduleInfo);
            }

            return container;
        }

        public static IContainer AddModules(this IContainer container, IConfiguration configuration)
        {
            var activeModules = configuration.Get<ModuleInfo[]>()
                ?.Where(i => i.Enabled)
                .ToArray();

            if (activeModules != null && activeModules.Any())
            {
                return AddModules(container, activeModules);
            }

            return container;
        }

        /// <summary>
        /// Добавить модуль вручную.
        /// </summary>
        /// <param name="container">Экземпляр контейнера DryIoc.</param>
        /// <param name="moduleInfo">Экземпляр <see cref="IModuleInfo"/></param>
        /// <returns>Контейнер DryIoc</returns>
        public static IContainer AddModule(this IContainer container, IModuleInfo moduleInfo)
        {
            if (moduleInfo == null || !moduleInfo.Enabled)
            {
                return container;
            }

            var moduleAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(i => string.Equals(i.GetName().Name, 
                moduleInfo.AssemblyName, StringComparison.CurrentCultureIgnoreCase));

            if (moduleAssembly == null)
            {
                moduleAssembly = AppDomain.CurrentDomain.Load(new AssemblyName { Name = moduleInfo.AssemblyName });
            }

            var moduleType = moduleAssembly.GetTypes()
                .FirstOrDefault(i => i.GetInterfaces().Contains(typeof(IModule)));

            if (moduleType == null)
            {
                return container;
            }

            var moduleInstance = Activator.CreateInstance(moduleType);
            if (!(moduleInstance is IModule module) ||
                !string.Equals(module.Name, moduleInfo.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                return container;
            }

            // если модуль уже зарегистрирован, то пропустите его
            if (container.IsRegistered(moduleType, module.Name))
            {
                return container;
            }

            module.Enabled = moduleInfo.Enabled;
            module.AssemblyName = moduleInfo.AssemblyName;
            module.RegisterTypes(container);
            container.RegisterInstance(module, serviceKey: module.Name);
            return container;
        }
    }
}
