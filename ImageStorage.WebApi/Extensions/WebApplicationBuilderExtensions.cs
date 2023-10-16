using ImageStorage.Application.Dependencies;
using ImageStorage.Application.Services;
using ImageStorage.Infrastructure.DbAccess;
using ImageStorage.WebApi.Handlers;
using ImageStorage.WebApi.Helpers;
using ImageStorage.WebApi.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
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

        AddApplicationServices(services);
    }

    private static void AddApplicationServices(IServiceCollection services)
    {
        services.AddTransient<UserApplicationService>();
        services.AddTransient<IDbAccessor, AppDbContext>();
        services.AddTransient<IHashCalculator, HashCalculator>();
    }
}
