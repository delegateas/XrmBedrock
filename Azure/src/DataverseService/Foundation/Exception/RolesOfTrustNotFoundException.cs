namespace DataverseService.Foundation.Exception;

public class RolesOfTrustNotFoundException : System.Exception
{
    public RolesOfTrustNotFoundException()
    {
    }

    public RolesOfTrustNotFoundException(string message)
        : base(message)
    {
    }

    public RolesOfTrustNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}