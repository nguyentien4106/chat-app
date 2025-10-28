using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using ChatApp.Domain.Repositories;

namespace ChatApp.Application.Queries.Messages.GetGroupMessages;

public class GetGroupMessagesHandler(IRepository<Message> messageRepository)
    : IQueryHandler<GetGroupMessagesQuery, AppResponse<List<MessageDto>>>
{
    public async Task<AppResponse<List<MessageDto>>> Handle(GetGroupMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await messageRepository.GetAllAsync(
            filter: m => m.GroupId == request.GroupId,
            orderBy: query => query.OrderByDescending(m => m.CreatedAt),
            includeProperties: ["Sender"],
            cancellationToken: cancellationToken);

        var messageDtos = messages
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
            .ToList();

        return AppResponse<List<MessageDto>>.Success(messageDtos);
    }
}
