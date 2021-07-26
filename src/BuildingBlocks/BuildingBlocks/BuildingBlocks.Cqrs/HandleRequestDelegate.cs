using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Cqrs
{
    public delegate Task<TResponse> HandleRequestDelegate<in TRequest, TResponse>(TRequest request,
        CancellationToken cancellationToken = default);
}