namespace DataverseService.Foundation.Exception;

public class TransactionsNotFoundException : System.Exception
{
    public TransactionsNotFoundException()
    {
    }

    public TransactionsNotFoundException(string message)
        : base(message)
    {
    }

    public TransactionsNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}