using ImageStorage.Application.Common;
using ImageStorage.Application.Dependencies;
using ImageStorage.Application.Exceptions;
using ImageStorage.Application.RequestModels;
using ImageStorage.Domain.Entities;
using ImageStorage.Domain.Exceptions;

namespace ImageStorage.Application.Services;

public class UserApplicationService
{
    private readonly IDbAccessor _dbAccessor;
    private readonly IHashCalculator _hashCalculator;

    public UserApplicationService(IDbAccessor dbAccessor, IHashCalculator hashCalculator)
    {
        _dbAccessor = dbAccessor;
        _hashCalculator = hashCalculator;
    }

    public async Task<OperationResult<User>> RegisterUser(UserRegisterRequest request)
    {
        var result = new OperationResult<User>();

        if (_dbAccessor.UsersAsNoTracking.Any(x => x.Name == request.Name))
        {
            result.AddError(new("User with the same name has already registered."));
        }

        if (_dbAccessor.UsersAsNoTracking.Any(x => x.Email == request.Email))
        {
            result.AddError(new("User with the same email has already registered."));
        }

        if(!result.IsSucceeded)
        {
            return result;
        }

        byte[] passwordHash = _hashCalculator.GetHash(request.Password);

        var user = User.CreateUser(request.Name, request.Email, passwordHash);
        _dbAccessor.AddUser(user);

        try
        {
            await _dbAccessor.SaveChangesAsync();
        }
        catch(ConcurrencyConflictException ex)
        {
            result.AddError(new($"User with the same {ex.PropertyName} has already registered."));
            return result;
        }

        result.Value = user;

        return result;
    }

    public async Task<OperationResult> AddFriend(UserAddFriendRequest request)
    {
        var result = new OperationResult();

        User? user = _dbAccessor.TrackedUsers.FirstOrDefault(x => x.Id == request.UserId);
        User? friend = _dbAccessor.UsersAsNoTracking.FirstOrDefault(x => x.Id == request.FriendId);

        // todo: get user with Include Friends

        // todo: это 404
        if (user == null)
        {
            result.AddError(new($"Cannot find user with id={request.UserId}."));
        }

        if(friend == null)
        {
            result.AddError(new($"Cannot find friend user with id={request.FriendId}."));
        }

        if (!result.IsSucceeded)
        {
            return result;
        }

        try
        {
            user!.AddFriend(friend!);
        }
        catch(DomainException ex)
        {
            // todo: эти ошибки не показываются в UI
            result.AddError(new(ex.Message));
            return result;
        }

        try
        {
            await _dbAccessor.SaveChangesAsync();
        }
        catch (ConcurrencyConflictException ex)
        {
            // todo: отлавливать ошибку, что этого друга уже добавили.
            // result.AddError(new($"User with the same {ex.PropertyName} has already registered."));
        }

        return result;
    }
}