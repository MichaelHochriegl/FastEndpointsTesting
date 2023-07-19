using Api.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DbConnection");
ArgumentException.ThrowIfNullOrEmpty(connectionString);

builder.Services.AddPersistence(connectionString);
builder.Services.AddAuthentication();
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "My API";
        s.Version = "v1";
    };
});

var app = builder.Build();

app.Services.ApplyMigrations();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.UseOpenApi();
app.UseSwaggerUi3(c => c.ConfigureDefaults());

app.Run();