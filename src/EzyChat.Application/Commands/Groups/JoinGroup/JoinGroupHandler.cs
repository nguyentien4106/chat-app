using EzyChat.Application.DTOs.Common;
using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Interfaces;
using EzyChat.Domain.Enums;
using MediatR;

namespace EzyChat.Application.Commands.Groups.JoinGroup;

public class JoinGroupHandler(
    IRepository<Group> groupRepository,
    IRepository<GroupMember> groupMemberRepository,
    IRepository<Message> messageRepository,
    IUserRepository userRepository,
    ISignalRService signalRService
) : ICommandHandler<JoinGroupByInviteCommand, AppResponse<Unit>>
{

    public async Task<AppResponse<Unit>> Handle(JoinGroupByInviteCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetSingleAsync(g => g.InviteCode == request.InviteCode, cancellationToken: cancellationToken);

        if(group == null)
        {
            return AppResponse<Unit>.Fail("Group not found");
        }
        if (group.InviteCodeExpiresAt < DateTime.Now)
        {
            return AppResponse<Unit>.Fail("Expired invite code");
        }

        var existingMember = await groupMemberRepository.GetSingleAsync(gm => gm.GroupId == group.Id && gm.UserId == request.UserId, cancellationToken: cancellationToken);

        if (existingMember != null)
        {
            return AppResponse<Unit>.Fail("User is already a member");
        }

        var newMember = await userRepository.GetByIdAsync(request.UserId, cancellationToken: cancellationToken);
        if (newMember == null)
        {
            return AppResponse<Unit>.Fail("User not found");
        }
        
        var groupMember = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            UserId = request.UserId,
            JoinedAt = DateTime.Now,
            IsAdmin = false
        };

        await groupMemberRepository.AddAsync(groupMember, cancellationToken: cancellationToken);

        // Create notification message
        var notificationMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = $"{newMember.UserName} joined the group via invite code.",
            MessageType = MessageTypes.Notification,
            SenderId = newMember.Id,
            GroupId = group.Id,
            CreatedAt = DateTime.Now,
        };
        group.MemberCount += 1;
        
        await messageRepository.AddAsync(notificationMessage, cancellationToken);
        await groupRepository.UpdateAsync(group, cancellationToken);
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
            UserId = newMember.Id,
            Group = groupDto,
            Message = messageDto
        };
        
        await signalRService.NotifyGroupAsync(group.Id.ToString(), "OnGroupHasNewMember", data, cancellationToken);
        await signalRService.NotifyUserAsync(newMember.Id.ToString(), "OnMemberJoinGroup", data, cancellationToken);
        
        return AppResponse<Unit>.Success(Unit.Value);
    }
}