using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Models;

namespace EzyChat.Application.Queries.Groups.GetGroupInfo;

public class GetGroupInfoQuery : IQuery<AppResponse<GroupDto>>
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
}
