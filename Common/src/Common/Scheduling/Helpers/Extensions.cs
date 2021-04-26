using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Messaging.Scheduling.Helpers
{
    public static class Extensions
    {
        public static System.Type GetPayloadType(this MessageSerializedObject messageSerializedObject)
        {
            if (messageSerializedObject?.AssemblyName == null)
                return null;

            var assemblies = GetAssemblies().ToList()
                .Where(x => x.GetName().Name.EndsWith(messageSerializedObject.AssemblyName))
                .Select(x => x.GetName().FullName)
                .ToList();

            var type = assemblies.SelectMany(x => Assembly.Load(x)
                .GetTypes()
                .Where(t => t.FullName == messageSerializedObject.FullTypeName)
                .ToList()).First();
            return type;
        }
        
        private static IEnumerable<Assembly> GetAssemblies()
        {
            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());

            do
            {
                var asm = stack.Pop();

                yield return asm;

                foreach (var reference in asm.GetReferencedAssemblies())
                {
                    if (!list.Contains(reference.FullName))
                    {
                        stack.Push(Assembly.Load(reference));
                        list.Add(reference.FullName);
                    }
                }
            }
            while (stack.Count > 0);
        }
    }
}