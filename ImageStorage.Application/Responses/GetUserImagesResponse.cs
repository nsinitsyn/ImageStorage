using ImageStorage.Application.Common;
using ImageStorage.Domain.Entities;

namespace ImageStorage.Application.Responses;

public class GetUserImagesResponse : OperationResult<IReadOnlyCollection<Image>>
{
}
