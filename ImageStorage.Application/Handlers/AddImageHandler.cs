using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.RequestModels;
using ImageStorage.Application.ResponseModels;
using ImageStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Handlers;

public class AddImageHandler : BaseUseCaseHandler<UserAddImageRequest, UserAddImageResponse>
{
    public AddImageHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<UserAddImageResponse> Handle(UserAddImageRequest request)
    {
        var result = new UserAddImageResponse();

        // todo: выкидывает exception
        Guid userId = SessionContext.GetRequiredAuthorizedUserId();

        Image image = Image.CreateImage(request.FileName);

        // todo: 400, если не подходит расширение
        var fileStream = ImagesStorageAccessor.CreateFileStreamForSaving(userId, request.FileName, image.Id);
        await request.FileUploader.CopyToAsync(fileStream);

        User user = await DbAccessor.Users
            .Include(x => x.Images)
        .FirstAsync(x => x.Id == userId);

        user.AddImage(image);

        try
        {
            var savedEntitiesCount = await DbAccessor.SaveChangesAsync();

            // todo:
        }
        catch (Exception ex)
        {
            // todo: откат сохранения в репозиторий
        }

        result.Value = image;

        return result;
    }
}
