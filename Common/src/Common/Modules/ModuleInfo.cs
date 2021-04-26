using System.Collections.Generic;
using System.Linq;

namespace Common.Modules
{
    // internal class ModuleInfo
    // {
    //     public string Name { get; }
    //     public string Path { get; }
    //     public IEnumerable<string> Policies { get; }
    //
    //     public ModuleInfo(string name, string path, IEnumerable<string> policies)
    //     {
    //         Name = name;
    //         Path = path;
    //         Policies = policies ?? Enumerable.Empty<string>();
    //     }
    // }
    
    internal record ModuleInfo(string Name, string Path, IEnumerable<string> Policies);
}