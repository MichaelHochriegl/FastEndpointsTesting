using FastEndpoints;
using FluentValidation;

namespace Api.Features.Users.CreateUser;

public class Validator : Validator<CreateUserRequest>
{
    public Validator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();
    }
}