using ImageStorage.Application.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace ImageStorage.Application.Handlers.Base;

public abstract class BaseUseCaseHandler<TRequest, TResponse> : IUseCaseHandler<TRequest, TResponse>
{
    protected readonly IDbAccessor DbAccessor;
    protected readonly IImagesStorageAccessor ImagesStorageAccessor;
    protected readonly IHashCalculator HashCalculator;
    protected readonly ISessionContext SessionContext;

    public BaseUseCaseHandler(IServiceProvider serviceProvider)
    {
        DbAccessor = serviceProvider.GetRequiredService<IDbAccessor>();
        ImagesStorageAccessor = serviceProvider.GetRequiredService<IImagesStorageAccessor>();
        HashCalculator = serviceProvider.GetRequiredService<IHashCalculator>();
        SessionContext = serviceProvider.GetRequiredService<ISessionContext>();
    }

    public abstract Task<TResponse> Handle(TRequest request);
}
