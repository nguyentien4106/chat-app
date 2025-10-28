using System.Linq.Expressions;

namespace ChatApp.Domain.Repositories;

public interface IUserRepository 
{
    Task<IEnumerable<ApplicationUser>> GetAllAsync(
        Expression<Func<ApplicationUser, bool>>? filter = null,
        Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>? orderBy = null,
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default
    );

    Task<ApplicationUser?> GetByIdAsync(
        Guid id,
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default
    );

    Task<ApplicationUser?> GetUserByUserNameAsync(
        string userName,
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default
    );
}