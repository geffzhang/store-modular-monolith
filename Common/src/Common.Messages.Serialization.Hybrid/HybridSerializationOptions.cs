using Common.Collections;
using Common.Messaging.Serialization;
using Common.Serialization;

namespace Common.Messages.Serialization.Hybrid
{
    public class HybridSerializationOptions
    {
        public ITypeList<IMessageSerializer> Providers { get; }

        public HybridSerializationOptions()
        {
            Providers = new TypeList<IMessageSerializer>();
        }
    }
}