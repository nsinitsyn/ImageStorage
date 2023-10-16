using ImageStorage.Application.Dependencies;

namespace ImageStorage.Infrastructure.Session;

public class SessionContext : ISessionContext, ISessionContextWriter
{
    public Guid? AuthorizedUserId { get; set; }

    public bool TryGetRequiredAuthorizedUserId(out Guid userId)
    {
        if(AuthorizedUserId == null)
        {
            userId = Guid.Empty;
            return false;
        }

        userId =(Guid)AuthorizedUserId;
        return true;
    }
}
