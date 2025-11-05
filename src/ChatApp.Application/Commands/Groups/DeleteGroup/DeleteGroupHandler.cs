using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Hubs;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Application.Commands.Groups.DeleteGroup;

public class DeleteGroupHandler(
    IRepository<Group> groupRepository,
    IHubContext<ChatHub> hubContext
) : ICommandHandler<DeleteGroupCommand, AppResponse<Unit>>
{
    public async Task<AppResponse<Unit>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetSingleAsync(
            g => g.Id == request.GroupId,
            includeProperties: new[] { "Members" },
            cancellationToken: cancellationToken
        );

        if (group == null)
            return AppResponse<Unit>.Fail("Group not found");

        // Only the group creator can delete the group
        if (group.CreatedById != request.UserId)
            return AppResponse<Unit>.Fail("Only the group creator can delete the group");

        // Delete the group - cascade delete will automatically remove members and messages
        await groupRepository.DeleteAsync(group, cancellationToken);

        // Notify all members that the group has been deleted
        await hubContext.Clients.Group(request.GroupId.ToString())
            .SendAsync("GroupDeleted", new { GroupId = request.GroupId, GroupName = group.Name }, cancellationToken);

        return AppResponse<Unit>.Success(Unit.Value);
    }
}
