using System;
using Microsoft.Extensions.Hosting;

namespace Common.Web.Extensions
{
    public static class HostEnvironmentEnvExtensions
    {
        public static bool IsTest(this IHostEnvironment hostEnvironment) =>
            hostEnvironment?.IsEnvironment(Consts.CustomEnvironments.Tests) ??
            throw new ArgumentNullException(nameof(hostEnvironment));
    }
}