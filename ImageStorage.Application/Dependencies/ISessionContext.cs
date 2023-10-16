namespace ImageStorage.Application.Dependencies;

public interface ISessionContext
{
    Guid? AuthorizedUserId { get; }

    Guid GetRequiredAuthorizedUserId();
}
