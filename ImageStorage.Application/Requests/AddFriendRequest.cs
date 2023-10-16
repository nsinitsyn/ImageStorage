using System.ComponentModel.DataAnnotations;

namespace ImageStorage.Application.Requests;

public class AddFriendRequest
{
    [Required]
    public Guid FriendId { get; set; }

    public override string ToString() => $"{nameof(AddFriendRequest)}: FriendId:{FriendId}";
}
