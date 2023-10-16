using ImageStorage.Application.Common;
using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.Requests;
using ImageStorage.Application.Responses;
using ImageStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Handlers;

public class GetOtherUserImagesHandler : BaseUseCaseHandler<GetOtherUserImagesRequest, GetOtherUserImagesResponse>
{
    public GetOtherUserImagesHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<GetOtherUserImagesResponse> Handle(GetOtherUserImagesRequest request)
    {
        var result = new GetOtherUserImagesResponse();

        if (!SessionContext.TryGetRequiredAuthorizedUserId(out Guid userId))
        {
            result.AddError(new(OperationErrorCode.NotAuthorized));
            return result;
        }

        User? otherUser = await DbAccessor.Users
            .AsNoTracking()
            .Include(x => x.Friends)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == request.UserId);

        if(otherUser == null)
        {
            result.AddError(new(OperationErrorCode.NotFound));
            return result;
        }

        if (!otherUser.IsFriend(userId))
        {
            result.AddError(new(OperationErrorCode.AccessDenied));
            return result;
        }

        result.Value = otherUser.Images;

        return result;
    }
}
