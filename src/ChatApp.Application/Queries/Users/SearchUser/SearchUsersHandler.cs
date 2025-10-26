<<<<<<< HEAD
namespace ChatApp.Application.Queries.Users.SearchUser;

public class SearchUsersHandler
{
    
=======
using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Queries.Users.SearchUser;

public class SearchUsersHandler: IQueryHandler<SearchUsersQuery, AppResponse<List<UserDto>>>
{
    private readonly IChatAppDbContext _context;

    public SearchUsersHandler(IChatAppDbContext context)
    {
        _context = context;
    }

    public async Task<AppResponse<List<UserDto>>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        
        var users = await _context.Users
            .Where(u => u.Id != request.CurrentUserId && 
                        (u.UserName.Contains(request.SearchTerm) || 
                         u.Email.Contains(request.SearchTerm)))
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.UserName,
                Email = u.Email
            })
            .Take(20)
            .ToListAsync(cancellationToken);

        return AppResponse<List<UserDto>>.Success(users);
    }
>>>>>>> a957673 (initial)
}