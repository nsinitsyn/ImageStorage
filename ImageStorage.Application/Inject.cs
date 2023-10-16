using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.Handlers;
using ImageStorage.Application.RequestModels;
using ImageStorage.Application.ResponseModels;
using Microsoft.Extensions.DependencyInjection;
using ImageStorage.Application.Services;

namespace ImageStorage.Application;

public static class Inject
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddScoped<UserService>()
            .AddScoped<IUseCaseHandler<UserRegisterRequest, UserRegisterResponse>, RegisterUserHandler>()
            .AddScoped<IUseCaseHandler<UserAddFriendRequest, UserAddFriendResponse>, AddFriendHandler>()
            .AddScoped<IUseCaseHandler<UserAddImageRequest, UserAddImageResponse>, AddImageHandler>()
            .AddScoped<IUseCaseHandler<UserGetImageContentRequest, UserGetImageContentResponse>, GetImageContentHandler>()
            .AddScoped<IUseCaseHandler<UserGetOtherUserImagesRequest, UserGetOtherUserImagesResponse>, GetOtherUserImagesHandler>()
            .AddScoped<IUseCaseHandler<GetUserImagesRequest, GetUserImagesResponse>, GetUserImagesHandler>();
    }
}
