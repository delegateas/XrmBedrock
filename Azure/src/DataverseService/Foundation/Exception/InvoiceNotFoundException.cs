namespace DataverseService.Foundation.Exception;

public class InvoiceNotFoundException : System.Exception
{
    public InvoiceNotFoundException()
    {
    }

    public InvoiceNotFoundException(string message)
        : base(message)
    {
    }

    public InvoiceNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}