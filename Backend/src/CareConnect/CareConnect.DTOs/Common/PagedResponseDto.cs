namespace CareConnect.DTOs.Common;

public sealed class PagedResponseDto<T>
{
    public required IReadOnlyList<T> Data { get; init; }

    public required int CurrentPage { get; init; }

    public required int PageSize { get; init; }

    public required int TotalRecords { get; init; }

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalRecords / (double)PageSize);
}
