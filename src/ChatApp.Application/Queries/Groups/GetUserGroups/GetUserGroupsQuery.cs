using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;

namespace ChatApp.Application.Queries.Groups.GetUserGroups;

public class GetUserGroupsQuery: IQuery<AppResponse<List<GroupDto>>>
{
    public Guid UserId { get; set; }
}