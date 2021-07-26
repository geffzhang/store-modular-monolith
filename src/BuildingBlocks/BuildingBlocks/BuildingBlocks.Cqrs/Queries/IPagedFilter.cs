using System.Collections.Generic;

namespace BuildingBlocks.Cqrs.Queries
{
    public interface IPagedFilter<TResult, in TQuery> where TQuery : IQuery
    {
        Paged<TResult> Filter(IEnumerable<TResult> values, TQuery query);
    }
}