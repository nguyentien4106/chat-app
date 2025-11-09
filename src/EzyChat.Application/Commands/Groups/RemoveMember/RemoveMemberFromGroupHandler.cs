using EzyChat.Application.DTOs.Common;
using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Hubs;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using EzyChat.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace EzyChat.Application.Commands.Groups.RemoveMember;

public class RemoveMemberFromGroupHandler(
    IRepository<GroupMember> groupMemberRepository,
    IHubContext<ChatHub> hubContext,
    IRepository<Group> groupRepository,
    IRepository<Message> messageRepository,
    ISignalRService signalRService
) : ICommandHandler<RemoveMemberFromGroupCommand, AppResponse<Unit>>
{

    public async Task<AppResponse<Unit>> Handle(RemoveMemberFromGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken: cancellationToken);
        if (group == null)
        {
            return AppResponse<Unit>.Fail("Group not found");
        }
        var removedBy = await groupMemberRepository.GetSingleAsync(
            gm => gm.GroupId == request.GroupId && gm.UserId == request.RemovedById,
            includeProperties: ["User"],
            cancellationToken: cancellationToken
        );

        if (removedBy == null || !removedBy.IsAdmin)
        {
            return AppResponse<Unit>.Fail("Only admins can remove members");
        }
        var memberToRemove = await groupMemberRepository.GetSingleAsync(
            gm => gm.GroupId == request.GroupId && gm.UserId == request.UserId,
            includeProperties: ["User"],
            cancellationToken: cancellationToken
        );

        if (memberToRemove == null)
            return AppResponse<Unit>.Fail("User is not a member of this group");

        // Prevent removing the group creator
        if (memberToRemove.UserId == group.CreatedById)
            return AppResponse<Unit>.Fail("Cannot remove the group creator");

        group.MemberCount -= 1;
        
        await groupMemberRepository.DeleteAsync(memberToRemove, cancellationToken: cancellationToken);
        await groupRepository.UpdateAsync(group, cancellationToken);
        // Create notification message
        var notificationMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = $"{memberToRemove.User.UserName} was removed from the group by {removedBy.User.UserName}",
            MessageType = MessageTypes.Notification,
            SenderId = request.RemovedById,
            GroupId = request.GroupId,
        };

        await messageRepository.AddAsync(notificationMessage, cancellationToken);
        var message = notificationMessage.Adapt<MessageDto>();

        var data = new
        {
            GroupId = request.GroupId, 
            UserId = request.UserId, 
            Message = message
        };
        
        await signalRService.NotifyGroupAsync(request.GroupId.ToString(), "OnGroupHasMemberLeft", data, cancellationToken);
        await signalRService.NotifyUserAsync(memberToRemove.Id.ToString(), "OnMemberLeftGroup", data, cancellationToken);
        
        return AppResponse<Unit>.Success(Unit.Value);
    }
}
