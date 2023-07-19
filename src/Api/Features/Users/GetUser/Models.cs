namespace Api.Features.Users.GetUser;

public class GetUserRequest
{
    public int Id { get; set; }
}

public class GetUserResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    
}