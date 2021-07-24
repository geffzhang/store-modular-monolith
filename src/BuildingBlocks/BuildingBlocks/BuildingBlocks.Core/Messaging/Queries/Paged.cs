using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Core.Messaging.Queries
{
    public class Paged<T> : PagedBase
    {
        public Paged()
        {
        }

        public Paged(IEnumerable<T> items,
            int currentPage, int resultsPerPage,
            int totalPages, long totalResults) :
            base(currentPage, resultsPerPage, totalPages, totalResults)
        {
            Items = items;
        }

        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        public bool IsEmpty => Items is null || !Items.Any();

        public static Paged<T> Empty => new();

        public static Paged<T> Create(IEnumerable<T> items,
            int currentPage, int resultsPerPage,
            int totalPages, long totalResults)
        {
            return new(items, currentPage, resultsPerPage, totalPages, totalResults);
        }

        public static Paged<T> From(PagedBase result, IEnumerable<T> items)
        {
            return new(items, result.CurrentPage, result.ResultsPerPage,
                result.TotalPages, result.TotalResults);
        }

        public Paged<TResult> Map<TResult>(Func<T, TResult> map)
        {
            return Paged<TResult>.From(this, Items.Select(map));
        }
    }
}