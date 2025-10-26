using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;

namespace ChatApp.Application.Commands.Groups.CreateGroup;


public class CreateGroupHandler : ICommandHandler<CreateGroupCommand, AppResponse<GroupDto>>
{
    private readonly IChatAppDbContext _context;

    public CreateGroupHandler(IChatAppDbContext context)
    {
        _context = context;
    }

    public async Task<AppResponse<GroupDto>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = new Group
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatedById = request.CreatedById,
            CreatedAt = DateTime.UtcNow
        };

        var member = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            UserId = request.CreatedById,
            JoinedAt = DateTime.UtcNow,
            IsAdmin = true
        };

        _context.Groups.Add(group);
        _context.GroupMembers.Add(member);
        await _context.SaveChangesAsync(cancellationToken);

        var groupDto = new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            CreatedById = group.CreatedById,
            CreatedAt = group.CreatedAt
        };

        return AppResponse<GroupDto>.Success(groupDto);
    }
}