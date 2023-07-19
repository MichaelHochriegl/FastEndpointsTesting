using Api.Domain;
using FastEndpoints;

namespace Api.Features.Users.GetUser;

public class Mapper : Mapper<GetUserRequest ,GetUserResponse, User>
{
    public override GetUserResponse FromEntity(User e) 
        => new(e.Id, e.FirstName, e.LastName, e.Email);
}