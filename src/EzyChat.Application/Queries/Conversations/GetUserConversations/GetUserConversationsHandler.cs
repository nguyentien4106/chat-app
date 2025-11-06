using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EzyChat.Application.Queries.Conversations.GetUserConversations;

public class GetUserConversationsHandler(
    IUserRepository userRepository,
    IRepositoryPagedQuery<Conversation> pagedConversationRepository,
    IEzyChatDbContext context
    ) : IRequestHandler<GetUserConversationsQuery, AppResponse<PagedResult<ConversationDto>>>
{
    public async Task<AppResponse<PagedResult<ConversationDto>>> Handle(GetUserConversationsQuery request, CancellationToken cancellationToken)
    {
        // Get paginated conversations where the user is a participant
        var pagedConversations = await pagedConversationRepository.GetPagedResultAsync(
            request,
            filter: c => c.SenderId == request.UserId || c.ReceiverId == request.UserId,
            includeProperties: ["Messages"],
            cancellationToken: cancellationToken
        );

        var result = new List<ConversationDto>();
        
        foreach (var conv in pagedConversations.Items)
        {
            var otherUserId = conv.GetOtherUserId(request.UserId);
            var otherUser = await userRepository.GetByIdAsync(otherUserId, cancellationToken: cancellationToken);
            if (otherUser == null) continue;

            var lastMessage = conv.Messages
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault();
            
            if (lastMessage == null) continue;

            result.Add(new ConversationDto
            {
                Id = conv.Id,
                UserId = otherUserId,
                UserName = otherUser.UserName ?? string.Empty,
                LastMessage = lastMessage.Content ?? string.Empty,
                LastMessageAt = conv.LastMessageAt,
                UnreadCount = conv.Messages.Count(m => m.SenderId == otherUserId && !m.IsRead),
                IsLastMessageMine = lastMessage.SenderId == request.UserId,
                UserFullName = otherUser.GetFullName(),
            });
        }

        // Order by last message time
        var orderedResult = result.OrderByDescending(c => c.LastMessageAt).ToList();

        // Create new PagedResult with DTOs using constructor
        var pagedResult = new PagedResult<ConversationDto>(
            orderedResult,
            pagedConversations.TotalCount,
            pagedConversations.PageNumber,
            pagedConversations.PageSize
        );

        return AppResponse<PagedResult<ConversationDto>>.Success(pagedResult);
    }
}
