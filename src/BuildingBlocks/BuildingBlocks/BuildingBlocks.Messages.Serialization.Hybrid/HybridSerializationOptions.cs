using BuildingBlocks.Core.Collections;
using BuildingBlocks.Core.Messaging.Serialization;

namespace BuildingBlocks.Messages.Serialization.Hybrid
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