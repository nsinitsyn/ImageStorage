namespace ImageStorage.Application.ResponseModels;

public class UserGetImageContentResponse
{
    public string FileName { get; set; }

    public FileStream FileStream { get; set; }
}
