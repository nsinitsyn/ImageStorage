namespace ImageStorage.Application.Dependencies;

public interface IImagesStorageAccessor
{
    void EnsureUserDirectoryExists(Guid userId);

    FileStream CreateFileStreamForSaving(Guid userId, string fileName, Guid imageId);

    FileStream OpenFileStreamForReading(Guid userId, Guid imageId);
}
