using System;
using BuildingBlocks.Core;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Web.Extensions
{
    public static class HostEnvironmentEnvExtensions
    {
        public static bool IsTest(this IHostEnvironment hostEnvironment) =>
            hostEnvironment?.IsEnvironment(Constants.CustomEnvironments.Test) ??
            throw new ArgumentNullException(nameof(hostEnvironment));
    }
}