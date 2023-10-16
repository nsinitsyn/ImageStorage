using ImageStorage.Application.Dependencies;

namespace ImageStorage.Application.Requests;

public class AddImageRequest
{
    public string FileName { get; set; }

    public IFileUploader FileUploader { get; set; }

    public override string ToString() => $"{nameof(AddImageRequest)}: FileName:{FileName}";
}
