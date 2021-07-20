using System;

namespace Common.Core.Messaging.Serialization
{
    public interface IMessageSerializer
    {
        bool CanHandle(Type type);
        string Serialize(object obj, bool camelCase = true, bool indented = false);
        T Deserialize<T>(string payload, bool camelCase = true);
        dynamic Deserialize(string payload, Type type, bool camelCase = true);
    }
}