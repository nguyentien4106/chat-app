using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;

namespace ChatApp.Application.Queries.Users.SearchUser;

// Application/Users/Queries/SearchUsersQuery.cs
public class SearchUsersQuery : IQuery<AppResponse<List<UserDto>>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public Guid CurrentUserId { get; set; }
}