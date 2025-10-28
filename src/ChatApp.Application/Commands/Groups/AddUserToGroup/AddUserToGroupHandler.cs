using ChatApp.Application.Hubs;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Application.Commands.Groups.AddUserToGroup;


public class AddMemberToGroupHandler(
    IRepository<GroupMember> groupMemberRepository,
    IRepository<Group> groupRepository,
    IHubContext<ChatHub> hubContext,
    IUserRepository userRepository
) : ICommandHandler<AddMemberToGroupCommand, AppResponse<Unit>>
{

    public async Task<AppResponse<Unit>> Handle(AddMemberToGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken: cancellationToken);
        if (group == null)
            return AppResponse<Unit>.Fail("Group not found");

        var addedBy = await groupMemberRepository.GetByIdAsync(request.AddedById, cancellationToken: cancellationToken);

        if (addedBy == null || !addedBy.IsAdmin)
            return AppResponse<Unit>.Fail("Only admins can add members");

        var newMember = await userRepository.GetUserByUserNameAsync(request.UserName, cancellationToken: cancellationToken);
        if (newMember == null)
        {
            return AppResponse<Unit>.Fail("User not found");
        }

        var existingMember = await groupMemberRepository.GetByIdAsync(newMember.Id, cancellationToken: cancellationToken);

        if (existingMember != null)
            return AppResponse<Unit>.Fail("User is already a member");

        var member = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = request.GroupId,
            UserId = newMember.Id,
            JoinedAt = DateTime.UtcNow,
            IsAdmin = false
        };

        await groupMemberRepository.AddAsync(member);

        await hubContext.Clients.Group(request.GroupId.ToString())
            .SendAsync("MemberAdded", new { GroupId = request.GroupId, UserId = newMember.Id, NewMemberName = newMember.UserName }, cancellationToken);

        return AppResponse<Unit>.Success(Unit.Value);
    }
}