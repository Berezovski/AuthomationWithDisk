namespace DryIocModules
{
    public interface IModuleInfo
    {
        /// <summary>
        /// Имя модуля.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Простое имя сборки, в которой определен модуль, причем имя должно быть расширением файла.
        /// </summary>
        string AssemblyName { get; set; }

        /// <summary>
        /// Определяет, включен ли модуль.
        /// </summary>
        bool Enabled { get; set; }

        //// <summary>
        //// Имена модулей, от которых зависит данный экземпляр.
        //// </summary>
        //ICollection<string> DependsOn { get; set; }
    }
}
