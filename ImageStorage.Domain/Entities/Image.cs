using ImageStorage.Domain.Exceptions;

namespace ImageStorage.Domain.Entities;

public class Image
{
    private Image(string fileName)
    {
        Id = Guid.NewGuid();
        FileName = fileName;
        UploadedDateTimeUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public User User { get; private set; }

    public string FileName { get; private set; }

    public DateTime UploadedDateTimeUtc { get; private set; }

    public static Image CreateImage(string fileName)
    {
        var image = new Image(fileName);
        return image;
    }

    /// <summary>
    /// Проверяет, имеет ли право пользователь userId просматривать данное изображение.
    /// </summary>
    public bool UserHasAccess(Guid userId)
    {
        // Пользователь может просматривать свое изображение.
        if (userId == UserId)
        {
            return true;
        }

        if(User.Friends == null)
        {
            throw new DomainException("Friends list cannot be null.");
        }

        // Пользователь может просматривать изображения тех пользователей, которые добавили его в друзья.
        return User.Friends.Any(x => x.Id == userId);
    }
}
