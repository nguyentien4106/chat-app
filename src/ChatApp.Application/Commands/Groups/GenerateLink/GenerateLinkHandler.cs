using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Commands.Groups.GenerateLink;

public class GenerateLinkHandler(
    IRepository<Group> groupRepository,
    IRepository<GroupMember> groupMemberRepository
) : ICommandHandler<GenerateLinkCommand, AppResponse<string>>
{

    public async Task<AppResponse<string>> Handle(GenerateLinkCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken: cancellationToken);
        if (group == null)
            return AppResponse<string>.Fail("Group not found");

        var member = await groupMemberRepository.GetSingleAsync(gm => gm.GroupId == request.GroupId && gm.UserId == request.RequestedById, cancellationToken: cancellationToken);

        if (member == null || !member.IsAdmin)
            return AppResponse<string>.Fail("Only admins can generate invite links");

        group.InviteCode = Guid.NewGuid().ToString("N").Substring(0, 8);
        group.InviteCodeExpiresAt = DateTime.UtcNow.AddDays(7);

        await groupRepository.UpdateAsync(group, cancellationToken: cancellationToken);

        return AppResponse<string>.Success(group.InviteCode);
    }
}