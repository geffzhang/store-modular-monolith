using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Common.Messaging.Serialization.Newtonsoft
{
    public class NewtonsoftJsonMessageSerializer : IMessageSerializer
    {
        private readonly NewtonsoftJsonUnSupportedTypeMatcher _newtonsoftJsonUnSupportedTypeMatcher;
        private readonly IList<JsonConverter> _converters;

        public NewtonsoftJsonMessageSerializer(
            IOptions<NewtonsoftJsonOptions> options,
            NewtonsoftJsonUnSupportedTypeMatcher newtonsoftJsonUnSupportedTypeMatcher,
            IServiceProvider serviceProvider)
        {
            _newtonsoftJsonUnSupportedTypeMatcher = newtonsoftJsonUnSupportedTypeMatcher;
            _converters = options.Value.Converters;
        }

        public bool CanHandle(Type type)
        {
            return !_newtonsoftJsonUnSupportedTypeMatcher.Match(type);
        }

        public string Serialize(object obj, bool camelCase = true, bool indented = false)
        {
            return JsonConvert.SerializeObject(obj, CreateSerializerSettings(camelCase, indented));
        }

        public T Deserialize<T>(string payload, bool camelCase = true)
        {
            return JsonConvert.DeserializeObject<T>(payload, CreateSerializerSettings(camelCase));
        }

        public object Deserialize(string payload, Type type, bool camelCase = true)
        {
            return JsonConvert.DeserializeObject(payload, type, CreateSerializerSettings(camelCase));
        }

        protected virtual JsonSerializerSettings CreateSerializerSettings(bool camelCase = true, bool indented = false)
        {
            var settings = new JsonSerializerSettings();

            ((List<JsonConverter>) settings.Converters).AddRange(_converters);

            if (camelCase)
            {
                settings.ContractResolver = new CamelCaseExceptDictionaryKeysResolver();
            }

            if (indented)
            {
                settings.Formatting = Formatting.Indented;
            }

            return settings;
        }

        private class CamelCaseExceptDictionaryKeysResolver : CamelCasePropertyNamesContractResolver
        {
            protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
            {
                var contract = base.CreateDictionaryContract(objectType);

                contract.DictionaryKeyResolver = propertyName => propertyName;

                return contract;
            }
        }
    }
}