using ChatApp.Application.Hubs;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using ChatApp.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Application.Commands.Groups.RemoveMember;

public class RemoveMemberFromGroupHandler(
    IRepository<GroupMember> groupMemberRepository,
    IHubContext<ChatHub> hubContext,
    IRepository<Group> groupRepository,
    IRepository<Message> messageRepository
) : ICommandHandler<RemoveMemberFromGroupCommand, AppResponse<Unit>>
{

    public async Task<AppResponse<Unit>> Handle(RemoveMemberFromGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken: cancellationToken);
        if (group == null)
            return AppResponse<Unit>.Fail("Group not found");

        var removedBy = await groupMemberRepository.GetSingleAsync(
            gm => gm.GroupId == request.GroupId && gm.UserId == request.RemovedById,
            includeProperties: new[] { "User" },
            cancellationToken: cancellationToken
        );

        if (removedBy == null || !removedBy.IsAdmin)
            return AppResponse<Unit>.Fail("Only admins can remove members");

        var memberToRemove = await groupMemberRepository.GetSingleAsync(
            gm => gm.GroupId == request.GroupId && gm.UserId == request.UserId,
            includeProperties: new[] { "User" },
            cancellationToken: cancellationToken
        );

        if (memberToRemove == null)
            return AppResponse<Unit>.Fail("User is not a member of this group");

        // Prevent removing the group creator
        if (memberToRemove.UserId == group.CreatedById)
            return AppResponse<Unit>.Fail("Cannot remove the group creator");

        await groupMemberRepository.DeleteAsync(memberToRemove, cancellationToken: cancellationToken);

        // Create notification message
        var notificationMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = $"{memberToRemove.User.UserName} was removed from the group by {removedBy.User.UserName}",
            MessageType = MessageTypes.Notification,
            SenderId = request.RemovedById,
            GroupId = request.GroupId,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await messageRepository.AddAsync(notificationMessage, cancellationToken);

        await hubContext.Clients.Group(request.GroupId.ToString())
            .SendAsync("MemberRemoved", new { GroupId = request.GroupId, UserId = request.UserId, RemovedMemberName = memberToRemove.User.UserName }, cancellationToken);

        return AppResponse<Unit>.Success(Unit.Value);
    }
}
