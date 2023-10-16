namespace ImageStorage.Application.Handlers.Base;

public interface IUseCaseHandler<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request);
}
