namespace DataverseService.Foundation.Exception;

public class RegistrationNotFoundException : System.Exception
{
    public RegistrationNotFoundException()
    {
    }

    public RegistrationNotFoundException(string message)
        : base(message)
    {
    }

    public RegistrationNotFoundException(string message, System.Exception innerException)
        : base(message, innerException)
    {
    }
}