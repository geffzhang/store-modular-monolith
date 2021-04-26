using Common.Collections;
using Newtonsoft.Json;

namespace Common.Messages.Serialization.Json.Newtonsoft
{
    public class NewtonsoftJsonOptions
    {
        public ITypeList<JsonConverter> Converters { get; }
        public ITypeList UnSupportedTypes { get; }
        
        public NewtonsoftJsonOptions()
        {
            Converters = new TypeList<JsonConverter>();
            UnSupportedTypes = new TypeList();
        }
    }
}