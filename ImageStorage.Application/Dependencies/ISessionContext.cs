namespace ImageStorage.Application.Dependencies;

public interface ISessionContext
{
    Guid? AuthorizedUserId { get; }

    bool TryGetRequiredAuthorizedUserId(out Guid userId);
}
