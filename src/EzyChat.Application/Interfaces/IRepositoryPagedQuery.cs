using EzyChat.Application.Models;
using EzyChat.Domain.Entities.Base;
using EzyChat.Domain.Repositories;
using System.Linq.Expressions;

namespace EzyChat.Application.Interfaces
{
    public interface IRepositoryPagedQuery<TEntity> : IRepository<TEntity> where TEntity : Entity<Guid>
    {
        Task<PagedResult<TEntity>> GetPagedResultAsync(
            PaginationRequest request,
            Expression<Func<TEntity, bool>>? filter = null,
            string[]? includeProperties = null,
            CancellationToken cancellationToken = default
        );

        Task<PagedResult<TEntity>> GetPagedResultAsync(
            DateTime beforeDateTime,
            int pageSize = 20,
            Expression<Func<TEntity, bool>>? filter = null,
            string[]? includeProperties = null,
            CancellationToken cancellationToken = default
        );
    }
}
