using ImageStorage.Application.Dependencies;
using ImageStorage.Application.Exceptions;
using ImageStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Reflection;

namespace ImageStorage.Infrastructure.DbAccess;

public class AppDbContext : DbContext, IDbAccessor
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public IQueryable<User> UsersAsNoTracking => Users.AsNoTracking();

    public IQueryable<User> TrackedUsers => Users;

    public void AddUser(User user) => Users.Add(user);

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) 
            when (ex.InnerException != null && ex.InnerException is PostgresException pgsEx && pgsEx.SqlState == PostgreSqlErrorCodes.UniqueViolation) 
        {
            string propertyName = pgsEx.ConstraintName!.Split("_").Last().ToLower();

            throw new ConcurrencyConflictException(propertyName);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
