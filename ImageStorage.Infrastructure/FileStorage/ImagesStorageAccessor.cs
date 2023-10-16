using ImageStorage.Application.Dependencies;
using ImageStorage.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace ImageStorage.Infrastructure.FileStorage;

public class ImagesStorageAccessor : IImagesStorageAccessor, IDisposable
{
    private readonly string _fileStoragePath;
    private readonly List<string> _allowedExtensions = new List<string> { ".jpg", ".jpeg", ".bmp", ".png", ".gif" };
    private readonly List<IDisposable> _forDispose = new List<IDisposable>();

    public ImagesStorageAccessor(IOptions<AppConfiguration> appConfiguration)
    {
        _fileStoragePath = appConfiguration.Value.StorageLocation;

        if (string.IsNullOrEmpty(_fileStoragePath))
        {
            throw new InvalidOperationException("Cannot find StorageLocation settings parameter.");
        }
    }

    public void EnsureUserDirectoryExists(Guid userId)
    {
        Directory.CreateDirectory(GetUserDirectoryAbsolutePath(userId));
    }

    public FileStream CreateFileStreamForSaving(Guid userId, string fileName, Guid imageId)
    {
        string? fileExtension = Path.GetExtension(fileName);

        if(string.IsNullOrEmpty(fileExtension) || !_allowedExtensions.Contains(fileExtension))
        {
            throw new InvalidOperationException("File extension not allowed.");
        }

        var fileStream = new FileStream(Path.Combine(_fileStoragePath, userId.ToString(), imageId.ToString()), FileMode.Create);

        _forDispose.Add(fileStream);

        return fileStream;
    }

    public FileStream OpenFileStreamForReading(Guid userId, Guid imageId)
    {
        var fileStream = new FileStream(GetImageAbsolutePath(userId, imageId), FileMode.Open);
        
        _forDispose.Add(fileStream);

        return fileStream;
    }

    private string GetUserDirectoryAbsolutePath(Guid userId) => Path.Combine(_fileStoragePath, userId.ToString());

    private string GetImageAbsolutePath(Guid userId, Guid imageId) => Path.Combine(GetUserDirectoryAbsolutePath(userId), imageId.ToString());

    public void Dispose()
    {
        foreach(var disposable in _forDispose)
        {
            disposable.Dispose();
        }
    }
}
