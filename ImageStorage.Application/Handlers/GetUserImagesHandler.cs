using ImageStorage.Application.Common;
using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.Requests;
using ImageStorage.Application.Responses;
using ImageStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Handlers;

public class GetUserImagesHandler : BaseUseCaseHandler<GetUserImagesRequest, GetUserImagesResponse>
{
    public GetUserImagesHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<GetUserImagesResponse> Handle(GetUserImagesRequest request)
    {
        var result = new GetUserImagesResponse();

        if (!SessionContext.TryGetRequiredAuthorizedUserId(out Guid userId))
        {
            result.AddError(new(OperationErrorCode.NotAuthorized));
            return result;
        }

        User user = await DbAccessor.Users
            .AsNoTracking()
            .Include(x => x.Images)
            .FirstAsync(x => x.Id == userId);

        result.Value = user.Images;

        return result;
    }
}
