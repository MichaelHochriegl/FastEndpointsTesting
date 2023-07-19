namespace Api.Features.Users.CreateUser;

public class CreateUserRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    
}

public class CreateUserResponse
{
    public int Id { get; set; }
}