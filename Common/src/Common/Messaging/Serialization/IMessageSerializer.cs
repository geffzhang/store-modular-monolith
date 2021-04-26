using System;

namespace Common.Serialization
{
    public interface IMessageSerializer
    {
        bool CanHandle(Type type);
        string Serialize(object obj, bool camelCase = true, bool indented = false);
        T Deserialize<T>(string payload, bool camelCase = true);
        object Deserialize(Type type, string payload, bool camelCase = true);
    }
}