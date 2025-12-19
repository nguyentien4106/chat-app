using System.Linq.Expressions;
using EzyChat.Domain.Entities.Base;

namespace EzyChat.Domain.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : IEntity<Guid>
{
    public abstract Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default
    );

    public abstract Task<TEntity?> GetByIdAsync(
        Guid id,
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default
    );

    public abstract Task<TEntity?> GetSingleAsync(
        Expression<Func<TEntity, bool>> filter,
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default
    );

    public abstract Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    public abstract Task<bool> AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    public abstract Task<bool> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    public abstract Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    public abstract Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    public abstract Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    public abstract Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public abstract Task<bool> ExistsAsync(Guid id);

    public abstract IQueryable<TEntity> GetQuery();
}
