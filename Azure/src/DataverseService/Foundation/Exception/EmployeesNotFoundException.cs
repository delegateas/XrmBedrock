namespace DataverseService.Foundation.Exception;

public class EmployeesNotFoundException : System.Exception
{
    public EmployeesNotFoundException()
    {
    }

    public EmployeesNotFoundException(string message)
        : base(message)
    {
    }

    public EmployeesNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}