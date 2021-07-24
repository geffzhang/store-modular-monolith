namespace BuildingBlocks.Core.Messaging.Queries
{
    public abstract class PagedBase
    {
        protected PagedBase()
        {
        }

        protected PagedBase(int currentPage, int resultsPerPage,
            int totalPages, long totalResults)
        {
            CurrentPage = currentPage > totalPages ? totalPages : currentPage;
            ResultsPerPage = resultsPerPage;
            TotalPages = totalPages;
            TotalResults = totalResults;
        }

        public int CurrentPage { get; set; }
        public int ResultsPerPage { get; set; }
        public int TotalPages { get; set; }
        public long TotalResults { get; set; }
    }
}