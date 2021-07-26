using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Cqrs
{
    public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}