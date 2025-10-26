<<<<<<< HEAD
namespace ChatApp.Application.Commands.Groups.GenerateLink;

public class GenerateLinkHandler
{
    
=======
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Commands.Groups.GenerateLink;

public class GenerateLinkHandler: ICommandHandler<GenerateLinkCommand, AppResponse<string>>
{
    private readonly IChatAppDbContext _context;

    public GenerateLinkHandler(IChatAppDbContext context)
    {
        _context = context;
    }

    public async Task<AppResponse<string>> Handle(GenerateLinkCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups.FindAsync(request.GroupId);
        if (group == null)
            return AppResponse<string>.Fail("Group not found");

        var member = await _context.GroupMembers
            .FirstOrDefaultAsync(gm => gm.GroupId == request.GroupId && gm.UserId == request.RequestedById, cancellationToken);

        if (member == null || !member.IsAdmin)
            return AppResponse<string>.Fail("Only admins can generate invite links");

        group.InviteCode = Guid.NewGuid().ToString("N").Substring(0, 8);
        group.InviteCodeExpiresAt = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync(cancellationToken);

        return AppResponse<string>.Success(group.InviteCode);
    }
>>>>>>> a957673 (initial)
}