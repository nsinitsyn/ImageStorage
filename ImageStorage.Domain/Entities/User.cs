using ImageStorage.Domain.Exceptions;

namespace ImageStorage.Domain.Entities;

public class User
{
    private User(string name, string email, byte[] passwordHash)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
    }

    private readonly ICollection<Image> _images = new List<Image>();

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public byte[] PasswordHash { get; private set; }

    /// <summary>
    /// Друзья пользователя.
    /// </summary>
    public ICollection<User>? Friends { get; private set; }

    /// <summary>
    /// Пользователи, у которых в друзьях текущий пользователь.
    /// </summary>
    public ICollection<User>? UsersAddedAsFriend { get; private set; }

    public IReadOnlyCollection<Image>? Images => _images.ToList();

    public static User CreateUser(string name, string email, byte[] passwordHash)
    {
        var user = new User(name, email, passwordHash);

        return user;
    }

    public void AddFriend(User user)
    {
        if(Friends == null)
        {
            throw new DomainException("Friends list cannot be null.");
        }

        if (Friends.Any(x => x.Id == user.Id))
        {
            throw new DomainException("Friends list has already contains this user.");
        }

        Friends.Add(user);
    }

    public void AddImage(Image image)
    {
        _images.Add(image);
    }

    /// <summary>
    /// Проверяет, является ли пользователь userId другом текущего пользователя.
    /// </summary>
    public bool IsFriend(Guid userId)
    {
        if (Friends == null)
        {
            throw new DomainException("Friends list cannot be null.");
        }

        return Friends.Any(x => x.Id == userId);
    }
}