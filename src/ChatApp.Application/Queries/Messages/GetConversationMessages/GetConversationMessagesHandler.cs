using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using ChatApp.Domain.Repositories;

namespace ChatApp.Application.Queries.Messages.GetConversationMessages;

public class GetConversationMessagesHandler(IRepository<Message> messageRepository)
    : IQueryHandler<GetConversationMessagesQuery, AppResponse<List<MessageDto>>>
{
    public async Task<AppResponse<List<MessageDto>>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await messageRepository.GetAllAsync(
            filter: m => m.GroupId == null &&
                        ((m.SenderId == request.User1Id && m.ReceiverId == request.User2Id) ||
                         (m.SenderId == request.User2Id && m.ReceiverId == request.User1Id)),
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