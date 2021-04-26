using System;
using System.Threading.Tasks;

namespace Common.Modules
{
    public sealed class ModuleBroadcastRegistration
    {
        public Type ReceiverType { get; set; }
        public Func<object, Task> Action { get; set; }
        public string Key => ReceiverType.Name;
        public string Module { get; set; }
    }
}
