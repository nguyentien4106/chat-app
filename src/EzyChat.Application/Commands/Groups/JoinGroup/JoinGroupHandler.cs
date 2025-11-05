using EzyChat.Domain.Enums;
using MediatR;

namespace EzyChat.Application.Commands.Groups.JoinGroup;

public class JoinGroupHandler(
    IRepository<Group> groupRepository,
    IRepository<GroupMember> groupMemberRepository,
    IRepository<Message> messageRepository,
    IUserRepository userRepository
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
            return AppResponse<Unit>.Fail("User is already a member");

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken: cancellationToken);
        if (user == null)
            return AppResponse<Unit>.Fail("User not found");

        var member = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            UserId = request.UserId,
            JoinedAt = DateTime.Now,
            IsAdmin = false
        };

        await groupMemberRepository.AddAsync(member, cancellationToken: cancellationToken);

        // Create notification message
        var notificationMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = $"{user.UserName} joined the group",
            MessageType = MessageTypes.Notification,
            SenderId = request.UserId,
            GroupId = group.Id,
            CreatedAt = DateTime.Now,
            IsRead = false
        };

        await messageRepository.AddAsync(notificationMessage, cancellationToken);

        return AppResponse<Unit>.Success(Unit.Value);
    }
}