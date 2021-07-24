using System.Collections.Generic;

namespace BuildingBlocks.Core.Messaging.Queries
{
    public interface IPagedFilter<TResult, in TQuery> where TQuery : IQuery
    {
        Paged<TResult> Filter(IEnumerable<TResult> values, TQuery query);
    }
}