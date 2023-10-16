namespace ImageStorage.Application.Dependencies;

public interface IImagesStorageAccessor
{
    void EnsureUserDirectoryExists(Guid userId);

    bool IsFileExtensionAllowed(string fileName);

    FileStream CreateFileStreamForSaving(Guid userId, string fileName, Guid imageId);

    FileStream OpenFileStreamForReading(Guid userId, Guid imageId);

    void DeleteFile(Guid userId, Guid imageId);
}
