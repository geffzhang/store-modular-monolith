using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace Common.Messages.Serialization.Json.Newtonsoft
{
    public class NewtonsoftJsonUnSupportedTypeMatcher
    {
        protected NewtonsoftJsonOptions Options { get; }

        public NewtonsoftJsonUnSupportedTypeMatcher(IOptions<NewtonsoftJsonOptions> options)
        {
            Options = options.Value;
        }

        public virtual bool Match([CanBeNull] Type type)
        {
            return Options.UnSupportedTypes.Contains(type);
        }
    }
}