using EzyChat.Application.Hubs;
using EzyChat.Application.Interfaces;
using EzyChat.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using EzyChat.Application.DTOs.Common;
using EzyChat.Application.DTOs.Messages;

namespace EzyChat.Application.Commands.Groups.AddUserToGroup;

public class AddMemberToGroupHandler(
    IRepository<GroupMember> groupMemberRepository,
    IRepository<Group> groupRepository,
    IRepository<Message> messageRepository,
    IUserRepository userRepository,
    ISignalRService signalRService
) : ICommandHandler<AddMemberToGroupCommand, AppResponse<Unit>>
{

    public async Task<AppResponse<Unit>> Handle(AddMemberToGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken: cancellationToken);
        if (group == null)
            return AppResponse<Unit>.Fail("Group not found");

        var newMember = await userRepository.GetUserByUserNameAsync(request.UserName, cancellationToken: cancellationToken);
        if (newMember == null)
        {
            return AppResponse<Unit>.Fail("User not found");
        }

        var existingMember = await groupMemberRepository.GetSingleAsync(m => m.UserId == newMember.Id && m.GroupId == request.GroupId, cancellationToken: cancellationToken);

        if (existingMember != null)
            return AppResponse<Unit>.Fail("User is already a member");

        var groupMember = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = request.GroupId,
            UserId = newMember.Id,
            JoinedAt = DateTime.Now,
            IsAdmin = false
        };

        await groupMemberRepository.AddAsync(groupMember, cancellationToken);

        // Create notification message
        var notificationMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = $"{newMember.UserName} joined the group",
            MessageType = MessageTypes.Notification,
            SenderId = newMember.Id,
            GroupId = request.GroupId,
            CreatedAt = DateTime.Now,
        };

        await messageRepository.AddAsync(notificationMessage, cancellationToken);
        var groupDto = group.Adapt<GroupDto>();
        var messageDto = new MessageDto
        {
            Id = notificationMessage.Id,
            Content = notificationMessage.Content,
            MessageType = notificationMessage.MessageType,
            SenderId = notificationMessage.SenderId,
            GroupId = notificationMessage.GroupId,
        };
        
        groupDto.MemberCount += 1;

        object data = new
        {
            GroupId = request.GroupId,
            UserId = newMember.Id,
            NewMemberName = newMember.UserName,
            Group = groupDto,
            Message = messageDto
        };
        
        await signalRService.NotifyGroupAsync(request.GroupId.ToString(), "MemberAdded", data, cancellationToken);

        return AppResponse<Unit>.Success(Unit.Value);
    }
}