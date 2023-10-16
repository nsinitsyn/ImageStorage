using ImageStorage.Application.Exceptions;
using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.RequestModels;
using ImageStorage.Application.ResponseModels;
using ImageStorage.Domain.Entities;
using ImageStorage.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Handlers;

public class AddFriendHandler : BaseUseCaseHandler<UserAddFriendRequest, UserAddFriendResponse>
{
    public AddFriendHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<UserAddFriendResponse> Handle(UserAddFriendRequest request)
    {
        var result = new UserAddFriendResponse();

        Guid? userId = SessionContext.AuthorizedUserId;

        if (userId == null)
        {
            result.AddError(new($"User not authorized."));
            return result;
        }

        if (userId == request.FriendId)
        {
            result.AddError(new($"Cannot add yourself as friend."));
            return result;
        }

        User user = await DbAccessor.Users
            .Include(x => x.Friends)
            .FirstAsync(x => x.Id == userId);

        User? friend = await DbAccessor.Users
            .FirstOrDefaultAsync(x => x.Id == request.FriendId);

        // todo: это 404
        if (friend == null)
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
        catch (DomainException ex)
        {
            // todo: эти ошибки не показываются в UI
            result.AddError(new(ex.Message));
            return result;
        }

        try
        {
            var savedEntitiesCount = await DbAccessor.SaveChangesAsync();

            if (savedEntitiesCount == 0)
            {
                // todo: ошибка не для клиента и код 500 должен быть
                result.AddError(new("Cannot save user to database."));
                return result;
            }
        }
        catch (ConcurrencyConflictException ex)
        {
            // todo: отлавливать ошибку, что этого друга уже добавили.
            // result.AddError(new($"User with the same {ex.PropertyName} has already registered."));
        }

        return result;
    }
}
