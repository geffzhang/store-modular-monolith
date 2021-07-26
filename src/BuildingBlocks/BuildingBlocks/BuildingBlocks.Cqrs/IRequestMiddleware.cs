using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Cqrs
{
    public interface IRequestMiddleware<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> RunAsync(TRequest request, CancellationToken cancellationToken,
            HandleRequestDelegate<TRequest, TResponse> next);
    }
}