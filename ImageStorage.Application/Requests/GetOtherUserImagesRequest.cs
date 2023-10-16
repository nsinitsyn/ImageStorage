namespace ImageStorage.Application.Requests;

public class GetOtherUserImagesRequest
{
    public Guid UserId { get; set; }

    public override string ToString() => $"{nameof(GetOtherUserImagesRequest)}: UserId:{UserId}";
}
