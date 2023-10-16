using ImageStorage.Application.Common;
using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.Requests;
using ImageStorage.Application.Responses;
using ImageStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Handlers;

public class AddImageHandler : BaseUseCaseHandler<AddImageRequest, AddImageResponse>
{
    public AddImageHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<AddImageResponse> Handle(AddImageRequest request)
    {
        var result = new AddImageResponse();

        if(!SessionContext.TryGetRequiredAuthorizedUserId(out Guid userId))
        {
            result.AddError(new(OperationErrorCode.NotAuthorized));
            return result;
        }

        Image image = Image.CreateImage(request.FileName);

        if(!ImagesStorageAccessor.IsFileExtensionAllowed(request.FileName))
        {
            result.AddError(new("File extension is not allowed."));
            return result;
        }

        var fileStream = ImagesStorageAccessor.CreateFileStreamForSaving(userId, request.FileName, image.Id);
        await request.FileUploader.CopyToAsync(fileStream);

        User user = await DbAccessor.Users
            .Include(x => x.Images)
        .FirstAsync(x => x.Id == userId);

        user.AddImage(image);

        int savedEntitiesCount;

        try
        {
            savedEntitiesCount = await DbAccessor.SaveChangesAsync();
        }
        catch (Exception)
        {
            // откатить сохранение изображения в файловой системе
            ImagesStorageAccessor.DeleteFile(userId, image.Id);

            throw;
        }

        if (savedEntitiesCount == 0)
        {
            ImagesStorageAccessor.DeleteFile(userId, image.Id);

            result.AddError(new(OperationErrorCode.ServerError, "Cannot save image to database."));
            return result;
        }

        result.Value = image;

        return result;
    }
}
