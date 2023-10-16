using ImageStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Dependencies;

public interface IDbAccessor
{
    DbSet<User> Users { get; }

    DbSet<Image> Images { get; }

    void AddUser(User user);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
