using System.Linq;
using System.Threading.Tasks;
using Api.Persistence;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
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
    private readonly TestcontainerDatabase _database = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "testDb",
            Username = "testUser",
            Password = "doesnt_matter"
        })
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
            services.AddPersistence(_database.ConnectionString);
        });
    }
    
    public async Task InitializeAsync()
    {
        // Start up our Docker container with the Postgres DB
        await _database.StartAsync();
    }

    public async Task DisposeAsync()
    {
        // Stop our Docker container with the Postgres DB
        await _database.DisposeAsync();
    }
}