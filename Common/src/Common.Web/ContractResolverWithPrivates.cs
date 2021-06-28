using System.Reflection;
using Newtonsoft.Json;

namespace Common.Web
{
    public class ContractResolverWithPrivates : Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver
    {
        protected override Newtonsoft.Json.Serialization.JsonProperty CreateProperty(System.Reflection.MemberInfo member,
            MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (!prop.Writable) {
                var property = member as PropertyInfo;
                var hasPrivateSetter = property?.GetSetMethod(true) != null;
                prop.Writable = hasPrivateSetter;
            }
            return prop;
        }
    }
}