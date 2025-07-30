namespace BuildingBlocks.Pagination;

public class PaginatedResult<TEntity>(
    int pageIndex,
    int pageSize,
    long count,
    IEnumerable<TEntity> data)
{
    private int _pageSize = pageSize > 50 ? 50 : pageSize;

    public int PageIndex { get; } = pageIndex;
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > 50 ? 50 : value;
    }
    public long Count { get; } = count;
    public IEnumerable<TEntity> Data { get; } = data;
}
