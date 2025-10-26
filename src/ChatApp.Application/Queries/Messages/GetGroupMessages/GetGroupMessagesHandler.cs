using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Queries.Messages.GetGroupMessages;

public class GetGroupMessagesHandler: IQueryHandler<GetGroupMessagesQuery, AppResponse<List<MessageDto>>>
{
    private readonly IChatAppDbContext _context;

    public GetGroupMessagesHandler(IChatAppDbContext context)
    {
        _context = context;
    }

    public async Task<AppResponse<List<MessageDto>>> Handle(GetGroupMessagesQuery request, CancellationToken cancellationToken)
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
                SenderUsername = m.Sender.UserName,
                GroupId = m.GroupId,
                CreatedAt = m.CreatedAt,
                IsRead = m.IsRead
            })
            .ToListAsync(cancellationToken);

        return AppResponse<List<MessageDto>>.Success(messages);
    }
}
