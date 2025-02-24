namespace DataverseService.Foundation.Exception;

public class RepresentativesNotFoundException : System.Exception
{
    public RepresentativesNotFoundException()
    {
    }

    public RepresentativesNotFoundException(string message)
        : base(message)
    {
    }

    public RepresentativesNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}