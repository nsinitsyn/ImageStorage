using ImageStorage.Application.Common;
using ImageStorage.Domain.Entities;

namespace ImageStorage.Application.Responses;

public class GetOtherUserImagesResponse : OperationResult<IReadOnlyCollection<Image>>
{
}
