using ImageStorage.Infrastructure.DbAccess;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static void ConfigureApplication(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MigrateDatabase();
    }

    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        using var db = services.GetRequiredService<AppDbContext>();

        try
        {
            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogCritical(ex, "An error occurred while migrating the database.");

            throw;
        }
    }
}
