using BuildingBlocks.Web.Contexts.Middleware;
using BuildingBlocks.Web.Middlewares;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UserWebApi(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<UserMiddleware>();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<ResourceIdGeneratorMiddleware>();
            app.UseMiddleware<LogContextMiddleware>();
            app.UseMiddleware<AddCorrelationContextToResponseMiddleware>();
            //https://github.com/khellang/Middleware
            app.UseProblemDetails();

            return app;
        }
    }
}