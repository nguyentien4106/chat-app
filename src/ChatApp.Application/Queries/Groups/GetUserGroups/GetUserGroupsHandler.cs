using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Queries.Groups.GetUserGroups;

public class GetUserGroupsHandler(
    IRepository<GroupMember> groupMemberRepository
) : IQueryHandler<GetUserGroupsQuery, AppResponse<List<GroupDto>>>
{

    public async Task<AppResponse<List<GroupDto>>> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
    {
        var result = await groupMemberRepository.GetAllAsync(
            gm => gm.UserId == request.UserId,
            includeProperties: new[] { "Group.Members" },
            cancellationToken: cancellationToken
        );

        var dtos = result.Select(gm => new GroupDto
        {
            Id = gm.Group.Id,
            Name = gm.Group.Name,
            Description = gm.Group.Description,
            CreatedById = gm.Group.CreatedById,
            CreatedAt = gm.Group.CreatedAt,
            MemberCount = gm.Group.Members.Count
        }).ToList();
        return AppResponse<List<GroupDto>>.Success(dtos);
    }
}
