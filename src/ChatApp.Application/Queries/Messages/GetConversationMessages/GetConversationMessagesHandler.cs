using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Queries.Messages;

public class GetConversationMessageHandler: IQueryHandler<GetConversationMessagesQuery, List<MessageDto>>
{
    private readonly IChatAppDbContext _context;

    public GetConversationMessageHandler(IChatAppDbContext context)
=======
using ChatApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Queries.Messages.GetConversationMessages;

public class GetConversationMessagesHandler: IQueryHandler<GetConversationMessagesQuery, AppResponse<List<MessageDto>>>
{
    private readonly IChatAppDbContext _context;

    public GetConversationMessagesHandler(IChatAppDbContext context)
>>>>>>> a957673 (initial)
    {
        _context = context;
    }

<<<<<<< HEAD
    public async Task<List<MessageDto>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
=======
    public async Task<AppResponse<List<MessageDto>>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
>>>>>>> a957673 (initial)
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

<<<<<<< HEAD
        return messages;
=======
        return AppResponse<List<MessageDto>>.Success(messages);
>>>>>>> a957673 (initial)
    }
}