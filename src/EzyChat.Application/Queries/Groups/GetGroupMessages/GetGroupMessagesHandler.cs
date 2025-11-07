using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Interfaces;

namespace EzyChat.Application.Queries.Groups.GetGroupMessages;

public class GetGroupMessagesHandler(
    IMessageRepository messageRepository,
    IRepository<Group> groupRepository
) : IQueryHandler<GetGroupMessagesQuery, AppResponse<PagedResult<MessageDto>>>
{
    public async Task<AppResponse<PagedResult<MessageDto>>> Handle(
        GetGroupMessagesQuery request, 
        CancellationToken cancellationToken)
    {
        // Verify the group exists
        var group = await groupRepository.GetByIdAsync(
            request.GroupId,
            includeProperties: ["Members"],
            cancellationToken: cancellationToken);

        if (group == null)
        {
            return AppResponse<PagedResult<MessageDto>>.Fail("Group not found");
        }

        // Verify the current user is a member of this group
        if (!group.Members.Any(m => m.UserId == request.UserId))
        {
            return AppResponse<PagedResult<MessageDto>>.Fail("Access denied. You are not a member of this group.");
        }

        // Get paginated messages for this group
        var pagedMessages = await messageRepository.GetPagedResultAsync(
            request.BeforeDateTime,
            id: request.GroupId,
            type: "group",
            includeProperties: ["Sender"],
            cancellationToken: cancellationToken);

        return AppResponse<PagedResult<MessageDto>>.Success(pagedMessages);
    }
}
