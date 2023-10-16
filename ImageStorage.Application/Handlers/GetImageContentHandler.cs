using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.RequestModels;
using ImageStorage.Application.ResponseModels;
using ImageStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Handlers;

public class GetImageContentHandler : BaseUseCaseHandler<UserGetImageContentRequest, UserGetImageContentResponse>
{
    public GetImageContentHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<UserGetImageContentResponse> Handle(UserGetImageContentRequest request)
    {
        var result = new UserGetImageContentResponse();

        // todo: выкидывает exception
        Guid userId = SessionContext.GetRequiredAuthorizedUserId();

        Image? image = await DbAccessor.Images
            .AsNoTracking()
            .Include(x => x.User)
                .ThenInclude(x => x.Friends)
            .FirstOrDefaultAsync(x => x.Id == request.ImageId);

        if (image == null || !image.UserHasAccess(userId))
        {
            // todo:
            //result.AddError();
            return result;
        }

        FileStream fileStream = ImagesStorageAccessor.OpenFileStreamForReading(image.UserId, request.ImageId);

        result.FileName = image.FileName;
        result.FileStream = fileStream;

        return result;
    }
}
