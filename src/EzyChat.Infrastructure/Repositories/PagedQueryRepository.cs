using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EzyChat.Domain.Entities.Base;
using EzyChat.Domain.Repositories;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using EzyChat.Domain.Exceptions;

namespace EzyChat.Infrastructure.Repositories;

public class PagedQueryRepository<TEntity>(
    EzyChatDbContext context, 
    ILogger<EfRepository<TEntity>> logger
    ) : EfRepository<TEntity>(context, logger), IRepositoryPagedQuery<TEntity> where TEntity : Entity<Guid>
{
    public async Task<PagedResult<TEntity>> GetPagedResultAsync(
        PaginationRequest request,
        Expression<Func<TEntity, bool>>? filter = null,
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<TEntity> query = dbSet;

            // Apply filter
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply includes
            if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            // Apply ordering based on PaginationRequest.SortBy and SortOrder
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                var prop = typeof(TEntity).GetProperty(request.SortBy!);
                if(prop == null)
                {
                    logger.LogWarning("SortBy '{SortBy}' is not a valid property of {EntityType}. Falling back to Created desc.", request.SortBy, typeof(TEntity).Name);
                    throw new InvalidOperationException($"SortBy '{request.SortBy}' is not a valid property of {typeof(TEntity).Name}");
                }
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var property = Expression.Property(parameter, prop);
                var lambda = Expression.Lambda(property, parameter);

                var methodName = string.Equals(request.SortOrder, "asc", StringComparison.OrdinalIgnoreCase)
                    ? "OrderBy"
                    : "OrderByDescending";

                var resultExpression = Expression.Call(
                    typeof(Queryable),
                    methodName,
                    [typeof(TEntity), prop.PropertyType],
                    query.Expression,
                    Expression.Quote(lambda)
                );

                query = query.Provider.CreateQuery<TEntity>(resultExpression);
  
            }
            else
            {
                // Default ordering by Created if no SortBy is provided
                query = string.Equals(request.SortOrder, "asc", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderBy(e => e.CreatedAt)
                    : query.OrderByDescending(e => e.CreatedAt);
            }

            // Apply pagination
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResult<TEntity>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving paged entities of type {EntityType} - Page: {PageNumber}, Size: {PageSize}",
                typeof(TEntity).Name, request.PageNumber, request.PageSize);
            throw;
        }
    }

    public async Task<PagedResult<TEntity>> GetPagedResultAsync(DateTime beforeDate, int pageSize, string[]? includeProperties = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = dbSet;

        // Apply filter
        query = query.Where(e => e.CreatedAt < beforeDate);

        // Apply includes
        if (includeProperties != null)
        {
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        query = query.OrderByDescending(e => e.CreatedAt);
        
        // Fetch PageSize + 1 to determine if more items exist
        var items = await query
            .Take(pageSize + 1)
            .ToListAsync(cancellationToken);

        var hasMoreItems = items.Count > pageSize;
        if (hasMoreItems)
        {
            items.RemoveAt(pageSize);
        }

        return new PagedResult<TEntity>
        {
            Items = items,
            TotalCount = hasMoreItems ? pageSize + 1 : pageSize, // Trick to make HasNextPage work
            PageSize = pageSize,
        };
    }

    public async Task<PagedResult<TEntity>> GetPagedResultAsync(DateTime beforeDateTime, int pageSize = 20, Expression<Func<TEntity, bool>>? filter = null, string[]? includeProperties = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }
        
        query = query.Where(e => e.CreatedAt < beforeDateTime);

        // Apply includes
        if (includeProperties != null)
        {
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        query = query.OrderByDescending(e => e.CreatedAt);
        
        // Fetch PageSize + 1 to determine if more items exist
        var items = await query
            .Take(pageSize + 1)
            .ToListAsync(cancellationToken);

        var hasMoreItems = items.Count > pageSize;
        if (hasMoreItems)
        {
            items.RemoveAt(pageSize);
        }

        return new PagedResult<TEntity>
        {
            Items = items,
            TotalCount = hasMoreItems ? pageSize + 1 : pageSize, // Trick to make HasNextPage work
            PageSize = pageSize,
        };
    }
} 