using System.ComponentModel.DataAnnotations;

namespace ImageStorage.Application.RequestModels;

public class UserAddFriendRequest
{
    [Required]
    public Guid FriendId { get; set; }
}
