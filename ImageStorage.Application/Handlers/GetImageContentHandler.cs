using ImageStorage.Application.Common;
using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.Requests;
using ImageStorage.Application.Responses;
using ImageStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Handlers;

public class GetImageContentHandler : BaseUseCaseHandler<GetImageContentRequest, GetImageContentResponse>
{
    public GetImageContentHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<GetImageContentResponse> Handle(GetImageContentRequest request)
    {
        var result = new GetImageContentResponse();

        if (!SessionContext.TryGetRequiredAuthorizedUserId(out Guid userId))
        {
            result.AddError(new(OperationErrorCode.NotAuthorized));
            return result;
        }

        Image? image = await DbAccessor.Images
            .AsNoTracking()
            .Include(x => x.User)
                .ThenInclude(x => x.Friends)
            .FirstOrDefaultAsync(x => x.Id == request.ImageId);

        if (image == null || !image.UserHasAccess(userId))
        {
            result.AddError(new(OperationErrorCode.AccessDenied));
            return result;
        }

        FileStream fileStream = ImagesStorageAccessor.OpenFileStreamForReading(image.UserId, request.ImageId);

        result.FileName = image.FileName;
        result.FileStream = fileStream;

        return result;
    }
}
