using ImageStorage.Application.Dependencies;

namespace ImageStorage.WebApi.Helpers;

/// <summary>
/// Для сокрытия IFormFile от слоя бизнес-логики.
/// </summary>
public class FileUploader : IFileUploader
{
    private readonly IFormFile _file;

    public FileUploader(IFormFile file) =>
        _file = file;

    public Task CopyToAsync(FileStream fileStream) =>
        _file.CopyToAsync(fileStream);
}
