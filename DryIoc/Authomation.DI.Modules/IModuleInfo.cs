using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DryIocModules
{
    public interface IModuleInfo
    {
        /// <summary>
        /// The name of the module.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Simple name of assembly where defined module, and name should be file extension.
        /// </summary>
        string AssemblyName { get; set; }

        /// <summary>
        /// Determines that the module is enabled.
        /// </summary>
        bool Enabled { get; set; }

        //// <summary>
        //// The module names this instance depends on.
        //// </summary>
        //ICollection<string> DependsOn { get; set; }
    }
}
