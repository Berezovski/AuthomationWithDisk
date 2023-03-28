namespace Authomation.DI.Modules
{
    public class ModuleInfo : IModuleInfo
    {
        public string Name { get; set; }
        public string AssemblyName { get; set; }
        public bool Enabled { get; set; }
    }
}
