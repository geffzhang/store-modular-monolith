using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Common.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Common.Web.Middlewares
{
    internal sealed class ResourceIdGeneratorMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Method != "POST")
            {
                await next(context);
                return;
            }

            var message = await context.Request.ReadFromJsonAsync<object>();
            var resourceId = Guid.NewGuid().ToString("N");
            if (message is JObject jObject)
            {
                jObject.SetResourceId(resourceId);
            }

            context.SetResourceIdFoRequest(resourceId);
            await using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
            context.Request.Body = memoryStream;
            await next(context);
        }
    }
}