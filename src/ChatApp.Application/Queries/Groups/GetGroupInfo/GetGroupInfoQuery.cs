using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;

namespace ChatApp.Application.Queries.Groups.GetGroupInfo;

public class GetGroupInfoQuery : IQuery<AppResponse<GroupDto>>
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
}
