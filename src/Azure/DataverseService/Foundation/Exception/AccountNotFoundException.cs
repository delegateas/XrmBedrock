namespace DataverseService.Foundation.Exception;

public class AccountNotFoundException : System.Exception
{
    public AccountNotFoundException()
    {
    }

    public AccountNotFoundException(string message)
        : base(message)
    {
    }

    public AccountNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}