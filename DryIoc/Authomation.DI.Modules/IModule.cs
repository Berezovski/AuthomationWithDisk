using DryIoc;
using System;

namespace DryIocModules
{
    public interface IModule : IModuleInfo
    {
        /// <summary>
        /// Used to register types with the DryIoc container.
        /// </summary>
        void RegisterTypes(IContainer container);
    }
}
