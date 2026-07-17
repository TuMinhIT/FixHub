namespace BookStore.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("ForbiddenAccess")
    {
    }

    public ForbiddenAccessException(string message) : base(message)
    {
    }
}