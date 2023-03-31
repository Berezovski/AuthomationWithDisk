using Authomation.Cipher.Interfaces;
using Authomation.DI.Modules;
using Authomation.RijndailCipher;
using DryIoc;

namespace Authomation.RijndailCipherModule
{
    public class CipherModule : IModule
    {
        public string Name { get; set; } = nameof(CipherModule);
        public string AssemblyName { get; set; }
        public bool Enabled { get; set; }

        public void RegisterTypes(IContainer container)
        {
            container.Register<ICipher, Rijndail>(Reuse.Singleton);
        }
    }
}
