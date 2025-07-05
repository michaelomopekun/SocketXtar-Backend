namespace Application.Users.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) {}
}
