namespace DataverseService.Foundation.Exception;

public class RollbackException : System.Exception
{
    public RollbackException()
    {
    }

    public RollbackException(string message)
        : base(message)
    {
    }

    public RollbackException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}