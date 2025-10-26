<<<<<<< HEAD
namespace ChatApp.Application.Queries.Users.GetUser;

public class GetUserHandler
{
    
=======
using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Queries.Users.GetUser;

public class GetUserHandler : IQueryHandler<GetUserQuery, AppResponse<UserDto>>
{
    private readonly IChatAppDbContext _context;

    public GetUserHandler(IChatAppDbContext context)
    {
        _context = context;
    }

    public async Task<AppResponse<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.UserName,
                Email = u.Email
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return AppResponse<UserDto>.Fail("User not found");
        }

        return AppResponse<UserDto>.Success(user);
    }
>>>>>>> a957673 (initial)
}