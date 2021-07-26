using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Cqrs
{
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}