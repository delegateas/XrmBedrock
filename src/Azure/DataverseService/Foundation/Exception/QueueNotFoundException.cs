namespace DataverseService.Foundation.Exception;

public class QueueNotFoundException : System.Exception
{
    public QueueNotFoundException()
    {
    }

    public QueueNotFoundException(string message)
        : base(message)
    {
    }

    public QueueNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}