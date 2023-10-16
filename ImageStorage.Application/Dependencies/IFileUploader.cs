namespace ImageStorage.Application.Dependencies;

public interface IFileUploader
{
    Task CopyToAsync(FileStream fileStream);
}
