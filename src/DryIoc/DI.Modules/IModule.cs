using DryIoc;

namespace Authomation.DI.Modules
{
    public interface IModule : IModuleInfo
    {
        /// <summary>
        /// Используется для регистрации типов в контейнере DryIoc.
        /// </summary>
        void RegisterTypes(IContainer container);
    }
}
