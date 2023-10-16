using ImageStorage.Application;
using ImageStorage.Application.Dependencies;
using ImageStorage.Infrastructure.Configuration;
using ImageStorage.Infrastructure.DbAccess;
using ImageStorage.Infrastructure.FileStorage;
using ImageStorage.Infrastructure.Session;
using ImageStorage.WebApi.Handlers;
using ImageStorage.WebApi.Helpers;
using ImageStorage.WebApi.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

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

        services.AddApplication();

        AddInfrastructure(services);
    }

    private static void AddInfrastructure(IServiceCollection services)
    {
        services
            .AddScoped<IDbAccessor, AppDbContext>(sp => sp.GetRequiredService<AppDbContext>())
            .AddTransient<IImagesStorageAccessor, ImagesStorageAccessor>()
            .AddTransient<IHashCalculator, HashCalculator>()
            .AddSingleton<SessionContext>()
            .AddSingleton<ISessionContext, SessionContext>(sp => sp.GetRequiredService<SessionContext>())
            .AddSingleton<ISessionContextWriter, SessionContext>(sp => sp.GetRequiredService<SessionContext>());
    }
}
