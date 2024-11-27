namespace DataverseService.Foundation.Exception;

public class CommitteesNotFoundException : System.Exception
{
    public CommitteesNotFoundException()
    {
    }

    public CommitteesNotFoundException(string message)
        : base(message)
    {
    }

    public CommitteesNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}