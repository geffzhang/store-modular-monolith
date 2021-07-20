using System.Collections.Generic;
using Common.Core.Collections;
using Newtonsoft.Json;

namespace Common.Core.Messaging.Serialization.Newtonsoft
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