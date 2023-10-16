using ImageStorage.Application.Common;

namespace ImageStorage.Application.ResponseModels;

public class UserGetImageContentResponse : OperationResult
{
    public string FileName { get; set; }

    public FileStream FileStream { get; set; }
}
