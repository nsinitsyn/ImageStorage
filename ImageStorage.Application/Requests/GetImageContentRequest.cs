namespace ImageStorage.Application.Requests;

public class GetImageContentRequest
{
    public Guid ImageId { get; set; }

    public override string ToString() => $"{nameof(GetImageContentRequest)}: ImageId:{ImageId}";
}
