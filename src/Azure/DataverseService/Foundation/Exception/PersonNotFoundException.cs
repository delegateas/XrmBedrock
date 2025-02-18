namespace DataverseService.Foundation.Exception;

public class PersonNotFoundException : System.Exception
{
    public PersonNotFoundException()
    {
    }

    public PersonNotFoundException(string message)
        : base(message)
    {
    }

    public PersonNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}