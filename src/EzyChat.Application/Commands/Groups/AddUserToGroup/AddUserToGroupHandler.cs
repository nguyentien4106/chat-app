using EzyChat.Application.Hubs;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using EzyChat.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using EzyChat.Application.DTOs.Common;

namespace EzyChat.Application.Commands.Groups.AddUserToGroup;


public class AddMemberToGroupHandler(
    IRepository<GroupMember> groupMemberRepository,
    IRepository<Group> groupRepository,
    IRepository<Message> messageRepository,
    IHubContext<ChatHub> hubContext,
    IUserRepository userRepository
) : ICommandHandler<AddMemberToGroupCommand, AppResponse<Unit>>
{

    public async Task<AppResponse<Unit>> Handle(AddMemberToGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken: cancellationToken);
        if (group == null)
            return AppResponse<Unit>.Fail("Group not found");

        // var addedBy = await groupMemberRepository.GetByIdAsync(request.AddedById, cancellationToken: cancellationToken);

        // if (addedBy == null || !addedBy.IsAdmin)
        //     return AppResponse<Unit>.Fail("Only admins can add members");

        var newMember = await userRepository.GetUserByUserNameAsync(request.UserName, cancellationToken: cancellationToken);
        if (newMember == null)
        {
            return AppResponse<Unit>.Fail("User not found");
        }

        var existingMember = await groupMemberRepository.GetSingleAsync(m => m.UserId == newMember.Id && m.GroupId == request.GroupId, cancellationToken: cancellationToken);

        if (existingMember != null)
            return AppResponse<Unit>.Fail("User is already a member");

        var member = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = request.GroupId,
            UserId = newMember.Id,
            JoinedAt = DateTime.Now,
            IsAdmin = false
        };

        await groupMemberRepository.AddAsync(member, cancellationToken);

        // Create notification message
        var notificationMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = $"{newMember.UserName} joined the group",
            MessageType = MessageTypes.Notification,
            SenderId = newMember.Id,
            GroupId = request.GroupId,
            CreatedAt = DateTime.Now,
            IsRead = false
        };

        await messageRepository.AddAsync(notificationMessage, cancellationToken);
        var groupDto = group.Adapt<GroupDto>();
        var messageDto = notificationMessage.Adapt<MessageDto>();

        groupDto.MemberCount += 1;

        await hubContext.Clients.Group(request.GroupId.ToString())
            .SendAsync("MemberAdded", new { GroupId = request.GroupId, UserId = newMember.Id, NewMemberName = newMember.UserName, Group = groupDto, Message = messageDto }, cancellationToken);

        await hubContext.Clients.User(newMember.Id.ToString())
            .SendAsync("MemberAdded", new { GroupId = request.GroupId, UserId = newMember.Id, NewMemberName = newMember.UserName, Group = groupDto, Message = messageDto }, cancellationToken);

        return AppResponse<Unit>.Success(Unit.Value);
    }
}