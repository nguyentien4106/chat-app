using System.Linq.Expressions;
using EzyChat.Application.Interfaces;
using EzyChat.Domain.Entities;
using EzyChat.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EzyChat.Domain.Repositories;

public class UserRepository(
    ILogger<UserRepository> logger,
    IEzyChatDbContext context

) : IUserRepository
{
    public async Task<ApplicationUser?> GetByIdAsync(Guid id, string[]? includeProperties = null, CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<ApplicationUser> query = context.Users;

            if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            var result = await query.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(typeof(ApplicationUser).Name, id);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving entity of type {EntityType} with ID: {Id}", 
                typeof(ApplicationUser).Name, id);
            throw;
        }
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync(
        Expression<Func<ApplicationUser, bool>>? filter = null,
        Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>? orderBy = null,
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting all entities of type {EntityType} with filter: {HasFilter}, orderBy: {HasOrderBy}, includes: {Includes}",
            typeof(ApplicationUser).Name, filter != null, orderBy != null, includeProperties?.Length ?? 0);

        try
        {
            IQueryable<ApplicationUser> query = context.Users;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            IEnumerable<ApplicationUser> result;
            if (orderBy != null)
            {
                result = await orderBy(query).ToListAsync(cancellationToken);
            }
            else
            {
                result = await query.ToListAsync(cancellationToken);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all entities of type {EntityType}", typeof(ApplicationUser).Name);
            throw;
        }
    }

    public async Task<ApplicationUser?> GetUserByUserNameAsync(string userName, string[]? includeProperties = null, CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<ApplicationUser> query = context.Users;

            if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            return await query.SingleOrDefaultAsync(e => e.UserName.Equals(userName), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving entity of type {EntityType} with UserName: {UserName}", 
                typeof(ApplicationUser).Name, userName);
            throw;
        }
    }
}