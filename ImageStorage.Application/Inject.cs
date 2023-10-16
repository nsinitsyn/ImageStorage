using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.Handlers;
using Microsoft.Extensions.DependencyInjection;
using ImageStorage.Application.Services;
using ImageStorage.Application.Requests;
using ImageStorage.Application.Responses;

namespace ImageStorage.Application;

public static class Inject
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddScoped<UserService>()
            .AddScoped<IUseCaseHandler<RegisterUserRequest, RegisterUserResponse>, RegisterUserHandler>()
            .AddScoped<IUseCaseHandler<AddFriendRequest, AddFriendResponse>, AddFriendHandler>()
            .AddScoped<IUseCaseHandler<AddImageRequest, AddImageResponse>, AddImageHandler>()
            .AddScoped<IUseCaseHandler<GetImageContentRequest, GetImageContentResponse>, GetImageContentHandler>()
            .AddScoped<IUseCaseHandler<GetOtherUserImagesRequest, GetOtherUserImagesResponse>, GetOtherUserImagesHandler>()
            .AddScoped<IUseCaseHandler<GetUserImagesRequest, GetUserImagesResponse>, GetUserImagesHandler>();
    }
}
