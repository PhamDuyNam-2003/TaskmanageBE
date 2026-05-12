namespace BE.DTOs.Common
{
    public class PaginationParams
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }

    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;
    }
}
