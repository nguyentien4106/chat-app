using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Queries.Groups.GetUserGroups;

public class GetUserGroupsHandler: IQueryHandler<GetUserGroupsQuery, AppResponse<List<GroupDto>>>
{
    private readonly IChatAppDbContext _context;

    public GetUserGroupsHandler(IChatAppDbContext context)
    {
        _context = context;
    }

    public async Task<AppResponse<List<GroupDto>>> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await _context.GroupMembers
            .Where(gm => gm.UserId == request.UserId)
            .Select(gm => new GroupDto
            {
                Id = gm.Group.Id,
                Name = gm.Group.Name,
                Description = gm.Group.Description,
                CreatedById = gm.Group.CreatedById,
                CreatedAt = gm.Group.CreatedAt,
                MemberCount = gm.Group.Members.Count
            })
            .ToListAsync(cancellationToken);

        return AppResponse<List<GroupDto>>.Success(groups);
    }
}
