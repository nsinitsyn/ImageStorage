using ImageStorage.Application.Dependencies;

namespace ImageStorage.Infrastructure.Session;

public class SessionContext : ISessionContext, ISessionContextWriter
{
    public Guid? AuthorizedUserId { get; set; }

    public Guid GetRequiredAuthorizedUserId()
    {
        if(AuthorizedUserId == null)
        {
            throw new InvalidOperationException("User not authorized.");
        }

        return (Guid)AuthorizedUserId;
    }
}
