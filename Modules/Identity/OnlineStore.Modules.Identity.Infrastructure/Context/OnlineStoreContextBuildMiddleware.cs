using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OnlineStore.Modules.Identity.Application.Context;

namespace OnlineStore.Modules.Identity.Infrastructure
{
    public class OnlineStoreContextBuildMiddleware : IMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IOnlineStoreContextAccessor _storeContextAccessor;
        private readonly Dictionary<string, object> _applicationSettings;

        public OnlineStoreContextBuildMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnvironment,
            IOnlineStoreContextAccessor storeContextAccessor, IConfiguration configuration)
        {
            _next = next;
            _hostingEnvironment = hostingEnvironment;
            _storeContextAccessor = storeContextAccessor;
            _configuration = configuration;

            //Load a user-defined  settings from the special section.
            //All of these settings are accessible from the themes and through access to StoreContext.ApplicationSettings 
            _applicationSettings = _configuration.GetSection("OnlineStore:AppSettings").AsEnumerable()
                .Where(x => x.Value != null)
                .ToDictionary(x => x.Key.Replace("OnlineStore:AppSettings:", ""), x => (object)x.Value);
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            return null;
        }
    }
}