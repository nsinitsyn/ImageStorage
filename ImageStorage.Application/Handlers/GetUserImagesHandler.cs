using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.RequestModels;
using ImageStorage.Application.ResponseModels;
using ImageStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Handlers;

public class GetUserImagesHandler : BaseUseCaseHandler<GetUserImagesRequest, GetUserImagesResponse>
{
    public GetUserImagesHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<GetUserImagesResponse> Handle(GetUserImagesRequest request)
    {
        var result = new GetUserImagesResponse();

        Guid userId = SessionContext.GetRequiredAuthorizedUserId();

        User user = await DbAccessor.Users
            .AsNoTracking()
            .Include(x => x.Images)
            .FirstAsync(x => x.Id == userId);

        result.Value = user.Images;

        return result;
    }
}
