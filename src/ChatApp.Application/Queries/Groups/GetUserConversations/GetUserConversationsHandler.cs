using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Queries.Groups.GetUserConversations;

public class GetUserConversationsHandler(
    IRepository<Conversation> conversationRepository,
    IChatAppDbContext _context
) : IRequestHandler<GetUserConversationsQuery, AppResponse<List<ConversationDto>>>
{

    public async Task<AppResponse<List<ConversationDto>>> Handle(GetUserConversationsQuery request, CancellationToken cancellationToken)
    {

        var conversations = await _context.Messages
            .Where(m => m.GroupId == null && (m.SenderId == request.UserId || m.ReceiverId == request.UserId))
            .GroupBy(m => m.SenderId == request.UserId ? m.ReceiverId : m.SenderId)
            .Select(g => new
            {
                OtherUserId = g.Key,
                LastMessage = g.OrderByDescending(m => m.CreatedAt).First()
            })
            .ToListAsync(cancellationToken);

        var result = new List<ConversationDto>();
        foreach (var conv in conversations)
        {
            if (!conv.OtherUserId.HasValue) continue;

            var otherUser = await _context.Users.FindAsync(conv.OtherUserId.Value, cancellationToken);
            if (otherUser == null) continue;

            result.Add(new ConversationDto
            {
                UserId = conv.OtherUserId.Value,
                Username = otherUser.UserName,
                LastMessage = conv.LastMessage.Content,
                LastMessageAt = conv.LastMessage.CreatedAt,
                UnreadCount = await _context.Messages
                    .CountAsync(m => m.SenderId == conv.OtherUserId.Value && 
                                     m.ReceiverId == request.UserId && 
                                     !m.IsRead, cancellationToken)
            });
        }

        return AppResponse<List<ConversationDto>>.Success(result.OrderByDescending(c => c.LastMessageAt).ToList());
    }
}