namespace Application.Users.Common.Exceptions;

public class InvalidOrExpiredTokenException : Exception
{
    public InvalidOrExpiredTokenException(string message) : base(message) {}
}
