using Microsoft.EntityFrameworkCore;

namespace Api.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApiDbContext>(options => 
            options.UseNpgsql(connectionString));

        services.AddScoped<ApiDbContext>();
        
        return services;
    }

    public static IServiceProvider ApplyMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        db.Database.Migrate();

        return serviceProvider;
    }
}