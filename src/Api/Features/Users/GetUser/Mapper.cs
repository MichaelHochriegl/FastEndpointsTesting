using Api.Domain;
using FastEndpoints;

namespace Api.Features.Users.GetUser;

public class Mapper : Mapper<GetUserRequest ,GetUserResponse, User>
{
    public override GetUserResponse FromEntity(User e) 
        => new() { Id = e.Id, FirstName = e.FirstName, LastName = e.LastName, Email = e.Email };
}