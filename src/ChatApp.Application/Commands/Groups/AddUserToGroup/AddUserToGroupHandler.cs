using ChatApp.Application.Hubs;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Application.Commands.Groups.AddUserToGroup;


public class AddMemberToGroupHandler : ICommandHandler<AddMemberToGroupCommand, AppResponse<Unit>>
{
    private readonly IChatAppDbContext _context;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IUserRepository _userRepository;

    public AddMemberToGroupHandler(IChatAppDbContext context, IHubContext<ChatHub> hubContext, IUserRepository userRepository)
    {
        _context = context;
        _hubContext = hubContext;
        _userRepository = userRepository;
    }

    public async Task<AppResponse<Unit>> Handle(AddMemberToGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups.FindAsync(request.GroupId);
        if (group == null)
            return AppResponse<Unit>.Fail("Group not found");

        var addedBy = await _context.GroupMembers
            .FirstOrDefaultAsync(gm => gm.GroupId == request.GroupId && gm.UserId == request.AddedById, cancellationToken);

        if (addedBy == null || !addedBy.IsAdmin)
            return AppResponse<Unit>.Fail("Only admins can add members");

        var newMember = await _userRepository.GetUserByUserNameAsync(request.UserName, cancellationToken: cancellationToken);
        if (newMember == null)
        {
            return AppResponse<Unit>.Fail("User not found");
        }

        var existingMember = await _context.GroupMembers
            .FirstOrDefaultAsync(gm => gm.GroupId == request.GroupId && gm.UserId == newMember.Id, cancellationToken);

        if (existingMember != null)
            return AppResponse<Unit>.Fail("User is already a member");

        var member = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = request.GroupId,
            UserId = newMember.Id,
            JoinedAt = DateTime.UtcNow,
            IsAdmin = false
        };

        _context.GroupMembers.Add(member);
        await _context.SaveChangesAsync(cancellationToken);

        await _hubContext.Clients.Group(request.GroupId.ToString())
            .SendAsync("MemberAdded", new { GroupId = request.GroupId, UserId = newMember.Id, NewMemberName = newMember.UserName }, cancellationToken);

        return AppResponse<Unit>.Success(Unit.Value);
    }
}