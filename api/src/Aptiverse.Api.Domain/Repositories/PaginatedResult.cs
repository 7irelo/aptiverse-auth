namespace Aptiverse.Api.Domain.Repositories
{
    public record PaginatedResult<T>(
        IEnumerable<T> Data,
        int TotalRecords,
        int PageNumber,
        int PageSize,
        int TotalPages,
        int CurrentPageRecords,
        bool HasPreviousPage,
        bool HasNextPage)
    {
        public PaginatedResult(IEnumerable<T> data, int totalRecords, int pageNumber, int pageSize)
            : this(
                data,
                totalRecords,
                pageNumber,
                pageSize,
                (int)Math.Ceiling(totalRecords / (double)pageSize),
                data.Count(),
                pageNumber > 1,
                pageNumber < (int)Math.Ceiling(totalRecords / (double)pageSize))
        {
        }
    }
}