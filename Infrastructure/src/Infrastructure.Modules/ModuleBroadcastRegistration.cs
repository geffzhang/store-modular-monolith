using System;
using System.Threading.Tasks;

namespace Infrastructure.Modules
{
    public sealed class ModuleBroadcastRegistration
    {
        public Type ReceiverType { get; set; }
        public Func<object, Task> Action { get; set; }
        public string Path => ReceiverType.Name;
        public string Module { get; set; }
    }
}
