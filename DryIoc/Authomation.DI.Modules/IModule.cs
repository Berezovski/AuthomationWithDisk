using DryIoc;

namespace DryIocModules
{
    public interface IModule : IModuleInfo
    {
        /// <summary>
        /// Используется для регистрации типов в контейнере DryIoc.
        /// </summary>
        void RegisterTypes(IContainer container);
    }
}
