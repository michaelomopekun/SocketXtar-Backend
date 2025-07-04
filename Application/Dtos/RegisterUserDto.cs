namespace Application.Dtos;

public class RegisterUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterUserResponse
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}
