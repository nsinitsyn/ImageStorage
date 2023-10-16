using ImageStorage.Domain.Entities;

namespace ImageStorage.Application.Dependencies;

public interface IDbAccessor
{
    IQueryable<User> UsersAsNoTracking { get; }

    IQueryable<User> TrackedUsers { get; }

    void AddUser(User user);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
