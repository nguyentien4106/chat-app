using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Hubs;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace EzyChat.Application.Commands.Groups.DeleteGroup;

public class DeleteGroupHandler(
    IRepository<Group> groupRepository,
    ISignalRService signalRService
) : ICommandHandler<DeleteGroupCommand, AppResponse<Unit>>
{
    public async Task<AppResponse<Unit>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetSingleAsync(
            g => g.Id == request.GroupId,
            cancellationToken: cancellationToken
        );

        if (group == null)
        {
            return AppResponse<Unit>.Fail("Group not found");
        }
        
        // Only the group creator can delete the group
        if (group.CreatedById != request.UserId)
        {
            return AppResponse<Unit>.Fail("Only the group creator can delete the group");
        }
        
        // Delete the group - cascade delete will automatically remove members and messages
        await groupRepository.DeleteAsync(group, cancellationToken);

        // Notify all members that the group has been deleted
        object data = new
        {
            GroupId = request.GroupId,
            GroupName = group.Name
        };
        await signalRService.NotifyGroupAsync(request.GroupId.ToString(), "GroupDeleted", data, cancellationToken);

        return AppResponse<Unit>.Success(Unit.Value);
    }
}
