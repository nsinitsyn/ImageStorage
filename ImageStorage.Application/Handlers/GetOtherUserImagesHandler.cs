using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.RequestModels;
using ImageStorage.Application.ResponseModels;
using ImageStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Handlers;

public class GetOtherUserImagesHandler : BaseUseCaseHandler<UserGetOtherUserImagesRequest, UserGetOtherUserImagesResponse>
{
    public GetOtherUserImagesHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<UserGetOtherUserImagesResponse> Handle(UserGetOtherUserImagesRequest request)
    {
        var result = new UserGetOtherUserImagesResponse();

        Guid userId = SessionContext.GetRequiredAuthorizedUserId();

        User otherUser = await DbAccessor.Users
            .AsNoTracking()
            .Include(x => x.Friends)
            .Include(x => x.Images)
            .FirstAsync(x => x.Id == request.UserId);

        if (!otherUser.IsFriend(userId))
        {
            // todo:
            return result;
        }

        result.Value = otherUser.Images;

        return result;
    }
}
