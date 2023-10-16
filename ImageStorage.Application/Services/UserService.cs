using ImageStorage.Application.Dependencies;
using ImageStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Services;

public class UserService
{
    private readonly IDbAccessor _dbAccessor;
    private readonly IHashCalculator _hashCalculator;

    public UserService(IDbAccessor dbAccessor, IHashCalculator hashCalculator)
    {
        _dbAccessor = dbAccessor;
        _hashCalculator = hashCalculator;
    }

    public async Task<User?> ValidateCredentials(string? name, string? password)
    {
        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        byte[] passwordHash = _hashCalculator.GetHash(password);

        return await _dbAccessor.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name && x.PasswordHash.SequenceEqual(passwordHash));
    }
}