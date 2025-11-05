using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace EzyChat.Application.Queries.Groups.GetGroupInfo;

public class GetGroupInfoHandler(
    IRepository<Group> groupRepository,
    IRepository<GroupMember> groupMemberRepository,
    IEzyChatDbContext dbContext
) : IQueryHandler<GetGroupInfoQuery, AppResponse<GroupDto>>
{
    public async Task<AppResponse<GroupDto>> Handle(GetGroupInfoQuery request, CancellationToken cancellationToken)
    {
        // Check if user is a member of the group
        var member = await groupMemberRepository.GetSingleAsync(
            gm => gm.GroupId == request.GroupId && gm.UserId == request.UserId,
            cancellationToken: cancellationToken
        );

        if (member == null)
        {
            return AppResponse<GroupDto>.Fail("You are not a member of this group");
        }

        var group = await groupRepository.GetByIdAsync(
            request.GroupId,
            includeProperties: new[] { "Members" },
            cancellationToken: cancellationToken
        );

        if (group == null)
        {
            return AppResponse<GroupDto>.Fail("Group not found");
        }

        var groupMembers = await dbContext.GroupMembers
            .Where(gm => gm.GroupId == request.GroupId)
            .Include(gm => gm.User)
            .ToListAsync(cancellationToken);

        var groupDto = new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            CreatedById = group.CreatedById,
            CreatedAt = group.CreatedAt,
            MemberCount = group.Members.Count,
            Members = groupMembers.Adapt<List<GroupMemberDto>>()
        };

        return AppResponse<GroupDto>.Success(groupDto);
    }
}
