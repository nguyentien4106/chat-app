using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using EzyChat.Domain.Entities;
using EzyChat.Domain.Repositories;
using Mapster;

namespace EzyChat.Infrastructure.Services;

public class MessagesServices(IRepository<Message> messageRepository) : IMessagesService
{
    private const int PageSize = 20;

    public async Task<AppResponse<PagedResult<MessageDto>>> GetGroupMessagesAsync(
        DateTime beforeDateTime, 
        CancellationToken cancellationToken = default)
    {
        // Query messages created before the given datetime, ordered by CreatedAt descending
        // Fetch PageSize + 1 to determine if there are more messages
        var messages = await messageRepository.GetAllAsync(
            filter: m => m.GroupId.HasValue && m.CreatedAt < beforeDateTime,
            orderBy: query => query.OrderByDescending(m => m.CreatedAt),
            includeProperties: ["Sender"],
            cancellationToken: cancellationToken
        );

        // Take PageSize + 1 to check if there are more messages
        var messagesList = messages.Take(PageSize + 1).ToList();
        
        // Determine if there are more messages
        var hasMore = messagesList.Count > PageSize;
        
        // Only return PageSize items
        var pagedMessages = messagesList.Take(PageSize).ToList();

        // Map to DTOs
        var messageDtos = pagedMessages.Adapt<List<MessageDto>>();

        // Create result with proper pagination info
        // For cursor-based pagination, we use TotalCount to indicate hasNextPage
        // Set TotalCount = PageSize + 1 if hasMore, otherwise PageSize
        var result = new PagedResult<MessageDto>(
            messageDtos,
            totalCount: hasMore ? PageSize + 1 : PageSize, // Trick to make HasNextPage work
            pageNumber: 1,
            pageSize: PageSize
        );

        return AppResponse<PagedResult<MessageDto>>.Success(result);
    }
}