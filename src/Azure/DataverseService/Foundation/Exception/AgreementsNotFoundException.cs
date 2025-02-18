namespace DataverseService.Foundation.Exception;

public class AgreementsNotFoundException : System.Exception
{
    public AgreementsNotFoundException()
    {
    }

    public AgreementsNotFoundException(string message)
        : base(message)
    {
    }

    public AgreementsNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}