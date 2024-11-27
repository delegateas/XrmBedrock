namespace DataverseService.Foundation.Exception;

public class InvalidPaymentTypeException : System.Exception
{
    public InvalidPaymentTypeException()
    {
    }

    public InvalidPaymentTypeException(string? message)
        : base(message)
    {
    }

    public InvalidPaymentTypeException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}