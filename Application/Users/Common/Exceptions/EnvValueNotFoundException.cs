namespace Application.Users.Common.Exceptions;

public class EnvValueNotFoundException : Exception
{
    public EnvValueNotFoundException(string key) : base($"Environment variable '{key}' not found."){}
}

public class EnvConfigurationException : Exception
{
    public EnvConfigurationException(string message) : base(message) {}
}