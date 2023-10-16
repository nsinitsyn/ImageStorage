using ImageStorage.Application.Dependencies;
using ImageStorage.Application.Services;
using ImageStorage.Infrastructure.Configuration;
using ImageStorage.Infrastructure.DbAccess;
using ImageStorage.Infrastructure.FileStorage;
using ImageStorage.Infrastructure.Session;
using ImageStorage.WebApi.Handlers;
using ImageStorage.WebApi.Helpers;
using ImageStorage.WebApi.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ImageStorage.WebApi.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddControllers();

        services.AddSwaggerGen();
        services.ConfigureOptions<SwaggerConfigurationOptions>();

        services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

        var connectionString = builder.Configuration.GetConnectionString("Default");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.Configure<AppConfiguration>(builder.Configuration.GetSection(nameof(AppConfiguration)));

        AddApplicationServices(services);
    }

    private static void AddApplicationServices(IServiceCollection services)
    {
        services.AddSingleton<SessionContext>();
        services.AddSingleton<ISessionContext, SessionContext>(sp => sp.GetRequiredService<SessionContext>());
        services.AddSingleton<ISessionContextWriter, SessionContext>(sp => sp.GetRequiredService<SessionContext>());
        services.AddTransient<IHashCalculator, HashCalculator>();
        services.AddScoped<IDbAccessor, AppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddTransient<UserApplicationService>();
        services.AddTransient<IImagesStorageAccessor, ImagesStorageAccessor>();
    }
}
