using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Queries.Groups.GetUserConversations;

public class GetUserConversationsHandler(
    IRepository<Conversation> conversationRepository,
    IRepository<Message> messageRepository,
    IUserRepository userRepository,
    IChatAppDbContext _context
) : IRequestHandler<GetUserConversationsQuery, AppResponse<List<ConversationDto>>>
{

    public async Task<AppResponse<List<ConversationDto>>> Handle(GetUserConversationsQuery request, CancellationToken cancellationToken)
    {
        // Get all conversations where the user is a participant
        var userConversations = await conversationRepository.GetAllAsync(
            filter: c => c.SenderId == request.UserId || c.ReceiverId == request.UserId,
            includeProperties: ["Messages"],
            cancellationToken: cancellationToken);

        var result = new List<ConversationDto>();
        foreach (var conv in userConversations)
        {
            var otherUserId = conv.GetOtherUserId(request.UserId);
            var otherUser = await userRepository.GetByIdAsync(otherUserId, cancellationToken: cancellationToken);
            if (otherUser == null) continue;

            var lastMessage = conv.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
            if (lastMessage == null) continue;

            result.Add(new ConversationDto
            {
                Id = conv.Id,
                UserId = otherUserId,
                UserName = otherUser.UserName ?? string.Empty,
                LastMessage = lastMessage.Content ?? string.Empty,
                LastMessageAt = conv.LastMessageAt,
                UnreadCount = conv.Messages.Count(m => m.SenderId == otherUserId && !m.IsRead)
            });
        }

        return AppResponse<List<ConversationDto>>.Success(result.OrderByDescending(c => c.LastMessageAt).ToList());
    }
}