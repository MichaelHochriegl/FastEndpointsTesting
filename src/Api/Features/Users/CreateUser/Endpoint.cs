using Api.Persistence;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users.CreateUser;

public class Endpoint : Endpoint<CreateUserRequest, CreateUserResponse, Mapper>
{
    private readonly ApiDbContext _dbContext;

    public Endpoint(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public override void Configure()
    {
        Post("/users");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var isAlreadyPresent = await _dbContext.Users.AnyAsync(x => x.Email == req.Email, ct);

        if (isAlreadyPresent)
        {
            Logger.LogInformation("User with this mail is already present");
            AddError(e => e.Email, $"A user with this mail is already present");
        }
        
        ThrowIfAnyErrors();

        var user = Map.ToEntity(req);
        user = _dbContext.Users.Add(user).Entity;
        await _dbContext.SaveChangesAsync(ct);
        
        Logger.LogInformation("User created, Id = '{userId}'", user.Id);

        var response = Map.FromEntity(user);
        await SendCreatedAtAsync<GetUser.Endpoint>(new { id = response.Id}, response, cancellation: ct);
    }
}