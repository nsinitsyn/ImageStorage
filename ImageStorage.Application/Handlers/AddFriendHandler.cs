using ImageStorage.Application.Common;
using ImageStorage.Application.Exceptions;
using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.Requests;
using ImageStorage.Application.Responses;
using ImageStorage.Domain.Entities;
using ImageStorage.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Handlers;

public class AddFriendHandler : BaseUseCaseHandler<AddFriendRequest, AddFriendResponse>
{
    public AddFriendHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<AddFriendResponse> Handle(AddFriendRequest request)
    {
        var result = new AddFriendResponse();

        if (!SessionContext.TryGetRequiredAuthorizedUserId(out Guid userId))
        {
            result.AddError(new(OperationErrorCode.NotAuthorized));
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

        if (friend == null)
        {
            result.AddError(new(OperationErrorCode.NotFound));
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
            result.AddError(new(ex.Message));
            return result;
        }

        try
        {
            var savedEntitiesCount = await DbAccessor.SaveChangesAsync();

            if (savedEntitiesCount == 0)
            {
                result.AddError(new(OperationErrorCode.ServerError, "Cannot save user to database."));
                return result;
            }
        }
        catch (ConcurrencyConflictException)
        {
            result.AddError(new($"Friends list has already contains this user."));
        }

        return result;
    }
}
