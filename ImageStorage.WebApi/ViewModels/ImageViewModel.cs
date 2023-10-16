using ImageStorage.Domain.Entities;
using ImageStorage.WebApi.Helpers;

namespace ImageStorage.WebApi.ViewModels;

public class ImageViewModel
{
    public ImageViewModel(Guid id, string fileName)
    {
        Id = id;
        FileName = fileName;
        Link = UrlHelper.GetImageLink(id);
    }

    public Guid Id { get; private set; }

    public string FileName { get; private set; }

    public string Link { get; private set; }

    public static ImageViewModel FromImage(Image image)
    {
        return new ImageViewModel(image.Id, image.FileName);
    }
}
