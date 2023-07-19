using Api.Domain;
using FastEndpoints;

namespace Api.Features.Users.CreateUser;

public class Mapper : Mapper<CreateUserRequest, CreateUserResponse, User>
{
    public override User ToEntity(CreateUserRequest r) 
        => new() { FirstName = r.FirstName, LastName = r.LastName, Email = r.Email};

    public override CreateUserResponse FromEntity(User e) => new() { Id = e.Id };
}