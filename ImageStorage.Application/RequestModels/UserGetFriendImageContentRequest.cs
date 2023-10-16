namespace ImageStorage.Application.RequestModels;

public class UserGetFriendImageContentRequest
{
    public Guid FriendUserId { get; set; }

    public Guid ImageId { get; set; }
}
