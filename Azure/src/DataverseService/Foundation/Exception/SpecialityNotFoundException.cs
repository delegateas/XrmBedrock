namespace DataverseService.Foundation.Exception;

public class SpecialityNotFoundException : System.Exception
{
    public SpecialityNotFoundException()
    {
    }

    public SpecialityNotFoundException(string message)
        : base(message)
    {
    }

    public SpecialityNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}