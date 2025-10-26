using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;

namespace ChatApp.Application.Queries.Users.GetUser;

public class GetUserQuery : IQuery<AppResponse<UserDto>>
{
    public Guid UserId { get; set; }
}