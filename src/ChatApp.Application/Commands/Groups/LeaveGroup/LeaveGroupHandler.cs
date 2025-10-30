using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Hubs;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using ChatApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Commands.Groups.LeaveGroup;

public class LeaveGroupHandler(
    IRepository<GroupMember> groupMemberRepository,
    IRepository<Group> groupRepository,
    IRepository<Message> messageRepository,
    IHubContext<ChatHub> hubContext
) : ICommandHandler<LeaveGroupCommand, AppResponse<Unit>>
{
    public async Task<AppResponse<Unit>> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken: cancellationToken);
        if (group == null)
            return AppResponse<Unit>.Fail("Group not found");

        var member = await groupMemberRepository.GetSingleAsync(
            gm => gm.GroupId == request.GroupId && gm.UserId == request.UserId,
            includeProperties: new[] { "User" },
            cancellationToken: cancellationToken
        );

        if (member == null)
            return AppResponse<Unit>.Fail("You are not a member of this group");

        // Prevent the group creator from leaving
        if (member.UserId == group.CreatedById)
            return AppResponse<Unit>.Fail("Group creator cannot leave the group. Please transfer ownership or delete the group.");

        await groupMemberRepository.DeleteAsync(member, cancellationToken: cancellationToken);

        // Create notification message
        var notificationMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = $"{member.User.UserName} left the group",
            MessageType = MessageTypes.Notification,
            SenderId = request.UserId,
            GroupId = request.GroupId,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await messageRepository.AddAsync(notificationMessage, cancellationToken);
        var message = notificationMessage.Adapt<MessageDto>();
        
        await hubContext.Clients.Group(request.GroupId.ToString())
            .SendAsync("MemberLeft", new { GroupId = request.GroupId, UserId = request.UserId, MemberName = member.User.UserName, Message = message }, cancellationToken);

        return AppResponse<Unit>.Success(Unit.Value);
    }
}
