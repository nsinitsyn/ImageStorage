using ImageStorage.Application.Common;

namespace ImageStorage.Application.Responses;

public class GetImageContentResponse : OperationResult
{
    public string FileName { get; set; }

    public FileStream FileStream { get; set; }
}
