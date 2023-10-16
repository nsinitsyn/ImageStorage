namespace ImageStorage.Domain.Entities;

public class Image
{
    private Image()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; private set; }
}
