namespace ImageStorage.Infrastructure.Session;

public interface ISessionContextWriter
{
    Guid? AuthorizedUserId { get; set; }
}
