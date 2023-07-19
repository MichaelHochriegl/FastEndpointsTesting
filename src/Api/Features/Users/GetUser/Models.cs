namespace Api.Features.Users.GetUser;

public record GetUserRequest(int Id);

public record GetUserResponse(int Id, string FirstName, string LastName, string Email);