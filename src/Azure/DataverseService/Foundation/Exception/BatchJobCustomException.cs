namespace DataverseService.Foundation.Exception;

public class BatchJobCustomException : System.Exception
{
    public BatchJobCustomException()
    {
    }

    public BatchJobCustomException(string message)
        : base(message)
    {
    }

    public BatchJobCustomException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}