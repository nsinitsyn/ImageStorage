using ImageStorage.Domain.Entities;

namespace ImageStorage.Infrastructure.DbAccess.Links;

internal class UserFriendLink
{
    public Guid UserId { get; set; }

    public Guid FriendId { get; set; }

    public User? User { get; set; }

    public User? Friend { get; set; }
}
