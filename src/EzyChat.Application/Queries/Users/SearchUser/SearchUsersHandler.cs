using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using EzyChat.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EzyChat.Application.Queries.Users.SearchUser;

public class SearchUsersHandler(
    IUserRepository userRepository
    )
    : IQueryHandler<SearchUsersQuery, AppResponse<List<UserDto>>>
{
    public async Task<AppResponse<List<UserDto>>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(
            filter: u => u.Id != request.CurrentUserId && 
                        ((u.UserName != null && u.UserName.Contains(request.SearchTerm)) || 
                         (u.Email != null && u.Email.Contains(request.SearchTerm))),
            cancellationToken: cancellationToken);
        
        var limitedUsers = users
            .Take(20)
            .ToList();

        return AppResponse<List<UserDto>>.Success(limitedUsers.Adapt<List<UserDto>>());
    }
}