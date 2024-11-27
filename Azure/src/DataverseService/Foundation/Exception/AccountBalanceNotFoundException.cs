namespace DataverseService.Foundation.Exception;

public class AccountBalanceNotFoundException : System.Exception
{
    public AccountBalanceNotFoundException()
    {
    }

    public AccountBalanceNotFoundException(string message)
        : base(message)
    {
    }

    public AccountBalanceNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}