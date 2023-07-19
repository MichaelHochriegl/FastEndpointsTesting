using Api.Persistence;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users.GetUser;

public class Endpoint : Endpoint<GetUserRequest, GetUserResponse, Mapper>
{
    private readonly ApiDbContext _dbContext;

    public Endpoint(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("users/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
    {
        var user = await _dbContext.Users.FindAsync(req.Id);

        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = Map.FromEntity(user);
        await SendAsync(response, cancellation: ct);
    }
}


