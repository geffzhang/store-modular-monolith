using System;

namespace Common.Messages.Serialization.Hybrid
{
    public interface IHybridMessageSerializer
    {
        string Serialize(object obj, bool camelCase = true, bool indented = false);
        T Deserialize<T>(string payload, bool camelCase = true);
        object Deserialize(Type type, string payload, bool camelCase = true);
    }
}