using ChatApp.Domain.Entities.Base;

namespace ChatApp.Application.Models;

public class PaginationRequest
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string? SortBy { get; set; } = nameof(Entity<Guid>.CreatedAt);

    public string? SortOrder { get; set; } = "desc";
}
