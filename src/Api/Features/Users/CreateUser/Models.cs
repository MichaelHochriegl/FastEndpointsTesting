namespace Api.Features.Users.CreateUser;

public record CreateUserRequest(string FirstName, string LastName, string Email);

public record CreateUserResponse(int Id);