<<<<<<< HEAD
namespace ChatApp.Application.Queries.Messages.GetGroupMessages;

public class GetGroupMessageHandler: IQueryHandler<GetGroupMessagesQuery, List<MessageDto>>
{
    private readonly IChatAppDbContext _context;

    public GetGroupMessagesQueryHandler(IChatAppDbContext context)
=======
using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Queries.Messages.GetGroupMessages;

public class GetGroupMessagesHandler: IQueryHandler<GetGroupMessagesQuery, AppResponse<List<MessageDto>>>
{
    private readonly IChatAppDbContext _context;

    public GetGroupMessagesHandler(IChatAppDbContext context)
>>>>>>> a957673 (initial)
    {
        _context = context;
    }

<<<<<<< HEAD
    public async Task<List<MessageDto>> Handle(GetGroupMessagesQuery request, CancellationToken cancellationToken)
=======
    public async Task<AppResponse<List<MessageDto>>> Handle(GetGroupMessagesQuery request, CancellationToken cancellationToken)
>>>>>>> a957673 (initial)
    {
        var messages = await _context.Messages
            .Where(m => m.GroupId == request.GroupId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
<<<<<<< HEAD
                SenderUsername = m.Sender.Username,
=======
                SenderUsername = m.Sender.UserName,
>>>>>>> a957673 (initial)
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
