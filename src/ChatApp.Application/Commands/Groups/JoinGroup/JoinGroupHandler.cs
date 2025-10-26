<<<<<<< HEAD
namespace ChatApp.Application.Commands.Groups.JoinGroup;

public class JoinGroupHandler
{
    
=======
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Commands.Groups.JoinGroup;

public class JoinGroupHandler: ICommandHandler<JoinGroupByInviteCommand, AppResponse<Unit>>
{
    private readonly IChatAppDbContext _context;

    public JoinGroupHandler(IChatAppDbContext context)
    {
        _context = context;
    }

    public async Task<AppResponse<Unit>> Handle(JoinGroupByInviteCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups
            .FirstOrDefaultAsync(g => g.InviteCode == request.InviteCode, cancellationToken);

        if (group == null || group.InviteCodeExpiresAt < DateTime.UtcNow)
            return AppResponse<Unit>.Fail("Invalid or expired invite code");

        var existingMember = await _context.GroupMembers
            .FirstOrDefaultAsync(gm => gm.GroupId == group.Id && gm.UserId == request.UserId, cancellationToken);

        if (existingMember != null)
            return AppResponse<Unit>.Fail("User is already a member");

        var member = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            UserId = request.UserId,
            JoinedAt = DateTime.UtcNow,
            IsAdmin = false
        };

        _context.GroupMembers.Add(member);
        await _context.SaveChangesAsync(cancellationToken);

        return AppResponse<Unit>.Success(Unit.Value);
    }
>>>>>>> a957673 (initial)
}