using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Queries.Groups.GetUserGroups;

public class GetUserGroupsHandler(
    IRepositoryPagedQuery<GroupMember> pagedGroupMemberRepository
) : IQueryHandler<GetUserGroupsQuery, AppResponse<PagedResult<GroupDto>>>
{

    public async Task<AppResponse<PagedResult<GroupDto>>> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
    {
        // Get paginated group members where the user is a member
        var pagedGroupMembers = await pagedGroupMemberRepository.GetPagedResultAsync(
            request,
            filter: gm => gm.UserId == request.UserId,
            includeProperties: new[] { "Group.Members" },
            cancellationToken: cancellationToken
        );

        var dtos = pagedGroupMembers.Items.Select(gm => new GroupDto
        {
            Id = gm.Group.Id,
            Name = gm.Group.Name,
            Description = gm.Group.Description,
            CreatedById = gm.Group.CreatedById,
            CreatedAt = gm.Group.CreatedAt,
            MemberCount = gm.Group.Members.Count
        }).ToList();

        // Create new PagedResult with DTOs
        var pagedResult = new PagedResult<GroupDto>(
            dtos,
            pagedGroupMembers.TotalCount,
            pagedGroupMembers.PageNumber,
            pagedGroupMembers.PageSize
        );

        return AppResponse<PagedResult<GroupDto>>.Success(pagedResult);
    }
}
