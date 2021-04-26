using System;
using MessagePack;
using MessagePack.Resolvers;

namespace Common.Modules
{
    internal class MessagePackModuleSerializer : IModuleSerializer
    {
        private readonly MessagePackSerializerOptions _options =
            ContractlessStandardResolverAllowPrivate.Options;

        public byte[] Serialize<T>(T value)
        {
            return MessagePackSerializer.Serialize(value, _options);
        }

        public T Deserialize<T>(byte[] value)
        {
            return MessagePackSerializer.Deserialize<T>(value, _options);
        }

        public object Deserialize(byte[] value, Type type)
        {
            return MessagePackSerializer.Deserialize(type, value, _options);
        }
    }
}