// using System.Diagnostics;
// using System.Threading.Tasks;
// using BuildingBlocks.Core;
// using BuildingBlocks.Cqrs.Queries;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Logging;
//
// namespace BuildingBlocks.Logging.Serilog
// {
//     [Decorator]
//     public class QueryHandlerLoggingDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
//         where TQuery : class, IQuery<TResult>
//     {
//         private readonly IQueryHandler<TQuery, TResult> _handler;
//         private readonly ILogger<IQueryHandler<TQuery, TResult>> _logger;
//         private readonly IHttpContextAccessor _httpContextAccessor;
//
//         public QueryHandlerLoggingDecorator(IQueryHandler<TQuery, TResult> handler,
//             ILogger<IQueryHandler<TQuery, TResult>> logger,
//             IHttpContextAccessor httpContextAccessor)
//         {
//             _handler = handler;
//             _logger = logger;
//             _httpContextAccessor = httpContextAccessor;
//         }
//
//         public async Task<TResult> HandleAsync(TQuery query)
//         {
//             const string prefix = nameof(QueryHandlerLoggingDecorator<TQuery, TResult>);
//
//             _logger.LogInformation("[{Prefix}] Handle request={X-RequestData} and response={X-ResponseData}",
//                 prefix, typeof(TQuery).Name, typeof(TResult).Name);
//
//             var timer = new Stopwatch();
//             timer.Start();
//
//             var response = await _handler.HandleAsync(query);
//
//             timer.Stop();
//             var timeTaken = timer.Elapsed;
//             if (timeTaken.Seconds > 3) // if the request is greater than 3 seconds, then log the warnings
//             {
//                 _logger.LogWarning("[{Perf-Possible}] The request {X-RequestData} took {TimeTaken} seconds.",
//                     prefix, typeof(TQuery).Name, timeTaken.Seconds);
//             }
//
//             _logger.LogInformation("[{Prefix}] Handled {X-RequestData}", prefix, typeof(TQuery).Name);
//             return response;
//         }
//     }
// }