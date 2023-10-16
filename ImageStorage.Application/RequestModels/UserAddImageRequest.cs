using ImageStorage.Application.Dependencies;

namespace ImageStorage.Application.RequestModels;

public class UserAddImageRequest
{
    public string FileName { get; set; }

    public IFileUploader FileUploader { get; set; }
}
