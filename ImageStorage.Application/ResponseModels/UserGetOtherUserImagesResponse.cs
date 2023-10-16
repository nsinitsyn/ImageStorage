using ImageStorage.Application.Common;
using ImageStorage.Domain.Entities;

namespace ImageStorage.Application.ResponseModels;

public class UserGetOtherUserImagesResponse : OperationResult<IReadOnlyCollection<Image>>
{
}
