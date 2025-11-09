using EzyChat.Application.DTOs.Common;
using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Hubs;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using EzyChat.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EzyChat.Application.Commands.Groups.LeaveGroup;

public class LeaveGroupHandler(
    IRepository<GroupMember> groupMemberRepository,
    IRepository<Group> groupRepository,
    IRepository<Message> messageRepository,
    IHubContext<ChatHub> hubContext,
    ISignalRService signalRService
) : ICommandHandler<LeaveGroupCommand, AppResponse<Unit>>
{
    public async Task<AppResponse<Unit>> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken: cancellationToken);
        if (group == null)
            return AppResponse<Unit>.Fail("Group not found");

        var memberToRemove = await groupMemberRepository.GetSingleAsync(
            gm => gm.GroupId == request.GroupId && gm.UserId == request.UserId,
            includeProperties: ["User"],
            cancellationToken: cancellationToken
        );

        if (memberToRemove == null)
        {
            return AppResponse<Unit>.Fail("You are not a member of this group");
        }
        // Prevent the group creator from leaving
        if (memberToRemove.UserId == group.CreatedById)
        {
            return AppResponse<Unit>.Fail("Group creator cannot leave the group. Please transfer ownership or delete the group.");
        }
        group.MemberCount -= 1;
        
        await groupMemberRepository.DeleteAsync(memberToRemove, cancellationToken: cancellationToken);
        await groupRepository.UpdateAsync(group, cancellationToken);
        // Create notification message
        var notificationMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = $"{memberToRemove.User.UserName} left the group",
            MessageType = MessageTypes.Notification,
            SenderId = request.UserId,
            GroupId = request.GroupId,
        };

        await messageRepository.AddAsync(notificationMessage, cancellationToken);
        var messageDto = notificationMessage.Adapt<MessageDto>();
        object data = new
        {
            GroupId = request.GroupId,
            RemoveMemberId = memberToRemove.Id,
            Message = messageDto,
            MemberCount = group.MemberCount 
        };
        
        await signalRService.NotifyGroupAsync(request.GroupId.ToString(), "OnGroupHasMemberLeft", data, cancellationToken);
        await signalRService.NotifyUserAsync(memberToRemove.Id.ToString(), "OnMemberLeftGroup", data, cancellationToken);

        return AppResponse<Unit>.Success(Unit.Value);
    }
}
