namespace DataverseService.Foundation.Exception;

public class EventNotFoundException : System.Exception
{
    public EventNotFoundException()
    {
    }

    public EventNotFoundException(string message)
        : base(message)
    {
    }

    public EventNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}