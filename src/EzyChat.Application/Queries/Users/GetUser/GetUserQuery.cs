using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Models;

namespace EzyChat.Application.Queries.Users.GetUser;

public class GetUserQuery : IQuery<AppResponse<UserDto>>
{
    public Guid UserId { get; set; }
}