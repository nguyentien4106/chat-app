using MediatR;

namespace ChatApp.Application.Commands.Groups.JoinGroup;

public class JoinGroupHandler(
    IRepository<Group> groupRepository,
    IRepository<GroupMember> groupMemberRepository
) : ICommandHandler<JoinGroupByInviteCommand, AppResponse<Unit>>
{

    public async Task<AppResponse<Unit>> Handle(JoinGroupByInviteCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetSingleAsync(g => g.InviteCode == request.InviteCode, cancellationToken: cancellationToken);

        if(group == null)
        {
            return AppResponse<Unit>.Fail("Group not found");
        }
        if (group.InviteCodeExpiresAt < DateTime.UtcNow)
        {
            return AppResponse<Unit>.Fail("Expired invite code");
        }

        var existingMember = await groupMemberRepository.GetSingleAsync(gm => gm.GroupId == group.Id && gm.UserId == request.UserId, cancellationToken: cancellationToken);

        if (existingMember != null)
            return AppResponse<Unit>.Fail("User is already a member");

        var member = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            UserId = request.UserId,
            JoinedAt = DateTime.UtcNow,
            IsAdmin = false
        };

        await groupMemberRepository.AddAsync(member, cancellationToken: cancellationToken);

        return AppResponse<Unit>.Success(Unit.Value);
    }
}