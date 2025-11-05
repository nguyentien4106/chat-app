using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Models;

namespace EzyChat.Application.Queries.Groups.GetUserGroups;

public class GetUserGroupsQuery : PaginationRequest, IQuery<AppResponse<PagedResult<GroupDto>>>
{
    public Guid UserId { get; set; }
}