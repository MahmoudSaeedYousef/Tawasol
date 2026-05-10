namespace Tawasol.Application.Common.Models;

public class PagedResult<T> : Result<IEnumerable<T>>
{
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    private PagedResult(IEnumerable<T> data, int count, int pageNumber, int pageSize)
        : base(true, "تم استرجاع البيانات بنجاح", data, null)
    {
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public static PagedResult<T> Success(IEnumerable<T> data, int count, int pageNumber, int pageSize)
    {
        return new PagedResult<T>(data, count, pageNumber, pageSize);
    }
}

public record PaginationParams(int PageNumber = 1, int PageSize = 10);
