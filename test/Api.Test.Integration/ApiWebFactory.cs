using System.Linq;
using System.Threading.Tasks;
using Api.Domain;
using Api.Persistence;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Xunit;

namespace Api.Test.Integration;

/// <summary>
/// Basic implementation of a WebApplicationFactory.
/// We will use this to spin up a server with our API running.
/// </summary>
/// <remarks>
/// Note that we use the <see cref="IApiMarker"/> interface to reference our API project.
/// We also implement the <see cref="IAsyncLifetime"/> interface to properly initialize and dispose of our used services.
/// </remarks>
public class ApiWebFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    // This defines our database container that will get spun up automatically for us for each test
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder()
        .WithDatabase("testDb")
        .WithUsername("testUser")
        .WithPassword("doesnt_matter")
        .Build();


    // We set up our test API server with this override
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // We disable any logging-providers for our test.
        // If you would like to use the `ITestOutputHelper` to visualize your log messages take a look at this:
        // https://github.com/martincostello/xunit-logging
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
        
        // We configure our services for testing
        builder.ConfigureTestServices(services =>
        {
            // remove any DbContextOptions registrations
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApiDbContext>));
            if (descriptor != null) services.Remove(descriptor);
            
            // Remove any DbContext registrations
            services.RemoveAll(typeof(ApiDbContext));
            
            // Register our DbContext with the test DB connection string provided from our container
            services.AddPersistence(_database.GetConnectionString());
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = builder.Build();
        var serviceScopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
        var dbContext = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ApiDbContext>();
        
        var userPresent = dbContext.Users.Any(u => u.FirstName == "SeededUserFirstName");
        if (userPresent is false)
        {
            dbContext.Users.Add(new User() { FirstName = "SeededUserFirstName", LastName = "SeededUserLastName", Email = "SeededUserEmail" });
        }
        dbContext.SaveChanges();
    
        host.Start();
        return host;
    }

    public async Task InitializeAsync()
    {
        // Start up our Docker container with the Postgres DB
        await _database.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        // Stop our Docker container with the Postgres DB
        await _database.DisposeAsync();
    }
}