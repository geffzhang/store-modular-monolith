// using System.Diagnostics;
// using System.Threading.Tasks;
// using BuildingBlocks.Core;
// using BuildingBlocks.Cqrs.Commands;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Logging;
//
// namespace BuildingBlocks.Logging.Serilog
// {
//     [Decorator]
//     public class CommandHandlerLoggingDecorator<T> : ICommandHandler<T> where T : class, ICommand
//     {
//         private readonly ICommandHandler<T> _handler;
//         private readonly ILogger<ICommandHandler<T>> _logger;
//         private readonly IHttpContextAccessor _httpContextAccessor;
//
//         public CommandHandlerLoggingDecorator(ICommandHandler<T> handler, ILogger<ICommandHandler<T>> logger,
//             IHttpContextAccessor httpContextAccessor)
//         {
//             _handler = handler;
//             _logger = logger;
//             _httpContextAccessor = httpContextAccessor;
//         }
//
//         public async Task HandleAsync(T command)
//         {
//             const string prefix = nameof(CommandHandlerLoggingDecorator<T>);
//             _logger.LogInformation("[{Prefix}] Handle request={X-RequestData}",
//                 prefix, typeof(T).Name);
//
//             var timer = new Stopwatch();
//             timer.Start();
//
//             await _handler.HandleAsync(command);
//
//             timer.Stop();
//             var timeTaken = timer.Elapsed;
//             if (timeTaken.Seconds > 3) // if the request is greater than 3 seconds, then log the warnings
//             {
//                 _logger.LogWarning("[{Perf-Possible}] The request {X-RequestData} took {TimeTaken} seconds.",
//                     prefix, typeof(T).Name, timeTaken.Seconds);
//             }
//
//             _logger.LogInformation("[{Prefix}] Handled {X-RequestData}", prefix, typeof(T).Name);
//         }
//     }
// }