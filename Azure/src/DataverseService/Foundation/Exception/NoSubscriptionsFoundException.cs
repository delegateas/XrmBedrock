namespace DataverseService.Foundation.Exception;

public class NoSubscriptionsFoundException : System.Exception
{
    public NoSubscriptionsFoundException()
    {
    }

    public NoSubscriptionsFoundException(string message)
        : base(message)
    {
    }

    public NoSubscriptionsFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}