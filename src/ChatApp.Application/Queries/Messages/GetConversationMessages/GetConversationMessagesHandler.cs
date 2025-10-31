using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using ChatApp.Domain.Repositories;

namespace ChatApp.Application.Queries.Messages.GetConversationMessages;

public class GetConversationMessagesHandler(
    IRepository<Message> messageRepository,
    IRepository<Conversation> conversationRepository)
    : IQueryHandler<GetConversationMessagesQuery, AppResponse<List<MessageDto>>>
{
    public async Task<AppResponse<List<MessageDto>>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        // Find the conversation between these two users
        var conversations = await conversationRepository.GetAllAsync(
            filter: c => (c.User1Id == request.User1Id && c.User2Id == request.User2Id) ||
                        (c.User1Id == request.User2Id && c.User2Id == request.User1Id),
            cancellationToken: cancellationToken);

        var conversation = conversations.FirstOrDefault();
        
        // If no conversation exists, return empty list
        if (conversation == null)
        {
            return AppResponse<List<MessageDto>>.Success(new List<MessageDto>());
        }

        // Get messages for this conversation
        var messages = await messageRepository.GetAllAsync(
            filter: m => m.ConversationId == conversation.Id,
            orderBy: query => query.OrderByDescending(m => m.CreatedAt),
            includeProperties: ["Sender"],
            cancellationToken: cancellationToken);

        var pagedMessages = messages
            .Skip(request.Skip)
            .Take(request.Take)
            .ToList();

        return AppResponse<List<MessageDto>>.Success(pagedMessages.Adapt<List<MessageDto>>());
    }
}