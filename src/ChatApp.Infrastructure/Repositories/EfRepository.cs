using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ChatApp.Domain.Entities.Base;
using ChatApp.Domain.Repositories;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using ChatApp.Domain.Exceptions;

namespace ChatApp.Infrastructure.Repositories;

public class EfRepository<TEntity>(
    ChatAppDbContext context, 
    ILogger<EfRepository<TEntity>> logger
    ) : Repository<TEntity>, IRepository<TEntity> where TEntity : Entity<Guid>
{
    protected readonly DbSet<TEntity> dbSet = context.Set<TEntity>();

    public override async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting all entities of type {EntityType} with filter: {HasFilter}, orderBy: {HasOrderBy}, includes: {Includes}",
            typeof(TEntity).Name, filter != null, orderBy != null, includeProperties?.Length ?? 0);

        try
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            IEnumerable<TEntity> result;
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
            logger.LogError(ex, "Error retrieving all entities of type {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public override async Task<TEntity> GetByIdAsync(Guid id, string[]? includeProperties = null, CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<TEntity> query = dbSet;

            if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            var result = await query.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving entity of type {EntityType} with ID: {Id}", 
                typeof(TEntity).Name, id);
            throw;
        }
    }


    public override async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter, string[]? includeProperties = null, CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<TEntity> query = dbSet;

            if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            var result = await query.FirstOrDefaultAsync(filter, cancellationToken);
            
            if(result == null)
            {
                logger.LogWarning("Entity of type {EntityType} with filter {Filter} not found", 
                    typeof(TEntity).Name, filter);
                throw new NotFoundException(typeof(TEntity).Name, filter);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving entity of type {EntityType} with ID: {Id}",
                typeof(TEntity).Name, filter);
            throw;
        }
    }

    public override async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            logger.LogWarning("Attempted to insert null entity of type {EntityType}", typeof(TEntity).Name);
            throw new ArgumentNullException(nameof(entity));
        }

        try
        {
            await dbSet.AddAsync(entity, cancellationToken);
            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            if (result)
            {
                logger.LogInformation("Successfully inserted entity of type {EntityType} with ID: {Id}",
                    typeof(TEntity).Name, entity.Id);
            }
            else
            {
                logger.LogWarning("Insert operation for entity of type {EntityType} with ID: {Id} returned false",
                    typeof(TEntity).Name, entity.Id);
            }

            return entity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inserting entity of type {EntityType} with ID: {Id}",
                typeof(TEntity).Name, entity.Id);

            throw;
        }
    }

    public async override Task<bool> AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id);
        if (exists)
        {
            return await UpdateAsync(entity, cancellationToken);
        }
        else
        {
            await AddAsync(entity, cancellationToken);
            return true;
        }
    }
    
    public override async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null || !entities.Any())
        {
            logger.LogWarning("Attempted to insert null or empty entity collection of type {EntityType}", typeof(TEntity).Name);
            throw new ArgumentNullException(nameof(entities));
        }

        try
        {
            await dbSet.AddRangeAsync(entities, cancellationToken);
            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            if (result)
            {
                logger.LogInformation("Successfully inserted {Count} entities of type {EntityType}",
                    entities.Count(), typeof(TEntity).Name);
            }
            else
            {
                logger.LogWarning("Insert operation for entities of type {EntityType} returned false",
                    typeof(TEntity).Name);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inserting entities of type {EntityType}",
                typeof(TEntity).Name);

            throw;
        }
    }

    public override async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            logger.LogWarning("Attempted to update null entity of type {EntityType}", typeof(TEntity).Name);
            throw new ArgumentNullException(nameof(entity));
        }

        try
        {
            dbSet.Update(entity);
            var result = await context.SaveChangesAsync(cancellationToken) > 0;
            
            if (result)
            {
                logger.LogInformation("Successfully updated entity of type {EntityType} with ID: {Id}", 
                    typeof(TEntity).Name, entity.Id);
            }
            else
            {
                logger.LogWarning("Update operation for entity of type {EntityType} with ID: {Id} returned false", 
                    typeof(TEntity).Name, entity.Id);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating entity of type {EntityType} with ID: {Id}", 
                typeof(TEntity).Name, entity.Id);
            
            throw;
        }
    }

    public override async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            logger.LogWarning("Attempted to delete null entity of type {EntityType}", typeof(TEntity).Name);
            throw new ArgumentNullException(nameof(entity));
        }

        try
        {
            dbSet.Remove(entity);
            var result = await context.SaveChangesAsync(cancellationToken) > 0;
            
            if (result)
            {
                logger.LogInformation("Successfully deleted entity of type {EntityType} with ID: {Id}", 
                    typeof(TEntity).Name, entity.Id);
            }
            else
            {
                logger.LogWarning("Delete operation for entity of type {EntityType} with ID: {Id} returned false", 
                    typeof(TEntity).Name, entity.Id);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting entity of type {EntityType} with ID: {Id}", 
                typeof(TEntity).Name, entity.Id);
            throw;
        }
    }

    public override async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await dbSet.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
            if (entity == null)
            {
                logger.LogWarning("Entity of type {EntityType} with ID {Id} not found for deletion", 
                    typeof(TEntity).Name, id);
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            dbSet.Remove(entity);
            var result = await context.SaveChangesAsync(cancellationToken) > 0;
            
            if (result)
            {
                logger.LogInformation("Successfully deleted entity of type {EntityType} with ID: {Id}", 
                    typeof(TEntity).Name, id);
            }
            else
            {
                logger.LogWarning("Delete operation for entity of type {EntityType} with ID: {Id} returned false", 
                    typeof(TEntity).Name, id);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting entity of type {EntityType} by ID: {Id}", 
                typeof(TEntity).Name, id);
            throw;
        }
    }
    public override async Task<bool> ExistsAsync(Guid id)
    {
        return await dbSet.FindAsync(id) != null;
    }

} 