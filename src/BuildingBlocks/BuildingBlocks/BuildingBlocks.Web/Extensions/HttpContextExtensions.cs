using System;
using System.Collections.Generic;
using BuildingBlocks.Web.Contexts;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace BuildingBlocks.Web.Extensions
{
    public static class HttpContextExtensions
    {
        public static void SetResourceId(this JObject jObject, string id)
        {
            var idProperty = jObject.Property("id", StringComparison.InvariantCultureIgnoreCase);
            if (idProperty is null)
            {
                jObject.Add("id", id);
                return;
            }

            idProperty.Value = id;
        }

        public static string GetResourceIdFoRequest(this HttpContext context, ExecutionContextOptions options = null)
            => context.Items.TryGetValue(options?.ResourceIdHeaderKey ?? "resource-id", out var id)
                ? id as string
                : string.Empty;

        public static void SetResourceIdFoRequest(this HttpContext context, string id, ExecutionContextOptions
            options = null)
            => context.Items.TryAdd(options?.ResourceIdHeaderKey ?? "resource-id", id);
    }
}