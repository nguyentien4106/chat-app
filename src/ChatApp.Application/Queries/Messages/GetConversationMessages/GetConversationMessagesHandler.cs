using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Queries.Messages;

public class GetConversationMessageHandler: IQueryHandler<GetConversationMessagesQuery, List<MessageDto>>
{
    private readonly IChatAppDbContext _context;

    public GetConversationMessageHandler(IChatAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MessageDto>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _context.Messages
            .Where(m => m.GroupId == null &&
                        ((m.SenderId == request.User1Id && m.ReceiverId == request.User2Id) ||
                         (m.SenderId == request.User2Id && m.ReceiverId == request.User1Id)))
            .OrderByDescending(m => m.CreatedAt)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                SenderUsername = m.Sender.UserName,
                ReceiverId = m.ReceiverId,
                GroupId = m.GroupId,
                CreatedAt = m.CreatedAt,
                IsRead = m.IsRead
            })
            .ToListAsync(cancellationToken);

        return messages;
    }
}