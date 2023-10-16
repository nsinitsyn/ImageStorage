using ImageStorage.Application.Common;
using ImageStorage.Domain.Entities;

namespace ImageStorage.Application.ResponseModels;

public class GetUserImagesResponse : OperationResult<IReadOnlyCollection<Image>>
{
}
