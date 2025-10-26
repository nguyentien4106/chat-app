using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Queries.Messages.GetConversationMessages;

public class GetConversationMessagesHandler(IChatAppDbContext context)
    : IQueryHandler<GetConversationMessagesQuery, AppResponse<List<MessageDto>>>
{
    public async Task<AppResponse<List<MessageDto>>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await context.Messages
            .Where(m => m.GroupId == null &&
                        ((m.SenderId == request.User1Id && m.ReceiverId == request.User2Id) ||
                         (m.SenderId == request.User2Id && m.ReceiverId == request.User1Id)))
            .OrderByDescending(m => m.CreatedAt)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(cancellationToken);

        return AppResponse<List<MessageDto>>.Success(messages.Adapt<List<MessageDto>>());
    }
}