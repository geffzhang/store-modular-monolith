using Common.Collections;
using Common.Serialization;

namespace Common.Messages.Serialization.Hybrid
{
    public class JsonOptions
    {
        public ITypeList<IMessageSerializer> Providers { get; }

        public JsonOptions()
        {
            Providers = new TypeList<IMessageSerializer>();
        }
    }
}