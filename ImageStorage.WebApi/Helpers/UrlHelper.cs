namespace ImageStorage.WebApi.Helpers;

public static class UrlHelper
{
    public static string GetImageLink(Guid imageId) => $"Image/GetImageContent/{imageId}";
}
