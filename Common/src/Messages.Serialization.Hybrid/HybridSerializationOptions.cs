using Common.Core.Collections;
using Common.Core.Messaging.Serialization;

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