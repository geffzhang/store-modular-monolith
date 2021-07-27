using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Cqrs
{
    public interface IRequestProcessor<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> ProcessAsync(TRequest message,  CancellationToken cancellationToken);
    }
}