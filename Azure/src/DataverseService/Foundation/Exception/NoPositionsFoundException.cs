namespace DataverseService.Foundation.Exception;

public class NoPositionsFoundException : System.Exception
{
    public NoPositionsFoundException()
    {
    }

    public NoPositionsFoundException(string message)
        : base(message)
    {
    }

    public NoPositionsFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}