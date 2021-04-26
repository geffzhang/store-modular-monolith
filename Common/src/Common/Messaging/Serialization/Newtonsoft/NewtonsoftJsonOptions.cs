using System.Collections;
using System.Collections.Generic;
using Common.Collections;
using Newtonsoft.Json;

namespace Common.Messaging.Serialization.Newtonsoft
{
    public class NewtonsoftJsonOptions
    {
        public IList<JsonConverter> Converters { get; set; }
        public ITypeList UnSupportedTypes { get; }

        public NewtonsoftJsonOptions()
        {
            Converters = new List<JsonConverter>();
            UnSupportedTypes = new TypeList();
        }
    }
}