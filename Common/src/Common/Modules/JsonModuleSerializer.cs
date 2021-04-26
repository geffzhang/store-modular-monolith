using System;
using System.Text;
using Newtonsoft.Json;

namespace Common.Modules
{
    internal class JsonModuleSerializer : IModuleSerializer
    {
        public byte[] Serialize<T>(T value)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
        }

        public T Deserialize<T>(byte[] value)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(value));
        }

        public object Deserialize(byte[] value, Type type)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(value), type);
        }
    }
}