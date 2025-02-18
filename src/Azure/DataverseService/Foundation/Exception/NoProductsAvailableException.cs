namespace DataverseService.Foundation.Exception;

public class NoProductsAvailableException : System.Exception
{
    public NoProductsAvailableException()
    {
    }

    public NoProductsAvailableException(string message)
        : base(message)
    {
    }

    public NoProductsAvailableException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}