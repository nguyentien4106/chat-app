using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using EzyChat.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EzyChat.Infrastructure.Repositories;

public class MessageRepository(EzyChatDbContext dbContext) : IMessageRepository
{
    private readonly DbSet<Domain.Entities.Message> _dbSet = dbContext.Set<Domain.Entities.Message>();

    public async Task<PagedResult<MessageDto>> GetPagedResultAsync(
        DateTime beforeDateTime,
        Guid groupId,
        int pageSize = 20,
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<Domain.Entities.Message> query = _dbSet;

        // Apply filter for messages before the specified DateTime
        query = query.Where(m => m.CreatedAt < beforeDateTime && m.GroupId == groupId);

        // Include related properties if specified
        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        // Order by CreatedAt descending to get latest messages first
        query = query.OrderByDescending(m => m.CreatedAt);

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var itemDtos = items.Adapt<List<MessageDto>>();

        return new PagedResult<MessageDto>
        {
            Items = itemDtos,
            TotalCount = totalCount,
            PageSize = pageSize,
            PageNumber = 1
        };
    }
}