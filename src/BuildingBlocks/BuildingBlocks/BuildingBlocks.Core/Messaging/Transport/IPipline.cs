using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Messaging.Transport
{
    public interface IPipeline
    {
        public Task Invoke(HttpContext context, Func<Task> next);
    }
}