using ChatApp.Application.Models;
using ChatApp.Domain.Entities.Base;
using ChatApp.Domain.Repositories;
using System.Linq.Expressions;

namespace ChatApp.Application.Interfaces
{
    public interface IRepositoryPagedQuery<TEntity> : IRepository<TEntity> where TEntity : Entity<Guid>
    {
        Task<PagedResult<TEntity>> GetPagedResultAsync(
            PaginationRequest request,
            Expression<Func<TEntity, bool>>? filter = null,
            string[]? includeProperties = null,
            CancellationToken cancellationToken = default
        );
    }
}
